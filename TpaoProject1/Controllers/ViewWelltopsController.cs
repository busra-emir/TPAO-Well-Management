﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using TpaoProject1.Areas.Identity.Data;
using TpaoProject1.Data;
using TpaoProject1.Model;

namespace TpaoProject1.Controllers
{
    public class ViewWelltopsController : Controller
    {
        private readonly DatabaseContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly MapsGeocodingService _geocodingService;


        public ViewWelltopsController(DatabaseContext dbContext, UserManager<ApplicationUser> userManager, MapsGeocodingService geocodingService)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _geocodingService = geocodingService;

        }


        [HttpGet]
        public async Task<IActionResult> AddWellTop()
        {
			ViewBag.ActionName = "AddWellTop";
			ViewBag.ButtonText = "Kaydet";

			return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddWellTop(WellTop model)
        {
			ViewBag.ActionName = "AddWellTop";
			ViewBag.ButtonText = "Kaydet";
			var WellTopList = _dbContext.WellTops.ToList();


			
			// WellTop well = new WellTop();
			if (ModelState.IsValid)
            {
                double lati = double.Parse(model.Latitude);
                double longi = double.Parse(model.Longitude);
                string apiKey = "AIzaSyDU_pWP66-BTzvW7AnEcQRSaBPutMzWxU4";

                string geocodingData = await _geocodingService.GetGeocodingData(lati, longi, apiKey);
                if (!string.IsNullOrEmpty(geocodingData))
                {
                    JObject jsonObject = JObject.Parse(geocodingData);
                    string city = jsonObject["results"][5]["address_components"]
                                         .FirstOrDefault(c => c["types"].Any(t => t.ToString() == "locality" || t.ToString() == "administrative_area_level_1"))?["long_name"]?.ToString();

                    model.City = city;
                }

                var user = await _userManager.GetUserAsync(User);
                
                model.UserId = user.Id;
                
                var numUserId= model.UserId.ToString();

                var Name = model.Name;
                var Latitude = model.Latitude;
                var Longitude = model.Longitude;
                var WellTopType = model.WellTopType;
                var City = model.City;
                var InsertionDate = DateTime.Now;
                var UpdateDate = DateTime.Now;

                WellTop welltop = new WellTop{ UserId = numUserId, Name = Name, Latitude = Latitude, Longitude = Longitude, WellTopType = WellTopType, City = City, InsertionDate = InsertionDate, UpdateDate = UpdateDate };
                var name = welltop.Name;
                var longitude = welltop.Longitude;
                var latitude = welltop.Latitude;

				if (!IsNameExists(name) && !IsLocationExists(longitude, latitude))
				{
					_dbContext.WellTops.Add(welltop);
				}
                else
                {
                    // Alert eklenecek.
					return View();
				}
				//_dbContext.SaveChanges();
				await _dbContext.SaveChangesAsync();
               
                
                return RedirectToAction("MainPage", "ViewWelltops");
            }

            return View();
        }
		public bool IsNameExists(string name)
		{
			return _dbContext.WellTops.Any(u => u.Name == name);
		}

		public bool IsLocationExists(string longitude, string latitude)
		{
			return _dbContext.WellTops.Any(u => u.Latitude == latitude) && _dbContext.WellTops.Any(u => u.Longitude == longitude);
		}

		public async Task<IActionResult> MainPage()
        {


            //var WellTopList= _dbContext.WellTops.ToList();
            var user = await _userManager.GetUserAsync(User);
            var WellTopList = _dbContext.WellTops.Where(w => w.UserId == user.Id).ToList();

            return View(WellTopList);
        }

        
       public IActionResult Delete(int id)
        {
			ViewBag.ActionName = "Delete";
			ViewBag.ButtonText = "Sil";

			var kuyu = _dbContext.WellTops.Find(id);
            _dbContext.Remove(kuyu);
            _dbContext.SaveChanges();
			return RedirectToAction("MainPage", "ViewWelltops");
		}
        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            ViewBag.ActionName = "Update";
            ViewBag.ButtonText = "Güncelle";
			var kuyu = _dbContext.WellTops.Find(id);
            var updateUserId = kuyu.UserId;
            TempData["userid"] = updateUserId;

			return View(kuyu);
		}
        [HttpPost]
		public async Task<IActionResult> Update(WellTop welltop)
        {
            double lati = double.Parse(welltop.Latitude);
            double longi = double.Parse(welltop.Longitude);
            string apiKey = "AIzaSyDU_pWP66-BTzvW7AnEcQRSaBPutMzWxU4";

            string geocodingData = await _geocodingService.GetGeocodingData(lati, longi, apiKey);
            if (!string.IsNullOrEmpty(geocodingData))
            {
                JObject jsonObject = JObject.Parse(geocodingData);
                string city = jsonObject["results"][5]["address_components"]
                                     .FirstOrDefault(c => c["types"].Any(t => t.ToString() == "locality" || t.ToString() == "administrative_area_level_1"))?["long_name"]?.ToString();

                welltop.City = city;
            }
            welltop.UserId = TempData["userid"].ToString();
			_dbContext.SaveChanges();
			return RedirectToAction("MainPage", "ViewWelltops");

		}



	}
}
