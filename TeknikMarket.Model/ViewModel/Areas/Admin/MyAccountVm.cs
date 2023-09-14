using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeknikMarket.Model.ViewModel.Areas.Admin
{
    public class MyAccountVm
    {
        public string Adi { get; set; }
        public string Soyadi { get; set; }
        public string Email { get; set; }
        public string Resim { get; set; }
        //public string Adres { get; set; }
        //public string CepTelefonu { get; set; }
        public int SehirId { get; set; }
        public string SehirAdi { get; set; }
        public string Sifre { get; set; }
        public string YeniSifre { get; set; }
        public string YeniSifreTekrar { get; set; }
    }
}
