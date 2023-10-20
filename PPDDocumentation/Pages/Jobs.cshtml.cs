using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PPDDocumentation.BusinessLogic;
using PPDDocumentation.Models.Job;

namespace PPDDocumentation.Pages
{
    public class JobsModel : PageModel
    {
        private readonly ILogger<JobsModel> _logger;
        private readonly IJobService _jobService;
        public List<JobModel> Jobs { get; set; }

        [TempData]
        public string JobResponseMessage { get; set; }

		[TempData]
		public bool? JobUpdated { get; set; }

		public JobsModel(ILogger<JobsModel> logger, IJobService jobService)
        {
            _logger = logger;
            _jobService = jobService;
        }

        public void OnGet()
        {
            var jobs = _jobService.GetJobs();
            Jobs = jobs.ToList();
        }
    }
}
