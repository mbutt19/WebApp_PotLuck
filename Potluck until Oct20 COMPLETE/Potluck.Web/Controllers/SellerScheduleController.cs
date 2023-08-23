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
    /// Contains actions for managing a sellers schedule
    /// </summary>
    public class SellerScheduleController : Controller
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
        /// 
        /// </summary>
        /// <returns>View("Index", pageOfSchedules)</returns>
        public async Task<IActionResult> Index()
        {
            UserDTO user = await apic.GetCurrentUser();
            PageOfSchedules pageOfSchedules = await apic.GetAllSchedulesByEmail(user.email);
            return View("Index", pageOfSchedules);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scheduleId">String</param>
        /// <returns>View("ScheduleDetails", schedule)</returns>
        public async Task<IActionResult> ShowScheduleDetails(string scheduleId)
        {
            ScheduleDTO schedule = await apic.GetScheduleById(scheduleId);

            UserDTO user = await apic.GetCurrentUser();

            PageOfItems pageOfItems = await apic.GetMenuByUserId(user.userId, 0, 25);
            
            ViewBag.SellerItems = pageOfItems.content;

            return View("ScheduleDetails", schedule);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>View("ScheduleCreate", schedule)</returns>
        public async Task<IActionResult> ShowScheduleCreate()
        {
            ScheduleCreate schedule = new ScheduleCreate();

            UserDTO user = await apic.GetCurrentUser();

            PageOfItems pageOfItems = await apic.GetMenuByUserId(user.userId, 0, 25);

            ViewBag.Items = pageOfItems.content;

            return View("ScheduleCreate", schedule);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="schedule">ScheduleCreate</param>
        /// <returns>RedirectToAction("ShowScheduleDetails", new { scheduleId = scheduleId })</returns>
        public async Task<IActionResult> CreateSchedule(ScheduleCreate schedule)
        {
            ScheduleVO newSchedule = new ScheduleVO(schedule);

            string scheduleId = await apic.CreateSchedule(newSchedule);

            return RedirectToAction("ShowScheduleDetails", new { scheduleId = scheduleId });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scheduleId">String</param>
        /// <returns>RedirectToAction("Index")</returns>
        public async Task<IActionResult> DeleteSchedule(string scheduleId)
        {
            await apic.DeleteSchedule(scheduleId);

            return RedirectToAction("Index");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="schedule">ScheduleDTO</param>
        /// <returns>RedirectToAction("ShowScheduleDetails", new { scheduleId = schedule.scheduleId })</returns>
        public async Task<IActionResult> UpdateDateTime(ScheduleDTO schedule)
        {
            ScheduleDTO oldSchedule = await apic.GetScheduleById(schedule.scheduleId);

            schedule.itemList = oldSchedule.itemList;
            schedule.user = oldSchedule.user;

            ScheduleVO updatedSchedule = new ScheduleVO(schedule);

            await apic.UpdateSchedule(updatedSchedule);

            return RedirectToAction("ShowScheduleDetails", new { scheduleId = schedule.scheduleId });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scheduleId">String</param>
        /// <param name="itemId">String</param>
        /// <returns>RedirectToAction("ShowScheduleDetails", new { scheduleId = scheduleId })</returns>
        public async Task<IActionResult> AddItemToSchedule(string scheduleId, string itemId)
        {
            ScheduleDTO oldSchedule = await apic.GetScheduleById(scheduleId);

            ScheduleVO updatedSchedule = new ScheduleVO(oldSchedule);

            updatedSchedule.itemList.Add(new SubItemVO(itemId));

            await apic.UpdateSchedule(updatedSchedule);

            return RedirectToAction("ShowScheduleDetails", new { scheduleId = scheduleId });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scheduleId">String</param>
        /// <param name="itemId">String</param>
        /// <returns>RedirectToAction("ShowScheduleDetails", new { scheduleId = scheduleId })</returns>
        public async Task<IActionResult> RemoveItemFromSchedule(string scheduleId, string itemId)
        {
            ScheduleDTO oldSchedule = await apic.GetScheduleById(scheduleId);

            ScheduleVO updatedSchedule = new ScheduleVO(oldSchedule);

            int index = updatedSchedule.itemList.FindIndex(0,updatedSchedule.itemList.Count, i => i.itemId == itemId);

            updatedSchedule.itemList.RemoveAt(index);

            await apic.UpdateSchedule(updatedSchedule);

            return RedirectToAction("ShowScheduleDetails", new { scheduleId = scheduleId });
        }
    }
}
