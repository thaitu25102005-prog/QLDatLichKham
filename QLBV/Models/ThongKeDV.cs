using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QLBV.Models
{
    public class ThongKeDV
    {
        public List<CHITIETHOADON> ct = new List<CHITIETHOADON>();
        public int MADV { get; set; }
        public string TENDV { get; set; }
        public int SOLANSUDUNG { get; set; }       
        public int TONGSOLUONG { get; set; }
        public int SoLuotDungDV 
        { 
            get { return SOLANSUDUNG; }
        }
        public ThongKeDV() { }

    }
}