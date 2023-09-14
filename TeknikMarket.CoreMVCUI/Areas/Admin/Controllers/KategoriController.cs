using Microsoft.AspNetCore.Mvc;
using TeknikMarket.Business.Abstract;
using TeknikMarket.CoreMVCUI.Areas.Admin.Filter;
using TeknikMarket.Model.ViewModel.Areas.Admin.Kategories;

namespace TeknikMarket.CoreMVCUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [RolFilter("Admin")]
    public class KategoriController : Controller
    {
        IKategoriBS kategoriBS;

        public KategoriController(IKategoriBS kategoriBS)
        {
            this.kategoriBS = kategoriBS;
        }

        public IActionResult List()
        {
            KategoriListeVm model = new KategoriListeVm();
            model.KategoriListesi = kategoriBS.GetAll(null).ToList();


            return View(model);
        }
        public IActionResult Ekle()
        {
            return View();
        }
    }
}
