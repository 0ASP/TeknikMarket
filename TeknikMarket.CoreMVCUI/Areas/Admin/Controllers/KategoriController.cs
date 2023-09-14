using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TeknikMarket.Business.Abstract;
using TeknikMarket.CoreMVCUI.Areas.Admin.Filter;
using TeknikMarket.Model.Entity;
using TeknikMarket.Model.ViewModel.Areas.Admin.Kategories;

namespace TeknikMarket.CoreMVCUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [RolFilter("Admin")]
    public class KategoriController : Controller
    {
        IKategoriBS kategoriBS;

        public KategoriController(IKategoriBS _kategoriBS)
        {
            kategoriBS = _kategoriBS;
        }

        public IActionResult List()
        {
            KategoriListeVm model = new KategoriListeVm();
            List<Kategori> kategoris = kategoriBS.GetAll().ToList();   //GetAll kısmınde repository de filter=null yap.

            model.KategoriListesi = kategoris;

            model.KategoriSelectList = kategoris.Select(x => new SelectListItem() { Text = x.KategoriAdiGorunumu, Value = x.Id.ToString() }).ToList();
            model.KategoriSelectList.Insert(0, new SelectListItem("Lütfen Üst Kategori Seçiniz", "-1"));
            int sira = 0;
            if (kategoris.Count == 0)
            {
                sira = 1;
            }
            else
            {
                sira = kategoris.OrderByDescending(x => x.Sira).FirstOrDefault().Sira ?? 1;
            }


            model.Sira = sira + 1;
            return View(model);
        }
        public IActionResult Ekle()
        {
            return View();
        }
    }
}
