﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SWM.Models;
using SWM.Models.Repositories;
using SWM.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SWM.Controllers.Web
{
    public class PublicController : Controller
    {
        private ISwmRepository _repo;
        private UserManager<SwmUser> _userManager;
        private SwmContext _ctx;

        public PublicController(ISwmRepository repo, UserManager<SwmUser> userManager, SwmContext ctx)
        {
            _repo = repo;
            _userManager = userManager;
            _ctx = ctx;
        }
        public IActionResult Index()
        {

            var random = new Random();
            var maxRepetitionsCount = 3;
            var numbers = Enumerable.Range(1, 100)
                   .SelectMany(i => Enumerable.Repeat(i, maxRepetitionsCount))
                   .OrderBy(i => random.Next())
                   .ToList();

            //for storing output
            List<int> output = new List<int>();
            //to keep track of numbers
            Dictionary<int, int> numberTrack = new Dictionary<int, int>();
            Random ran = new Random();
            //define max limit and range of numbers
            int maxLimit = 2, min = 1, max = 100 ;
            int no, count;
            //this for loop must run for any number less than max*maxLimit otherwise it will last for long time(may be forever). 
            for (int i = 0; i < max*maxLimit; i++)
            {
                //get random number.
                no = ran.Next(min, max+1);
                //check if random number exists in dictionary
                if (numberTrack.TryGetValue(no, out count))
                {
                    //if exists than check for it occurrences 
                    if (count >= maxLimit)
                    {
                        //if occurrence is greater than maxLimit continue for next number.
                        i--;
                        continue;
                    }
                    else
                    {
                        //else add number to output and update its occurrence count
                        numberTrack[no] += 1;
                        output.Add(no);
                    }
                }
                else
                {
                    //if random number does not exists in the dictionary than add it 
                    //with occurrence of 1 and also add it to the ouptput.
                    numberTrack.Add(no, 1);
                    output.Add(no);
                }
            }

            ViewBag.Title = "Public";
            return View();
        }

        [HttpPost]
        public IActionResult SearchUser(string FullName)
        {
            ViewBag.Title = "Search User";
            ViewBag.FullName = FullName;
            return View();
        }

        [Route("Public/User/{subId}")]
        public IActionResult UserDetails(string subId)
        {
            ViewBag.Title = "User Details";
            var result = _repo.GetUserDetails(subId);
            return View(result);
        }

        [Route("Public/UserDetails/{subId}")]
        public IActionResult MoreUserDetails(string subId)
        {
            try
            {
                ViewBag.Title = "More User Details";
                UserDetailsModel userDetail = _repo.GetUserDetailsLight(subId);
                return View(userDetail);
            }
            catch (Exception)
            {
                return Redirect("/Error/404");
            }
        }

        public IActionResult AdvanceSearch()
        {
            ViewBag.Title = "Advance Search";
            return View(new Tuple<AdvanceSearchModel, List<SearchUserModel>>(new AdvanceSearchModel(), new List<SearchUserModel>()));
        }

        public IActionResult ShowAllUsers()
        {
            ViewBag.Title = "All Users";
            return View();
        }
    }
}
