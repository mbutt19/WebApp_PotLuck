using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Potluck.Web.Models;
using Potluck.Web.Infrastructure;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Localization;

namespace Potluck.Web.Controllers
{
    /// <summary>
    /// Handles profile actions
    /// </summary>
    public class ProfileController : Controller
    {
        public ProfileController()
        {
            ;
        }
       
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
        /// Shows the main profile index
        /// </summary>
        /// <returns>View("Index", user)</returns>
        public async Task<IActionResult> Index()
        {
            UserDTO user = await apic.GetCurrentUser();

            if (user == null)
            {
                return RedirectToAction("Login","Home");
            }
            return View("Index", user);
        }

        /// <summary>
        /// Displays the profile edit view
        /// </summary>
        /// <returns>View("Index", user)</returns>
        public async Task<IActionResult> ProfileEdit()
        {
            UserDTO user = await apic.GetCurrentUser();

            if (user == null)
            {
                return RedirectToAction("Login", "Home");
            }
            return View("ProfileEdit", user);
        }

        /// <summary>
        /// Edits a users profile details
        /// </summary>
        /// <param name="user"></param>
        /// <returns>RedirectToAction("Index")</returns>
        public async Task<IActionResult> EditUser(UserDTO user)
        {
            UserDTO currentUser = await apic.GetCurrentUser();

            UserVO updatedUser = new UserVO(currentUser);

            updatedUser.name = user.name;
            updatedUser.email = user.email;
            updatedUser.picture = user.imageUrl;
            updatedUser.telephone = user.telephone;

            var httpClient = new HttpClient();

            if (user.imageUrl.StartsWith("http"))
            {
                byte[] picture = null;
                picture = await httpClient.GetByteArrayAsync(user.imageUrl);
                updatedUser.picture = Convert.ToBase64String(picture);
            }
            
            await apic.EditUser(updatedUser);

            HttpContext.Session.SetJson("CurrentUser", updatedUser);

            return RedirectToAction("Index");
        }

        /// <summary>
        /// Returns the address manager view
        /// </summary>
        /// <returns>View("ProfileAddress", user)</returns>
        public async Task<IActionResult> ShowAddressManager()
        {

            if(TempData["NoAddress"]!= null && (bool)TempData["NoAddress"] == true)
            {
                ViewBag.NoAddress = true;
            }
            else
            {
                ViewBag.NoAddress = false;
            }

            UserDTO user = await apic.GetCurrentUser();
            return View("ProfileAddress", user);
        }

        /// <summary>
        /// Returns the address management view
        /// </summary>
        /// <returns>View("ProfileAddAddress")</returns>
        public async Task<IActionResult> ShowAddAddress()
        {
            UserDTO user = await apic.GetCurrentUser();
            return View("ProfileAddAddress");
        }

        /// <summary>
        /// Adds an address to a user account
        /// </summary>
        /// <param name="address">AddressVO</param>
        /// <returns>View("ProfileAddress", user)</returns>
        public async Task<IActionResult> AddAddress(AddressVO address)
        {
            string addressString = $"{address.addressLine1}+{address.addressLine2}+{address.city}+{address.postalCode}";
            GoogleGeo googleResponse = await apic.GetCoords(addressString);

            address.latitude = googleResponse.results.ElementAt(0).geometry.location.lat;
            address.longitude = googleResponse.results.ElementAt(0).geometry.location.lng;

            UserDTO user = await apic.GetCurrentUser();

            await apic.AddAddress(user.email, address);

            user = await apic.GetCurrentUser();

            return RedirectToAction("ShowAddressManager");
        }

        /// <summary>
        /// Return the view for updating an address
        /// </summary>
        /// <param name="addressId">String</param>
        /// <returns></returns>
        public async Task<IActionResult> ShowUpdateAddress(string addressId)
        {
            UserDTO user = await apic.GetCurrentUser();
            AddressDTO address = user.addresses.Where(addr => addr.addressId == addressId).FirstOrDefault();

            return View("ProfileUpdateAddress", address);
        }

