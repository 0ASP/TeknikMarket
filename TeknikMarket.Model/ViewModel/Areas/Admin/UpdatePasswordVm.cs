using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeknikMarket.Model.ViewModel.Areas.Admin
{
    public class UpdatePasswordVm
    {
        public int Id { get; set; }
        public string Sifre { get; set; }
        public string SifreTekrar { get; set; }
        public string UniqueID { get; set; }
    }
}
