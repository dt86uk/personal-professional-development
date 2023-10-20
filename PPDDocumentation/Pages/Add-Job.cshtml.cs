using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PPDDocumentation.BusinessLogic;
using PPDDocumentation.Models.Job;
using PPDDocumentation.Models.Requests;

namespace PPDDocumentation.Pages
{
    public class AddJobModel : PageModel
    {
        private readonly ILogger<AddJobModel> _logger;
        private readonly IJobService _jobService;

        public AddJobModel(ILogger<AddJobModel> logger, IJobService jobService)
        {
            _logger = logger;
            _jobService = jobService;
        }

        [BindProperty]
        public JobModel? NewJob { get; set; }

        public void OnGet()
        {
        }

        public IActionResult OnPostAddJob()
        {
            if (!ModelState.IsValid || NewJob == null)
            {
                return Page();
            }

            var request = new JobRequest
            {
                Id = Guid.NewGuid(),
                Job = new JobModel(Guid.NewGuid())
                {
                    Title = NewJob.Title,
                    Description = NewJob.Description
                }
            };

            _logger.LogInformation($"Add Job Info: Request to add Job ('{NewJob.Title}') was called.");
            var response = _jobService.AddJob(request);

            if (!response.IsSuccess)
            {
                _logger.LogError($"Add Job Error: Request to add Job ('{NewJob.Title}') failed.");
                foreach (var error in response.ErrorMessages)
                {
                    ModelState.AddModelError(string.Empty, error);
                }

                return Page();
            }

            _logger.LogInformation($"Add Job Info: Request to add Job ('{NewJob.Title}') completed.");
            return RedirectToPage("./jobs");
        }
    }
}
