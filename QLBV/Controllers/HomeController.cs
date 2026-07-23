using QLBV.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.Data.Entity.Core.Objects;

using System.Web.UI;
namespace QLBV.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        private QL_BENHVIENEntities db = new QL_BENHVIENEntities();
       
        public ActionResult Index()
        {
            ViewBag.ListKhoa = db.KHOAs.ToList();
            ViewBag.ListBacSi = db.BACSIs.ToList(); // nếu muốn load tất cả bác sĩ
            return View();
        }
        public JsonResult GetBacSiTheoKhoa(int makhoa)
        {
            var list = db.BACSIs
             .Where(b => b.KHOA_ID == makhoa)
             .OrderBy(b => b.TENBACSI)
             .Select(b => new {
                 b.BACSI_ID,
                 b.TENBACSI
             })
             .ToList();


            return Json(list, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult DatLichKham(int mabacsi, string ngaykham, int cakhams, string cccd, string hoten, string sdt, string gioitinh)
        {
            try
            {
                DateTime ngay = DateTime.ParseExact(ngaykham, "yyyy-MM-dd", null);
                if (ngay.Date < DateTime.Today)
                {
                    return Json(new { success = false, message = "Không thể đặt lịch vào ngày trong quá khứ!" });
                }

                // Check bác sĩ tồn tại
                var bacsi = db.BACSIs.FirstOrDefault(b => b.BACSI_ID == mabacsi);
                if (bacsi == null)
                    return Json(new { success = false, message = "Bác sĩ không tồn tại!" });

                // Check ca khám tồn tại
                if (!db.CAKHAM_MAPPING.Any(c => c.CAKHAM_ID == cakhams))
                    return Json(new { success = false, message = "Ca khám không hợp lệ!" });

                // Check bệnh nhân
                var bn = db.BENHNHANs.FirstOrDefault(b => b.CCCD == cccd);
                if (bn == null)
                {
                    bn = new BENHNHAN
                    {

                        HOTENBENHNHAN = string.IsNullOrEmpty(hoten) ? "Bệnh nhân mới" : hoten,
                        SDT = sdt ?? "",
                        GIOITINH = gioitinh ?? "",
                        BACSI_ID = mabacsi,
                        CCCD = cccd
                    };
                    db.BENHNHANs.Add(bn);

                    db.SaveChanges(); 
                }

                // Check trùng lịch
                bool exists = db.DATLICHKHAMs.Any(d =>
                    d.BACSI_ID == mabacsi &&
                    d.NGAYKHAM == ngay &&
                    d.CAKHAM_ID == cakhams
                );

                if (exists)
                    return Json(new { success = false, message = "LỖI: Bác sĩ đã có lịch vào ca/ngày này!" });

                // Tạo lịch khám
                var lich = new DATLICHKHAM
                {
                    BNHAN_ID = bn.BNHAN_ID,
                    BACSI_ID = mabacsi,
                    NGAYKHAM = ngay,
                    CAKHAM_ID = cakhams,
                    TRANGTHAI = "Đã đặt",
                    THOIGIANDAT = DateTime.Now
                };
                db.DATLICHKHAMs.Add(lich);
                db.SaveChanges();

                return Json(new { success = true, message = "Đăng ký thành công!" });
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException dbEx)
            {
                // Lấy inner exception
                var inner = dbEx.InnerException;
                while (inner?.InnerException != null)
                    inner = inner.InnerException;

                return Json(new { success = false, message = "Lỗi DB chi tiết: " + inner?.Message ?? dbEx.Message });
            }

            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }
        public ActionResult ChuyenKhoa()
        {
            List<KHOA> listKhoa = db.KHOAs.ToList();
            return View(listKhoa);
        }
        //public ActionResult ChuyenGia()
        //{
        //    List<BACSI> listBacsi = db.BACSIs.ToList();
        //    return View(listBacsi);
        //}
        public ActionResult DichVu()
        {
            List<DICHVU> listDichvu = db.DICHVUs.ToList();
            return View(listDichvu);
        }
        public ActionResult LocBSTheoKhoa(int id)
        {
            var list = db.BACSIs.Where(b => b.KHOA_ID == id).ToList();

            // Dropdown Khoa
            ViewBag.KHOA_ID = new SelectList(db.KHOAs, "KHOA_ID", "TENKHOA", id);

            // Dropdown Học hàm
            ViewBag.HOCHAM = new SelectList(
                db.BACSIs.Select(x => x.HOCHAM).Where(x => !string.IsNullOrEmpty(x)).Distinct()
            );

            // Dropdown Học vị
            ViewBag.HOCVI = new SelectList(
                db.BACSIs.Select(x => x.HOCVI).Where(x => !string.IsNullOrEmpty(x)).Distinct()
            );

            // Dropdown Chức vụ
            ViewBag.CHUCVU = new SelectList(
                db.BACSIs.Select(x => x.CHUCVU).Where(x => !string.IsNullOrEmpty(x)).Distinct()
            );

            return View("ChuyenGia", list);
        }




        //public JsonResult GetBacSiTheoTen(string term)
        //{
        //    // Gọi function SQL trả về danh sách bác sĩ đầy đủ
        //    var list = db.Database.SqlQuery<BACSI>(
        //        "SELECT * FROM TimBacSiTheoTen(@p0)", term
        //    ).ToList();

        //    // Chuyển sang object JSON đầy đủ thông tin
        //    var result = list.Select(b => new {
        //        id = b.BACSI_ID,
        //        value = b.TENBACSI,
        //        label = b.TENBACSI,
        //        AVATAR = b.AVATAR,
        //        HOCVI = b.HOCVI,
        //        HOCHAM = b.HOCHAM,
        //        CHUCVU = b.CHUCVU,
        //        SOBN = b.SOBN
        //    }).ToList();

        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}
        public JsonResult GetBacSiTheoTen(string term)
        {
            var ds = db.TimBacSiTheoTen(term ?? "").ToList();

            var result = ds.Select(b => new
            {
                id = b.BACSI_ID,
                value = b.TENBACSI,
                label = b.TENBACSI,
                avatar = b.AVATAR,
                hocvi = b.HOCVI,
                hocham = b.HOCHAM,
                chucvu = b.CHUCVU,
                sobn = b.SOBN
            }).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetKhoaTheoTen(string term)
        {
            term = term?.Trim();

            var ds = db.FN_TimKiemKhoa(term)
                       .Select(k => new
                       {
                           id = k.MAKHOA,
                           label = k.TENKHOA,
                           value = k.TENKHOA
                       })
                       .ToList();

            return Json(ds, JsonRequestBehavior.AllowGet);
        }



        public ActionResult TraCuu()
        {
            // Hiển thị form nhập CCCD và Mã bệnh nhân
            return View();
        }

        public ActionResult TraCuuKetQua(string cccd, string mabn)
        {
            // Loại bỏ khoảng trắng
            cccd = cccd?.Trim();
            mabn = mabn?.Trim();

            // Gọi TVF EF
            var ketqua = db.FN_XuatKetQuaDaKham_TheoBN(mabn, cccd)
                           .OrderByDescending(k => k.NGAYKHAM)
                           .ToList();

            ViewBag.KetQuaCount = ketqua.Count;

            return View(ketqua);
        }




        public ActionResult XemCTBS(int id)
        {
            var bacsi = db.BACSIs.FirstOrDefault(b => b.BACSI_ID == id);
            if (bacsi == null)
            {
                return HttpNotFound();
            }
            return View(bacsi);
        }





        public ActionResult ChuyenGia(int? khoaId, string hocHam, string hocVi, string chucVu)
        {
            var list = db.BACSIs.AsQueryable();

            if (khoaId.HasValue && khoaId.Value > 0)
                list = list.Where(x => x.KHOA_ID == khoaId.Value);

            if (!string.IsNullOrEmpty(hocHam) && hocHam != "Tất cả")
                list = list.Where(x => x.HOCHAM == hocHam);

            if (!string.IsNullOrEmpty(hocVi) && hocVi != "Tất cả")
                list = list.Where(x => x.HOCVI == hocVi);

            if (!string.IsNullOrEmpty(chucVu) && chucVu != "Tất cả")
                list = list.Where(x => x.CHUCVU == chucVu);

            ViewBag.KHOA_ID = new SelectList(db.KHOAs, "KHOA_ID", "TENKHOA", khoaId);
            ViewBag.HOCHAM = new SelectList(db.BACSIs.Select(x => x.HOCHAM).Where(x => !string.IsNullOrEmpty(x)).Distinct(), hocHam);
            ViewBag.HOCVI = new SelectList(db.BACSIs.Select(x => x.HOCVI).Where(x => !string.IsNullOrEmpty(x)).Distinct(), hocVi);
            ViewBag.CHUCVU = new SelectList(db.BACSIs.Select(x => x.CHUCVU).Where(x => !string.IsNullOrEmpty(x)).Distinct(), chucVu);

            return View(list.ToList());
        }

    }
}