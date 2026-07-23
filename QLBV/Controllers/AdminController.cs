using QLBV.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using QLBV.Filters;

namespace QLBV.Controllers
{
    [AdminAuth]
    public class AdminController : Controller
    {
        QL_BENHVIENEntities db = new QL_BENHVIENEntities();
        // GET: Admin
        public ActionResult Index()
        {   
            return View();
        }
        public ActionResult LoadBacSi()
        {
            return View(db.BACSIs.ToList());
        }
        //[HttpPost]
        //public ActionResult CreateBS(BACSI model)
        //{
        //    db.Database.ExecuteSqlCommand(
        //    "EXEC PROC_CRUD_BACSI @ACTION = {0}, @TENBACSI = {1}, @GIOITINH = {2}, @NAMSINH = {3}, @KHOA_ID = {4}, @SOBN = {5}, @HOCHAM = {6}, @HOCVI = {7}, @CHUCVU = {8}",
        //    "ADD", model.TENBACSI, model.GIOITINH, model.NAMSINH, model.KHOA_ID, model.SOBN, model.HOCHAM, model.HOCVI, model.CHUCVU
        //    );
        //    return RedirectToAction("Index");
        //}

        //[HttpPost]
        //public ActionResult EditBS(BACSI model)
        //{
        //    db.Database.ExecuteSqlCommand(
        //        "EXEC PROC_CRUD_BACSI @ACTION = {0}, @ID = {1}, @TENBACSI = {2}, @GIOITINH = {3}, @NAMSINH = {4}, @KHOA_ID = {5}, @SOBN = {6}, @HOCHAM = {7}, @HOCVI = {8}, @CHUCVU = {9}",
        //        "UPDATE", model.BACSI_ID, model.TENBACSI, model.GIOITINH, model.NAMSINH, model.KHOA_ID, model.SOBN, model.HOCHAM, model.HOCVI, model.CHUCVU
        //    );
        //    return RedirectToAction("Index");
        //}
        //public ActionResult DeleteBS(int id)
        //{
        //    db.Database.ExecuteSqlCommand(
        //        "EXEC PROC_CRUD_BACSI @ACTION = {0}, @ID = {1}",
        //        "DELETE", id
        //    );
        //    return RedirectToAction("Index");
        //}
        public ActionResult CreateBS()
        {
            ViewBag.BS = new SelectList(db.BACSIs.ToList(), "TENBACSI", "KHOA_ID");
            ViewBag.HocHamList = new List<SelectListItem>
            {
                new SelectListItem { Text = "ThS", Value = "ThS" },
                new SelectListItem { Text = "TS", Value = "TS" },
                new SelectListItem { Text = "PGS", Value = "PGS" },
                new SelectListItem { Text = "GS", Value = "GS" }
            };
            ViewBag.HocViList = new List<SelectListItem>
            {
                new SelectListItem { Text = "Bác sĩ CKI", Value = "Bác sĩ CKI" },
                new SelectListItem { Text = "Bác sĩ CKII", Value = "Bác sĩ CKII" },
                new SelectListItem { Text = "Bác sĩ Nội trú", Value = "Bác sĩ Nội trú" }
            };
            ViewBag.ChucVuList = new List<SelectListItem>
            {
                new SelectListItem { Text = "Trưởng khoa", Value = "Trưởng khoa" },
                new SelectListItem { Text = "Phó khoa", Value = "Phó khoa" },
                new SelectListItem { Text = "Bác sĩ", Value = "Bác sĩ" }
            };
            return View();
        }

        public ActionResult CreateOnSubmit(BACSI bsMoi, HttpPostedFileBase Image)
        {
            if (ModelState.IsValid)
            {
                string FileName = "";
                string Dir = "/images/";
                if (Image != null && Image.ContentLength > 0)
                {
                    string ext = Path.GetExtension(Image.FileName).ToLower();
                    if (ext == ".jpg" || ext == ".png" || ext == ".jpeg" || ext == ".gif")
                    {
                        FileName = Path.GetFileName(Image.FileName);
                        string physicalDir = Server.MapPath(Dir);
                        if (!Directory.Exists(physicalDir))
                        {
                            Directory.CreateDirectory(physicalDir);
                        }
                        string path = Path.Combine(Server.MapPath(Dir), FileName);
                        Image.SaveAs(path);
                        bsMoi.AVATAR = Dir + FileName;
                    }
                }

                db.BACSIs.Add(bsMoi);
                db.SaveChanges();
            }
            return RedirectToAction("LoadBacSi", "Admin");

        }

        public ActionResult EditBS(string id)
        {
            ViewBag.BS = new SelectList(db.BACSIs.ToList(), "TENBACSI", "KHOA_ID");
            BACSI item = db.BACSIs.FirstOrDefault(x => x.MABACSI == id);
            ViewBag.HocHamList = new List<SelectListItem>
            {
                new SelectListItem { Text = "ThS", Value = "ThS" },
                new SelectListItem { Text = "TS", Value = "TS" },
                new SelectListItem { Text = "PGS", Value = "PGS" },
                new SelectListItem { Text = "GS", Value = "GS" }
            };
            ViewBag.HocViList = new List<SelectListItem>
            {
                new SelectListItem { Text = "Bác sĩ CKI", Value = "Bác sĩ CKI" },
                new SelectListItem { Text = "Bác sĩ CKII", Value = "Bác sĩ CKII" },
                new SelectListItem { Text = "Bác sĩ Nội trú", Value = "Bác sĩ Nội trú" }
            };
            ViewBag.ChucVuList = new List<SelectListItem>
            {
                new SelectListItem { Text = "Trưởng khoa", Value = "Trưởng khoa" },
                new SelectListItem { Text = "Phó khoa", Value = "Phó khoa" },
                new SelectListItem { Text = "Bác sĩ", Value = "Bác sĩ" }
            };
            return View(item);
        }

