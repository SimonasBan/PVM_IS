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
    public class RoutesAndPlacesController : HomeController
    {
        private readonly ApplicationDbContext _context;

        public RoutesAndPlacesController(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IActionResult> OpenAllPlaces()
        {
            ViewBag.places = GetPlaces();
            return View("PlacesOfInterestView");
        }

        public async Task<IActionResult> OpenPlaceRating(int id)
        {
            ViewBag.id = id;
            return View("RatePlaceOfInterestView");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateComment(int id, [Bind("Comment, Rating")] PlaceOfInterestComment evaluation)
        {

            if(evaluation.Comment == null)
            {
                evaluation.Comment = "";
            }
            if(evaluation.Rating == null)
            {
                TempData["ErrorMessage"] = "Neužpildėte visų laukų";
                return RedirectToAction("OpenPlaceRating", new { id = id });
            }

            //if (!ModelState.IsValid)
            //{
            //    TempData["ErrorMessage"] = "Neužpildėte visų laukų";
            //    return RedirectToAction("OpenPlaceRating", new { id = id });
            //}

            try
            {
                evaluation.PlaceOfInterest_Id = id;
                _context.PlaceOfInterestComment.Add(evaluation);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return NotFound();
                throw;
            }

            TempData["SuccessMessage"] = "Vertinimas išsaugotas";
            return RedirectToAction("OpenAllPlaces");
        }

        private List<PlaceOfInterest> GetPlaces()
        {
            return _context.PlaceOfInterest.ToList();
        }

    }
}
