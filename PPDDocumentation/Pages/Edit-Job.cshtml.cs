using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PPDDocumentation.BusinessLogic;
using PPDDocumentation.Models.Job;

namespace PPDDocumentation.Pages
{
    public class EditJobModel : PageModel
    {
        private readonly ILogger<EditJobModel> _logger;
        private IJobService _jobService;

        public EditJobModel(ILogger<EditJobModel> logger, IJobService jobService)
        {
            _logger = logger;
            _jobService = jobService;
        }

		[ViewData]
		public bool? JobUpdated { get; set; }

		public void OnGet(Guid id)
        {
            _logger.LogInformation($"Edit Job Info: Request to get Job '{id}' at {DateTime.Now.ToShortDateString()} {DateTime.Now.ToShortTimeString()}");
            var jobResponse = _jobService.GetJobById(id);

            if (EditModel == null)
            {
                EditModel = new JobViewModel();
            }

            EditModel.Id = jobResponse.Job.Id;
            EditModel.Title = jobResponse.Job.Title;
            EditModel.Description = jobResponse.Job.Description;
            EditModel.IsComplete = jobResponse.Job.IsComplete;
            EditModel.IsDeleted = jobResponse.Job.IsDeleted;
        }

        [BindProperty]
        public JobViewModel? EditModel { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _logger.LogInformation($"Edit Job Info: Request to update Job ('{EditModel?.Id}') '{EditModel?.Title}' was called.");

            var job = new JobModel(EditModel.Id)
            {
                Title = EditModel.Title,
                Description = EditModel.Description,
                IsComplete = EditModel.IsComplete,
                IsDeleted = EditModel.IsDeleted
            };
            var response = _jobService.UpdateJob(job);

            if (!response.IsSuccess)
            {
                _logger.LogError($"Edit Job Error: Request to update Job ('{EditModel.Id}') '{EditModel.Title}' failed.");
                foreach (var error in response.ErrorMessages)
                {
                    ModelState.AddModelError(string.Empty, error);
                }

                return Page();
            }

            _logger.LogInformation($"Edit Job Info: Request to update Job ('{EditModel.Id}') '{EditModel.Title}' completed.");

            TempData["JobUpdated"] = true;
			return RedirectToPage("./jobs");
        }
    }
}
