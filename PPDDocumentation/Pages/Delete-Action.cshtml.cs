using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PPDDocumentation.BusinessLogic;

namespace PPDDocumentation.Pages
{
    public class DeleteActionModel : PageModel
    {
        private readonly ILogger<DeleteActionModel> _logger;
        private readonly IActionService _actionService;

        public DeleteActionModel(ILogger<DeleteActionModel> logger, IActionService actionService)
        {
            _logger = logger;
            _actionService = actionService;
        }

        public IActionResult OnGet(Guid id)
        {
            _logger.LogInformation($"Delete Action Info: Request made to delete Action '{id}'");

            var deleteResult = _actionService.DeleteAction(id);
            var successfulMessage = deleteResult.IsSuccess ? "Successful" : "Unsuccessful";
            _logger.LogInformation($"Delete Action Info: Delete request for Action '{id}' successful: {successfulMessage}");

            TempData["JobResponseMessage"] = "Job successfully updated.";
            return Redirect("~/Index");
        }
    }
}
