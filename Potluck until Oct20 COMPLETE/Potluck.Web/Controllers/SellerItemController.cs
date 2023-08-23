using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Potluck.Web.Models;
using Potluck.Web.Infrastructure;
using System.IO;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;

namespace Potluck.Web.Controllers
{
    /// <summary>
    /// SellerItemController contains action for sellers to manage their items
    /// </summary>
    public class SellerItemController : Controller
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
            return RedirectToAction("ShowMenu");
        }

        /////////////////////////////////////////////////////////////

        /// <summary>
        /// Returns the seller menu view
        /// </summary>
        /// <returns>View("Menu", pageOfItems)</returns>
        public async Task<IActionResult> ShowMenu()
        {
            UserDTO user = await apic.GetCurrentUser();
            PageOfItems pageOfItems = await apic.GetMenuByUserId(user.userId);

            return View("Menu", pageOfItems);
        }

        /// <summary>
        /// Returns the seller menu view by page
        /// </summary>
        /// <param name="pageNumber">Long</param>
        /// <returns>View("Menu", pageOfItems)</returns>
        public async Task<IActionResult> ShowMenuPageByNumber(long pageNumber)
        {
            UserDTO user = await apic.GetCurrentUser();
            PageOfItems pageOfItems = await apic.GetMenuByUserId(user.userId, pageNumber);
            return View("Menu", pageOfItems);
        }

        /// <summary>
        /// Return a view with item details
        /// </summary>
        /// <param name="itemId">String</param>
        /// <returns>View("ItemDetails", itemResponse)</returns>
        public async Task<IActionResult> ShowItemDetails(string itemId)
        {
            PageOfCustoms pageOfCustoms = await apic.GetAllCustoms(0,25);
            ItemDTO itemResponse = await apic.GetItemById(itemId);
            
            ViewBag.SellerCustoms = pageOfCustoms.content;

            return View("ItemDetails", itemResponse);
        }

        /////////////////////////////////////////////////////////////

        /// <summary>
        /// returns the item creation view
        /// </summary>
        /// <returns>View("ItemCreate", item)</returns>
        public async Task<IActionResult> ShowItemCreate()
        {
            ItemVO item = new ItemVO();

            List<Category> categories = await apic.GetAllCategories();

            ViewBag.Categories = categories;

            return View("ItemCreate", item);
        }

        /// <summary>
        /// Creates an item to be add to the seller item collection
        /// </summary>
        /// <param name="item">ItemVO</param>
        /// <returns>RedirectToAction("ShowItemDetails", new { itemId = itemId })</returns>
        public async Task<IActionResult> CreateItem(ItemVO item)
        {
            string itemId = await apic.CreateItem(item);

            return RedirectToAction("ShowItemDetails", new { itemId = itemId });
        }

        /////////////////////////////////////////////////////////////

        /// <summary>
        /// Return the Item update view
        /// </summary>
        /// <param name="itemId">String</param>
        /// <returns>View("ItemUpdate", item)</returns>
        public async Task<IActionResult> ShowItemUpdate(string itemId)
        {
            ItemDTO item = await apic.GetItemById(itemId);

            List<Category> categories = await apic.GetAllCategories();

            ViewBag.Categories = categories;

            return View("ItemUpdate", item);
        }

        /// <summary>
        /// Updates an item
        /// </summary>
        /// <param name="item">ItemDTO</param>
        /// <returns>RedirectToAction("ShowItemDetails", new { itemId = itemId })</returns>
        public async Task<IActionResult> UpdateItem(ItemDTO item)
        {
            ItemVO updatedItem = new ItemVO(item);

            var httpClient = new HttpClient();

            if (item.imageUrl.StartsWith("https://pot-luck-pictures.s3.amazonaws.com"))
            {
                byte[] picture = null;
                picture = await httpClient.GetByteArrayAsync(item.imageUrl);
                updatedItem.picture = Convert.ToBase64String(picture);
            }
            
            string itemId = await apic.UpdateItem(updatedItem);


            return RedirectToAction("ShowItemDetails", new { itemId = itemId });
        }

        /// <summary>
        /// Async task for enabling and disabling item availability
        /// </summary>
        /// <param name="itemId">String</param>
        /// <param name="enabled">Bool</param>
        /// <returns></returns>
        public async Task ToggleEnable(string itemId, bool enabled)
        {
            await apic.ItemEnableSwitch(itemId);
        }
        /////////////////////////////////////////////////////////////

        /// <summary>
        /// Adds a custom collection to an item
        /// </summary>
        /// <param name="itemId">String</param>
        /// <param name="customId">String</param>
        /// <returns>RedirectToAction("ShowItemDetails", new { itemId=itemId })</returns>
        public async Task<IActionResult> AddCustomToItem(string itemId, string customId)
        {
            await apic.AddCustomToItem(itemId,customId);
            return RedirectToAction("ShowItemDetails", new { itemId=itemId });
        }

        /// <summary>
        /// Removes a custom collection to an item
        /// </summary>
        /// <param name="itemId">String</param>
        /// <param name="customId">String</param>
        /// <returns>RedirectToAction("ShowItemDetails", new { itemId = itemId })</returns>
        public async Task<IActionResult> RemoveCustomFromItem(string itemId, string customId)
        {
            await apic.RemoveCustomFromItem(itemId, customId);
            return RedirectToAction("ShowItemDetails", new { itemId = itemId });
        }

        /////////////////////////////////////////////////////////////

        /// <summary>
        /// Return the item delete confirmation view
        /// </summary>
        /// <param name="itemId">String</param>
        /// <returns>View("ItemDelete", itemResponse)</returns>
        public async Task<IActionResult> ShowItemDelete(string itemId)
        {
            ItemDTO itemResponse = await apic.GetItemById(itemId);
            return View("ItemDelete", itemResponse);
        }

        /// <summary>
        /// Deletes and item from a users item colecton
        /// </summary>
        /// <param name="itemId">String</param>
        /// <returns></returns>
        public async Task<IActionResult> DeleteItem(string itemId)
        {
            if (await apic.DeleteItem(itemId))
            {
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("ShowItemDelete", itemId);
            }
        }    
        /////////////////////////////////////////////////////////////
    }
}