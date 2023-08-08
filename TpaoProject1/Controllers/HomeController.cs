﻿using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO.Pipelines;
using System.Reflection;
using TpaoProject1.Data;
using TpaoProject1.Model;
using TpaoProject1.Models;

namespace TpaoProject1.Controllers
{
    public class HomeController : BaseController
    {
        private readonly DatabaseContext _context;

        public HomeController(DatabaseContext context)
        {
            _context = context;
        }
		[HttpGet]
		public async Task<IActionResult> Index()
		{
			var tespit = _context.WellTops.Where(p => p.WellTopType == "tespit").Count();
			var uretim = _context.WellTops.Where(p => p.WellTopType == "üretim").Count();
			var arama = _context.WellTops.Where(p => p.WellTopType == "arama").Count();
			var toplam = tespit + uretim + arama;
			ViewBag.uretim = uretim;
			ViewBag.arama = arama;
			ViewBag.tespit = tespit;
			ViewBag.toplam = toplam;
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Index(WellTop model)
		{

			return View();
		}

       
        public async Task<IActionResult> LineSimplification()
        {
			//List<(double x, double y)> newList = GeoSimplifier.LineSimplificator.ReadData("testVerisi2.json");
			//List<(double x, double y)> pureList = GeoSimplifier.LineSimplificator.SimplifyPoints(newList, 0, newList.Count - 1, 100);
			//pureList.Add((newList[newList.Count() - 1].x, newList[newList.Count() - 1].y));
			//double, double)[] points = pureList.ToArray();

			return View();
        }
        
        public async Task<IActionResult> LineSimplificationMap()
        {
            //List<(double x, double y)> newList = GeoSimplifier.LineSimplificator.ReadData("testVerisi2.json");
            //List<(double x, double y)> pureList = GeoSimplifier.LineSimplificator.SimplifyPoints(newList, 0, newList.Count - 1, 100);
            //pureList.Add((newList[newList.Count() - 1].x, newList[newList.Count() - 1].y));
            //double, double)[] points = pureList.ToArray();

            return View();
        }


        public IActionResult History() {
            return View();
        }

        //[HttpPost]
        //public ActionResult UploadJson(IFormFile jsonFile)
        //{
        //	if (jsonFile != null && jsonFile.Length > 0)
        //	{
        //		using (var reader = new StreamReader(jsonFile.OpenReadStream()))
        //		{
        //			string jsonContent = reader.ReadToEnd();

        //			var jsonData = JsonConvert.DeserializeObject<JsonDataModel>(jsonContent);

        //			List<HatKoordinat> coordinates = jsonData.HatKoordinatlari;
        //			List<SadelestirilmisHatKoordinat> simplifiedCoordinates = jsonData.SadelestirilmisHat;

        //			List<(double x, double y)> jsonList = new();
        //			List<(double x, double y)> jsonPureList = new();

        //			foreach (var coordinate in coordinates)
        //			{
        //				jsonList.Add((coordinate.X, coordinate.Y));
        //			}
        //			foreach (var simplifiedCoordinate in simplifiedCoordinates)
        //			{
        //				jsonPureList.Add((simplifiedCoordinate.X, simplifiedCoordinate.Y));
        //			}
        //                  List<(double x, double y)> pureList = GeoSimplifier.LineSimplificator.SimplifyPoints(jsonList, 0, jsonList.Count - 1, 100);
        //                  ViewBag.listCount = pureList.Count;
        //                  //  BasicNotification("Anasayfaya yönlendiriliyorsunuz...", NotificationType.Success, "Dosya Başarıyla yüklendi!");
        //                  pureList.Add((jsonList[jsonList.Count() - 1].x, jsonList[jsonList.Count() - 1].y));
        //                  // (double, double)[] points = pureList.ToArray();

        //                  ViewBag.PureListJson = JsonConvert.SerializeObject(pureList);

        //                  return View(pureList);
        //              }

        //	}
        //	return View();
        //}

        [HttpPost]
        public ActionResult UploadJson(IFormFile jsonFile)
        {
            if (jsonFile != null && jsonFile.Length > 0)
            {
                using (var reader = new StreamReader(jsonFile.OpenReadStream()))
                {
                    string jsonContent = reader.ReadToEnd();

                    var jsonData = JsonConvert.DeserializeObject<JsonDataModel>(jsonContent);

                    List<HatKoordinat> coordinates = jsonData.HatKoordinatlari;
                    List<SadelestirilmisHatKoordinat> simplifiedCoordinates = jsonData.SadelestirilmisHat;

                    List<(double x, double y)> jsonList = new();
                    List<(double x, double y)> jsonPureList = new();

                    foreach (var coordinate in coordinates)
                    {
                      
                        jsonList.Add((coordinate.X, coordinate.Y));
                    }
                    foreach (var simplifiedCoordinate in simplifiedCoordinates)
                    {
                        jsonPureList.Add((simplifiedCoordinate.X, simplifiedCoordinate.Y));
                    }
                    List<(double x, double y)> pureList = GeoSimplifier.LineSimplificator.SimplifyPoints(jsonList, 0, jsonList.Count - 1, 100);
                    pureList.Add((jsonList[jsonList.Count() - 1].x, jsonList[jsonList.Count() - 1].y));
                    ViewBag.listCount = pureList.Count;

                    // JavaScript tarafına doğrudan pureList verilerini gönderin

                    ViewBag.pureListJson = JsonConvert.SerializeObject(pureList);
                    //  BasicNotification("Anasayfaya yönlendiriliyorsunuz...", NotificationType.Success, "Dosya Başarıyla yüklendi!");
                    ViewBag.pureList = JsonConvert.SerializeObject(pureList);

                    // Verileri işleme başarılı olduğunda, ViewMap metoduna yönlendirelim.
                    TempData["pureList"] = JsonConvert.SerializeObject(pureList);

                    // Redirect to ViewMap action
                    return RedirectToAction("LineSimplificationMap");



                }

            }
         
            return View();

        }
        public IActionResult Line3D()
        {
            return View();        }

        public ActionResult ViewMap()
        {

            var pureListData = TempData["pureList"] as string;

            // Pass the pureList data to the view using ViewBag
            ViewBag.PureListJson = pureListData;

            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}