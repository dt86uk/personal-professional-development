using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PPDDocumentation.BusinessLogic;

namespace PPDDocumentation.Pages
{
    public class Complete_JobModel : PageModel
    {
        private readonly ILogger<Incomplete_JobModel> _logger;

        private IJobService _jobService;

        public Complete_JobModel(ILogger<Incomplete_JobModel> logger, IJobService jobService)
        {
            _logger = logger;
            _jobService = jobService;
        }

        public IActionResult OnGet(Guid id)
        {
            _logger.LogInformation($"Complete Job Info: Request made to complete Job '{id}'");

            var setJobCompletionResponse = _jobService.SetJobCompletion(id, true);
            var successfulMessage = setJobCompletionResponse.IsSuccess ? "Successful" : "Unsuccessful";
            _logger.LogInformation($"Complete Job Info: Complete request for Job '{id}' successful: {successfulMessage}");

            TempData["JobResponseMessage"] = "Job successfully set to completed.";
            return Redirect("~/Jobs");
        }
    }
}
