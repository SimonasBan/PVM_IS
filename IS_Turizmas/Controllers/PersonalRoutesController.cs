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

        public async Task<IActionResult> GetWeatherForecastByDate(string latitude, string longtitude)
        {
            /*TO DO
             * Get all routes places and get a forecast
              for each place 
            -------
             set variable to get weather for specific clientRoute Id
             */
            var currClientRouteId = 1;

            //var currRoute = _context.Route_PlaceOfInterest.ToArray();

            var currRoute = _context.ClientRoute
                .Include(o => o.Route_idNavigation)
                .Where(o => o.Id == currClientRouteId)
                .Select(o => o.Route_idNavigation)
                .FirstOrDefault();

            var currPlaces = _context.Route_PlaceOfInterest
                .Include(o => o.Route_idNavigation)
                .Include(o => o.PlaceOfInterest_idNavigation)
                .Where(o => o.Route_idNavigation.Id == currRoute.Id)
                .Select(o => o.PlaceOfInterest_idNavigation.Koordinates).ToArray();




            //var places = _context.Route_PlaceOfInterest
            //    .Include(o => o.PlaceOfInterest_idNavigation)
            //    .Include(o => o.Route_idNavigation)
            //    .Where(o => o.)
            //.Include(o => o.RouteNavigation).Include(o => o.)   .MarsrutoObjektai.Include(o => o.FkLankytinasObjektasNavigation)
            //    .Where(o => o.FkMarsrutas == id).OrderBy(o => o.EilesNr).Select(o => o.FkLankytinasObjektasNavigation.Pavadinimas).ToArray();

            //var p = _context.MarsrutoObjektai.Include(o => o.FkLankytinasObjektasNavigation)
            //    .Where(o => o.FkMarsrutas == id).OrderBy(o => o.EilesNr).Select(o => o.FkLankytinasObjektasNavigation.Pavadinimas).ToArray();

            var a = latitude;

            JObject forecast = new ClientController(_context).GetPlaceForecast(latitude, longtitude).Result;

            var f = (string)forecast["list"][0]["weather"][0]["main"];

            /*TO DO
             * Update weather forecast in ClientRoute entity*/

            TempData["SuccessMessage"] = "Orai: " + f;
            return RedirectToAction("OpenFavouriteRoutes");
        }
    }
}
