using System;
using System.Linq;
using System.Web.Mvc;
using tzskSite.Models;
using tzskSite_v1;
using tzskSite_v1.Models;
using tzskSitedb.lib.Entity;

namespace tzskSite.Controllers
{
    public class AccountController : Controller
    {
        tzskSiteEntities dbContext;
        const int _countLogin = 3;
        const int _minuteBan = 10;
        public AccountController()
        {
            dbContext = new tzskSiteEntities();
        }

        public ActionResult hydra ()
        {
            return View();
        }

        //
        // GET: /Account/Login
        public ActionResult Login()
        {
            if (isBlock() != null)
                return RedirectToAction("Message", "Msg", isBlock());

            if (Session["id"] != null)
                Session.Clear();

            return View();
        }

        //
        // GET: /Account/Register       
        public ActionResult Register()
        {
            if (Session["id"] != null && (bool)Session["isAdmin"])
                return View();
            else
            {
                Session.Clear();
                return View("Login");
            }
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public RedirectToRouteResult Login(LoginViewModel model)
        {
            if (isBlock() != null)
                return RedirectToAction("Message", "Msg", isBlock());

            var user = dbContext.tbUsers.FirstOrDefault(l => (l.cLogin == model.Login) && (l.cPassword == model.Password));
            if (user != null)
            {
                user.сCountFailedLogin = 0;
                dbContext.SaveChanges();

                Session["id"] = user.id;
                Session["name"] = user.cLogin;
                Session["isAdmin"] = user.cIsAdmin;
                Logger.user("Пользователь '" + user.cLogin + "' залогинился в [time - " + DateTime.Now + "], с данными: id - " + user.id + ", isAdmin - " + user.cIsAdmin + ", IP - " + _get_IP_Client());
            }
            else
            {
                Logger.user("Попытка несанкционированного входа, время - [" + DateTime.Now + "], с данными: логин - " + model.Login + ", пароль - " + model.Password + ", IP - " + _get_IP_Client());

                user = dbContext.tbUsers.FirstOrDefault(l => l.cLogin == model.Login);
                if(user != null)
                {
                    if (user.сCountFailedLogin == null)
                        user.сCountFailedLogin = 0;

                    user.сCountFailedLogin++;
                    dbContext.SaveChanges();

                    if(user.сCountFailedLogin > _countLogin)
                    {
                        user.сCountFailedLogin = 0;
                        dbContext.tbBans.Add(new tbBans {
                            cIPaddress = _get_IP_Client(),
                            cDateBan = DateTime.Now + TimeSpan.FromMinutes(_minuteBan) });                    
                        dbContext.SaveChanges();
                    }
                }
            }

            return RedirectToAction("Index", "Home", null);
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public RedirectToRouteResult Register(RegisterViewModel model)
        {
            if ((bool)Session["isAdmin"])
            {
                var user = dbContext.tbUsers.FirstOrDefault(l => (l.cLogin == model.Login) || (l.id == model.id));

                if (user != null)
                    return RedirectToAction("Message", "Msg", new MsgViewModel
                    {
                        Title = "Ошибка Регистрации.",
                        Message = "Такой замерщик уже существует",
                        nameLink = "Регистрация",
                        nameView = "Register",
                        nameController = "Account"
                    });

                dbContext.tbUsers.Add(new tbUsers {
                    id = model.id,
                    cLogin = model.Login,
                    cPassword = model.Password,
                    cIsAdmin = model.isAdmin});

                dbContext.SaveChanges();

                return RedirectToAction("Message", "Msg", new MsgViewModel
                {
                    Title = "Регистрация прошла успешно.",
                    Message = "Замерщик успешно зарегистрирован",
                    nameLink = "На Главную",
                    nameView = "Index",
                    nameController = "Home"
                });
            }
            else
                return RedirectToAction("Index", "Home", null);
        }

        public RedirectResult Exit ()
        {
            Session.Clear();
            return Redirect("/Account/Login");
        }

        private string _get_IP_Client()
        {
            string _ip = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (!string.IsNullOrEmpty(_ip))
            {
                string[] ipRange = _ip.Split(',');
                int le = ipRange.Length - 1;
                string trueIP = ipRange[le];
            }
            else          
                _ip = Request.ServerVariables["REMOTE_ADDR"];
                       
            return _ip;
        }

        private MsgViewModel isBlock ()
        {
            //FIXME 
            string _ip = _get_IP_Client();
            DateTime DateTimeBan = DateTime.Now;
            var isUserBan = dbContext.tbBans.FirstOrDefault(l => (l.cIPaddress == _ip) && (l.cDateBan >= DateTimeBan));
            if (isUserBan != null)
            {
                MsgViewModel msg = new MsgViewModel();
                msg.Title = "Блокировка.";
                msg.Message = "Произошло несколько неудачных попыток входа с этой учетной записи или IP-адреса. Подождите " + ((isUserBan.cDateBan - DateTime.Now).Minutes + 1) + " Мин. и повторите попытку.";
                msg.nameLink = "Повторить";
                msg.nameView = "Login";
                msg.nameController = "Account";
                return msg;
            }
            return null;           
        }
    }
}