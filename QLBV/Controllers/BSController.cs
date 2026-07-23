using QLBV.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using QLBV.Filters;

namespace QLBV.Controllers
{
    [BSAuth]
    public class BSController : Controller
    {
        // GET: BS
        private QL_BENHVIENEntities db = new QL_BENHVIENEntities();
        public ActionResult Index()
        {
            return RedirectToAction("LayID");
            
        }

        // Lấy ID bác sĩ từ session và redirect
        public ActionResult LayID()
        {
            if (Session["BacSiID"] != null)
            {
                int id = (int)Session["BacSiID"];
                return RedirectToAction("DanhSachBenhNhan", new { id = id });
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

        // Danh sách bệnh nhân của bác sĩ
        public ActionResult DanhSachBenhNhan(int? id) // id có thể null
        {
            if (id == null && Session["BacSiID"] != null)
            {
                id = (int)Session["BacSiID"];
            }
            if (id == null)
            {
                return RedirectToAction("Login", "Login");
            }

            var bacsi = db.BACSIs.Find(id);
            if (bacsi == null) return HttpNotFound();

            ViewBag.Title = "Bệnh nhân của tôi";
            ViewBag.BacSi = bacsi;
            ViewBag.ID_BS = id;

            // Lấy danh sách bệnh nhân có lịch hôm nay (tất cả trạng thái)
            var benhnhans = db.DATLICHKHAMs
             .Where(d =>
                 d.BACSI_ID == id &&
                 d.NGAYKHAM == DateTime.Today
             )
             .Select(d => d.BENHNHAN)
             .Distinct()
             .ToList();

            return View(benhnhans);

        }

        // Thông tin cá nhân bác sĩ
        public ActionResult ThongTinCaNhan()
        {
            int? id = Session["BacSiID"] as int?;
            if (id == null)
            {
                return RedirectToAction("Login", "Login");
            }

            var bacsi = db.BACSIs.Find(id);
            if (bacsi == null) return HttpNotFound();

            ViewBag.Title = "Thông tin cá nhân";
            return View(bacsi);
        }
        public ActionResult SuaBenhNhan(int id)
        {
            var bn = db.BENHNHANs.Find(id);
            if (bn == null) return HttpNotFound();

            ViewBag.Title = "Sửa thông tin bệnh nhân";
            return View(bn);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SuaBenhNhan(BENHNHAN model)
        {
            if (ModelState.IsValid)
            {
                var bn = db.BENHNHANs.Find(model.BNHAN_ID);
                if (bn == null) return HttpNotFound();

                bn.HOTENBENHNHAN = model.HOTENBENHNHAN;
                bn.SDT = model.SDT;
                bn.GIOITINH = model.GIOITINH;

                db.SaveChanges();
                return RedirectToAction("DanhSachBenhNhan");
            }

            return View(model);
        }
        public ActionResult KhamBenh(int datLichKhamId)
        {
            var lich = db.DATLICHKHAMs.Find(datLichKhamId);
            if (lich == null) return HttpNotFound();

            
            // Lấy hoặc tạo hóa đơn cho lịch khám này
            var hd = db.HOADONs.FirstOrDefault(h => h.DATLICHKHAM_ID == datLichKhamId);
            if (hd == null)
            {
                hd = new HOADON
                {
                    DATLICHKHAM_ID = datLichKhamId,
                    NGAYLAP = DateTime.Today,
                    TONGTIEN = 0,
                    HINHTHUCTHANHTOAN = "Chưa thanh toán"
                };
                db.HOADONs.Add(hd);
                db.SaveChanges();
            }

            // Lấy danh sách dịch vụ
            ViewBag.DichVus = db.DICHVUs.ToList();
            ViewBag.TongTien = hd.CHITIETHOADONs
                            .Select(c => (decimal?)c.THANHTIEN)
                            .Sum() ?? 0;
            return View(hd); // Truyền HOADON_ID để thêm dịch vụ
        }
        [HttpPost]
        public ActionResult ThemDichVu(int HD_ID, int DICHVU_ID, int SOLUONG)
        {
            var hd = db.HOADONs.Find(HD_ID);
            var dv = db.DICHVUs.Find(DICHVU_ID);
            if (hd != null && dv != null)
            {
                var cthd = db.CHITIETHOADONs
                             .FirstOrDefault(c => c.HOADON_ID == HD_ID && c.DICHVU_ID == DICHVU_ID);
                

                if (cthd != null)
                {
                    cthd.SOLUONG += SOLUONG;
                    cthd.THANHTIEN = cthd.SOLUONG * dv.DONGIA;
                    db.SaveChanges();
                }
                else
                {
                    cthd = new CHITIETHOADON
                    {
                        HOADON_ID = HD_ID,
                        DICHVU_ID = DICHVU_ID,
                        SOLUONG = SOLUONG,
                        THANHTIEN = dv.DONGIA * SOLUONG
                    };
                    db.CHITIETHOADONs.Add(cthd);
                    db.SaveChanges();
                }

                hd.TONGTIEN = db.CHITIETHOADONs
                         .Where(c => c.HOADON_ID == HD_ID)
                         .Sum(c => c.THANHTIEN);

                db.SaveChanges();
            }

            return RedirectToAction("KhamBenh", new { datLichKhamId = hd.DATLICHKHAM_ID });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult XoaDichVu(int cthdId)
        {
            var cthd = db.CHITIETHOADONs.Find(cthdId);
            if (cthd != null)
            {
                var hd = db.HOADONs.Find(cthd.HOADON_ID);
                if (hd != null)
                {
                    if (cthd.SOLUONG > 1)
                    {
                        cthd.SOLUONG -= 1;
                        cthd.THANHTIEN = cthd.SOLUONG * cthd.DICHVU.DONGIA;
                        db.SaveChanges();
                    }
                    else
                    {
                        db.CHITIETHOADONs.Remove(cthd);
                        db.SaveChanges();
                    }

                    hd.TONGTIEN = db.CHITIETHOADONs
                     .Where(c => c.HOADON_ID == hd.HD_ID)
                     .Select(c => (decimal?)c.THANHTIEN)
                     .Sum() ?? 0;

                    db.SaveChanges();
                }
                return RedirectToAction("KhamBenh", new { datLichKhamId = hd.DATLICHKHAM_ID });
            }

            return HttpNotFound();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult HoanTatKham(int datLichKhamId)
        {
            var lich = db.DATLICHKHAMs.Find(datLichKhamId);
            if (lich == null) return HttpNotFound();

            lich.TRANGTHAI = "Đã khám";
            db.SaveChanges();

            return RedirectToAction("DanhSachBenhNhan");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LuuKetQua(int datLichKhamId, string chuandoan, string ketluan, string thanhvien, string ghichu)
        {
            var lich = db.DATLICHKHAMs.Find(datLichKhamId);
            if (lich == null) return HttpNotFound();
            
            var kq = db.KETQUAKHAMs.FirstOrDefault(k => k.DATLICHKHAM_ID == datLichKhamId);
            if (kq == null)
            {
                kq = new KETQUAKHAM
                {
                    DATLICHKHAM_ID = datLichKhamId,
                    CHUANDOAN = chuandoan,
                    KETLUAN = ketluan,
                    THANHVIEN = Session["UserName"] != null ? Session["UserName"].ToString() : thanhvien,
                    GHICHU = ghichu
                };
                db.KETQUAKHAMs.Add(kq);
            }
            else
            {
                kq.CHUANDOAN = chuandoan;
                kq.KETLUAN = ketluan;
                kq.THANHVIEN = Session["UserName"] != null ? Session["UserName"].ToString() : thanhvien;
                kq.GHICHU = ghichu;
            }

            lich.TRANGTHAI = "Đã khám";
            db.SaveChanges();

            return RedirectToAction("DanhSachBenhNhan");
        }

        public ActionResult LichSapToi()
        {
            if (Session["BacSiID"] == null) return RedirectToAction("Login", "Login");
            int bacsiId = (int)Session["BacSiID"];

            var today = DateTime.Today;
            var nextWeek = today.AddDays(7);

            var lichList = db.DATLICHKHAMs
                .Where(d => d.BACSI_ID == bacsiId
                    && d.NGAYKHAM >= today
                    && d.NGAYKHAM <= nextWeek)
                .OrderBy(d => d.NGAYKHAM)
                .ToList();

            ViewBag.ID_BS = bacsiId;
            return View(lichList);
        }
    }
}