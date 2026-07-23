using QLBV.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DoAnCuoiKy.Models
{
    public class ThongKeItem
    {
        private readonly QL_BENHVIENEntities db = new QL_BENHVIENEntities();

        // Dùng SQL aggregate thay vì load toàn bộ list
        public decimal DoanhThuNgay()
        {
            var today = DateTime.Today;
            return db.HOADONs
                .Where(x => x.NGAYLAP.HasValue && x.NGAYLAP.Value == today)
                .Sum(x => (decimal?)x.TONGTIEN) ?? 0;
        }

        public decimal DoanhThuThang()
        {
            int month = DateTime.Now.Month;
            int year  = DateTime.Now.Year;
            return db.HOADONs
                .Where(x => x.NGAYLAP.HasValue
                         && x.NGAYLAP.Value.Month == month
                         && x.NGAYLAP.Value.Year  == year)
                .Sum(x => (decimal?)x.TONGTIEN) ?? 0;
        }

        public List<decimal> DoanhThuNam()
        {
            int year = DateTime.Now.Year;
            // Một query duy nhất group by tháng
            var raw = db.HOADONs
                .Where(x => x.NGAYLAP.HasValue && x.NGAYLAP.Value.Year == year)
                .GroupBy(x => x.NGAYLAP.Value.Month)
                .Select(g => new { Month = g.Key, Total = g.Sum(x => x.TONGTIEN) })
                .ToList();

            var result = new List<decimal>(new decimal[12]);
            foreach (var item in raw)
                result[item.Month - 1] = item.Total ?? 0;
            return result;
        }

        public int SoLuongBS()
        {
            return db.BACSIs.Count();
        }

        public int SoLichKhamHomNay()
        {
            var today = DateTime.Today;
            return db.DATLICHKHAMs
                .Count(x => x.NGAYKHAM.HasValue && x.NGAYKHAM.Value == today);
        }

        public int SoBenhNhanHomNay()
        {
            var today = DateTime.Today;
            return db.DATLICHKHAMs
                .Where(x => x.NGAYKHAM.HasValue && x.NGAYKHAM.Value == today)
                .Select(x => x.BNHAN_ID)
                .Distinct()
                .Count();
        }

        public List<ThongKeDV> DVSuDung()
        {
            return db.Database.SqlQuery<ThongKeDV>("SP_DICHVUSUDUNGNHIEU").ToList();
        }
    }
}