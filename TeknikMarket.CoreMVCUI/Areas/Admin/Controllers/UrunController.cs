using AutoMapper;
using Core.Utility;
using Microsoft.AspNetCore.Mvc;
using TeknikMarket.Business.Abstract;
using TeknikMarket.Business.Concrete;
using TeknikMarket.CoreMVCUI.Areas.Admin.Filter;
using TeknikMarket.Model.Entity;
using TeknikMarket.Model.ViewModel.Areas.Admin;

namespace TeknikMarket.CoreMVCUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UrunController : Controller
    {
        IKategoriBS kategoriBs;
        IUrunFiyatBS urunFiyatBs;
        IUrunFotografBS urunFotografBs;
        IUrunBS urunBs;
        IMapper mapper;
        ISessionManager session;
        public UrunController(IKategoriBS _kategoriBs, IMapper _mapper, ISessionManager _session, IUrunBS _urunBs, IUrunFiyatBS _urunFiyatBs, IUrunFotografBS _urunFotografBs)
        {
            kategoriBs = _kategoriBs;
            mapper = _mapper;
            session = _session;
            urunBs = _urunBs;
            urunFiyatBs = _urunFiyatBs;
            urunFotografBs = _urunFotografBs;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Add()
        {
            UrunAddViewModel model = new UrunAddViewModel()
            {
                KategoriListesi = kategoriBs.GetAll().Select(x => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem() { Text = x.KategoriAdiGorunumu, Value = x.Id.ToString() }).ToList()
            };

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Add(UrunAddViewModel vm)
        {
            Urun u = mapper.Map<Urun>(vm);
            Urun added = urunBs.Insert(u);

            foreach (UrunFiyat item in vm.UrunFiyatlari)
            {
                item.UrunId = added.Id;
                urunFiyatBs.Insert(item);
            }

            return Json(new { result = true, id = added.Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddProductPhoto(IFormCollection data)
        {

            int urunid = int.Parse(data["UrunId"]);
            List<IFormFile> files = data.Files.ToList();
            int hata = 0;
            string hatamesaj = "";


            foreach (IFormFile item in files)
            {
                if (!item.ContentType.Contains("image/"))
                {
                    hata++;
                    hatamesaj += item.FileName + " dosyası resim değil" + Environment.NewLine;
                }
                else
                {

                    // Burada resimse 
                    if (item.Length > 1048576) // 1 MBdan büyükse (byte cinsinden) 
                    {

                        hata++;
                        hatamesaj += item.FileName + " dosyası 1MB dan daha büyük" + Environment.NewLine;

                    }
                    else
                    {

                        // 

                        string extension = Path.GetExtension(item.FileName);
                        string filename = RandomValueGenerator.UniqueFileName(extension);
                        string uploadpath = Directory.GetCurrentDirectory() + "/wwwroot/images/products/" + filename;
                        using (FileStream fs = new FileStream(uploadpath, FileMode.Create))
                        {
                            item.CopyTo(fs);
                        }
                        UrunFotograf urunFotograf = new UrunFotograf()
                        {
                            UrunId = urunid,
                            FotografAdresi = "/images/products/" + filename

                        };
                        hatamesaj += item.FileName + " dosyası başarıyla eklendi" + Environment.NewLine;
                        urunFotografBs.Insert(urunFotograf);

                    }
                }

            }
            if (hata > 0)
            {
                return Json(new { result = false, mesaj = hatamesaj });
            }

            return Json(new { result = true, mesaj = "Ürün Başarıyla Eklendi" });
        }
    }
}