        public ActionResult EditOnSubmit(BACSI bsMoi, HttpPostedFileBase Image)
        {
            BACSI selectedNew = db.BACSIs.FirstOrDefault(x => x.MABACSI == bsMoi.MABACSI);
            if (ModelState.IsValid)
            {
                string FileName = "";
                string Dir = "/images/";
                if (Image != null && Image.ContentLength > 0)
                {
                    string ext = Path.GetExtension(Image.FileName).ToLower();
                    if (ext == ".jpg" || ext == ".png" || ext == ".jpeg" || ext == ".gif")
                    {
                        FileName = Path.GetFileName(Image.FileName);
                        string physicalDir = Server.MapPath(Dir);
                        if (!Directory.Exists(physicalDir))
                        {
                            Directory.CreateDirectory(physicalDir);
                        }
                        string path = Path.Combine(Server.MapPath(Dir), FileName);
                        Image.SaveAs(path);
                        bsMoi.AVATAR = Dir + FileName;
                    }
                    else
                    {
                        bsMoi.AVATAR = selectedNew.AVATAR;
                    }
                }
                else
                {
                    bsMoi.AVATAR = selectedNew.AVATAR;
                }
                selectedNew.MABACSI = bsMoi.MABACSI;
                selectedNew.TENBACSI = bsMoi.TENBACSI;
                selectedNew.GIOITINH = bsMoi.GIOITINH;
                selectedNew.NAMSINH = bsMoi.NAMSINH;
                selectedNew.KHOA_ID = bsMoi.KHOA_ID;
                selectedNew.SOBN = bsMoi.SOBN;
                selectedNew.HOCHAM = bsMoi.HOCHAM;
                selectedNew.HOCVI = bsMoi.HOCVI;
                selectedNew.CHUCVU = bsMoi.CHUCVU;
                selectedNew.AVATAR = bsMoi.AVATAR;
                db.Entry(selectedNew).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();//Luu xuong DB
            }
            return RedirectToAction("LoadBacSi", "Admin");
        }

        public ActionResult Delete(string id)
        {
            BACSI bs = db.BACSIs.FirstOrDefault(x => x.MABACSI == id);
            return View(bs);
        }

        public ActionResult DeleteOnSubmit(BACSI bsXoa)
        {
            if (ModelState.IsValid)
            {
                BACSI item = db.BACSIs.FirstOrDefault(t => t.MABACSI == bsXoa.MABACSI);
                db.BACSIs.Remove(item);
                db.SaveChanges();
            }
            return RedirectToAction("LoadBacSi");

        }

        public ActionResult LoadDichVu()
        {
            return View(db.DICHVUs.ToList());
        }

        public ActionResult CreateDV()
        {
            ViewBag.DV = new SelectList(db.DICHVUs.ToList(), "TENDV", "MADV");
            
            return View();
        }
        public ActionResult CreateDVOnSubmit(DICHVU dvMoi)
        {
            if (ModelState.IsValid)
            {

                db.DICHVUs.Add(dvMoi);
                db.SaveChanges();
            }
            return RedirectToAction("LoadDichVu", "Admin");
        }
        public ActionResult EditDV(string id)
        {
            ViewBag.DV = new SelectList(db.DICHVUs.ToList(), "TENDV", "MADV");
            DICHVU item = db.DICHVUs.FirstOrDefault(x => x.MADV == id);
            
            return View(item);
        }
        public ActionResult EditDVOnSubmit(DICHVU dvMoi)
        {
            DICHVU selectedNew = db.DICHVUs.FirstOrDefault(x => x.MADV == dvMoi.MADV);
            if (ModelState.IsValid)
            {
                
                selectedNew.TENDV = dvMoi.TENDV;
                selectedNew.DONGIA = dvMoi.DONGIA;
                selectedNew.GHICHU = dvMoi.GHICHU;
                selectedNew.KHOA_ID = dvMoi.KHOA_ID;
                db.Entry(selectedNew).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();//Luu xuong DB
            }
            return RedirectToAction("LoadDichVu", "Admin");
        }
        public ActionResult DeleteDV(string id)
        {
            DICHVU dv = db.DICHVUs.FirstOrDefault(x => x.MADV == id);
            return View(dv);
        }

        public ActionResult DeleteDVOnSubmit(DICHVU dvXoa)
        {
            if (ModelState.IsValid)
            {
                DICHVU item = db.DICHVUs.FirstOrDefault(t => t.MADV == dvXoa.MADV);
                db.DICHVUs.Remove(item);
                db.SaveChanges();
            }
            return RedirectToAction("LoadDichVu");

        }
        public ActionResult QuanLyHoaDon()
        {
            var hoadons = db.HOADONs
                            .Where(h => h.HINHTHUCTHANHTOAN == "Chưa thanh toán")
                            .OrderBy(h => h.NGAYLAP)
                            .ToList();
            return View(hoadons);
        }

        [HttpPost]
        public ActionResult XacNhanThanhToan(int id)
        {
            var hd = db.HOADONs.FirstOrDefault(h => h.HD_ID == id);
            if (hd != null)
            {
                hd.HINHTHUCTHANHTOAN = "Đã thanh toán";
                db.SaveChanges();
            }
            return RedirectToAction("QuanLyHoaDon");
        }

        public ActionResult InHoaDon(int id)
        {
            var hd = db.HOADONs.FirstOrDefault(h => h.HD_ID == id);
            if (hd == null) return HttpNotFound();
            return View(hd);
        }
    }
}