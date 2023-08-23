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
    /// SellerDeliveryController contins actions for creating and modifying seller delivery settings
    /// </summary>
    public class SellerDeliveryController : Controller
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

        /// <summary>
        /// Returns the delivery options view 
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index()
        {
            UserDTO user = await apic.GetCurrentUser();
            PageOfDeliveries pageOfDeliveries = await apic.GetAllDeliveriesByEmail(user.email);
            return View("Index", pageOfDeliveries);
        }

        /// <summary>
        /// Creates a deliveryVO object
        /// </summary>
        /// <param name="name">String</param>
        /// <param name="coverageRadius">String</param>
        /// <param name="feePercentage">String</param>
        /// <returns>RedirectToAction("Index")</returns>
        public async Task<IActionResult> CreateDelivery(string name, string coverageRadius, string feePercentage)
        {
            DeliveryVO delivery = new DeliveryVO(name, "0", feePercentage, coverageRadius);

            await apic.CreateDelivery(delivery);

            return RedirectToAction("Index");
        }

        /// <summary>
        /// Edits the details of an existing delivery object
        /// </summary>
        /// <param name="deliveryId">String</param>
        /// <param name="name">String</param>
        /// <param name="coverageRadius">String</param>
        /// <param name="feePercentage">String</param>
        /// <returns>RedirectToAction("Index")</returns>
        public async Task<IActionResult> EditDelivery(string deliveryId, string name, string coverageRadius, string feePercentage)
        {
            DeliveryVO delivery = new DeliveryVO(name, deliveryId, feePercentage, coverageRadius);

            await apic.UpdateDelivery(delivery);

            return RedirectToAction("Index");
        }

        /// <summary>
        /// Deletes a delivery object
        /// </summary>
        /// <param name="deliveryId">string</param>
        /// <returns>RedirectToAction("Index")</returns>
        public async Task<IActionResult> DeleteDelivery(string deliveryId)
        {
            await apic.DeleteDelivery(deliveryId);

            return RedirectToAction("Index");
        }
    }
}
