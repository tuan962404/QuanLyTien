using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace QuanLyTien.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        QuanLyTienEntities db = new QuanLyTienEntities();

        public ActionResult Index()
        {
            return View();
        }

        public bool CheckLogin(string user,string password)
        {
            var kq = db.User.Count(x => x.User1 == user && x.Password == password);
            if (kq > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public User GetUser(string user)
        {
            return db.User.SingleOrDefault(x=>x.User1 == user);
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(User mod)
        {
            var temp = "@*@!" + mod.Password + "#^%";
            var kq = CheckLogin(mod.User1, MD5Hash(temp));
            if (kq)
            {
                User userSession = new User();
                var user = GetUser(mod.User1);
                userSession.Id = user.Id;
                userSession.User1 = user.User1;
                userSession.Name = user.Name;

                Session.Add("USER_SESSION", userSession);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("", "Tài khoản hoặc mật khẩu không đúng!");
            }
            return View();
        }
        
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login","Account");
        }

        public string MD5Hash(string text)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(text));
            byte[] result = md5.Hash;
            StringBuilder strBuilder = new StringBuilder();
            for(int i=0; i< result.Length; i++)
            {
                strBuilder.Append(result[i].ToString("x2"));
            }
            return strBuilder.ToString();
        }
    }
}