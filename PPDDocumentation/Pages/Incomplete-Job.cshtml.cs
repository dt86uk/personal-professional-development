using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PPDDocumentation.BusinessLogic;

namespace PPDDocumentation.Pages
{
    public class Incomplete_JobModel : PageModel
    {
        private readonly ILogger<Incomplete_JobModel> _logger;
        private IJobService _jobService;

        public Incomplete_JobModel(ILogger<Incomplete_JobModel> logger, IJobService jobService)
        { 
            _logger = logger;
            _jobService = jobService;
        }

        public IActionResult OnGet(Guid id)
        {
            _logger.LogInformation($"Delete Job Info: Request made to delete Job '{id}'");

            var setJobCompletionResponse = _jobService.SetJobCompletion(id, false);
            var successfulMessage = setJobCompletionResponse.IsSuccess ? "Successful" : "Unsuccessful";
            _logger.LogInformation($"Delete Job Info: Delete request for Job '{id}' successful: {successfulMessage}");

            TempData["JobResponseMessage"] = "Job successfully set to incomplete.";
            return Redirect("~/Jobs");
        }
    }
}
