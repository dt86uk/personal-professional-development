using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PPDDocumentation.BusinessLogic;
using PPDDocumentation.Models;

namespace PPDDocumentation.Pages
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [IgnoreAntiforgeryToken]
    public class ViewActionModel : PageModel
    {
        private readonly ILogger<ViewActionModel> _logger;
        private readonly IActionService _actionService;

        public Guid GoalId { get; set; }
        public Models.ActionModel Action { get; set; }
        public string ActionResponseMessage { get; set; }

        public ViewActionModel(ILogger<ViewActionModel> logger, IActionService actionService)
        {
            _logger = logger;
            _actionService = actionService;
        }

        public void OnGet(Guid? id)
        {
            if (!id.HasValue || id == null)
            {
                _logger.LogError($"View Action Error: The ID querystring value was null");
                Redirect("./Error");
            }

            _logger.LogInformation($"View Action Info: Request to get Action '{id}' at {DateTime.Now.ToShortDateString()} {DateTime.Now.ToShortTimeString()}");
            var actionResponse = _actionService.GetActionById(id.Value);

            if (!actionResponse.IsSuccess)
            {
                Redirect("./Error");
            }
          
            GoalId = actionResponse.GoalId;
            Action = actionResponse.Action;
        }
    }
}