        /// <summary>
        /// Updates an address
        /// </summary>
        /// <param name="address">AddressDTO</param>
        /// <returns>View("ProfileAddress", user)</returns>
        public async Task<IActionResult> UpdateAddress(AddressDTO address)
        {
            string addressString = $"{address.addressLine1}+{address.addressLine2}+{address.city}+{address.postalCode}";
            GoogleGeo googleResponse = await apic.GetCoords(addressString);

            address.latitude = googleResponse.results.ElementAt(0).geometry.location.lat;

            address.longitude = googleResponse.results.ElementAt(0).geometry.location.lng;

            UserDTO user = await apic.GetCurrentUser();
            AddressVO updatedAdress = new AddressVO(address);

            await apic.UpdateAddress(user.email, updatedAdress);

            user = await apic.GetCurrentUser();

            return RedirectToAction("ShowAddressManager");
        }

        /// <summary>
        /// REmoves an address from a user account
        /// </summary>
        /// <param name="addressId">string</param>
        /// <returns>View("ProfileAddress", user)</returns>
        public async Task<IActionResult> RemoveAddress(string addressId)
        {
            UserDTO user = await apic.GetCurrentUser();
            await apic.RemoveAddress(user.email, addressId);

            user = await apic.GetCurrentUser();

            return RedirectToAction("ShowAddressManager");
        }

        /// <summary>
        /// Returns the discover view, will return a map of sellers within the range
        /// </summary>
        /// <param name="maxRange">string</param>
        /// <returns>View("Discover", users)</returns>
        public async Task<IActionResult> ShowDiscover(string maxRange = "1")
        {
            List<UserDTO> users = new List<UserDTO>();
            UserDTO user = await apic.GetCurrentUser();

            users.Add(user);

            AddressDTO mainAddress = user.addresses.Where(addr => addr.mainAddress == true).First();
            List<UserDTO> discoveryPoints = await apic.GetGeoFence(mainAddress.addressId, maxRange);

            users.AddRange(discoveryPoints);

            ViewBag.MaxRange = maxRange;

            return View("Discover", users);
        }

        /// <summary>
        /// Adds a favorite to a users favorites collection
        /// </summary>
        /// <param name="itemId">String</param>
        /// <returns></returns>
        public async Task<IActionResult> AddFavorite(string itemId)
        {
            UserDTO user = await apic.GetCurrentUser();
            await apic.AddFavorite(user.email, itemId);

            PageOfItems pageOfItems = await apic.GetFavoritesByEmail(user.email);

            return View("Favorites", pageOfItems);
        }

        /// <summary>
        /// Adds a favorite to a users favorites collection
        /// </summary>
        /// <param name="itemId">String</param>
        /// <returns>View("Favorites", pageOfItems)</returns>
        public async Task<IActionResult> RemoveFavorite(string itemId)
        {
            UserDTO user = await apic.GetCurrentUser();
            await apic.RemoveFavorite(user.email, itemId);
            PageOfItems pageOfItems = await apic.GetFavoritesByEmail(user.email);

            return View("Favorites", pageOfItems);
        }

        /// <summary>
        /// Returns view displaying a page of favorites 
        /// </summary>
        /// <param name="page">Long</param>
        /// <param name="size">Long</param>
        /// <returns>View("Favorites", pageOfItems)</returns>
        public async Task<IActionResult> ShowFavorites(long page=0, long size=9)
        {
            UserDTO user = await apic.GetCurrentUser();
            PageOfItems pageOfItems = await apic.GetFavoritesByEmail(user.email,page,size);

            return View("Favorites", pageOfItems);
        }

        /// <summary>
        /// Logs the user out, this will null any session variables to ensure a proper log out
        /// </summary>
        /// <returns>RedirectToAction("Index", "Home")</returns>
        public IActionResult LogOutUser()
        {
            HttpContext.Session.SetJson("CurrentProfile", null);
            HttpContext.Session.SetJson("CurrentUser", null);
            HttpContext.Session.SetJson("CurrentRole", null);
            HttpContext.Session.SetJson("CurrentOrderId", null);
            HttpContext.Session.SetJson("Token", null);

            return RedirectToAction("Index", "Home");
        }
    }
}