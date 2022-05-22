using Fooreco.CAP;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using Fooreco.Cap.Common.Demo;

namespace Fooreco.Cap.Client.Demo.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ICapPublisher _publisher;

        public IndexModel(ICapPublisher publisher)
        {
            _publisher = publisher;
        }

        public void OnGet()
        {

        }

        public void OnPostSchedule()
        {
            var command = new SendEmailCommand();
            _publisher.Publish(command.Name, command, scheduleDate: DateTime.UtcNow.AddSeconds(30));
        }
    }
}
