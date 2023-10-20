using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PPDDocumentation.BusinessLogic;

namespace PPDDocumentation.Pages
{
    public class DeleteGoalModel : PageModel
    {
        private readonly ILogger<DeleteGoalModel> _logger;
        private readonly IGoalService _goalService;

        public DeleteGoalModel(ILogger<DeleteGoalModel> logger, IGoalService goalService)
        {
            _logger = logger;
            _goalService = goalService;
        }

        public IActionResult OnGet(Guid id)
        {
            _logger.LogInformation($"Delete Goal Info: Request made to delete Action '{id}'");

            var deleteResult = _goalService.DeleteGoal(id);
            var successfulMessage = deleteResult.IsSuccess ? "Successful" : "Unsuccessful";
            _logger.LogInformation($"Delete Goal Info: Delete request for Goal '{id}' successful: {successfulMessage}");

            TempData["ActionResponseMessage"] = "Goal successfully deleted.";
            return Redirect("~/Index");
        }
    }
}
