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
    public class PlaceOfInterestController : HomeController
    {
        private readonly ApplicationDbContext _context;

        public PlaceOfInterestController(ApplicationDbContext context):base(context)
        {
            _context = context;
        }



        public async Task<IActionResult> OpenPlaces()
        {
            //Console.WriteLine("Called");
            //var userId = _signInManager.UserManager.GetUserId(User);
            //if (userId == null)
            //{
            //    return LocalRedirect("/");
            //}
            //int id = int.Parse(userId);
            //return View(await _context.Marsrutai.Include(o => o.MarsrutoObjektai).Where(o => o.FkRegistruotasVartotojas == id).ToListAsync());
            ViewBag.a = "aa";
            ViewBag.places = _context.PlaceOfInterest.ToList();
            return View();
        }

        public async Task<IActionResult> CreatePlaceOfInterest()
        {
            //ViewBag.places = _context.PlaceOfInterest.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveNewPlaceOfInterest([Bind("Pavadinimas, Aprasymas, Miestas, Adresas, Bilieto_kaina," +
            "Savivaldybe, Koordinates, Taskai")] PlaceOfInterest place)
        {


            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Neužpildėte visų laukų";
                return RedirectToAction("CreatePlaceOfInterest");
            }
            
            try
            {
                _context.PlaceOfInterest.Add(place);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return NotFound();
                throw;
            }

            TempData["SuccessMessage"] = "Objektas užsaugotas";
            return RedirectToAction("OpenPlaces");
        }

        

        public async Task<IActionResult> EditPlaceOfInterest(int id)
        {
            PlaceOfInterest placeToEdit = _context.PlaceOfInterest.Find(id);
            ViewBag.PlaceToEdit = placeToEdit;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveEditedPlaceOfInterest(int id, [Bind("Pavadinimas, Aprasymas, Miestas, Adresas, Bilieto_kaina," +
            "Savivaldybe, Koordinates, Taskai")] PlaceOfInterest place)
        {
            PlaceOfInterest oldPlace = _context.PlaceOfInterest.Find(id);
            //if (oldRoute.FkRegistruotasVartotojas != int.Parse(_signInManager.UserManager.GetUserId(User)))
            //{
            //    return NotFound();
            //}
            oldPlace.Pavadinimas = place.Pavadinimas;
            oldPlace.Aprasymas = place.Aprasymas;
            oldPlace.Miestas = place.Miestas;
            oldPlace.Adresas = place.Adresas;
            oldPlace.Bilieto_kaina = place.Bilieto_kaina;
            oldPlace.Savivaldybe = place.Savivaldybe;
            oldPlace.Koordinates = place.Koordinates;
            oldPlace.Taskai = place.Taskai;

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
            return RedirectToAction("OpenPlaces");
        }

        public async Task<IActionResult> TryDeletePlaceOfInterest(int id)
        {
            ViewBag.PlaceId = id;
            return View();
        }

        public async Task<IActionResult> DeletePlaceOfInterest(int id)
        {
            //if (_signInManager.IsSignedIn(User) && _signInManager.UserManager.GetUserId(User) == _context.Marsrutai.Find(id).FkRegistruotasVartotojas.ToString())

            _context.PlaceOfInterest.Remove(_context.PlaceOfInterest.Find(id));
            _context.SaveChanges();

            //TempData["SuccessMessage"] = "Maršrutas pašalintas";
            return RedirectToAction("OpenPlaces");
        }

        

    }
}
