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
    /// <summary>
    /// SellerCustomController handles actions for custom collections
    /// </summary>
    public class SellerCustomController : Controller
    {
        private APIController apic = new APIController();

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

        /////////////////////////////////////////////////////////////

        /// <summary>
        /// Return the custom create view
        /// </summary>
        /// <returns>View("CustomCreate", custom)</returns>
        public IActionResult ShowCustomCreate()
        {
            CustomVO custom = new CustomVO();
            return View("CustomCreate", custom);
        }

        /// <summary>
        /// Creates a custom
        /// </summary>
        /// <param name="custom">CustomVO</param>
        /// <returns>RedirectToAction("ShowCustomDetails", new { customId = customId })</returns>
        public async Task<IActionResult> CreateCustom(CustomVO custom)
        {
            custom.dateTimeCreated = Convert.ToString(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
            string customId = await apic.CreateCustom(custom);
            ViewBag.Message = "Step 2. Add some items to your combo...";
            return RedirectToAction("ShowCustomDetails", new { customId = customId });
        }

        /// <summary>
        /// Deletes a custom
        /// </summary>
        /// <param name="customId"></param>
        /// <returns></returns>
        public async Task<IActionResult> DeleteCustom(string customId)
        {
            await apic.DeleteCustom(customId);
            return RedirectToAction("ShowAllCustoms");
        }
        /////////////////////////////////////////////////////////////

        /// <summary>
        /// Adds an item to a custom
        /// </summary>
        /// <param name="customId">String</param>
        /// <param name="itemId">String</param>
        /// <returns>RedirectToAction("ShowCustomDetails", new { customId = customId })</returns>
        public async Task<IActionResult> AddItemToCustom(string customId, string itemId)
        {
            await apic.AddItemToCustom(customId,itemId);
            CustomDTO custom = await apic.GetCustomById(customId);
            custom.dateTimeModified = Convert.ToString(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
            CustomVO newCustom = new CustomVO(custom);
            await apic.UpdateCustom(newCustom);
            return RedirectToAction("ShowCustomDetails", new { customId = customId });
        }

        /// <summary>
        /// Removes an item to a custom
        /// </summary>
        /// <param name="customId">String</param>
        /// <param name="itemId">String</param>
        /// <returns>RedirectToAction("ShowCustomDetails", new { customId = customId })</returns>
        public async Task<IActionResult> RemoveItemFromCustom(string customId, string itemId)
        {
            await apic.RemoveItemFromCustom(customId, itemId);
            CustomDTO custom = await apic.GetCustomById(customId);
            custom.dateTimeModified = Convert.ToString(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
            CustomVO newCustom = new CustomVO(custom);
            await apic.UpdateCustom(newCustom);
            return RedirectToAction("ShowCustomDetails", new { customId = customId });
        }
        /////////////////////////////////////////////////////////////

        /// <summary>
        /// Returns a page with seller customs
        /// </summary>
        /// <returns>View("Index", pageOfCustoms)</returns>
        public async Task<IActionResult> ShowAllCustoms()
        {
            PageOfCustoms pageOfCustoms = await apic.GetAllCustoms();
            return View("Index", pageOfCustoms);
        }

        /// <summary>
        /// Returns a page with seller customs based on page number
        /// </summary>
        /// <param name="pageNumber">Long</param>
        /// <returns>View("Index", pageOfCustoms)</returns>
        public async Task<IActionResult> ShowCustomPageByNumber(long pageNumber)
        {
            PageOfCustoms pageOfCustoms = await apic.GetAllCustoms(pageNumber, 9);
            return View("Index", pageOfCustoms);
        }

        /////////////////////////////////////////////////////////////

        /// <summary>
        /// Shows the details of a custom
        /// </summary>
        /// <param name="customId">String</param>
        /// <returns>View("CustomDetails", customResponse)</returns>
        public async Task<IActionResult> ShowCustomDetails(string customId)
        {
            UserDTO user = await apic.GetCurrentUser();
            PageOfItems pageOfItems = await apic.GetMenuByUserId(user.userId,0,25);
            CustomDTO customResponse = await apic.GetCustomById(customId);
            ViewBag.SellerItems = pageOfItems.content;

            return View("CustomDetails", customResponse);
        }
        /////////////////////////////////////////////////////////////
    }
}
