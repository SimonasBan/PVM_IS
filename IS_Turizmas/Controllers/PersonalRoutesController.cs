using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using IS_Turizmas.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IS_Turizmas.Controllers
{
    public class PersonalRoutesController : HomeController
    {
        private readonly ApplicationDbContext _context;

        public PersonalRoutesController(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IActionResult> OpenFavouriteRoutes()
        {
            //ViewBag.places = _context.PlaceOfInterest.ToList();
            
            return View();
        }

        public async Task<IActionResult> SubmitRouteInfo(int clientRouteId)
        {
            bool forecastRes = await GetWeatherForecastByDate(clientRouteId);

            return RedirectToAction("OpenFavouriteRoutes");
        }

        /*
         * True if everything worked correctly.
         * False if anything failed.
         */
        public async Task<bool> GetWeatherForecastByDate(int clientRouteId)
        {
            var currClientRoute = _context.ClientRoute.Find(clientRouteId);

            var currRoute = _context.ClientRoute
                .Include(o => o.Route_idNavigation)
                .Where(o => o.Id == clientRouteId)
                .Select(o => o.Route_idNavigation)
                .FirstOrDefault();

            var routePlacesCoords = _context.Route_PlaceOfInterest
                .Include(o => o.Route_idNavigation)
                .Include(o => o.PlaceOfInterest_idNavigation)
                .Where(o => o.Route_idNavigation.Id == currRoute.Id)
                .Select(o => o.PlaceOfInterest_idNavigation.Koordinates).ToArray();

            bool needsUmbrella = false;

            //--------For first place object--------
            var firstPlaceCoords = routePlacesCoords[0].Split(',');
            var firstLat = firstPlaceCoords[0];
            var firstLong = firstPlaceCoords[1];

            JObject firstPlaceForecast = new ClientController(_context).GetPlaceForecast(firstLat, firstLong).Result;

            //Checks if it's possible to get weather forecast for the required calendar date
            var lastDate = (string)firstPlaceForecast["list"][39]["dt_txt"];
            DateTime lastDateTime = DateTime.Parse(lastDate.Split(' ')[0]);
            if (lastDateTime < currClientRoute.Calendar_date)
            {
                TempData["ErrorMessage"] = "Nepavyko gauti orų. Per tolima data.";
                return false;
                //return RedirectToAction("OpenFavouriteRoutes");
            }

            var firstForecastList = firstPlaceForecast["list"]
                .Select(o =>
                {
                    if (DateTime.Parse((string)o["dt_txt"]).DayOfYear == currClientRoute.Calendar_date.Value.DayOfYear)
                    {
                        return o;
                    }
                    else
                    {
                        return null;
                    }
                    
                })
                .Where(jo => jo != null)
                .ToList();

            firstForecastList[0]["weather"][0]["main"] = "rain";


            if (firstForecastList.Any(o => (string)o["weather"][0]["main"] == "rain"))
            {
                needsUmbrella = true;
            }

            //----others----
            foreach (var place in routePlacesCoords.Skip(1))
            {
                var tempCoords = place.Split(',');
                var tempLat = tempCoords[0];
                var tempLong = tempCoords[1];


                JObject tempPlaceForecast = new ClientController(_context).GetPlaceForecast(tempLat, tempLong).Result;

                var tempForecastList = tempPlaceForecast["list"]
                    .Select(o =>
                    {
                        if (DateTime.Parse((string)o["dt_txt"]).DayOfYear == currClientRoute.Calendar_date.Value.DayOfYear)
                        {
                            return o;
                        }
                        else
                        {
                            return null;
                        }

                    })
                    .Where(jo => jo != null)
                    .ToList();


                if (tempForecastList.Any(o => (string)o["weather"][0]["main"] == "rain"))
                {
                    needsUmbrella = true;
                }
            }

            if (needsUmbrella)
            {
                currClientRoute.Item_id = 1;
                try
                {
                    _context.Update(currClientRoute);
                    await _context.SaveChangesAsync();                    
                }
                catch (DbUpdateConcurrencyException)
                {
                    return false;
                    throw;
                }
            }
            return true;
        }
    }
}
