using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DotNetOpenAuth.AspNet;
using Microsoft.Web.WebPages.OAuth;
using WebMatrix.WebData;
using EProcurement.Filters;
using EProcurement.Models;
using EProcurement.Areas.Procurement.Models;

namespace EProcurement.Controllers
{
   // [Authorize]
   // [InitializeSimpleMembership]
    public class AccountController : Controller
    {
        public ActionResult Login()
        {
            if (Session["mUserName"] != null && Session["mUserName"].ToString() != "")
            {
                //MenuModel mdl = new MenuModel();
                //ViewBag.Menu = mdl.get_group_menu2(Convert.ToInt32(Session["mIdUserGroup"]), 1, Convert.ToInt32(Session["mIdUser"]));
            }

            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.CheckMembership(model.Username, model.Password))
                {
                    MemberModel mbr = new MemberModel();
                    mbr = mbr.get_member_detail(model.Username);

                    Session["Username"] = model.Username;
                    Session["IdUser"] = mbr.IdUser;
                    Session["IdUserGroup"] = mbr.Id_group;
                    //cek sessionlog


                    return RedirectToAction("About", "Home"); //ini ganti nanti
                }
                else
                {
                    ModelState.AddModelError("Username", "Username/Password yang Anda masukkan salah");
                }
            }

            return View();
        }
    }   
}
