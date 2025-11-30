using DoAnCuoiKy.Models;
using QLBV.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace DoAnCuoiKy.Models
{
    public class ThongKeItem
    {
        QL_BENHVIENEntities db = new QL_BENHVIENEntities();
        public List<HOADON> listhd = new List<HOADON>();
        public List<BACSI> listbs = new List<BACSI>();
        public List<DATLICHKHAM> listdl = new List<DATLICHKHAM>();
        public ThongKeItem()
        {
            listhd = db.HOADONs.ToList();
            listbs = db.BACSIs.ToList();
            listdl = db.DATLICHKHAMs.ToList();
        }
        public decimal? DoanhThuNgay()
        {
            return listhd.Where(x => x.NGAYLAP.Value.Date == DateTime.Today).Sum(x => x.TONGTIEN);
        }
        public decimal? DoanhThuThang()
        {
            return listhd.Where(x => x.NGAYLAP.Value.Month == DateTime.Now.Month && x.NGAYLAP.Value.Year == DateTime.Now.Year).Sum(x => x.TONGTIEN);
        }
        public List<decimal> DoanhThuNam()
        {
            List<decimal> doanhThu = new List<decimal>();
            for (int i = 1; i <= 12; i++)
            {
                decimal dt = listhd.Where(x => x.NGAYLAP.HasValue && x.NGAYLAP.Value.Month == i && x.NGAYLAP.Value.Year == DateTime.Now.Year).Sum(x => x.TONGTIEN) ?? 0;
                doanhThu.Add(dt);
            }
            return doanhThu;
        }
        public int SoLuongBS()
        {
            return listbs.Count();
        }
        public int SoLichKhamHomNay()
        {
            return listdl.Where(x => x.NGAYKHAM.Value.Date == DateTime.Today).Count();
        }
        public int SoBenhNhanHomNay()
        {
            return listdl.Where(x => x.NGAYKHAM.Value.Date == DateTime.Today).Select(x => x.BNHAN_ID).Distinct().Count();
        }

        public List<ThongKeDV> DVSuDung()
        {
            var DVsudungnhieu = db.Database.SqlQuery<ThongKeDV>("SP_DICHVUSUDUNGNHIEU").ToList();
            return DVsudungnhieu;
        }
    }
}