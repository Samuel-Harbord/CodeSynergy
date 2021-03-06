﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using CodeSynergy.Models;
using CodeSynergy.Data;
using CodeSynergy.Models.Repositories;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace CodeSynergy.Controllers
{
    public class StarredController : Controller
    {
        private readonly UserRepository _users;
        private readonly StarRepository _stars;

        public StarredController(UserStore<ApplicationUser, IdentityRole<string>, ApplicationDbContext, string> users, IJoinTableRepository<Star, string, int> stars) : base()
        {
            ApplicationDbContext _context = new ApplicationDbContext();
            _users = (UserRepository) users;
            _stars = (StarRepository) stars;
        }

        // Loaded starred questions page
        // GET: /Starred/
        [HttpGet]
        public IActionResult Index(string Modal)
        {
            // If the user is not logged in, redirect them to the homepage with a login modal
            if (HttpContext.User.Identity.Name == null)
                return Redirect("/?modal=Account/Login");

            ViewData["Modal"] = Modal;

            return View();
        }

        // Starred questions grid loaded
        // GET: /Starred/StarredQuestionsGrid
        [HttpGet]
        public async Task<IActionResult> StarredQuestionGrid(string ColumnIndex = "-1", string SortAsc = "false", string UseSearchGrid = "false")
        {
            ApplicationUser user = await _users.FindByEmailAsync(Request.HttpContext.User.Identity.Name);
            int columnIndex = -1;
            bool sortAsc = false;
            bool useSearchGrid = false;
            bool.TryParse(UseSearchGrid, out useSearchGrid);
            if (useSearchGrid)
            {
                int.TryParse(ColumnIndex, out columnIndex);
                bool.TryParse(SortAsc, out sortAsc);
            }
            IEnumerable<Star> starList = _stars.GetAllForUser(user);
            ViewData["columnIndex"] = columnIndex;
            ViewData["sortAsc"] = sortAsc;
            ViewData["useSearchGrid"] = useSearchGrid;

            if (!useSearchGrid && columnIndex == -1)
                starList = starList.OrderByDescending(s => s.StarredDate);

            return PartialView("MvcGrid/_StarredQuestionGrid", starList);
        }
    }
}
