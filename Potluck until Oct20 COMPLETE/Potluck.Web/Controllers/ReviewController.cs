using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Potluck.Web.Infrastructure;
using Potluck.Web.Models;

namespace Potluck.Web.Controllers
{
    public class ReviewController : Controller
    {
        APIController apic = new APIController();

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            GetTokenFromSessionUser();
            PreLoadViewBag();
        }

        public void GetTokenFromSessionUser()
        {
            string token = HttpContext.Session.GetJson<string>("Token");
            apic.Token = token;
        }

        public void PreLoadViewBag()
        {
            string currentRole = HttpContext.Session.GetJson<string>("CurrentRole");

            if (currentRole != null)
            {
                ViewBag.CurrentRole = currentRole;
            }

        }
        /// <summary>
        /// Returns a view with a page of reviews displaying the sellers reviews.
        /// </summary>
        /// <param name="email">String</param>
        /// <returns>View("Index", pageOfReviews)</returns>
        public async Task<IActionResult> ShowSellerReviews(string email)
        {
            PageOfReviews pageOfReviews = await apic.GetAllReviewsByEmail(email);
            UserDTO user = await apic.GetUserByEmail(email);
            ViewBag.seller = user;
            return View("Index", pageOfReviews);
        }

        /// <summary>
        /// Returns a view for creating new reviews
        /// The view will prefill certain fields based on seller data
        /// </summary>
        /// <param name="email">String</param>
        /// <returns>View("ReviewCreate", newReview)</returns>
        public async Task<IActionResult> ShowReviewCreate(string email)
        {
            ReviewVO newReview = new ReviewVO();
            newReview.sellerEmail = email;
            newReview.reviewId = "0";

            UserDTO user = await apic.GetUserByEmail(email);
            ViewBag.seller = user;
            return View("ReviewCreate", newReview);
        }

        /// <summary>
        /// Creates a review by passing a review vo to the back end
        /// then loads the sellers page of reviews.
        /// </summary>
        /// <param name="review">ReviewVO</param>
        /// <returns>View("Index", pageOfReviews)</returns>
        public async Task<IActionResult> CreateReview(ReviewVO review)
        {
            await apic.CreateReview(review);
            PageOfReviews pageOfReviews = await apic.GetAllReviewsByEmail(review.sellerEmail);
            return View("Index", pageOfReviews);
        }
    }
}
