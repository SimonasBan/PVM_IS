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
            public DateTime? Calendar_date { get; set; }
            public string? Route_Item { get; set; }
            public int? Route_Id { get; set; }
            public int? CurrentNumber { get; set; }
            public int? Item_id { get; set; }
        }
        public class PlaceOfInterestAndOrder
        {
            public string Pavadinimas { get; set; }
            public string? Aprasymas { get; set; }
            public string? Miestas { get; set; }
            public string? Savivaldybe { get; set; }
            public string? Koordinates { get; set; }
            public string? Adresas { get; set; }
            public double? Bilieto_kaina { get; set; }
            public int? Taskai { get; set; }
            public int? Eile { get; set; }
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
                    State_Id = cRoute.State_Id,
                    Route_Id = route.Id
                };
            var clientRoutesList = clientRoutesWithRoutes.ToList();
            ViewBag.clientRoutes = clientRoutesList;
            return View();
        }
        public async Task<IActionResult> OpenClientRouteList()
        {            
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
        public async Task<IActionResult> OpenRouteObjects()
        {
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
        public async Task<IActionResult> OpenItemList(int id)
        {
            //var clientRoutesWithRoutes =
            //    from cRoute in _context.ClientRoute
            //    join items in _context.PersonalRouteItem on cRoute.ID equals item.fk_client_id
            //    select new Item
            //    {
            //        
            //    };
            //var clientRoutesList = clientRoutesWithRoutes.ToList();
            //ViewBag.clientRoutes = clientRoutesList;
            //var clientRoute = _context.ClientRoute.Find(id);
            //var route = _context.Route.Find(clientRoute.Route_id);
            //var route_place = _context.PersonalRouteItem
            //    .Where(p => p.Id == route.Id);
            var clientRoute = _context.ClientRoute.Find(id);
            var route = _context.Route.Find(clientRoute.Route_id);
            var route_place = _context.PersonalRouteItem
                .Where(p => p.userRoute_id == clientRoute.Id);


            

            ViewBag.personalItemList = route_place.ToList();
            return View();
        }
        public async Task<IActionResult> OpenRouteObjectss(int id)
        {
           
            var route = _context.Route.Find(id);
            var route_place = _context.Route_PlaceOfInterest
                .Where(p => p.Route_id == route.Id);

            var duomenukai =
                from routePlaceOfIn in _context.Route_PlaceOfInterest
                join placeOfIn in _context.PlaceOfInterest on routePlaceOfIn.PlaceOfInterest_id equals placeOfIn.Id
                where routePlaceOfIn.Route_id == id
                orderby routePlaceOfIn.Number
                select new PlaceOfInterestAndOrder
                {
                    Pavadinimas = placeOfIn.Pavadinimas,
                    Aprasymas = placeOfIn.Aprasymas,
                    Miestas = placeOfIn.Miestas,
                    Adresas = placeOfIn.Adresas,
                    Bilieto_kaina = placeOfIn.Bilieto_kaina,
                    Savivaldybe = placeOfIn.Savivaldybe,
                    Koordinates = placeOfIn.Koordinates,
                    Taskai = placeOfIn.Taskai,
                    Eile= routePlaceOfIn.Number
                };

            ViewBag.personalRoutePlaceOfInterestList = duomenukai.OrderBy(o=>o.Eile).ToList();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveEditedRoute(int id, [Bind("Start_date, Finish_date, State_Id, Calendar_date, Route_Id," +
            "CurrentNumber")] ClientRoute place)
        {
            ClientRoute oldPlace = _context.ClientRoute.Find(id);
            //if (oldRoute.FkRegistruotasVartotojas != int.Parse(_signInManager.UserManager.GetUserId(User)))
            //{
            //    return NotFound();
            //}
            oldPlace.Start_date = place.Start_date;
            oldPlace.Finish_date = place.Finish_date;
            oldPlace.State_Id = place.State_Id;
            oldPlace.Calendar_date = place.Calendar_date;
            oldPlace.Route_id = place.Route_id;
            oldPlace.CurrentNumber = place.CurrentNumber;

            //if (!ModelState.IsValid)
            //{
            //    ViewBag.LaikoIverciai = _context.LaikoIverciai.ToList();
            //    return View("~/Views/Routes/EditRouteDescription.cshtml");
            //}
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Neužpildėte visų laukų";
                return RedirectToAction("EditPlaceOfInterest", new { id = id });
            }


            try
            {
                _context.Update(oldPlace);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return NotFound();
                throw;
            }
            TempData["SuccessMessage"] = "Objektas užsaugotas";
            return RedirectToAction("OpenClientRouteList");
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
        public async Task<IActionResult> EditPersonalRoute(int id)
        {
            //ClientRoute personalRouteToEdit = _context.ClientRoute.Find(id);
            //ViewBag.PlaceToEdit = personalRouteToEdit;
            var clientRouteImportantItem =
                from route in _context.Route
                join cRoute in _context.ClientRoute on route.Id equals cRoute.Route_id
                join itemRoute in _context.PersonalRouteItem on cRoute.Item_id equals itemRoute.Id
                where route.Id == id
                select new ClientRoute_Route
                {
                    Item_id = itemRoute.Id
                };
            ViewBag.clientItemEdit =  clientRouteImportantItem.ToList();


            var clientRoutesWithRoutes =
                from cRoute in _context.ClientRoute
                join route in _context.Route on cRoute.Route_id equals route.Id
                where cRoute.Id == id
                select new ClientRoute_Route
                {
                    ClientRoute_Id = cRoute.Id,
                    Length = route.Length,
                    Name = route.Name,
                    Description = route.Description,
                    Rating = Math.Round((double)route.Rating, 2),
                    Start_date = cRoute.Start_date.ToString(),
                    Finish_date = cRoute.Finish_date.ToString(),
                    State_Id = cRoute.State_Id,
                    Calendar_date = cRoute.Calendar_date,
                    Route_Id=route.Id,
                    CurrentNumber=cRoute.CurrentNumber,
                };
            var clientRoutesList = clientRoutesWithRoutes.ToList();
            ViewBag.clientRouteToEdit = clientRoutesList;
            return View();
        }
        public async Task<IActionResult> TryDeleteClientRoute(int id)
        {
            ViewBag.ClientRouteID = id;
            return View();
        }
        public async Task<IActionResult> ViewPersonalRoute(int id)
        {
            //ViewBag.ClientRouteID = id;
            var clientRoutesWithRoutes =
                from route in _context.Route
                join cRoute in _context.ClientRoute on route.Id equals cRoute.Route_id
                where route.Id == cRoute.Route_id
                select new ClientRoute_Route
                {
                    ClientRoute_Id = cRoute.Id,
                    Length = route.Length,
                    Name = route.Name,
                    Description = route.Description,
                    Rating = Math.Round((double)route.Rating, 2),
                    Start_date = cRoute.Start_date.ToString(),
                    Finish_date = cRoute.Finish_date.ToString(),
                    State_Id = cRoute.State_Id,
                    Calendar_date = cRoute.Calendar_date
                };
            var clientRoutesList = clientRoutesWithRoutes.ToList();
            ViewBag.clientRW = clientRoutesList;

            var clientRouteImportantItem =
                from route in _context.Route
                join cRoute in _context.ClientRoute on route.Id equals cRoute.Route_id
                join itemRoute in _context.PersonalRouteItem on cRoute.Item_id equals itemRoute.Id
                where route.Id == id
                select new ClientRoute_Route
                {
                    Route_Item = itemRoute.Item
                };
            var itemList = clientRouteImportantItem.ToList();
            ViewBag.clientItem = itemList;



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
