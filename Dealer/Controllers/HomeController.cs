using Dealer.Helper;
using Dealer.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace Dealer.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration _IConfiguration;
        private readonly IHttpClientFactory _httpClientFactory;

        public HomeController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _IConfiguration = configuration;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new List<PropertyCategoryViewModel>();

            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync(_IConfiguration["BaseWebAPIURL"] + "GetCategories");

            if (!response.IsSuccessStatusCode)
                return View(new List<PropertyCategory>());

            var json = await response.Content.ReadAsStringAsync();
            var categories = JsonConvert.DeserializeObject<List<PropertyCategory>>(json);

            viewModel = categories.Select(c => new PropertyCategoryViewModel
            {
                NameEn = c.NameEn,
                NameAr = c.NameAr,
                ImagePath = c.ImagePath,
                EncryptedId = EncryptionHelper.Encrypt(c.Id.ToString())
            }).ToList();

            return View(viewModel);
        }

        public async Task<IActionResult> property(string PropertyId)
        {
            var id = Convert.ToInt32(EncryptionHelper.Decrypt(PropertyId));

            var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(_IConfiguration["BaseWebAPIURL"] + "GetPropertiesImg/" + id);

            if (!response.IsSuccessStatusCode)
            {
                return View(new List<PropertyImage>());
            }

            var jsonString = await response.Content.ReadAsStringAsync();
            var propertiesImg = JsonConvert.DeserializeObject<List<PropertyImage>>(jsonString);

            return View(propertiesImg);
        }

        public async Task<IActionResult> Addproperty()
        {
            return View(new PropertyRequest());
        }

        public async Task<IActionResult> Items(string encId)
        {
            var id = Convert.ToInt32(EncryptionHelper.Decrypt(encId));
            var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(_IConfiguration["BaseWebAPIURL"] + "GetProperties/" + id);

            if (!response.IsSuccessStatusCode)
            {
                return View(new List<Property>());
            }

            var jsonString = await response.Content.ReadAsStringAsync();
            var properties = JsonConvert.DeserializeObject<List<Property>>(jsonString);
            var viewModel = properties.Select(c => new PropertyViewModel
            {
                Location = c.Location,
                Area = c.Area,
                DescEn = c.DescEn,
                DescAr = c.DescAr,
                CountBeds = c.CountBeds,
                CountBaths = c.CountBaths,
                Space = c.Space,
                CategoryId = c.CategoryId,
                Price = c.Price,
                Image = c.Image,
                EncryptedId = EncryptionHelper.Encrypt(c.Id.ToString())
            }).ToList();

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Addproperty([FromForm] PropertyRequest model)
        {
            if (model.TakePicture == null)
            {
                model.TakePicture = false;
            }
            var client = _httpClientFactory.CreateClient();
            var responses = await client.GetAsync(_IConfiguration["BaseWebAPIURL"] + "GetCategories");

            if (!responses.IsSuccessStatusCode)
                return Json(new { success = false, message = "Failed to get categories." });

            var json = await responses.Content.ReadAsStringAsync();
            var categories = JsonConvert.DeserializeObject<List<PropertyCategory>>(json);
            var selectedCategory = categories.FirstOrDefault(x => x.NameEn == model.PropertyType);

            if (selectedCategory == null)
                return Json(new { success = false, message = "Invalid property type." });

            model.CategoryId = selectedCategory.Id;

            foreach (var formFile in model.ImageUpload)
            {
                if (formFile != null && formFile.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await formFile.CopyToAsync(memoryStream);
                        model.Images.Add(new PropertyRequestImage
                        {
                            ImageData = memoryStream.ToArray(),
                            PropertyRequestId = model.Id // adjust this based on actual relationship
                        });
                    }
                }
            }

            foreach (var formFile in model.VideoUpload)
            {
                if (formFile != null && formFile.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await formFile.CopyToAsync(memoryStream);
                        model.Images.Add(new PropertyRequestImage
                        {
                            ImageData = memoryStream.ToArray(),
                            PropertyRequestId = model.Id // adjust this based on actual relationship
                        });
                    }
                }
            }

            using (var httpClient = new HttpClient())
            {
                var apiUrl = _IConfiguration["BaseWebAPIURL"] + "SubmitProperty";
                var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    return Json(new { success = true, message = "Property submitted successfully!" });
                }
                else
                {
                    return Json(new { success = false, message = "Failed to submit property." });
                }
            }
        }


    }
}
