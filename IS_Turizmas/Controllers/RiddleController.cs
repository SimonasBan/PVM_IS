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
    public class RiddleController : HomeController
    {
        private readonly ApplicationDbContext _context;

        public RiddleController(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        //private enum ClientOption
        //{
        //    ToStart = 1,
        //    ToContinue = 2,
        //    NoneBecauseFinished = 3
        //}

        public async Task<IActionResult> OpenOrientationGames()
        {
            var orienGames = _context.OrientationGame.ToList();
            var clientOrienGames = _context.ClientOrientationGame.ToList()/*where user id matches*/;
            
            var states = new int[orienGames.Count];
            for (int i = 0; i < orienGames.Count; i++)
            {
                states[i] = 1; //ClientOption.ToStart;
                for (int j = 0; j < clientOrienGames.Count; j++)
                {
                    if (clientOrienGames[j].OrientationGame_Id == orienGames[i].Id)
                    {
                        if (clientOrienGames[j].State == ClientOrientationGameState.Ended)
                            states[i] = 3; //ClientOption.NoneBecauseFinished;
                        else
                            states[i] = 2; //ClientOption.ToContinue;
                        break;
                    }
                }
            }

            ViewBag.games = orienGames;
            ViewBag.states = states.ToList();
            return View();
        }
        public async Task<IActionResult> StartGame(int id)
        {
            ClientOrientationGame clientGame = new ClientOrientationGame();
            clientGame.OrientationGame_Id = id;
            clientGame.CurrentNumber = 1;
            clientGame.State = ClientOrientationGameState.InCodeSubmission;

            try
            {
                _context.ClientOrientationGame.Add(clientGame);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return NotFound();
                throw;
            }
            var ClientOrienGame = _context.ClientOrientationGame
                .Where(p => p.OrientationGame_Id == id /*p.Client_Id == Client_Id*/).FirstOrDefault();

            return RedirectToAction("ShowPlaceCode", new { id = ClientOrienGame.Id });
        }

        public async Task<IActionResult> ContinueGame(int id)
        {
            var ClientOrienGame = _context.ClientOrientationGame
                .Where(p => p.OrientationGame_Id == id /*p.Client_Id == Client_Id*/).FirstOrDefault();

            if (ClientOrienGame.State == ClientOrientationGameState.InCodeSubmission)
            {
                return RedirectToAction("ShowPlaceCode", new { id = ClientOrienGame.Id });
            }
            else // is in RiddleSubmission
            {
                return RedirectToAction("ShowRiddle", new { id = ClientOrienGame.Id });
            }
        }
        public async Task<IActionResult> ShowRiddle(int id)
        {
            var ClientOrienGame = _context.ClientOrientationGame.Find(id);

            ViewBag.clientGame = ClientOrienGame.OrientationGame_Id;
            var OrienRiddle = _context.OrientationGame_Riddle
                .Where(p => p.OrientationGame_Id == ClientOrienGame.OrientationGame_Id)
                .Where(p => p.Number == ClientOrienGame.CurrentNumber).FirstOrDefault();

            var Riddle = _context.Riddle.Find(OrienRiddle.Riddle_Id);

            var Place = _context.PlaceOfInterest.Find(Riddle.PlaceOfInterest_Id);

            ViewBag.id = id;
            ViewBag.Riddle = Riddle;
            ViewBag.Place = Place;
            return View();
        }

        public async Task<IActionResult> AnswerRiddle(int id, [Bind("Answer")] Riddle riddle)
        {
            var ClientOrienGame = _context.ClientOrientationGame.Find(id);

            ViewBag.clientGame = ClientOrienGame.OrientationGame_Id;
            var OrienRiddle = _context.OrientationGame_Riddle
                .Where(p => p.OrientationGame_Id == ClientOrienGame.OrientationGame_Id)
                .Where(p => p.Number == ClientOrienGame.CurrentNumber).FirstOrDefault();

            var Riddle = _context.Riddle.Find(OrienRiddle.Riddle_Id);

            if (Riddle.Answer == riddle.Answer)
            {
                ClientOrienGame.State = ClientOrientationGameState.InCodeSubmission;
                ClientOrienGame.CurrentNumber += 1;
                var NextOrienRiddle = _context.OrientationGame_Riddle
                    .Where(p => p.OrientationGame_Id == ClientOrienGame.OrientationGame_Id)
                    .Where(p => p.Number == ClientOrienGame.CurrentNumber).FirstOrDefault();
                if (NextOrienRiddle == null)
                {              
                    return RedirectToAction("OpenGameEndView", new { id = id });
                }
                try
                {
                    _context.ClientOrientationGame.Update(ClientOrienGame);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    return NotFound();
                    throw;
                }
                TempData["SucessMessage"] = "Teisingai įvėdėte lauką";
                return RedirectToAction("ShowPlaceCode", new { id = id });
            }
            else
            {
                TempData["ErrorMessage"] = "Neteisingai įvėdėte lauką";
                return RedirectToAction("ShowRiddle", new { id = id });
            }
        }

        public async Task<IActionResult> ShowPlaceCode(int id)
        {

            var ClientOrienGame = _context.ClientOrientationGame.Find(id);

            ViewBag.clientGame = ClientOrienGame.OrientationGame_Id;
            var OrienRiddle = _context.OrientationGame_Riddle
                .Where(p => p.OrientationGame_Id == ClientOrienGame.OrientationGame_Id)
                .Where(p => p.Number == ClientOrienGame.CurrentNumber).FirstOrDefault();
            var Riddle = _context.Riddle.Find(OrienRiddle.Riddle_Id);

            var Place = _context.PlaceOfInterest.Find(Riddle.PlaceOfInterest_Id);

            ViewBag.id = id;
            ViewBag.Riddle = Riddle;
            ViewBag.Place = Place;

            return View();
        }

        public async Task<IActionResult> AnswerPlaceCode(int id, [Bind("PlaceCodeAnswer")] Riddle riddle)
        {
            var ClientOrienGame = _context.ClientOrientationGame.Find(id);

            ViewBag.clientGame = ClientOrienGame.OrientationGame_Id;
            var OrienRiddle = _context.OrientationGame_Riddle
                .Where(p => p.OrientationGame_Id == ClientOrienGame.OrientationGame_Id)
                .Where(p => p.Number == ClientOrienGame.CurrentNumber).FirstOrDefault();

            var Riddle = _context.Riddle.Find(OrienRiddle.Riddle_Id);

            if (Riddle.PlaceCodeAnswer == riddle.PlaceCodeAnswer)
            {
                ClientOrienGame.State = ClientOrientationGameState.InRiddleSubmission;
                try
                {
                    _context.ClientOrientationGame.Update(ClientOrienGame);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    return NotFound();
                    throw;
                }
                TempData["SucessMessage"] = "Teisingai įvėdėte lauką";
                return RedirectToAction("ShowRiddle", new { id = id });
            }
            else
            {
                TempData["ErrorMessage"] = "Neteisingai įvėdėte lauką";
                return RedirectToAction("ShowPlaceCode", new { id = id });
            }
           

        }
        public async Task<IActionResult> OpenGameEndView(int id)
        {
            var ClientOrienGame = _context.ClientOrientationGame.Find(id);
            ClientOrienGame.State = ClientOrientationGameState.Ended;
            try
            {
                _context.ClientOrientationGame.Update(ClientOrienGame);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return NotFound();
                throw;
            }
            ViewBag.Points = _context.OrientationGame.Find(ClientOrienGame.OrientationGame_Id).Points_For_Completion;
            return View();
        }
    }
}
