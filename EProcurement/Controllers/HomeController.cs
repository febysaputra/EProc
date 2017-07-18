using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EProcurement.Areas.Procurement.Models;

namespace EProcurement.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            if (Session["mUserName"] != null && Session["mUserName"].ToString() != "")
            {
                //MenuModel mdl = new MenuModel();
                //ViewBag.Menu = mdl.get_group_menu2(Convert.ToInt32(Session["mIdUserGroup"]), 1, Convert.ToInt32(Session["mIdUser"]));
            }

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
