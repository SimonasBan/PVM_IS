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
    public class ClientController : HomeController
    {
        private readonly ApplicationDbContext _context;

        public ClientController(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }


        public async Task<JObject> GetPlaceForecast(string latitude, string longtitude)
        {

            //TO DO
            //    UML pakeisti, kad geolocation
            //    jau anksciau paima
            HttpClient client = new HttpClient();

            string url = "http://api.openweathermap.org/data/2.5/forecast?";
            url += HttpUtility.UrlPathEncode("lat=" + latitude);
            url += HttpUtility.UrlPathEncode("&lon=" + longtitude + "&appid=c5f241ad2670a83cc3b38551c15cbd4f");

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(url)
            };

            var response = await client.SendAsync(request).ConfigureAwait(false);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                return null;
            }
            else
            {
                var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var responseJson = JObject.Parse(responseBody);
                return responseJson;
            }


        //http://api.openweathermap.org/data/2.5/forecast?lat=54.926220099999995&lon=23.6664645&appid=c5f241ad2670a83cc3b38551c15cbd4f
        }
    }
}
