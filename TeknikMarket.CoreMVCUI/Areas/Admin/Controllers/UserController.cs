using System.Security.Cryptography;
using System.Security.Principal;
using Core.CrossCuttingConcern.Crypto;
using Core.CrossCuttingConcern.MailOp;
using Microsoft.AspNetCore.Mvc;
using TeknikMarket.Business.Abstract;
using TeknikMarket.Business.Concrete;
using TeknikMarket.CoreMVCUI.Areas.Admin.Filter;
using TeknikMarket.CoreMVCUI.Extensions;
using TeknikMarket.Model.Entity;
using TeknikMarket.Model.Static;
using TeknikMarket.Model.ViewModel.Areas.Admin;
using static System.Net.WebRequestMethods;

namespace TeknikMarket.CoreMVCUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserController : Controller
    {
        private readonly IKullaniciBS _kullaniciBs;
        private readonly ISessionManager sessionManager;

        public UserController(IKullaniciBS kullaniciBs, ISessionManager _sessionManager)
        {
            _kullaniciBs = kullaniciBs;
            sessionManager = _sessionManager;
        }

        public IActionResult LogIn()
        {
            LogInVm model = new LogInVm();

            return View(model);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult LogIn(LogInVm vm)
        {
            LogInVm model = new LogInVm();

            if (!ModelState.IsValid) //ModelState.IsValid prop u , Validasyonlardan verinin geçip geçmediği bilgisini bize verir . Bu sayede sunucuda gereksiz kod çalışmaz.
            {
                ViewBag.Mesaj = "İşlemler HATALI";

                return View(model);
            }



            Kullanici kullanici = _kullaniciBs.Get(x => x.Email == vm.Email && x.Sifre==vm.Sifre&&x.Aktif==true,"KullanciRol","KullaniciRol.Rol");
            if (kullanici!=null)
            {
                //return RedirectToAction("Index","Home");

                return Redirect("/Admin/Home/Index");
            }
            ViewBag.Mesaj = "Giriş Başarısız";

            return View(model);
        }

        public IActionResult LogIn2()
        {
            LogInVm model = new LogInVm();

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult LogIn2(LogInVm vm)
        {
            LogInVm model = new LogInVm();

            if (!ModelState.IsValid)
            {
                ViewBag.Mesaj = "İşlemler Hatalı";
                return Json(new { result = false, Mesaj = "Validasyon Hatası Oldu" });
            }

            string sifre = CryptoManager.MD5Encrypt(vm.Sifre);

            Kullanici kullanici = _kullaniciBs.Get(x => x.Email == vm.Email && x.Sifre == sifre && x.Aktif == true, "KullaniciRols", "KullaniciRols.Rol");
            if (kullanici != null)
            {

                //HttpContext.Session["asdasd"] = 15515;
                //HttpContext.Session.SetObject("AktifKullanici", kullanici);
                sessionManager.AktifKullanici = kullanici;

                return Json(new { result = true,Mesaj= "Giriş Başarılı" });
            }
            else
            {
                return Json(new { result = false, Mesaj = "Kullanıcı bulunamadı." });
            }
        }
        public IActionResult Logout()
        {
            sessionManager.AktifKullanici = null;

            return RedirectToAction("Login2", "User");
        }

        public IActionResult NonAuthentication() 
        { 

            return View(); 
        }

        public IActionResult ForgotPassword()
        {
            ForgotPasswordViewModel model = new ForgotPasswordViewModel();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Mesaj = "İşlemler Hatalı";
                return Json(new { result = false, Mesaj = "Validasyon Hatası Oldu" });
            }

            Kullanici kullanici = _kullaniciBs.Get(x=>x.Email== model.Email && x.Adi.ToLower() == model.Adi.ToLower() && x.Soyadi.ToUpper() == model.SoyAdi.ToUpper());

            if (kullanici != null)
            {
                MailManager.Send(kullanici.Email, "Şifre Değiştirme", "Merhabalar Sayın : " + kullanici.Adi + " " + kullanici.Soyadi + "<br>Şifrenizi Değiştirmek İçin Lütfen <a href='" + Keys.SITEADDRESS + "Admin/User/UpdatePassword?UniqueID=" + kullanici.UniqueId + "'>Tıklayınız</a>");


                return Json(new { result = true,Mesaj = "Şifre Değiştirme Linki Mail Adresinize Gönderildi. Lütfen Mailinizi Kontrol Ediniz." });
            }
            else
            {
                return Json(new { result = false,Mesaj = "Bilgileriniz Hatalı. Lütfen Yeniden Deneyin"});
            }
        }

        public IActionResult UpdatePassword(string UniqueID)
        {
            UpdatePasswordVm vm = new UpdatePasswordVm();
            // UpdatePasswordVm nin validayon işlemleri Samed e Ödev
            // string Unique= RouteData.Values["id"].ToString();

            vm.UniqueID = UniqueID;


            Kullanici kullanici = _kullaniciBs.Get(x => x.UniqueId.ToString() == vm.UniqueID);

            if (kullanici == null)
            {
                return RedirectToAction("TehlikeliIslem", "Home");

            }


            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdatePassword(UpdatePasswordVm vm)
        {
            //string id= Request.RouteValues["UniqueID"].ToString();

            // UpdatePasswordVm nin validayon işlemleri Samed e Ödev
            if (!ModelState.IsValid)
            {
                ViewBag.Mesaj = " İşlemler Hatalı ";
                return Json(new { result = false, Mesaj = "Validasyon Hatası Oldu" });
            }

            Kullanici kullanici = _kullaniciBs.Get(x => x.UniqueId.ToString() == vm.UniqueID);



            if (kullanici != null && vm.Sifre == vm.SifreTekrar)
            {

                //kullanici.Sifre = CryptoManager.MD5Encrypt(vm.Sifre);
                kullanici.UniqueId = Guid.NewGuid();
                kullanici.Sifre = CryptoManager.MD5Encrypt(vm.Sifre);
                kullanici.GuncellemeTarihi = DateTime.Now;


                _kullaniciBs.Update(kullanici);
                return Json(new { result = true, Mesaj = "Şifreniz Başarıyla Değiştirildi. Giriş Yapabilirsiniz." });
            }
            else
            {

                return Json(new { result = false, Mesaj = "Ip Adresiniz Kayıt Ediliyor. Lütfen Yetkisiz İşlem Yapmayınız.." });

            }


        }


        [AktifKullaniciFilter]
        public IActionResult MyAccount(MyAccountVm model)
        {
            //vm dolu bir biçimde gitsin ki o sayfadaki bilgileri görebilelim.
            Kullanici kullanici = new Kullanici();
            model.Adi =kullanici.Adi;
            model.Soyadi =kullanici.Soyadi;
            model.Email =kullanici.Email;
            //model.SehirAdi = kullanici.İl.IlAdi;
            model.SehirId = kullanici.İlId.HasValue ? kullanici.İlId.Value : 0;
            model.Sifre = kullanici.Sifre;
            
            return View(model);
        }



    }
}
