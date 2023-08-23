using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Potluck.Web.Infrastructure;
using Potluck.Web.Models;

namespace Potluck.Web.Controllers
{
    /// <summary>
    /// AdminController contains high level administration actions 
    /// </summary>
    public class AdminController : Controller
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

        /// <summary>
        /// Opens the category config view
        /// </summary>
        /// <returns>View("CategoryConfig", categories)</returns>
        public async Task<IActionResult> ShowCategoryConfig()
        {
            List<Category> categories = new List<Category>();

            categories = await apic.GetAllCategories();

            return View("CategoryConfig", categories);
        }

        /// <summary>
        /// Creates a new category
        /// </summary>
        /// <param name="name">String</param>
        /// <returns>RedirectToAction("ShowCategoryConfig")</returns>
        public async Task<IActionResult> CreateCategory(string name)
        {
            Category category = new Category("0", name);

            await apic.CreateCategory(category);

            return RedirectToAction("ShowCategoryConfig");
        }

        /// <summary>
        /// Updates a category
        /// </summary>
        /// <param name="categoryId">String</param>
        /// <param name="name">String</param>
        /// <returns>RedirectToAction("ShowCategoryConfig")</returns>
        public async Task<IActionResult> UpdateCategory(string categoryId, string name)
        {
            Category category = new Category(categoryId, name);

            await apic.UpdateCategory(category);

            return RedirectToAction("ShowCategoryConfig");
        }

        /// <summary>
        /// Deletes a category
        /// </summary>
        /// <param name="categoryId">String</param>
        /// <returns>RedirectToAction("ShowCategoryConfig")</returns>
        public async Task<IActionResult> DeleteCategory(string categoryId)
        {
            await apic.DeleteCategory(categoryId);

            return RedirectToAction("ShowCategoryConfig");
        }

        /////////////////////////////
        //Country and Region Config//
        /////////////////////////////

        /// <summary>
        /// Opens the country config view
        /// </summary>
        /// <returns>View("CountryConfig", countries)</returns>
        public async Task<IActionResult> ShowCountryConfig()
        {
            List<string> countries = new List<string>();

            countries = await apic.GetAllCountries();

            return View("CountryConfig", countries);
        }

        /// <summary>
        /// Opens the regional config view based on country
        /// </summary>
        /// <param name="country">String</param>
        /// <returns>View("RegionConfig", regions)</returns>
        public async Task<IActionResult> ShowRegionConfig(string country)
        {
            List<StateCountry> regions = new List<StateCountry>();

            regions = await apic.FindRegionsByCountry(country);

            return View("RegionConfig", regions);
        }

        /// <summary>
        /// Opens the regional create view based on country
        /// </summary>
        /// <param name="country">String</param>
        /// <returns>View("RegionCreate", newRegion)</returns>
        public IActionResult ShowRegionCreate(string country)
        {
            StateCountry newRegion = new StateCountry();

            newRegion.country = country;

            return View("RegionCreate", newRegion);
        }

        /// <summary>
        /// Creates a region
        /// </summary>
        /// <param name="region">StateCountry</param>
        /// <returns>RedirectToAction("ShowRegionConfig", new { country = region.country })</returns>
        public async Task<IActionResult> CreateRegion(StateCountry region)
        {
            await apic.CreateStateCountry(region);

            return RedirectToAction("ShowRegionConfig", new { country = region.country });
        }

        /// <summary>
        /// Deletes a region based on id
        /// </summary>
        /// <param name="regionId">String</param>
        /// <returns>RedirectToAction("ShowRegionConfig")</returns>
        public async Task<IActionResult> DeleteRegion(string regionId)
        {
            await apic.DeleteStateCountry(regionId);
            return RedirectToAction("ShowRegionConfig");
        }

        /// <summary>
        /// Opens the regional update view based on id
        /// </summary>
        /// <param name="regionId">String</param>
        /// <returns>View("RegionUpdate", region)</returns>
        public async Task<IActionResult> ShowRegionUpdate(string regionId)
        {
            StateCountry region = await apic.GetStateCountryById(regionId);
            return View("RegionUpdate", region);
        }

        /// <summary>
        /// Updates a region
        /// </summary>
        /// <param name="region">StateCountry</param>
        /// <returns>RedirectToAction("ShowRegionDetails", new { regionId = region.stateCountryId })</returns>
        public async Task<IActionResult> UpdateRegion(StateCountry region)
        {
            await apic.UpdateStateCountry(region);
            return RedirectToAction("ShowRegionDetails", new { regionId = region.stateCountryId });
        }

        /// <summary>
        /// Opens the regional details view based on id
        /// </summary>
        /// <param name="regionId">String</param>
        /// <returns>View("RegionDetails", region)</returns>
        public async Task<IActionResult> ShowRegionDetails(string regionId)
        {
            StateCountry region = new StateCountry();

            region = await apic.GetStateCountryById(regionId);

            return View("RegionDetails", region);
        }
    }
}
