using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using Potluck.Web.Infrastructure;
using Potluck.Web.Models;

namespace Potluck.Web.Controllers
{
    /// <summary>
    /// BuyerOrderController handles orders from the buyers perspective
    /// </summary>
    public class BuyerOrderController : Controller
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

        public OrderVO GetSessionOrder()
        {
            OrderVO order = HttpContext.Session.GetJson<OrderVO>("SessionOrder");

            if (order == null)
            {
                CreateSessionOrder();
                order = HttpContext.Session.GetJson<OrderVO>("SessionOrder");
            }

            return order;
        }

        public void CreateSessionOrder()
        {
            OrderVO order = new OrderVO();
            HttpContext.Session.SetJson("SessionOrder", order);
        }

        /// <summary>
        /// Adds an item to an order
        /// </summary>
        /// <param name="json">String</param>
        /// <returns></returns>
        public async Task<IActionResult> AddItemToOrder(string json)
        {
            OrdersItemVO ordersItem = JsonConvert.DeserializeObject<OrdersItemVO>(json);
            OrderDTO orderResponse;

            string currentOrderId = HttpContext.Session.GetJson<string>("CurrentOrderId");

            if (currentOrderId == null || currentOrderId == "0")
            {
                OrderVO order = new OrderVO();
                order.orderId = "0";
                order.deliveryType = "PICK_UP";
                order.ordersItems = new List<OrdersItemVO>();
                order.ordersItems.Add(ordersItem);

                currentOrderId = await apic.CreateOrder(order);
                HttpContext.Session.SetJson("CurrentOrderId", currentOrderId);
            }
            else
            {
                ItemDTO itemResponse = await apic.GetItemById(ordersItem.itemId);
                orderResponse = await apic.GetOrderById(currentOrderId);

                if (orderResponse.seller.userId == itemResponse.user.userId)
                {
                    await apic.AddOrderItemToOrder(currentOrderId, ordersItem);
                }
                else
                {
                    ViewBag.SellerConflict = true;
                    return View("../BuyerItem/ItemDetails", itemResponse);
                }
            }

            orderResponse = await apic.GetOrderById(currentOrderId);

            return RedirectToAction("ShowOrder",new { orderId = orderResponse.orderId });
        }

        /// <summary>
        /// Creates a new order
        /// </summary>
        /// <param name="itemId">String</param>
        /// <returns></returns>
        public async Task<IActionResult> NewOrder(string itemId)
        {
            string currentOrderId = HttpContext.Session.GetJson<string>("CurrentOrderId");

            if (currentOrderId != "0")
            {
                await apic.ChangeOrderStatus(currentOrderId, "CANCELED");
            }

            HttpContext.Session.SetJson("CurrentOrderId", "0");

            return RedirectToAction("ShowItemDetails", "BuyerItem", new { itemId });
        }

        /// <summary>
        /// Returns a view with with all orders
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            return RedirectToAction("ShowAllOrders");
        }

        /// <summary>
        /// Creates a new order
        /// </summary>
        /// <param name="order">OrderVO</param>
        /// <returns>String</returns>
        public async Task<string> CreateOrder(OrderVO order = null)
        {
            if (order == null)
            {
                order = new OrderVO();
            }

            order.dateTimeCreated = Convert.ToString(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
            string orderId = await apic.CreateOrder(order);
            return orderId;
        }

        /// <summary>
        /// Returns a view thats displays a page of orders
        /// </summary>
        /// <param name="status">String</param>
        /// <param name="page">Long</param>
        /// <param name="size">Long</param>
        /// <returns></returns>
        public async Task<IActionResult> GetPageOfOrders(string status="OPENED", long page = 0, long size = 9)
        {
            PageOfOrders pageOfOrders = await apic.GetOrdersOfBuyer(status, page, size);

            ViewBag.OrderStatus = status;

            return View("Index", pageOfOrders);
        }

        /// <summary>
        /// Returns a view displaying an order
        /// </summary>
        /// <param name="orderId">String</param>
        /// <param name="status">String</param>
        /// <returns></returns>
        public async Task<IActionResult> ShowOrder(string orderId, string status)
        {
            ViewBag.OrderStatus = status;
            OrderDTO orderResponse = await apic.GetOrderById(orderId);

            return View("OrderDetails", orderResponse);
        }

        /// <summary>
        /// Returns the most recent order
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> GetMostRecentOpenOrder()
        {
            PageOfOrders pageOfOrders = await apic.GetOrdersOfBuyer("OPENED");
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
        /// Returns a view with closed orders
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> GetClosedOrders()
        {
            PageOfOrders pageOfOrders = await apic.GetOrdersOfBuyer("CLOSED");
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
        /// returns a view with cancelled orders
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
        /// Returns a view where the buyers can checkout
        /// </summary>
        /// <param name="orderId">String</param>
        /// <returns>View("Checkout", orderResponse)</returns>
        public async Task<IActionResult> ShowCheckout(string orderId)
        {
            OrderDTO orderResponse = await apic.GetOrderById(orderId);
            return View("Checkout", orderResponse);
        }

        /// <summary>
        /// REmoves an item from an order
        /// </summary>
        /// <param name="orderId">String</param>
        /// <param name="ordersItemsId">String</param>
        /// <returns>View("OrderDetails", orderResponse)</returns>
        public async Task<IActionResult> RemoveItemFromOrder(string orderId, string ordersItemsId)
        {
            OrderDTO orderResponse = null;
            if (await apic.RemoveOrderItemFromOrder(orderId, ordersItemsId))
            {
                orderResponse = await apic.GetOrderById(orderId);
                return View("OrderDetails", orderResponse);
            }
            else
            {
                orderResponse = await apic.GetOrderById(orderId);
                return View("OrderDetails", orderResponse);
            }
        }

        /// <summary>
        /// Cancels an order
        /// </summary>
        /// <param name="orderId">String</param>
        /// <returns>RedirectToAction("GetPageOfOrders")</returns>
        public async Task<IActionResult> CancelOrder(string orderId)
        {
            await apic.ChangeOrderStatus(orderId, "CANCELED");

            HttpContext.Session.SetJson("CurrentOrderId", "0");

            return RedirectToAction("GetPageOfOrders");
        }

        /// <summary>
        /// Updates an order
        /// </summary>
        /// <param name="order">OrderVO</param>
        /// <returns>OrderDTO</returns>
        public async Task<OrderDTO> UpdateOrder(OrderVO order)
        {
            order.dateTimeModified = Convert.ToString(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
            string orderId = await apic.UpdateOrder(order);
            OrderDTO orderResponse = await apic.GetOrderById(orderId);
            return orderResponse;
        }
    }
}