﻿using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace JovemProgramadorMvc.Controllers
{
    public class FiltrosController : Controller
    {
        public FiltrosController()
        {

        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
