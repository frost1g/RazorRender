using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RazorRender;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : Controller
    {
        private readonly IRazorRenderService _razorRenderService;

        public ValuesController(IRazorRenderService razorRenderService)
        {
            _razorRenderService = razorRenderService;
        }

        [HttpGet]
        public async Task<ActionResult> GetAsync()
        {
            var helloWord = new HelloWord();
            helloWord.Username = "Alex";
            helloWord.Date = DateTime.Now;
            var html = await _razorRenderService.ViewToStringAsync("HelloWordView.cshtml", helloWord);

            return Ok(html);
        }
    }
}
