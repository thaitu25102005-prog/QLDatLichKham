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

        public JsonResult BieuDoLichKhamTheoThang()
        {
            int year = DateTime.Now.Year;
            var data = Enumerable.Range(1, 12).Select(m =>
                db.DATLICHKHAMs.Count(d => d.NGAYKHAM.HasValue &&
                    d.NGAYKHAM.Value.Year == year &&
                    d.NGAYKHAM.Value.Month == m)
            ).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult BieuDoBenhNhanTheoKhoa()
        {
            var data = db.KHOAs.Select(k => new {
                tenKhoa = k.TENKHOA,
                soLuong = k.BACSIs.SelectMany(b => b.BENHNHANs).Count()
            }).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Top5BacSi()
        {
            var data = db.BACSIs
                .Select(b => new {
                    ten = b.TENBACSI,
                    soLich = b.DATLICHKHAMs.Count()
                })
                .OrderByDescending(x => x.soLich)
                .Take(5)
                .ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}