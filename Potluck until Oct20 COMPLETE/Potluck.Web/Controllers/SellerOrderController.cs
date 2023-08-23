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
    /// Contains action for seller to manage orders
    /// </summary>
    public class SellerOrderController : Controller
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

        public IActionResult Index()
        {
            return RedirectToAction("ShowAllOrders");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="status">String</param>
        /// <param name="page">Long</param>
        /// <param name="size">Long</param>
        /// <returns>View("Index", pageOfOpenedOrders)</returns>
        public async Task<IActionResult> ShowAllOrders(string status = "OPENED", long page = 0, long size = 9)
        {
            
            PageOfOrders pageOfOpenedOrders = await apic.GetOrdersOfSeller(status, page, size);
            ViewBag.OrderStatus = status;

            return View("Index", pageOfOpenedOrders);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="status">String</param>
        /// <param name="page">Long</param>
        /// <param name="size">Long</param>
        /// <returns>View("Index", pageOfOpenedOrders)</returns>
        public async Task<IActionResult> ShowAllOrdersOfStatus(string status = "OPENED", long page = 0, long size = 9)
        {
            ViewBag.OrderStatus = status;
            PageOfOrders pageOfOpenedOrders = await apic.GetOrdersOfSeller(status, page, size);

            return View("Index", pageOfOpenedOrders);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="orderId">String</param>
        /// <param name="status">String</param>
        /// <returns>return View("OrderDetails", orderResponse)</returns>
        public async Task<IActionResult> ShowOrder(string orderId, string status="OPENED")
        {
            ViewBag.OrderStatus = status;
            OrderDTO orderResponse = await apic.GetOrderById(orderId);

            return View("OrderDetails", orderResponse);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> GetMostRecentOpenOrder()
        {
            PageOfOrders pageOfOrders = await apic.GetOrdersOfSeller("OPENED");
            if (pageOfOrders.content.Count > 0)
            {
                return View("OrderDetails", pageOfOrders.content.LastOrDefault());
            }
            else
            {
                return View("OrderDetails", new OrderDTO());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> GetClosedOrders()
        {
            PageOfOrders pageOfOrders = await apic.GetOrdersOfSeller("CLOSED");
            if (pageOfOrders.content.Count > 0)
            {
                return View("OrderDetails", pageOfOrders.content.LastOrDefault());
            }
            else
            {
                return View("OrderDetails", new OrderDTO());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> GetCancelledOrders()
        {
            PageOfOrders pageOfOrders = await apic.GetOrdersOfSeller("CANCELED");
            if (pageOfOrders.content.Count > 0)
            {
                return View("OrderDetails", pageOfOrders.content.LastOrDefault());
            }
            else
            {
                return View("OrderDetails", new OrderDTO());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="orderId">String</param>
        /// <param name="itemId">String</param>
        /// <returns>RedirectToAction("ShowOrder", orderId)</returns>
        public async Task<IActionResult> RemoveItemFromOrder(string orderId, string itemId)
        {
            if (await apic.RemoveItemFromOrder(orderId, itemId))
            {
                return RedirectToAction("ShowOrder", orderId);
            }
            else
            {
                return RedirectToAction("ShowOrder", orderId);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="orderId">String</param>
        /// <returns>RedirectToAction("ShowAllOrders")</returns>
        public async Task<IActionResult> DeleteOrder(string orderId)
        {

            OrderDTO orderResponse = await apic.GetOrderById(orderId);

            foreach (OrdersItemDTO ordersItemResponse in orderResponse.ordersItems)
            {
                await apic.RemoveItemFromOrder(orderId, ordersItemResponse.item.itemId);
            }

            await apic.DeleteOrder(orderId);

            return RedirectToAction("ShowAllOrders");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="order">OrderVO</param>
        /// <returns>OrderDTO</returns>
        public async Task<OrderDTO> UpdateOrder(OrderVO order)
        {
            string orderId = await apic.UpdateOrder(order);
            OrderDTO orderResponse = await apic.GetOrderById(orderId);
            return orderResponse;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeInSeconds">Long</param>
        /// <returns>Bool</returns>
        public bool LongerThanADay(long timeInSeconds)
        {
            int dayInSeconds = 86400;
            long unixTimeNow = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            long differenceInSeconds = unixTimeNow - timeInSeconds;

            if (differenceInSeconds > dayInSeconds)
            {
                return true;
            }

            return false;
        }
    }
}
