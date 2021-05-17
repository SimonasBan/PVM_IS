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
        public class ClientRoute_Route
        {
            public int ClientRoute_Id { get; set; }
            public int? Length { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public double? Rating { get; set; }
            public string Start_date { get; set; }
            public string Finish_date { get; set; }
            public int State_Id { get; set; }
        }
        public async Task<IActionResult> OpenFavouriteRoutes()
        {
            //ViewBag.places = _context.PlaceOfInterest.ToList();
            //var clientRoutes = _context.ClientRoute.ToList();
            //ViewBag.clientRoutes = clientRoutes;
            //var routeData = new List<ClientRoute_Route>();

            var clientRoutesWithRoutes =
                from cRoute in _context.ClientRoute
                join route in _context.Route on cRoute.Route_id equals route.Id
                select new ClientRoute_Route
                {
                    ClientRoute_Id = cRoute.Id,
                    Length = route.Length,
                    Name = route.Name,
                    Description = route.Description,
                    Rating = Math.Round((double)route.Rating, 2),
                    Start_date = cRoute.Start_date.ToString(),
                    Finish_date = cRoute.Finish_date.ToString(),
                    State_Id = cRoute.State_Id
                };
            var clientRoutesList = clientRoutesWithRoutes.ToList();
            ViewBag.clientRoutes = clientRoutesList;
            return View();
        }

        public async Task<IActionResult> SubmitRouteInfo(int clientRouteId)
        {
            bool forecastRes = await GetWeatherForecastByDate(clientRouteId);

            return RedirectToAction("OpenFavouriteRoutes");
        }
        public async Task<IActionResult> StartRoute(int id)
        {
            ClientRoute clientRoute = _context.ClientRoute.Find(id);
            clientRoute.State_Id = 2;
            try
            {
                _context.ClientRoute.Update(clientRoute);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return NotFound();
                throw;
            }
            return RedirectToAction("PlaceOfInterestView", new { id = id });
        }
        public async Task<IActionResult> ContinueRoute(int id)
        {
            return RedirectToAction("PlaceOfInterestView", new { id = id });
        }
        public async Task<IActionResult> PlaceOfInterestView(int id)
        {
            var clientRoute = _context.ClientRoute.Find(id);
            var route = _context.Route.Find(clientRoute.Route_id);
            var route_place = _context.Route_PlaceOfInterest
                .Where(p => p.Route_id == route.Id)
                .Where(p => p.Number == clientRoute.CurrentNumber).FirstOrDefault();
            var place = _context.PlaceOfInterest.Find(route_place.PlaceOfInterest_id);

            ViewBag.place = place;
            ViewBag.route = route;
            ViewBag.Id = id;
            //var currClientRoute = _context.ClientRoute.Find(id);
            //var currRoute = _context.Route.Find(currClientRoute.Route_id);

            //var ClientOrienGame = _context.ClientOrientationGame.Find(id);

            //ViewBag.clientGame = ClientOrienGame.OrientationGame_Id;
            //var OrienRiddle = _context.OrientationGame_Riddle
            //    .Where(p => p.OrientationGame_Id == ClientOrienGame.OrientationGame_Id)
            //    .Where(p => p.Number == ClientOrienGame.CurrentNumber).FirstOrDefault();

            //var Riddle = _context.Riddle.Find(OrienRiddle.Riddle_Id);

            //var Place = _context.PlaceOfInterest.Find(Riddle.PlaceOfInterest_Id);

            //ViewBag.id = id;
            //ViewBag.Riddle = Riddle;
            //ViewBag.Place = Place;
            //return View();
            return View();
        }
        public class LocationDetails_IpApi
        {
            public string query { get; set; }
            public string city { get; set; }
            public string country { get; set; }
            public string countryCode { get; set; }
            public string isp { get; set; }
            public double lat { get; set; }
            public double lon { get; set; }
            public string org { get; set; }
            public string region { get; set; }
            public string regionName { get; set; }
            public string status { get; set; }
            public string timezone { get; set; }
            public string zip { get; set; }
        }

        public static double DistanceTo(double lat1, double lon1, double lat2, double lon2, char unit = 'K')
        {
            double rlat1 = Math.PI * lat1 / 180;
            double rlat2 = Math.PI * lat2 / 180;
            double theta = lon1 - lon2;
            double rtheta = Math.PI * theta / 180;
            double dist =
                Math.Sin(rlat1) * Math.Sin(rlat2) + Math.Cos(rlat1) *
                Math.Cos(rlat2) * Math.Cos(rtheta);
            dist = Math.Acos(dist);
            dist = dist * 180 / Math.PI;
            dist = dist * 60 * 1.1515;

            switch (unit)
            {
                case 'K': //Kilometers -> default
                    return dist * 1.609344;
                case 'N': //Nautical Miles 
                    return dist * 0.8684;
                case 'M': //Miles
                    return dist;
            }

            return dist;
        }

        public async Task<IActionResult> ApproveVisit(int id)
        {
            var clientRoute = _context.ClientRoute.Find(id);

            var route_PlaceOfInterestTemp = _context.Route_PlaceOfInterest
                .Where(p => p.Route_id == clientRoute.Route_id)
                .Where(p => clientRoute.CurrentNumber == p.Number).FirstOrDefault();

            var currentPlace = _context.PlaceOfInterest.Find(route_PlaceOfInterestTemp.PlaceOfInterest_id);

            clientRoute.CurrentNumber += 1;

            var nextPlace = _context.Route_PlaceOfInterest
                .Where(p => p.Route_id == clientRoute.Route_id)
                .Where(p => clientRoute.CurrentNumber == p.Number).FirstOrDefault();

            var remoteIpAddress = Request.HttpContext.Connection.RemoteIpAddress;
            var Ip_Api_Url = "http://ip-api.com/json/" + "88.222.14.81"; //remoteIpAddress.ToString();
            string geoLocationInfo = "{ \"status\": \"fail\" }";
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                // Pass API address to get the Geolocation details 
                httpClient.BaseAddress = new Uri(Ip_Api_Url);
                HttpResponseMessage httpResponse = httpClient.GetAsync(Ip_Api_Url).GetAwaiter().GetResult();
                // If API is success and receive the response, then get the location details
                if (httpResponse.IsSuccessStatusCode)
                {
                    geoLocationInfo = httpResponse.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                }
            }
            var geoJson = JObject.Parse(geoLocationInfo);
            double userLatitude = geoJson.SelectToken("lat").Value<double>();
            double userLongitude = geoJson.SelectToken("lon").Value<double>();

            var routePlaceCoords = currentPlace.Koordinates;
            var placeCoords = routePlaceCoords.Split(',');
            double placeLatitude = Double.Parse(placeCoords[0]);
            double placeLongitude = Double.Parse(placeCoords[1]);

            var distance = DistanceTo(userLatitude, userLongitude, placeLatitude, placeLongitude);
            if (distance > 1)
            {
                TempData["ErrorMessage"] = "Esate per toli nuo objekto! " + " Esate nutole " + Math.Round(distance, 2).ToString() + " km nuo vietos!";
                return RedirectToAction("PlaceOfInterestView", new { id = id });
            }
            else
            {
                TempData["SuccessMessage"] = "Aplankymas uzskaitytas! " + " Esate nutole " + Math.Round(distance, 2).ToString() + " km nuo vietos!";
            }
            

            if (nextPlace == null)
            {
                clientRoute.CurrentNumber -= 1;
                clientRoute.State_Id = 3;
                try
                {
                    _context.ClientRoute.Update(clientRoute);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    return NotFound();
                    throw;
                }
                return RedirectToAction("OpenFavouriteRoutes");
            }
            try
            {
                _context.ClientRoute.Update(clientRoute);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return NotFound();
                throw;
            }

            TempData["BANDOM"] = distance.ToString();
            return RedirectToAction("PlaceOfInterestView", new { id = id });
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
            } else if (currClientRoute.Calendar_date < DateTime.Today)
            {
                TempData["ErrorMessage"] = "Nepavyko gauti orų. Data jau praėjusi.";
                return false;
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

            if (firstForecastList.Count == 0)
            {
                TempData["ErrorMessage"] = "Nepavyko gauti orų.";
                return false;
            }

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
