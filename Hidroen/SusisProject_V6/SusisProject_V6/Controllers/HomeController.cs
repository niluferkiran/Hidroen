using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using static SusisProject_V6.Controllers.HomeController;


namespace SusisProject_V6.Controllers
{
    public class HomeController : BaseController
    {
        private static readonly HttpClient client = new HttpClient();
        private string apiKey = ConfigurationManager.AppSettings["test_key_w0101"];
        private string mail = "http://212.64.204.142:21147/api/Susis";
        private string mail1 = "http://212.64.204.142:21147/api/Susis";
        private readonly string baseApiUrl = "http://212.64.204.142:21147/api/Susis";
        private readonly string baseApiUrl1 = "http://localhost:21147/api/Susis";

        private readonly string susisApi = "https://susisservis.reklamat.net/api/Susis";
       
        public bool CheckSession()
        {
            if (Session["UserID"] == null)
            {
                return false; // Kullanıcı giriş yapmadıysa
            }
            return true;
        }

        // Bu action, Loginn.cshtml dosyasını döndürecek
        public ActionResult Login()
        {
            return View();
        }


        [AllowAnonymous]
        [HttpPost]
        public async Task<JsonResult> Login(string Username, string Password)
        {

            //ServicePointManager.Expect100Continue = true;
            //ServicePointManager.DefaultConnectionLimit = 9999;
            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            //sertifikayı devre dışı bırakarak giriş yaptırdım ama güvenli değil bunu sertifika güncellenince yeniden aşağıdaki kodu using parantezinin içine al ve httphandler ı sil.
            //HttpClient client = new HttpClient() // yenisi => var client = new HttpClient(handler)
            //var handler = new HttpClientHandler();
            //handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
            
            using (HttpClient client = new HttpClient())
            {
                string apiUrl = $"{baseApiUrl}/Login/{Username}/{Password}";

                HttpResponseMessage response = await client.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadAsAsync<ApiResponse>();
                    if (responseData.message == "Success.")
                    {
                        // Session
                        Session["UserID"] = responseData.content.ID;
                        Session["UserName"] = responseData.content.Name; // Kullanıcı adını session'a kaydet
                        //Session["UserName"] = Username; // Kullanıcı adını session'a kaydet
                        return Json(new { success = true, message = "Success" });
                    }
                }
                return Json(new { success = false, message = "Invalid login" });
            }

        }
        //Register sayfasını sonra oluşturursun burada metodu dursun
        //public ActionResult Register()
        //{
        //    return View();
        //}
        //public ActionResult SifremiUnuttum()
        //{
        //    return View();
        //}
        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon(); // Oturumu sona erdir
            // Tüm tarayıcı cookie'lerini silin
            foreach (var cookie in Request.Cookies.AllKeys)
            {
                Response.Cookies[cookie].Expires = DateTime.Now.AddDays(-1);
            }

