using DoAnCuoiKy.Models;
using QLBV.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DoAnCuoiKy.Controllers
{
    public class DashBoardController : Controller
    {
        // GET: DoanhThu
        QL_BENHVIENEntities db = new QL_BENHVIENEntities();
        ThongKeItem dh = new ThongKeItem();
        public ActionResult DoanhThuNgay()
        {
            return Content(String.Format("{0:N0} VNĐ", dh.DoanhThuNgay()));
        }
        public ActionResult DoanhThuThang()
        {
            return Content(String.Format("{0:N0} VNĐ", dh.DoanhThuThang()));
        }
        public JsonResult BieuDoDoanhThuThang()
        {
            return Json(dh.DoanhThuNam(), JsonRequestBehavior.AllowGet);
        }
        public ActionResult SoLuongBS()
        {
            return Content(String.Format("{0} Y - Bác Sĩ", dh.SoLuongBS()));
        }
        public ActionResult LichKhamHomNay()
        {
            return Content(String.Format("{0} Lịch Khám", dh.SoLichKhamHomNay()));
        }
        public ActionResult SoBenhNhanHomNay()
        {
            return Content(String.Format("{0} Bệnh Nhân", dh.SoBenhNhanHomNay()));
        }
        public ActionResult DichVuSuDungNhieu()
        {
            return View(dh.DVSuDung());
        }
        
    }
}