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
    /// BuyerItemController Contains all the methods for buyers to browse and search for items
    /// </summary>
    public class BuyerItemController : Controller
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

        public async Task PreLoadViewBag()
        {
            string currentRole = HttpContext.Session.GetJson<string>("CurrentRole");
            UserDTO user = await apic.GetCurrentUser();

            if (currentRole != null)
            {
                ViewBag.CurrentRole = currentRole;
            }

            if (user != null)
            {
                ViewBag.CurrentUser = user;
            }
        }

        public IActionResult Index()
        {
            return RedirectToAction("ShowSearch");
        }

        /////////////////////////////////////////////////////////////

        /// <summary>
        /// Returns the search view with results after a search action is performed
        /// </summary>
        /// <param name="searchVal">String</param>
        /// <param name="time">String</param>
        /// <returns>View("Search", pageOfItems)</returns>
        public async Task<IActionResult> ShowSearch(string searchVal, string time = "00:00")
        {
            ViewBag.Searched = searchVal;
            UserDTO user = await apic.GetCurrentUser();

            AddressDTO address = user.addresses.Where(x => x.mainAddress == true).FirstOrDefault();

            if (address != null) 
            {
                PageOfItems pageOfItems = await apic.GetMenuBySearch(searchVal, address.latitude, address.longitude, 0, time);
                return View("Search", pageOfItems);
            }
            else
            {
                PageOfItems pageOfItems = await apic.GetMenuBySearch(searchVal, 0, 0, 0, time);
                TempData["NoAddress"] = true;
                return RedirectToAction("ShowAddressManager", "Profile");
            }
            
        }

        /// <summary>
        /// Will adjust view to the selected page of the current search
        /// </summary>
        /// <param name="searchVal">String</param>
        /// <param name="pageNumber">Long</param>
        /// <returns>View("Search", pageOfItems)</returns>
        public async Task<IActionResult> ShowSearchPageByNumber(string searchVal, long pageNumber)
        {
            ViewBag.Searched = searchVal;

            UserDTO user = await apic.GetCurrentUser();

            AddressDTO address = user.addresses.Where(x => x.mainAddress == true).FirstOrDefault();

            PageOfItems pageOfItems = await apic.GetMenuBySearch(searchVal, address.latitude, address.longitude, pageNumber);

            return View("Search", pageOfItems);
        }

        /// <summary>
        /// Returns a seller menu view based on user id
        /// </summary>
        /// <param name="userId">String</param>
        /// <returns>View("Menu", pageOfItems)</returns>
        public async Task<IActionResult> ShowMenu(string userId)
        {
            PageOfItems pageOfItems = await apic.GetMenuByUserId(userId);

            return View("Menu", pageOfItems);
        }

        /// <summary>
        /// Will adjust view to the selected page of the current seller menu
        /// </summary>
        /// <param name="userId">String</param>
        /// <param name="pageNumber">Long</param>
        /// <returns>View("Menu", pageOfItems)</returns>
        public async Task<IActionResult> ShowMenuPageByNumber(string userId, long pageNumber)
        {
            PageOfItems pageOfItems = await apic.GetMenuByUserId(userId, pageNumber);
            return View("Menu", pageOfItems);
        }

        /// <summary>
        /// Will return view with item details
        /// </summary>
        /// <param name="itemId">String</param>
        /// <returns>View("ItemDetails", itemResponse)</returns>
        public async Task<IActionResult> ShowItemDetails(string itemId)
        {
            ItemDTO itemResponse = await apic.GetItemById(itemId);
            List<CustomDTO> customs = new List<CustomDTO>();

            

            foreach (CustomDTO customResponse in itemResponse.customs)
            {
                CustomDTO newCustom = new CustomDTO(customResponse);
                newCustom.subItems = new List<SubItemDTO>();

                foreach (SubItemDTO subItem in customResponse.subItems)
                {
                    if (subItem.enable && subItem.name != null && subItem.itemId != "0")
                    {
                        newCustom.subItems.Add(subItem);
                    }
                }

                if(newCustom.subItems.Count != 0)
                {
                    customs.Add(newCustom);
                }
            }
            itemResponse.customs = customs;

            return View("ItemDetails", itemResponse);
        }
    }
}