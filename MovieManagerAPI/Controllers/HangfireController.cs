using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MovieManagerAPI.Data.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieManagerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HangfireController : ControllerBase
    {
        [HttpPost("welcome")]
        public IActionResult Welcome()
        {
            var jobId = BackgroundJob.Enqueue(() => MailsService.SendMail("Welcome to our app!"));

            return Ok($"Job Id: {jobId}. Welcome email sent to the user!");
        }

        [HttpPost("discount")]
        public IActionResult Discount()
        {
            int timeInSeconds = 30;
            var jobId = BackgroundJob.Schedule(() => MailsService.SendMail("Discount for you!"), TimeSpan.FromSeconds(timeInSeconds));

            return Ok($"Job Id: {jobId}. Discount email will be sent in {timeInSeconds} seconds!");
        }

        [HttpPost("databaseUpdate")]
        public IActionResult DatabaseUpdate()
        {
            RecurringJob.AddOrUpdate(() => Console.WriteLine("Database updated"), Cron.Minutely);

            return Ok($"Database check job initialted");
        }

        [HttpPost("confirm")]
        public IActionResult Confirm()
        {
            int timeInSeconds = 30;
            var parentJobId = BackgroundJob.Schedule(() => Console.WriteLine("You asked to be unsubscribed!"), TimeSpan.FromSeconds(timeInSeconds));

            BackgroundJob.ContinueJobWith(parentJobId, () => Console.WriteLine("You were unsubscribed!"));

            return Ok("Confirmation job created");
        }
    }
}
