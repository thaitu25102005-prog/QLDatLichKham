using QLBV.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QLBV.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        private QL_BENHVIENEntities db = new QL_BENHVIENEntities();
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(string name, string pass)
        {
            if (pass != "123")
            {
                ViewBag.Error = "Sai mật khẩu!";
                return View();
            }

            // Nếu là Admin
            if (name == "Admin")
            {
                Session["UserName"] = name;
                Session["Role"] = "Admin";
                return RedirectToAction("Index", "Admin");
            }

            // Nếu là bác sĩ
            var bacsi = db.BACSIs.FirstOrDefault(b => b.MABACSI == name);
            if (bacsi != null)
            {
                Session["UserName"] = bacsi.TENBACSI;
                Session["BacSiID"] = bacsi.BACSI_ID;
                Session["Role"] = "BS";
                Session["BacSiAvatar"]=bacsi.AVATAR;
                Session["BacSiChucVu"]=bacsi.CHUCVU;
                return RedirectToAction("Index", "BS"); 
            }

            ViewBag.Error = "Không tìm thấy bác sĩ!";
            return View();
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login");
        }

    }
}