using System;
using System.Linq;
using System.Web.Mvc;
using tzskSite.Models;
using tzskSite_v1;
using tzskSitedb.lib.Entity;

namespace tzskSite.Controllers
{
    public class AccountController : Controller
    {
        tzskSiteEntities dbContext;

        public AccountController()
        {
            dbContext = new tzskSiteEntities();
        }

        //
        // GET: /Account/Login
        public ActionResult Login()
        {
            if (Session["id"] != null)
                Session.Clear();

            return View();
        }

        //
        // GET: /Account/Register       
        public ActionResult Register()
        {
            if ((bool)Session["isAdmin"])
                return View();
            else
            {
                Session.Clear();
                return View("Login");
            }
        }

        //
        // GET: /Accoiunt/Message
        public ActionResult Message()
        {
            if (Session["Message"] == null)
                return View("Login");
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public RedirectResult Login(LoginViewModel model)
        {
            var user = dbContext.tbUsers.FirstOrDefault(l => (l.cLogin == model.Login) && (l.cPassword == model.Password));
            if (user != null)
            {
                Session["id"] = user.id;
                Session["name"] = user.cLogin;
                Session["isAdmin"] = user.cIsAdmin;
                Logger.user("User '" + user.cLogin + "' login [time - " + DateTime.Now + "] + info user: id - " + user.id + ", isAdmin - " + user.cIsAdmin + ", IP - " + HttpContext.Request.UserHostAddress + ", browser - " + HttpContext.Request.Browser.Browser);
            }
            else
                Logger.user("Попытка несанкционированного входа, время - [" + DateTime.Now + "], с данными: логин - " + model.Login + ", пароль - " + model.Password + ", IP - " + HttpContext.Request.UserHostAddress + ", browser - " + HttpContext.Request.Browser.Browser);               

            return Redirect("/Home/Index");
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public RedirectResult Register(RegisterViewModel model)
        {
            if ((bool)Session["isAdmin"])
            {
                Session["Title"] = "Регистрация.";
                var user = dbContext.tbUsers.FirstOrDefault(l => (l.cLogin == model.Login) || (l.id == model.id));
                if (user != null)
                {
                    Session["Message"] = "Такой замерщик уже зарегистрирован.";
                    Session["nameLink"] = "Регистрация";
                    Session["nameView"] = "Register";
                    Session["nameController"] = "Account";
                    return Redirect("/Account/Message"); 
                }

                tbUsers tbUser = new tbUsers();
                tbUser.id = model.id;
                tbUser.cLogin = model.Login;
                tbUser.cPassword = model.Password;
                tbUser.cIsAdmin = model.isAdmin;

                dbContext.tbUsers.Add(tbUser);
                dbContext.SaveChanges();
            }

            Session["Message"] = "Регистрация прошла успешно.";
            Session["nameLink"] = "На Г0лавную";
            Session["nameView"] = "Index";
            Session["nameController"] = "Home";
            return Redirect("/Account/Message");
        }

        public RedirectResult Exit ()
        {
            Session["id"] = null;
            return Redirect("/Account/Login");
        }
    }
}