            return RedirectToAction("Login");
        }
        public ActionResult Harita()
        {
            if (!CheckSession()) // Oturum kontrolü
            {
                return RedirectToAction("Login");
            }
            int userId = (int)HttpContext.Session["UserID"];

            return View();
        }


        public ActionResult CihazKayitEt()
        {
            //id = Convert.ToInt32(id);

            return View();
        }

        //[AllowAnonymous]
        //[HttpPost]
        //public async Task<JsonResult> CihazAddKayit(int id, string cihazAdi, string model, string seriNo)
        //{


        //    // ServicePointManager ayarları (daha global bir yerde de yapabilirsiniz)
        //    ServicePointManager.Expect100Continue = true;
        //    ServicePointManager.DefaultConnectionLimit = 9999;
        //    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

        //    // API URL'si
        //    string apiUrl = $"{baseApiUrl}/CihazAdd";

        //    // Gönderilecek veriler
        //    var cihazData = new
        //    {
        //        CihazAdi = cihazAdi,
        //        Model = model,
        //        SeriNo = seriNo
        //    };

        //    try
        //    {
        //        using (HttpClient client = new HttpClient())
        //        {
        //            // JSON formatında içerik oluştur
        //            StringContent content = new StringContent(JsonConvert.SerializeObject(cihazData), Encoding.UTF8, "application/json");

        //            // POST isteği gönder
        //            HttpResponseMessage response = await client.PostAsync(apiUrl, content);

        //            // Başarılı yanıt kontrolü
        //            if (response.IsSuccessStatusCode)
        //            {
        //                var responseData = await response.Content.ReadAsAsync<ApiResponse>();
        //                if (responseData.message == "Success.")
        //                {
        //                    return Json(new { success = true, message = "Cihaz başarıyla eklendi" });
        //                }
        //            }

        //            // Başarısız olursa
        //            return Json(new { success = false, message = "Cihaz eklenemedi" });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        // Beklenmeyen hata durumları
        //        return Json(new { success = false, message = "Bir hata oluştu: " + ex.Message });
        //    }
        //}
        [AllowAnonymous]
        [HttpPost]
  
        public async Task<JsonResult> CihazAddKayit(CihazAdd cihaz)
        {
            // Global ServicePointManager settings
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.DefaultConnectionLimit = 9999;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            // Validate the model
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Geçersiz cihaz bilgileri." });
            }

            // API URL
            string apiUrl = $"{baseApiUrl}/CihazAdd";

            try
            {
                using (var client = new HttpClient())
                {
                    // Create JSON content
                    StringContent content = new StringContent(JsonConvert.SerializeObject(cihaz), Encoding.UTF8, "application/json");

                    // Send POST request
                    HttpResponseMessage response = await client.PostAsync(apiUrl, content);

                    // Handle response
                    if (response.IsSuccessStatusCode)
                    {
                        var responseData = await response.Content.ReadAsAsync<ApiResponse>();

                        if (responseData.message.Equals("Success.", StringComparison.OrdinalIgnoreCase))
                        {
                            return Json(new { success = true, message = "Cihaz başarıyla eklendi" });
                        }
                        else
                        {
                            return Json(new { success = false, message = $"API yanıtı: {responseData.message}" });
                        }
                    }

                    return Json(new { success = false, message = "Cihaz eklenemedi. API ile iletişim kurulamıyor." });
                }
            }
            catch (HttpRequestException httpEx)
            {
                return Json(new { success = false, message = $"HTTP Hatası: {httpEx.Message}" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Bir hata oluştu: {ex.Message}" });
            }
        }

   

        [HttpGet]
        public async Task<ActionResult> GetHarita()
        {

            if (!CheckSession()) // Oturum kontrolü
            {
                return RedirectToAction("Login");
            }
            // Oturumda bulunan kullanıcı bilgilerine erişim
            int userId = (int)HttpContext.Session["UserID"];

            ServicePointManager.Expect100Continue = true;
            ServicePointManager.DefaultConnectionLimit = 9999;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;


            string apiUrl = $"{baseApiUrl}/GetCihazList";

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        string jsonData = await response.Content.ReadAsStringAsync();
                        // Haritaya konumları eklemek için gerekli işlemleri yapın
                        // JSON verisini döndürelim
                        return Content(jsonData, "application/json");
                    }
                    else
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "API isteği başarısız oldu.");
                    }
                }
                catch (HttpRequestException ex)
                {
                    // Log the exception
                    System.Diagnostics.Debug.WriteLine($"API request exception: {ex.Message}");
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "API isteği başarısız oldu: " + ex.Message);
                }
            }
            //if (Session["UserID"] == null)
            //{
            //    // Kullanıcı giriş yapmadıysa Loginn sayfasına yönlendir
            //    return RedirectToAction("Login");
            //}
            //int userId = (int)Session["UserID"];

            ViewBag.ApiKey = apiKey;
            return View();
        }
        [HttpGet]
        public async Task<ActionResult> GetActiveIller()
        {

            if (!CheckSession()) // Oturum kontrolü
            {
                return RedirectToAction("Login");
            }
            // Oturumda bulunan kullanıcı bilgilerine erişim
            int userId = (int)HttpContext.Session["UserID"];

            ServicePointManager.Expect100Continue = true;
            ServicePointManager.DefaultConnectionLimit = 9999;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;


            string apiUrl = $"{baseApiUrl}/GetActiveIller";

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        string jsonData = await response.Content.ReadAsStringAsync();
                        // Haritaya konumları eklemek için gerekli işlemleri yapın
                        // JSON verisini döndürelim
                        return Content(jsonData, "application/json");
                    }
                    else
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "API isteği başarısız oldu.");
                    }
                }
                catch (HttpRequestException ex)
                {
                    // Log the exception
                    System.Diagnostics.Debug.WriteLine($"API request exception: {ex.Message}");
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "API isteği başarısız oldu: " + ex.Message);
                }
            }
            //if (Session["UserID"] == null)
            //{
            //    // Kullanıcı giriş yapmadıysa Loginn sayfasına yönlendir
            //    return RedirectToAction("Login");
            //}
            //int userId = (int)Session["UserID"];

            ViewBag.ApiKey = apiKey;
            return View();
        }
        public async Task<ActionResult> TümCihazlar()
        {
            if (!CheckSession()) // Oturum kontrolü
            {
                return RedirectToAction("Login");
            }
            int userId = (int)HttpContext.Session["UserID"];

            ServicePointManager.Expect100Continue = true;
            ServicePointManager.DefaultConnectionLimit = 9999;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            string apiUrl = $"{baseApiUrl}/GetTumCihazlar"; //mail vardı base a geçti

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        string jsonData = await response.Content.ReadAsStringAsync();
                        //string xmlData = ConvertJsonToXml(jsonData); // JSON verisini XML'e dönüştür

                        // JSON dizisini C# nesnesine dönüştür
                        var devices = JsonConvert.DeserializeObject<List<CihazModel>>(jsonData);

                        //var xmlData = await response.Content.ReadAsStringAsync();
                        //var model = ConvertXmlToModel(xmlData);

                        // Log the count of devices to check if the model is populated
                        //System.Diagnostics.Debug.WriteLine($"Number of devices: {model.Count}");

                        //return View(model);
                        System.Diagnostics.Debug.WriteLine($"Number of devices: {devices.Count}");

                        // ViewBag.ApiKey değeri atanıyor
                        ViewBag.ApiKey = apiKey;

                        // View'e devices modeli ile birlikte yönlendirme yapılıyor
                        return View(devices);
                    }
                    else
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "API isteği başarısız oldu.");
                    }
                }
                catch (HttpRequestException ex)
                {
                    // Log the exception
                    System.Diagnostics.Debug.WriteLine($"API request exception: {ex.Message}");
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "API isteği başarısız oldu: " + ex.Message);
                }
            }
        }

        public ActionResult IlBazinda()
        {
            if (!CheckSession()) // Oturum kontrolü
            {
                return RedirectToAction("Login");
            }
            int userId = (int)HttpContext.Session["UserID"];
            //if (Session["UserID"] == null)
            //{
            //    // Kullanıcı giriş yapmadıysa Loginn sayfasına yönlendir
            //    return RedirectToAction("Loginn");
            //}
            //int userId = (int)Session["UserID"];
            ViewBag.ApiKey = apiKey;

            return View();
        }
        [HttpGet]
        public async Task<JsonResult> GetCityList()
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.DefaultConnectionLimit = 9999;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            string apiUrl = $"{baseApiUrl}/GetCityList"; // API URL
            List<Cities> cityList = new List<Cities>();

            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = await client.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        var jsonData = await response.Content.ReadAsStringAsync();
                        cityList = JsonConvert.DeserializeObject<List<Cities>>(jsonData);
                    }
                    else
                    {
                        return Json(new { success = false, message = "API'den veri çekilemedi" });
                    }
                }

                return Json(cityList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hata: " + ex.Message });
            }

        }

        public async Task<ActionResult> HavzaBazinda()
        {
            if (!CheckSession()) // Oturum kontrolü
            {
                return RedirectToAction("Login");
            }
            int userId = (int)HttpContext.Session["UserID"];

            ServicePointManager.Expect100Continue = true;
            ServicePointManager.DefaultConnectionLimit = 9999;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            string apiUrl = $"{baseApiUrl}/GetHavza";//mail vardı base a geçti

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        string jsonData = await response.Content.ReadAsStringAsync();
                        //string xmlData = ConvertJsonToXml(jsonData); // JSON verisini XML'e dönüştür

                        // JSON dizisini C# nesnesine dönüştür


                        //var xmlData = await response.Content.ReadAsStringAsync();
                        //var model = ConvertXmlToModel(xmlData);

                        // Log the count of devices to check if the model is populated
                        //System.Diagnostics.Debug.WriteLine($"Number of devices: {model.Count}");
                        var havzalar = JsonConvert.DeserializeObject<List<HavzaModel>>(jsonData);
                        //return View(model);
                        System.Diagnostics.Debug.WriteLine($"Number of devices: {havzalar.Count}");

                        return View(havzalar);
                    }
                    else
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "API isteği başarısız oldu.");
                    }
                }
                catch (HttpRequestException ex)
                {
                    // Log the exception
                    System.Diagnostics.Debug.WriteLine($"API request exception: {ex.Message}");
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "API isteği başarısız oldu: " + ex.Message);
                }
            }
            //if (Session["UserID"] == null)
            //{
            //    // Kullanıcı giriş yapmadıysa Loginn sayfasına yönlendir
            //    return RedirectToAction("Loginn");
            //}
            //int userId = (int)Session["UserID"];
            ViewBag.ApiKey = apiKey;
            return View();
        }
        public async Task<ActionResult> CihazBazinda()
        {
            if (!CheckSession()) // Oturum kontrolü
            {
                return RedirectToAction("Login");
            }
            int userId = (int)HttpContext.Session["UserID"];

            ServicePointManager.Expect100Continue = true;
            ServicePointManager.DefaultConnectionLimit = 9999;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            string apiUrl = $"{baseApiUrl}/GetActiveCihazTipleri"; // Doğru API URL'i burada olmalı //mail vardı base a geçti

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        string jsonData = await response.Content.ReadAsStringAsync();

                        // Debugging için: JSON verisini kontrol edelim
                        System.Diagnostics.Debug.WriteLine($"Received JSON: {jsonData}");

                        var simpleData = JsonConvert.DeserializeObject<List<string>>(jsonData)
                                         .Select(x => new Cihaz { fkTip = x })
                                         .ToList();

                        // Dönüştürülen veriyi kontrol et
                        System.Diagnostics.Debug.WriteLine($"Converted data count: {simpleData.Count}");
                        foreach (var marka in simpleData)
                        {
                            System.Diagnostics.Debug.WriteLine($"Marka: {marka.fkTip}");
                        }

                        return View(simpleData);
                    }
                    else
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "API isteği başarısız oldu.");
                    }
                }
                catch (HttpRequestException ex)
                {
                    // Log the exception
                    System.Diagnostics.Debug.WriteLine($"API request exception: {ex.Message}");
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "API isteği başarısız oldu: " + ex.Message);
                }
            }


            //if (Session["UserID"] == null)
            //{
            //    // Kullanıcı giriş yapmadıysa Loginn sayfasına yönlendir
            //    return RedirectToAction("Loginn");
            //}
            //int userId = (int)Session["UserID"];
            ViewBag.ApiKey = apiKey;

            return View();
        }
        public async Task<ActionResult> MarkaBazinda()
        {

            if (!CheckSession()) // Oturum kontrolü
            {
                return RedirectToAction("Login");
            }
            int userId = (int)HttpContext.Session["UserID"];

            ServicePointManager.Expect100Continue = true;
            ServicePointManager.DefaultConnectionLimit = 9999;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            string apiUrl = $"{baseApiUrl}/GetMarka"; // Doğru API URL'i burada olmalı //mail vardı base a geçti

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        string jsonData = await response.Content.ReadAsStringAsync();

                        // Debugging için: JSON verisini kontrol edelim
                        System.Diagnostics.Debug.WriteLine($"Received JSON: {jsonData}");

                        //var simpleData = JsonConvert.DeserializeObject<List<string>>(jsonData)
                        //                 .Select(x => new Marka { fkMarka = x })
                        //                 .ToList();

                        var simpleData = JsonConvert.DeserializeObject<List<string>>(jsonData)
                                 .Select(x => new Marka { fkMarka = x }) // Markaları büyük harfe dönüştürüyoruz
                                 .ToList();


                        // Dönüştürülen veriyi kontrol et
                        System.Diagnostics.Debug.WriteLine($"Converted data count: {simpleData.Count}");
                        foreach (var marka in simpleData)
                        {
                            System.Diagnostics.Debug.WriteLine($"Marka: {marka.fkMarka}");
                        }

                        return View(simpleData);
                    }
                    else
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "API isteği başarısız oldu.");
                    }
                }
                catch (HttpRequestException ex)
                {
                    // Log the exception
                    System.Diagnostics.Debug.WriteLine($"API request exception: {ex.Message}");
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "API isteği başarısız oldu: " + ex.Message);
                }
            }


            //if (Session["UserID"] == null)
            //{
            //    // Kullanıcı giriş yapmadıysa Loginn sayfasına yönlendir
            //    return RedirectToAction("Loginn");
            //}
            //int userId = (int)Session["UserID"];

            ViewBag.ApiKey = apiKey;

            return View();
        }
        public ActionResult PasifCihazlar()
        {
            if (!CheckSession()) // Oturum kontrolü
            {
                return RedirectToAction("Login");
            }
            int userId = (int)HttpContext.Session["UserID"];

            return View();
        }
        [HttpGet]
        public async Task<ActionResult> GetPasifCihazlar()
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.DefaultConnectionLimit = 9999;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            int durum = 0;

            string apiUrl = $"{baseApiUrl}/GetCihazdurum/{durum}";

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        string jsonData = await response.Content.ReadAsStringAsync();
                        // Haritaya konumları eklemek için gerekli işlemleri yapın
                        // JSON verisini döndürelim
                        return Content(jsonData, "application/json");
                    }
                    else
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "API isteği başarısız oldu.");
                    }
                }
                catch (HttpRequestException ex)
                {
                    // Log the exception
                    System.Diagnostics.Debug.WriteLine($"API request exception: {ex.Message}");
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "API isteği başarısız oldu: " + ex.Message);
                }
            }

            //if (Session["UserID"] == null)
            //{
            //    // Kullanıcı giriş yapmadıysa Loginn sayfasına yönlendir
            //    return RedirectToAction("Loginn");
            //}
            //int userId = (int)Session["UserID"];
            ViewBag.ApiKey = apiKey;
            return View();
        }
        //public async Task<ActionResult> GetCihazDataInterval(int cihazId)
        //{
        //    if (Session["UserID"] == null)
        //    {
        //        // Kullanıcı giriş yapmadıysa Loginn sayfasına yönlendir
        //        return RedirectToAction("Loginn");
        //    }

        //    // Session'dan userId almak
        //    int userId = (int)Session["UserID"];

        //    // HttpClient ayarları (ServicePointManager gibi şeyleri Global.asax ya da bir startup dosyasına taşıyabilirsin)
        //    ServicePointManager.Expect100Continue = true;
        //    ServicePointManager.DefaultConnectionLimit = 9999;
        //    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

        //    string apiUrl = $"{baseApiUrl}/GetCihazDataInterval/{cihazId}";
        //    using (HttpClient client = new HttpClient())
        //    {
        //        try
        //        {
        //            HttpResponseMessage response = await client.GetAsync(apiUrl);

        //            if (response.IsSuccessStatusCode)
        //            {
        //                // Gelen veriyi JSON olarak okuyup modele deserialize ediyoruz
        //                string jsonData = await response.Content.ReadAsStringAsync();
        //                var cihazData = JsonConvert.DeserializeObject<List<CihazDataInterval>>(jsonData);

        //                // Veriyi View'a döndürüyoruz
        //                return View(cihazData);
        //            }
        //            else
        //            {
        //                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "API isteği başarısız oldu.");
        //            }
        //        }
        //        catch (HttpRequestException ex)
        //        {
        //            // Log the exception
        //            System.Diagnostics.Debug.WriteLine($"API request exception: {ex.Message}");
        //            return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "API isteği başarısız oldu: " + ex.Message);
        //        }
        //    }

        //    ViewBag.ApiKey = apiKey;
        //    return View();
        //}
        public async Task<ActionResult> CihazTanımlamaları()
        {
            if (!CheckSession()) // Oturum kontrolü
            {
                return RedirectToAction("Login");
            }
            int userId = (int)HttpContext.Session["UserID"];

            ServicePointManager.Expect100Continue = true;
            ServicePointManager.DefaultConnectionLimit = 9999;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            string apiUrl = $"{baseApiUrl}/GetTumCihazlar";//mail vardı base a geçti

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        string jsonData = await response.Content.ReadAsStringAsync();
                        //string xmlData = ConvertJsonToXml(jsonData); // JSON verisini XML'e dönüştür

                        // JSON dizisini C# nesnesine dönüştür
                        var devices = JsonConvert.DeserializeObject<List<CihazModel>>(jsonData);


                        //var xmlData = await response.Content.ReadAsStringAsync();
                        //var model = ConvertXmlToModel(xmlData);

                        // Log the count of devices to check if the model is populated
                        //System.Diagnostics.Debug.WriteLine($"Number of devices: {model.Count}");

                        //return View(model);
                        System.Diagnostics.Debug.WriteLine($"Number of devices: {devices.Count}");

                        return View(devices);
                    }
                    else
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "API isteği başarısız oldu.");
                    }
                }
                catch (HttpRequestException ex)
                {
                    // Log the exception
                    System.Diagnostics.Debug.WriteLine($"API request exception: {ex.Message}");
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "API isteği başarısız oldu: " + ex.Message);
                }
            }
            //if (Session["UserID"] == null)
            //{
            //    // Kullanıcı giriş yapmadıysa Loginn sayfasına yönlendir
            //    return RedirectToAction("Loginn");
            //}
            //int userId = (int)Session["UserID"];
            ViewBag.ApiKey = apiKey;
            return View();
        }
        public async Task<ActionResult> CihazTipiTanımları()
        {
            if (!CheckSession()) // Oturum kontrolü
            {
                return RedirectToAction("Login");
            }
            int userId = (int)HttpContext.Session["UserID"];

            ServicePointManager.Expect100Continue = true;
            ServicePointManager.DefaultConnectionLimit = 9999;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            string apiUrl = $"{baseApiUrl}/GetTumCihazlar";//mail vardı base a geçti

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        string jsonData = await response.Content.ReadAsStringAsync();
                        //string xmlData = ConvertJsonToXml(jsonData); // JSON verisini XML'e dönüştür

                        // JSON dizisini C# nesnesine dönüştür
                        var devices = JsonConvert.DeserializeObject<List<CihazModel>>(jsonData);


                        //var xmlData = await response.Content.ReadAsStringAsync();
                        //var model = ConvertXmlToModel(xmlData);

                        // Log the count of devices to check if the model is populated
                        //System.Diagnostics.Debug.WriteLine($"Number of devices: {model.Count}");

                        //return View(model);
                        System.Diagnostics.Debug.WriteLine($"Number of devices: {devices.Count}");

                        return View(devices);
                    }
                    else
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "API isteği başarısız oldu.");
                    }
                }
                catch (HttpRequestException ex)
                {
                    // Log the exception
                    System.Diagnostics.Debug.WriteLine($"API request exception: {ex.Message}");
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "API isteği başarısız oldu: " + ex.Message);
                }
            }

            //if (Session["UserID"] == null)
            //{
            //    // Kullanıcı giriş yapmadıysa Loginn sayfasına yönlendir
            //    return RedirectToAction("Loginn");
            //}
            //int userId = (int)Session["UserID"];
            ViewBag.ApiKey = apiKey;
            return View();
        }
        public ActionResult IlTanimi()
         {
            if (!CheckSession()) // Oturum kontrolü
            {
                return RedirectToAction("Login");
            }
            int userId = (int)HttpContext.Session["UserID"];

            //if (Session["UserID"] == null)
            //{
            //    // Kullanıcı giriş yapmadıysa Loginn sayfasına yönlendir
            //    return RedirectToAction("Loginn");
            //}
            //int userId = (int)Session["UserID"];
            ViewBag.ApiKey = apiKey;
            return View();
        }
        public ActionResult IlceTanimi()
        {
            if (!CheckSession()) // Oturum kontrolü
            {
                return RedirectToAction("Login");
            }
            int userId = (int)HttpContext.Session["UserID"];

            //if (Session["UserID"] == null)
            //{
            //    // Kullanıcı giriş yapmadıysa Loginn sayfasına yönlendir
            //    return RedirectToAction("Loginn");
            //}
            //int userId = (int)Session["UserID"];
            ViewBag.ApiKey = apiKey;
            return View();
        }

 
        [HttpGet]

        public async Task<ActionResult> CihazKayitDetay(int id)
        {
            if (!CheckSession()) // Oturum kontrolü
            {
                return RedirectToAction("Login");
            }

            ServicePointManager.Expect100Continue = true;
            ServicePointManager.DefaultConnectionLimit = 9999;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            var viewModel = new CihazKayitViewModel();

            // Yeni bir kayıt için sayfa boş döndürülecek
            if (id == 0)
            {
                // Yeni cihaz kaydı için model oluşturma
                viewModel.Cihazlar = new List<CihazModel>{ new CihazModel() };
                viewModel.Markalar = await GetMarkalarAsync(); // Markalar listesi
                ViewBag.ApiKey = apiKey;

                // Hiçbir API çağrısı yapmadan boş form döndürülüyor
                return View(viewModel);
            }
            
            // Mevcut kaydı düzenlemek için id != 0 olduğunda API çağrılarına devam edilecek.
            string apiUrl = $"{mail}/GetTumCihazlar";

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        string jsonData = await response.Content.ReadAsStringAsync();

                        var devices = JsonConvert.DeserializeObject<List<CihazModel>>(jsonData);

                        var device = devices.FirstOrDefault(d => d.ID == id);

                        if (device == null)
                        {
                            return HttpNotFound("Device not found");
                        }

                        viewModel.Cihazlar = new List<CihazModel> { device };
                        viewModel.Markalar = await GetMarkalarAsync(); // Markaları API'den alın ve küçük harfe dönüştürülmüş halini kullan
                        var cihazDataResult = await GetCihazDataInterval3(id);

                        if (cihazDataResult != null)
                        {
                            viewModel.CihazDataInterval = cihazDataResult.CihazDataInterval ?? new List<CihazDataInterval>();
                            viewModel.CihazFromId = cihazDataResult.CihazFromId ?? new List<CihazFromId>();
                        }

                        ViewBag.ApiKey = apiKey;
                        return View(viewModel);
                    }
                    else
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "API isteği başarısız oldu.");
                    }
                }
                catch (HttpRequestException ex)
                {
                    System.Diagnostics.Debug.WriteLine($"API request exception: {ex.Message}");
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "API isteği başarısız oldu: " + ex.Message);
                }
            }
        }

        //public ActionResult DetayGor()
        //{
        //    if (!CheckSession()) // Oturum kontrolü
        //    {
        //        return RedirectToAction("Login");
        //    }
        //    int userId = (int)HttpContext.Session["UserID"];

        //    List<CihazCombinedModel> cihazCombinedModels = new List<CihazCombinedModel>();
        //    ViewBag.ApiKey = apiKey;
        //    return View(cihazCombinedModels);
        //}
        [HttpGet]
        //public async Task<JsonResult> GetCihazDataInterval()
        //{
        //    // ID listesi
        //    int[] ids = new int[] { 2001, 2003, 2004, 2006 };

        //    ServicePointManager.Expect100Continue = true;
        //    ServicePointManager.DefaultConnectionLimit = 9999;
        //    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

        //    List<CihazDataInterval> allCihazlar = new List<CihazDataInterval>();

        //    using (HttpClient client = new HttpClient())
        //    {
        //        foreach (var id in ids)
        //        {
        //            string apiUrl = $"{baseApiUrl}/GetCihazDataInterval/{id}";

        //            HttpResponseMessage response = await client.GetAsync(apiUrl);

        //            if (response.IsSuccessStatusCode)
        //            {
        //                var data = await response.Content.ReadAsStringAsync();
        //                var cihazlar = JsonConvert.DeserializeObject<List<CihazDataInterval>>(data);

        //                if (cihazlar != null)
        //                {
        //                    allCihazlar.AddRange(cihazlar); 
        //                }
        //            }
        //            else
        //            {
        //                return Json(new { success = false, message = $"API'den veri alınamadı. ID: {id}" }, JsonRequestBehavior.AllowGet);
        //            }
        //        }
        //    }

        //    return Json(allCihazlar, JsonRequestBehavior.AllowGet);
        //}

        public async Task<JsonResult> GetCihazDataInterval()
        {
            // ID her zaman 2004 olarak ayarlanacak
            int id = 2003;

            ServicePointManager.Expect100Continue = true;
            ServicePointManager.DefaultConnectionLimit = 9999;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            string apiUrl = $"{baseApiUrl}/GetCihazDataInterval/{id}";

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    var cihazlar = JsonConvert.DeserializeObject<List<CihazDataInterval>>(data);
                    return Json(cihazlar, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = false, message = "API'den veri alınamadı." }, JsonRequestBehavior.AllowGet);
                }
            }
        }

        //public async Task<List<CihazDataInterval>> GetCihazDataInterval2(int? id)
        //{
        //    var cihazId = "";

        //    string apiUrl2 = $"{baseApiUrl}/CihazFromID/{id}";
        //    using (HttpClient client = new HttpClient())
        //    {
        //        HttpResponseMessage markaResponse = await client.GetAsync(apiUrl2);

        //        if (markaResponse.IsSuccessStatusCode)
        //        {
        //            var response = await markaResponse.Content.ReadAsStringAsync();
        //            var jsonResponse = JObject.Parse(response);
        //            cihazId = jsonResponse["CihazId"].Value<string>();
        //        }

        //    }

        //    string apiUrl = $"{baseApiUrl}/GetCihazDataInterval/{cihazId}";

        //    using (HttpClient client = new HttpClient())
        //    {
        //        try
        //        {
        //            HttpResponseMessage response = await client.GetAsync(apiUrl);

        //            if (response.IsSuccessStatusCode)
        //            {
        //                string jsonData = await response.Content.ReadAsStringAsync();
        //                var cihazData = JsonConvert.DeserializeObject<List<CihazDataInterval>>(jsonData);
        //                return cihazData;
        //            }
        //            else
        //            {
        //                throw new Exception("API isteği başarısız oldu.");
        //            }
        //        }
        //        catch (HttpRequestException ex)
        //        {
        //            System.Diagnostics.Debug.WriteLine($"API request exception: {ex.Message}");
        //            throw new Exception("API isteği başarısız oldu: " + ex.Message);
        //        }
        //    }
        //}

     
        public async Task<List<CihazDataInterval>> GetCihazDataInterval2(int? id)
        {
            var cihazId = "";

            string apiUrl2 = $"{baseApiUrl}/CihazFromID/{id}";
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage markaResponse = await client.GetAsync(apiUrl2);

                if (markaResponse.IsSuccessStatusCode)
                {
                    var response = await markaResponse.Content.ReadAsStringAsync();
                    // Dönen veri bir liste olduğu için DeserializeObject ile liste olarak alıyoruz
                    var cihazList = JsonConvert.DeserializeObject<List<CihazFromId>>(response);

                    // İlk elemanı alıyoruz, eğer varsa
                    if (cihazList != null && cihazList.Count > 0)
                    {
                        cihazId = cihazList[0].CihazId.ToString(); // CihazId'yi alıyoruz
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("Cihaz listesi boş döndü!");
                        throw new Exception("Cihaz bulunamadı.");
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"İlk API çağrısı başarısız oldu: {markaResponse.StatusCode}");
                    throw new Exception("İlk API isteği başarısız oldu.");
                }
            }

            string apiUrl = $"{baseApiUrl}/GetCihazDataInterval/{cihazId}";

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        string jsonData = await response.Content.ReadAsStringAsync();
                        var cihazData = JsonConvert.DeserializeObject<List<CihazDataInterval>>(jsonData);
                        return cihazData;
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"İkinci API çağrısı başarısız oldu: {response.StatusCode}");
                        throw new Exception("API isteği başarısız oldu.");
                    }
                }
                catch (HttpRequestException ex)
                {
                    System.Diagnostics.Debug.WriteLine($"API request exception: {ex.Message}");
                    throw new Exception("API isteği başarısız oldu: " + ex.Message);
                }
            }
        }

        [HttpGet]
        public async Task<ActionResult> DetayGor(int id, string cihazad)
        {
            var images = new List<string>();
            var cihazId = "";
            var cihazList = new List<CihazFromId>();
            DateTime tar1 = DateTime.Today;
            DateTime tar2 = DateTime.Today;
            id = Convert.ToInt32(id);
            cihazad = Convert.ToString(cihazad);
            ViewBag.cihazAd = cihazad;
            ViewBag.id = id;
            int tip = 0;
            if (cihazad == "TEUS" || cihazad.Contains("TEUS"))
            {
                tip = 1;
            }
            else
            {
                tip = 2;
            }
            try
            {
                 tar1 = DateTime.ParseExact("01.01.2024", "dd.MM.yyyy", CultureInfo.InvariantCulture);
                 tar2 = DateTime.ParseExact("31.12.2024", "dd.MM.yyyy", CultureInfo.InvariantCulture);
            }
            catch (FormatException ex)
            {
                System.Diagnostics.Debug.WriteLine($"Date parsing error: {ex.Message}");
            }

            //string apiUrl2 = $"{baseApiUrl}/CihazFromID/{id}";
            //using (HttpClient client = new HttpClient())
            //{
            //    HttpResponseMessage markaResponse = await client.GetAsync(apiUrl2);

            //    if (markaResponse.IsSuccessStatusCode)
            //    {
            //        try
            //        {
            //            var response = await markaResponse.Content.ReadAsStringAsync();
            //            cihazList = JsonConvert.DeserializeObject<List<CihazFromId>>(response);
            //        }
            //        catch (JsonSerializationException ex)
            //        {
            //            System.Diagnostics.Debug.WriteLine($"Deserialization error: {ex.Message}");
            //        }


            //        // İlk elemanı alıyoruz, eğer varsa
            //        if (cihazList != null && cihazList.Count > 0)
            //        {
            //            cihazId = cihazList[0].CihazId.ToString(); // CihazId'yi alıyoruz
            //        }
            //        else
            //        {
            //            System.Diagnostics.Debug.WriteLine("Cihaz listesi boş döndü!");
            //            throw new Exception("Cihaz bulunamadı.");
            //        }
            //    }
            //    else
            //    {
            //        System.Diagnostics.Debug.WriteLine($"İlk API çağrısı başarısız oldu: {markaResponse.StatusCode}");
            //        throw new Exception("İlk API isteği başarısız oldu.");
            //    }
            //}

            var cihazlarResult1 = await GetCihazDataInterval2(id);
            var cihazlar1 = cihazlarResult1 as List<CihazDataInterval> ?? new List<CihazDataInterval>();
            if (cihazlar1.Any())
            {
                var alarm1 = cihazlar1.OrderByDescending(device => device.Alarm1).FirstOrDefault()?.Alarm1;
                var alarm2 = cihazlar1.OrderByDescending(device => device.Alarm2).FirstOrDefault()?.Alarm2;
                ViewBag.Alarm1 = alarm1;
                ViewBag.Alarm2 = alarm2;
            }
            var maxminList = await GetCihazMaxMin(id, tar1, tar2);
            if (maxminList != null && maxminList.Count > 0)
            {
                //ViewBag.MaxMinList = maxminList;
                //decimal maxi = 0, mini = 0, ort = 0;
                //if (maxminList.Any())
                //{
                //    maxi = maxminList
                //        .Select(item =>
                //        {
                //            decimal.TryParse(item.Split(',')[0].Split(':')[1].Trim(), out decimal value);
                //            return value;
                //        })
                //        .FirstOrDefault();
                //    mini = maxminList
                //        .Select(item =>
                //        {
                //            decimal.TryParse(item.Split(',')[1].Split(':')[1].Trim(), out decimal value);
                //            return value;
                //        })
                //        .FirstOrDefault();
                //    ort = maxminList
                //        .Select(item =>
                //        {
                //            decimal.TryParse(item.Split(',')[2].Split(':')[1].Trim(), out decimal value);
                //            return value;
                //        })
                //        .FirstOrDefault();

                //}

                //ViewBag.maxi = maxi;
                //ViewBag.mini = mini;
                //ViewBag.ort = ort;
                try
                {
                    if (maxminList != null && maxminList.Count > 0)
                    {
                        decimal maxi = 0, mini = 0, ort = 0;

                        foreach (var item in maxminList)
                        {
                            var parts = item.Split(',');
                            if (parts.Length == 3)
                            {
                                decimal.TryParse(parts[0].Split(':')[1].Trim(), out maxi);
                                decimal.TryParse(parts[1].Split(':')[1].Trim(), out mini);
                                decimal.TryParse(parts[2].Split(':')[1].Trim(), out ort);
                            }
                            else
                            {
                                System.Diagnostics.Debug.WriteLine("Unexpected data format: " + item);
                            }
                        }

                        ViewBag.maxi = Math.Round(maxi, 2);
                        ViewBag.mini = Math.Round(mini, 2);
                        ViewBag.ort = Math.Round(ort, 2);
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("maxminList is empty or null.");
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Error processing maxminList: " + ex.Message);
                }
            }

            var cihazlarResult = await GetCihazList3(id, tar1, tar2, tip);
            var cihazlar = cihazlarResult.Data as List<CihazListModel3>;

            if (cihazlar != null)
            {
                var filteredDevices = cihazlar
                    .OrderByDescending(device => device.Real_Data_Time) 
                    .Take(300) // İlk 250 kaydı al
                    .ToList();

                return View(filteredDevices); 
            }
            else
            {
                return View(new List<CihazListModel3>());
            }
        }

     
        [HttpGet]
        public async Task<List<string>> GetCihazMaxMin(int id, DateTime tar1, DateTime tar2)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.DefaultConnectionLimit = 9999;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            string apiUrl = $"{baseApiUrl}/GetCihazMaxMin/{id}/{tar1:yyyy-MM-dd}/{tar2:yyyy-MM-dd}";

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    var maxminList = JsonConvert.DeserializeObject<List<CihazMaxiMini>>(data) ?? new List<CihazMaxiMini>();

                    // Create a flat list with default values if the list is empty or contains null items
                    var düzListe = maxminList.Select(item =>
                        $"Maksimum: {item.maxi}, Minimum: {item.mini}, Ortalama: {item.ort}"
                    ).ToList();

                    // If the list is empty, add a default entry
                    if (!düzListe.Any())
                    {
                        düzListe.Add("Maksimum: 0, Minimum: 0, Ortalama: 0");
                    }
                    return düzListe;
                }
                else
                {
                    // Hata durumunda boş bir liste döndür
                    return new List<string>();
                }
            }
        }



        [HttpGet]
        private async Task<List<string>> GetMarkalarAsync()
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.DefaultConnectionLimit = 9999;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            string markaApiUrl = $"{mail}/GetMarka";

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage markaResponse = await client.GetAsync(markaApiUrl);

                if (markaResponse.IsSuccessStatusCode)
                {
                    string markaData = await markaResponse.Content.ReadAsStringAsync();
                    var markalar = JsonConvert.DeserializeObject<List<string>>(markaData);
                    return markalar;
                }
                else
                {
                    return new List<string>();
                }
            }
        }
        public async Task<CihazDataResult> GetCihazDataInterval3(int? id)
        {
            var cihazDataResult = new CihazDataResult(); // CihazDataResult modelini oluşturuyoruz
            string cihazId = "";

            string apiUrl2 = $"{baseApiUrl}/CihazFromID/{id}";
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage markaResponse = await client.GetAsync(apiUrl2);

                if (markaResponse.IsSuccessStatusCode)
                {
                    var response = await markaResponse.Content.ReadAsStringAsync();
                    // Dönen veri bir liste olduğu için DeserializeObject ile liste olarak alıyoruz
                    var cihazList = JsonConvert.DeserializeObject<List<CihazFromId>>(response);

                    // İlk elemanı alıyoruz, eğer varsa
                    if (cihazList != null && cihazList.Count > 0)
                    {
                        cihazDataResult.CihazFromId = cihazList; // CihazFromId verisini cihazDataResult'a atıyoruz
                        cihazId = cihazList[0].CihazId.ToString(); // İlk cihazın CihazId'sini alıyoruz
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("Cihaz listesi boş döndü!");
                        throw new Exception("Cihaz bulunamadı.");
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"İlk API çağrısı başarısız oldu: {markaResponse.StatusCode}");
                    throw new Exception("İlk API isteği başarısız oldu.");
                }
            }

            // İkinci API çağrısı (CihazDataInterval için)
            string apiUrl = $"{baseApiUrl}/GetCihazDataInterval/{cihazId}";

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        string jsonData = await response.Content.ReadAsStringAsync();
                        var cihazData = JsonConvert.DeserializeObject<List<CihazDataInterval>>(jsonData);

                        cihazDataResult.CihazDataInterval = cihazData; // CihazDataInterval verisini de cihazDataResult'a ekliyoruz
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"İkinci API çağrısı başarısız oldu: {response.StatusCode}");
                        throw new Exception("API isteği başarısız oldu.");
                    }
                }
                catch (HttpRequestException ex)
                {
                    System.Diagnostics.Debug.WriteLine($"API request exception: {ex.Message}");
                    throw new Exception("API isteği başarısız oldu: " + ex.Message);
                }
            }

            return cihazDataResult; // Tüm veriyi içeren cihazDataResult döndürülüyor
        }



        [HttpGet]
        public async Task<ActionResult> GetLocationsTip(string marka)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.DefaultConnectionLimit = 9999;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;


            System.Diagnostics.Debug.WriteLine($"GetLocationsTip called with marka: {marka}");

            // GetTumCihazlar metodunu çağırarak tüm cihazları alın
            var tumCihazlarResponse = await GetTumCihazlar();


            if (tumCihazlarResponse != null)
            {
                //var tumCihazlarJson = JsonConvert.SerializeObject(tumCihazlarResponse.Data);
                string tumCihazlarJson = JsonConvert.SerializeObject(tumCihazlarResponse.Data);

                // JSON verisini debug çıktısına yazdırarak kontrol edin
                System.Diagnostics.Debug.WriteLine($"Tüm Cihazlar JSON: {tumCihazlarJson}");

                try
                {
                    var tumCihazlar = JsonConvert.DeserializeObject<List<CihazModel>>(tumCihazlarJson);

                    // Seçilen markaya göre eşleşen fkTip değerini bulun
                    var fkTip = tumCihazlar.FirstOrDefault(c => c.fkTip == marka)?.fkTip;

                    if (fkTip != null)
                    {
                        // Bulunan fkTip değeriyle GetCihazbyTip metoduna istek yapın
                        string apiUrl = $"{mail}/GetCihazbyTip/{fkTip}";

                        using (HttpClient client = new HttpClient())
                        {
                            try
                            {
                                HttpResponseMessage response = await client.GetAsync(apiUrl);

                                if (response.IsSuccessStatusCode)
                                {
                                    string jsonData = await response.Content.ReadAsStringAsync();
                                    // Haritaya konumları eklemek için gerekli işlemleri yapın
                                    // JSON verisini döndürelim
                                    return Content(jsonData, "application/json");
                                }
                                else
                                {
                                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "API isteği başarısız oldu.");
                                }
                            }
                            catch (HttpRequestException ex)
                            {
                                // Log the exception
                                System.Diagnostics.Debug.WriteLine($"API request exception: {ex.Message}");
                                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "API isteği başarısız oldu: " + ex.Message);
                            }
                        }
                    }
                    else
                    {
                        // Belirtilen markaya göre uygun fkTip değeri bulunamadı
                        return new HttpStatusCodeResult(HttpStatusCode.NotFound, "Belirtilen marka için uygun cihaz bulunamadı.");
                    }
                }
                catch (JsonException ex)
                {
                    // Handle the JSON deserialization exception
                    System.Diagnostics.Debug.WriteLine($"JSON deserialization error: {ex.Message}");
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "JSON dönüşüm hatası: " + ex.Message);
                }
            }
            else
            {
                // GetTumCihazlar metodundan null döndü
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "Tüm cihazlar alınamadı.");
            }
        }
        [HttpGet]
        public async Task<ActionResult> GetLocationsMarka(string marka)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.DefaultConnectionLimit = 9999;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;


            System.Diagnostics.Debug.WriteLine($"GetLocationsTip called with marka: {marka}");

            // GetTumCihazlar metodunu çağırarak tüm cihazları alın
            var tumCihazlarResponse = await GetTumCihazlar();


            if (tumCihazlarResponse != null)
            {
                //var tumCihazlarJson = JsonConvert.SerializeObject(tumCihazlarResponse.Data);
                string tumCihazlarJson = JsonConvert.SerializeObject(tumCihazlarResponse.Data);

                // JSON verisini debug çıktısına yazdırarak kontrol edin
                System.Diagnostics.Debug.WriteLine($"Tüm Cihazlar JSON: {tumCihazlarJson}");

                try
                {
                    var tumCihazlar = JsonConvert.DeserializeObject<List<CihazModel>>(tumCihazlarJson);
                    var filteredCihazlar = tumCihazlar.Where(c => string.Equals(c.fkMarka, marka, StringComparison.OrdinalIgnoreCase)).ToList(); //burayı filtrelenmiş markalar için yazdım

                    // Seçilen markaya göre eşleşen fkTip değerini bulun
                    var fkMarka = tumCihazlar.FirstOrDefault(c => c.fkMarka == marka)?.fkMarka;

                    if (fkMarka != null)
                    {
                        // Bulunan fkTip değeriyle GetCihazbyTip metoduna istek yapın
                        string apiUrl = $"{mail}/GetCihazbyMarka/{fkMarka}";

                        using (HttpClient client = new HttpClient())
                        {
                            try
                            {
                                HttpResponseMessage response = await client.GetAsync(apiUrl);

                                if (response.IsSuccessStatusCode)
                                {
                                    string jsonData = await response.Content.ReadAsStringAsync();
                                    // Haritaya konumları eklemek için gerekli işlemleri yapın
                                    // JSON verisini döndürelim
                                    return Content(jsonData, "application/json");
                                }
                                else
                                {
                                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "API isteği başarısız oldu.");
                                }
                            }
                            catch (HttpRequestException ex)
                            {
                                // Log the exception
                                System.Diagnostics.Debug.WriteLine($"API request exception: {ex.Message}");
                                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "API isteği başarısız oldu: " + ex.Message);
                            }
                        }
                    }
                    else
                    {
                        // Belirtilen markaya göre uygun fkTip değeri bulunamadı
                        return new HttpStatusCodeResult(HttpStatusCode.NotFound, "Belirtilen marka için uygun cihaz bulunamadı.");
                    }
                }
                catch (JsonException ex)
                {
                    // Handle the JSON deserialization exception
                    System.Diagnostics.Debug.WriteLine($"JSON deserialization error: {ex.Message}");
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "JSON dönüşüm hatası: " + ex.Message);
                }
            }
            else
            {
                // GetTumCihazlar metodundan null döndü
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "Tüm cihazlar alınamadı.");
            }
        }

        [HttpGet]
        //public async Task<ActionResult> GetCityList()
        //{
        //    ServicePointManager.Expect100Continue = true;
        //    ServicePointManager.DefaultConnectionLimit = 9999;
        //    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

        //    string apiUrl = $"{baseApiUrl}/GetCityList";

        //    using (HttpClient client = new HttpClient())
        //    {
        //        try
        //        {
        //            HttpResponseMessage response = await client.GetAsync(apiUrl);

        //            if (response.IsSuccessStatusCode)
        //            {
        //                string jsonData = await response.Content.ReadAsStringAsync();
        //                var cities = JsonConvert.DeserializeObject<List<Cities>>(jsonData);

        //                // Debugging için: JSON verisini kontrol edelim
        //                System.Diagnostics.Debug.WriteLine($"Received JSON: {jsonData}");

        //                return Json(cities, JsonRequestBehavior.AllowGet);
        //            }
        //            else
        //            {
        //                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "API isteği başarısız oldu.");
        //            }
        //        }
        //        catch (HttpRequestException ex)
        //        {
        //            // Log the exception
        //            System.Diagnostics.Debug.WriteLine($"API request exception: {ex.Message}");
        //            return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "API isteği başarısız oldu: " + ex.Message);
        //        }
        //    }
        //}
        public async Task<ActionResult> GetDistrictList(int ilKod)
        {
            string apiUrl = $"{baseApiUrl}/GetProvience/{ilKod}";

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        string jsonData = await response.Content.ReadAsStringAsync();
                        var districts = JsonConvert.DeserializeObject<List<District>>(jsonData);

                        // Debugging için: JSON verisini kontrol edelim
                        System.Diagnostics.Debug.WriteLine($"Received JSON: {jsonData}");

                        return Json(districts, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "API isteği başarısız oldu.");
                    }
                }
                catch (HttpRequestException ex)
                {
                    // Log the exception
                    System.Diagnostics.Debug.WriteLine($"API request exception: {ex.Message}");
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "API isteği başarısız oldu: " + ex.Message);
                }
            }
        }

        // Marka verilerini çekmek için action
        public async Task<ActionResult> GetDistinctMarka()
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.DefaultConnectionLimit = 9999;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            string apiUrl = $"{mail}/GetMarka";
            HttpResponseMessage response = await client.GetAsync(apiUrl);
            if (response.IsSuccessStatusCode)
            {
                string jsonData = await response.Content.ReadAsStringAsync();
                var markaList = JsonConvert.DeserializeObject<List<string>>(jsonData);
                return Json(markaList, JsonRequestBehavior.AllowGet);
            }
            return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
        }

        [HttpGet]

        public async Task<JsonResult> GetTumCihazlar()
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.DefaultConnectionLimit = 9999;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            string apiUrl = $"{mail}/GetTumCihazlar";

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    var cihazlar = JsonConvert.DeserializeObject<List<CihazModel>>(data);
                    return Json(cihazlar, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    // Handle error
                    return Json(null, JsonRequestBehavior.AllowGet);
                }
            }
        }
        [HttpGet]
        public async Task<JsonResult> GetCihazList(int fkcihaz, DateTime tar1, DateTime tar2)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.DefaultConnectionLimit = 9999;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            string apiUrl = $"{baseApiUrl}/GetCihazDataList/{fkcihaz}/{tar1:yyyy-MM-dd}/{tar2:yyyy-MM-dd}"; 

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    var cihazlar = JsonConvert.DeserializeObject<List<CihazListModel2>>(data);
                    return Json(cihazlar, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    // Handle error
                    return Json(null, JsonRequestBehavior.AllowGet);
                }
            }
        }
        public async Task<JsonResult> GetCihazList2(int fkcihaz, DateTime tar1, DateTime tar2, int tip)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.DefaultConnectionLimit = 9999;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            string apiUrl = $"{baseApiUrl}/GetCihazDataList2/{fkcihaz}/{tar1:yyyy-MM-dd}/{tar2:yyyy-MM-dd}/{tip}"; ;

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(apiUrl);

                //if (response.IsSuccessStatusCode)
                //{
                //    var data = await response.Content.ReadAsStringAsync();
                //    var cihazlar = JsonConvert.DeserializeObject<List<CihazListModel2>>(data);
                //    return Json(cihazlar, JsonRequestBehavior.AllowGet);

                //}
                //else
                //{
                //    // Handle error
                //    return Json(null, JsonRequestBehavior.AllowGet);
                //}
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    var cihazlar = JsonConvert.DeserializeObject<List<CihazListModel2>>(data);

                    if (cihazlar != null && cihazlar.Any())
                    {
                        // Filter out devices where Seviye or RealData might be null or invalid
                        var filteredDevices = cihazlar
                            .Where(device => device.Seviye != null && device.Real_Data_Time != null) // Ensure Seviye and RealData exist
                            
                            .OrderBy(device => device.Real_Data_Time) // Further sort by RealData if needed
                            .Take(300) // Take the top 250 records
                            .ToList();

                        return Json(filteredDevices, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        // If cihazlar is null or empty, return an empty list
                        return Json(new List<CihazListModel2>(), JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    // Handle error response
                    return Json(null, JsonRequestBehavior.AllowGet);
                }

            }
        }
        public async Task<JsonResult> GetCihazList3(int fkcihaz, DateTime tar1, DateTime tar2, int tip)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.DefaultConnectionLimit = 9999;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            tar2 = tar2.AddDays(1);
            string apiUrl = $"{baseApiUrl}/GetCihazDataList3/{fkcihaz}/{tar1:yyyy-MM-dd}/{tar2:yyyy-MM-dd}/{tip}"; ;

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(apiUrl);

                //if (response.IsSuccessStatusCode)
                //{
                //    var data = await response.Content.ReadAsStringAsync();
                //    var cihazlar = JsonConvert.DeserializeObject<List<CihazListModel2>>(data);
                //    return Json(cihazlar, JsonRequestBehavior.AllowGet);

                //}
                //else
                //{
                //    // Handle error
                //    return Json(null, JsonRequestBehavior.AllowGet);
                //}
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    var cihazlar = JsonConvert.DeserializeObject<List<CihazListModel3>>(data);

                    if (cihazlar != null && cihazlar.Any())
                    {
                        // Filter out devices where Seviye or RealData might be null or invalid
                        var filteredDevices = cihazlar
                           .Where(device => device.Seviye != null && device.Real_Data_Time != null && device.Real_Data_Time >= tar1 &&
                    device.Real_Data_Time <= tar2) // Ensure Seviye and RealData exist
                         
                           .OrderByDescending(device => device.Real_Data_Time) // Further sort by RealData if needed
                           .Take(300) // Take the top 250 records
                           .ToList();

                        return Json(filteredDevices, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        // If cihazlar is null or empty, return an empty list
                        return Json(new List<CihazListModel3>(), JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    // Handle error response
                    return Json(null, JsonRequestBehavior.AllowGet);
                }

            }
        }

        public class CihazDataResult
        {
            public List<CihazFromId> CihazFromId { get; set; }
            public List<CihazDataInterval> CihazDataInterval { get; set; }
        }
        public class CihazAdd
        {
            public int ID { get; set; }
            public string CihazNo { get; set; }
            public int? CihazId { get; set; } // Nullable int
            public string CihazAd { get; set; }
            public string fkMarka { get; set; }
            public string fkTip { get; set; }
            public string IpNo { get; set; }
            public string SimCardNo { get; set; }
            public string ImeiNo { get; set; }
            public int? fkKonum { get; set; } // Nullable int
            public string Durum { get; set; }
            public DateTime? SonData { get; set; } // Nullable DateTime
            public double? SuSeviye { get; set; } // Nullable double
            public double? Aku { get; set; } // Nullable double
            public int? VeriInt { get; set; } // Nullable int
            public int? ResimInt { get; set; } // Nullable int
            public int? ResimKalite { get; set; } // Nullable int
            public int? ResimW { get; set; } // Nullable int
            public int? ResimH { get; set; } // Nullable int
            public int? Alarm1 { get; set; } // Nullable int
            public int? Alarm2 { get; set; } // Nullable int
            public int? Alarm3 { get; set; } // Nullable int
            public int? ResimIntA1 { get; set; } // Nullable int
            public int? ResimIntA2 { get; set; } // Nullable int
            public int? ResimIntA3 { get; set; } // Nullable int
            public string A1Tel1 { get; set; }
            public string A1Tel2 { get; set; }
            public string A1Tel3 { get; set; }
            public string A1Tel4 { get; set; }
            public string A1Tel5 { get; set; }
            public string A2Tel1 { get; set; }
            public string A2Tel2 { get; set; }
            public string A2Tel3 { get; set; }
            public string A2Tel4 { get; set; }
            public string A2Tel5 { get; set; }
            public string A3Tel1 { get; set; }
            public string A3Tel2 { get; set; }
            public string A3Tel3 { get; set; }
            public string A3Tel4 { get; set; }
            public string A3Tel5 { get; set; }
            public int? SensorOffset { get; set; } // Nullable int
            public int? AdcOffset { get; set; } // Nullable int
            public int? MinDeger { get; set; } // Nullable int
            public int? MaxDeger { get; set; } // Nullable int
            public int? RadarSure { get; set; } // Nullable int
            public string SensorTip { get; set; }
            public string Bolge { get; set; }
            public decimal? Derinlik { get; set; } // Nullable decimal
            public decimal? CihazMontaj { get; set; } // Nullable decimal
            public decimal? Kot { get; set; } // Nullable decimal
            public decimal? StatikSeviye { get; set; } // Nullable decimal
            public decimal? DinamikSeviye { get; set; } // Nullable decimal
            public int? fkHavza { get; set; } // Nullable int
        }
        public class CihazDataInterval
        {
            public int? VeriInt { get; set; }
            [JsonProperty("ResimInt")]
            public int? ResimInt { get; set; }
            public int? ResimKalite { get; set; }
            public int? ResimW { get; set; }
            public int? ResimH { get; set; }
            public int? Alarm1 { get; set; }
            public int? Alarm2 { get; set; }
            public int? Alarm3 { get; set; }
            public int? SensorOffset { get; set; }
            public int? AdcOffset { get; set; }
            public string RadarSure { get; set; } // Eğer "RadarSure" sadece `null` veya `string` değerler alıyorsa string türünde tanımlayın.

            // A1, A2, A3 telefon numaraları
            public string A1Tel1 { get; set; }
            public string A1Tel2 { get; set; }
            public string A1Tel3 { get; set; }
            public string A1Tel4 { get; set; }
            public string A1Tel5 { get; set; }
            public string A2Tel1 { get; set; }
            public string A2Tel2 { get; set; }
            public string A2Tel3 { get; set; }
            public string A2Tel4 { get; set; }
            public string A2Tel5 { get; set; }
            public string A3Tel1 { get; set; }
            public string A3Tel2 { get; set; }
            public string A3Tel3 { get; set; }
            public string A3Tel4 { get; set; }
            public string A3Tel5 { get; set; }
        }


        public class CihazFromId
        {
            public int ID { get; set; }
            public string CihazNo { get; set; }
            public int CihazId { get; set; }
            public string CihazAd { get; set; }
            public string fkMarka { get; set; }
            public string fkTip { get; set; }
            public string IpNo { get; set; }
            public string SimCardNo { get; set; }
            public string ImeiNo { get; set; }
            public int? fkKonum { get; set; }
            public string Durum { get; set; }
            public double? SuSeviye { get; set; }
            public double? Aku { get; set; }
            public int VeriInt { get; set; }
            public int? ResimInt { get; set; }
            public int? ResimKalite { get; set; }
            public int? ResimW { get; set; }
            public int? ResimH { get; set; }
            public int? Alarm1 { get; set; }
            public int? Alarm2 { get; set; }
            public int? Alarm3 { get; set; }
            public int? ResimIntA1 { get; set; }
            public int? ResimIntA2 { get; set; }
            public int? ResimIntA3 { get; set; }
            public string A1Tel1 { get; set; }
            public string A1Tel2 { get; set; }
            public string A1Tel3 { get; set; }
            public string A1Tel4 { get; set; }
            public string A1Tel5 { get; set; }
            public string A2Tel1 { get; set; }
            public string A2Tel2 { get; set; }
            public string A2Tel3 { get; set; }
            public string A2Tel4 { get; set; }
            public string A2Tel5 { get; set; }
            public string A3Tel1 { get; set; }
            public string A3Tel2 { get; set; }
            public string A3Tel3 { get; set; }
            public string A3Tel4 { get; set; }
            public string A3Tel5 { get; set; }
            public int? SensorOffset { get; set; }
            public int? AdcOffset { get; set; }
            public int? MinDeger { get; set; }
            public int? MaxDeger { get; set; }
            public int? RadarSure { get; set; }
            public string SensorTip { get; set; }
            public string Bolge { get; set; }
            public double? Derinlik { get; set; }
            public string CihazMontaj { get; set; }
            public double? Kot { get; set; }
            public double? StatikSeviye { get; set; }
            public double? DinamikSeviye { get; set; }
            public int? fkHavza { get; set; }
        }

        public class CihazListModel2
        {
            public DateTime Real_Data_Time { get; set; }
            public DateTime Gönderim_Zamanı { get; set; }
            public decimal? Akü_Veri { get; set; }
            public decimal SensorVeri { get; set; }
            public decimal Seviye { get; set; }
            public bool Alarm_Durum { get; set; }
            public decimal? Sicaklik { get; set; } // Sıcaklık verisi null olabiliyor
        }
        public class CihazListModel3
        {
            public DateTime Real_Data_Time { get; set; }
            public DateTime Gönderim_Zamanı { get; set; }
            public decimal? Akü_Veri { get; set; }
            public decimal SensorVeri { get; set; }
            public decimal Seviye { get; set; }
            public bool Alarm_Durum { get; set; }
            public decimal? Sicaklik { get; set; } // Sıcaklık verisi null olabiliyor
            public string Resim { get; set; }
        }
        public class CihazPageViewModel
        {
            public List<CihazDataInterval> CihazDataInterval { get; set; }
            public List<CihazListModel2> CihazList { get; set; }
        }


        //Model oluşturduk
        public class CihazKayitViewModel
        {
            public List<CihazModel> Cihazlar { get; set; }
            public List<string> Markalar { get; set; }
            public List<CihazDataInterval> CihazDataInterval { get; set; }
            public List<CihazFromId> CihazFromId { get; set; } // Yeni eklenen alan
            public List<CihazAdd> CihazAdd { get; set; } // Yeni eklenen alan
        }
        //Modeller
        public class District
        {
            public int KayitNo { get; set; }
            public int IlKod { get; set; }
            public int? IlceKod { get; set; } 
            public string Ilce { get; set; }
        }
        public class HavzaModel
        {
            public int pkHavza { get; set; }
            public string HavzaAdi { get; set; }
        }
        public class ApiResponse
        {
            public bool success { get; set; }
            public string message { get; set; }
            public Content content { get; set; }
        }

        public class Content
        {
            public int ID { get; set; }
            public string Name { get; set; }
            public int fkRol { get; set; }
        }
        public class LoginViewModel
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }
        public partial class Cities
        {
            public int KayitNo { get; set; }
            public Nullable<int> IlKod { get; set; }
            public string IlAd { get; set; }
        }
        public class Iller
        {

            public string IlAd { get; set; }

        }
        public class Tip
        {

            public string fkTip { get; set; }

        }
        public class Cihaz
        {

            public string fkTip { get; set; }

        }
        public class Marka
        {

            public string fkMarka { get; set; }

        }
        public class CihazModel
        {
            public int ID { get; set; }
            public string CihazAd { get; set; }
            public string fkMarka { get; set; }
            public string fkTip { get; set; }
            public string IpNo { get; set; }
            public string SimCardNo { get; set; }
            public string ImeiNo { get; set; }
            public string Durum { get; set; }
            public DateTime? SonData { get; set; }
            public double? SuSeviye { get; set; }
            public double? Aku { get; set; }
            public decimal KonumX { get; set; }
            public decimal KonumY { get; set; }
            public string OlcumYeri { get; set; }
            public string IlAd { get; set; }
            public string Ilce { get; set; }
        }

        
        public class KonumlarModel
        {
            public int ID { get; set; }
            public string CihazAd { get; set; }
            public string CihazNo { get; set; }
            public string fkMarka { get; set; }
            public string fkTip { get; set; }
            public string IpNo { get; set; }
            public string SimCardNo { get; set; }
            public string ImeiNo { get; set; }
            public string Durum { get; set; }
            public DateTime? SonData { get; set; }
            public double? SuSeviye { get; set; }
            public double? Aku { get; set; }
            public decimal KonumX { get; set; }
            public decimal KonumY { get; set; }
            public string OlcumYeri { get; set; }
            public string IlAd { get; set; }
            public string Ilce { get; set; }
        }
        public class CihazMaxiMini
        {
            public decimal? maxi { get; set; }
            public decimal? mini { get; set; }
            public decimal? ort { get; set; }
           
        }


   



       

    }
}