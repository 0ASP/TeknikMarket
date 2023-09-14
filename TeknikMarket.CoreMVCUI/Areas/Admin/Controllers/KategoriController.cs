using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TeknikMarket.Business.Abstract;
using TeknikMarket.Business.Concrete;
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
		IMapper mapper;
		ISessionManager sessionManager;

		public KategoriController(IKategoriBS _kategoriBS, IMapper _mapper, ISessionManager _sessionManager)
        {
            kategoriBS = _kategoriBS;
			mapper = _mapper;
			sessionManager = _sessionManager;
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


		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Add(KategoriListeVm model)
		{
			Kategori kategori = new Kategori();
			kategori.KategoriAdi = model.KategoriAdi;
			kategori.Sira = model.Sira;
			kategori.Aktif = model.Aktif;
			kategori.UstKategoriId = model.UstKategoriId;

			//Kategori kategori = mapper.Map<Kategori>(model);


			if (model.UstKategoriId != -1)
			{

				Kategori ustkategori = kategoriBS.Get(x => x.Id == model.UstKategoriId);

				kategori.KategoriAdiGorunumu = ustkategori.KategoriAdiGorunumu + " > " + model.KategoriAdi;
			}
			else
			{

				//kategori.UstKategoriId = null;
				kategori.KategoriAdiGorunumu = model.KategoriAdi;
			}
			kategori.Id = null;
			kategori.Aktif = model.Aktif;
			kategori.Sira = model.Sira;
			kategori.OlusturmaTarihi = DateTime.Now;
			kategori.GuncellemeTarihi = DateTime.Now;
			kategori.Olusturan = sessionManager.AktifKullanici.Id;
			kategori.Guncelleyen = sessionManager.AktifKullanici.Id;

			kategoriBS.Insert(kategori);

			List<Kategori> kategoriler = kategoriBS.GetAll();

			int sira = kategoriler.OrderByDescending(x => x.Sira).FirstOrDefault().Sira ?? 1;

			sira = sira + 1;


			return Json(new { result = true, mesaj = "Kategori Başarıyla Eklendi", kategoriListesi = kategoriler, sira = sira });

		}

		public IActionResult Delete(int KategoriId)
		{
			Kategori kategori = kategoriBS.Get(x => x.Id == KategoriId);
			kategoriBS.Delete(kategori);



			List<Kategori> kategoriler = kategoriBS.GetAll();

			return Json(new { result = true, mesaj = "Kategori Başarıyla Silindi", kategoriListesi = kategoriler });
		}

		public IActionResult Update(KategoriListeVm model)
		{
			Kategori kategori = mapper.Map<Kategori>(model);
			if (model.UstKategoriId != -1)
			{

				Kategori ustkategori = kategoriBS.Get(x => x.Id == model.UstKategoriId);

				kategori.KategoriAdiGorunumu = ustkategori.KategoriAdiGorunumu + " > " + model.KategoriAdi;
			}
			else
			{

				//kategori.UstKategoriId = null;
				kategori.KategoriAdiGorunumu = model.KategoriAdi;
			}

			kategori.Aktif = model.Aktif;
			kategori.Sira = model.Sira;
			kategori.GuncellemeTarihi = DateTime.Now;
			kategori.Guncelleyen = sessionManager.AktifKullanici.Id;

			kategoriBS.Update(kategori);


			List<Kategori> kategoriler = kategoriBS.GetAll();

			//int sira = kategoriler.OrderByDescending(x => x.Sira).FirstOrDefault().Sira ?? 1;

			//sira = sira + 1;


			return Json(new { result = true, mesaj = "Kategori Başarıyla Güncellendi", kategoriListesi = kategoriler });
		}
		public IActionResult GetById(int KategoriId)
		{

			Kategori kategori = kategoriBS.Get(x => x.Id == KategoriId);
			List<Kategori> kategoriler = kategoriBS.GetAll();
			return Json(new { result = true, kategori = kategori, kategoriListesi = kategoriler });
		}


		public IActionResult Aktiflik(int KategoriId, bool Aktiflik)
		{

			Kategori k = kategoriBS.Get(x => x.Id == KategoriId);
			if (k != null)
			{
				k.Aktif = Aktiflik;
				kategoriBS.Update(k);
			}
			string mesaj = "";
			if (k.Aktif)
			{
				mesaj = "Kategori Başarıyla Aktifleştirildi";
			}
			else
			{
				mesaj = "Kategori Başarıyla Pasfileştirildi";
			}
			return Json(new { result = true, mesaj = mesaj });
		}
	}
}
