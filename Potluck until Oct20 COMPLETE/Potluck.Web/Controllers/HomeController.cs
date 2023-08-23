using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Potluck.Web.Models;
using System.Net.Http;
using Potluck.Web.Infrastructure;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Potluck.Web.Controllers
{
    /// <summary>
    /// HomeController handles any publicly accessible views such as the home page and login
    /// </summary>
    public class HomeController : Controller
    {
        private APIController apic = new APIController();

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            PreLoadViewBag();
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
        /// Returns the landing page for the app
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Returns the sign up view
        /// </summary>
        /// <returns></returns>
        public IActionResult ShowSignUp()
        {
            return View("SignUp");
        }

        /// <summary>
        /// Handles a sign up request and redirects to appropriate page
        /// </summary>
        /// <param name="signUp">SignUpRequest</param>
        /// <returns></returns>
        public async Task<IActionResult> SignUpUser(SignUpRequest signUp)
        {
            if(await apic.SignUpUser(signUp))
            {
                ViewBag.SignupSuccess = true;
                return View("Login");
            }
            else
            {
                ViewBag.SignupSuccess = false;
                return View("SignUp");
            }           
        }

        /// <summary>
        /// Handles the navigation flow when the profile icon is clicked
        /// If the user has an existing session they redirected to their profile
        /// if not they are redirected to the login screen
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Login()
        {
            string currentRole = HttpContext.Session.GetJson<string>("CurrentRole");
            string token = HttpContext.Session.GetJson<string>("Token");
            apic.Token = token;
            UserDTO user = await apic.GetCurrentUser();
            if (user != null && currentRole != null)
            {
                return RedirectToAction("Index", "Profile");
            }
            else if (user != null && currentRole == null)
            {
                HttpContext.Session.SetJson("CurrentRole", "ROLE_USER");
                return RedirectToAction("Index", "Profile");
            }
            return View("Login");
        }

        /// <summary>
        /// Handles a login request and redirect to appropriate view
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        public async Task<IActionResult> LoginUser(LoginRequest login)
        {
            string token = await apic.LoginUser(login);

            HttpContext.Session.SetJson("Token", token);

            apic.Token = token;

            UserDTO user = await apic.GetCurrentUser();

            HttpContext.Session.SetJson("CurrentUser", user);

            if (user != null && user.roles.Count == 1)
            {
                HttpContext.Session.SetJson("CurrentRole", user.roles.ElementAt(0).authority);
                return RedirectToAction("Index", "Profile");
            }
            else if (user != null && user.roles.Count > 1)
            {
                return View("LoginRoleSelect", user.roles);
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        /// <summary>
        /// Very similar to LoginUser with the exception that google or facebook handles the authentication
        /// </summary>
        /// <param name="token">string</param>
        /// <param name="error">string</param>
        /// <returns></returns>
        public async Task<IActionResult> Oauth2(string token, string error)
        {
            HttpContext.Session.SetJson("Token", token);

            apic.Token = token;

            UserDTO user = await apic.GetCurrentUser();

            if (user != null && user.roles.Count == 1)
            {
                HttpContext.Session.SetJson("CurrentRole", user.roles.ElementAt(0).authority);
                return RedirectToAction("Index", "Profile");
            }
            else if (user != null && user.roles.Count > 1)
            {
                return View("LoginRoleSelect", user.roles);
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        /// <summary>
        /// Selects a role, if the user has more than one, that they wish to log in as
        /// </summary>
        /// <param name="authority">String</param>
        /// <returns>RedirectToAction("Index", "Profile")</returns>
        public IActionResult SelectRole(string authority)
        {
            HttpContext.Session.SetJson("CurrentRole", authority);
            return RedirectToAction("Index", "Profile");
        }


        public IActionResult ShowTermsOfService()
        {
            return View("TOS");
        }

        public IActionResult ShowPrivacyPolicy()
        {
            return View("Privacy");
        }

    }
}
