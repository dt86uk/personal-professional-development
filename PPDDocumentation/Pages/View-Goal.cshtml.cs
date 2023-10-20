using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PPDDocumentation.BusinessLogic;
using PPDDocumentation.Models.Goal;

namespace PPDDocumentation.Pages
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [IgnoreAntiforgeryToken]
    public class ViewGoalModel : PageModel
    {
        private readonly ILogger<ViewGoalModel> _logger;
        private readonly IGoalService _goalService;

        public GoalModel Goal { get; set; }
        public string GoalResponseMessage { get; set; }

        public ViewGoalModel(ILogger<ViewGoalModel> logger, IGoalService goalService)
        {
            _logger = logger;
            _goalService = goalService;
        }

        public void OnGet(Guid? id)
        {
            if (!id.HasValue || id == null)
            {
                _logger.LogError($"View Goal Error: The ID querystring value was null");
                Redirect("./Error");
            }

            _logger.LogInformation($"View Goal Info: Request to get Goal '{id}' at {DateTime.Now.ToShortDateString()} {DateTime.Now.ToShortTimeString()}");
            var goalResponse = _goalService.GetGoalById(id.Value);

            if (!goalResponse.IsSuccess)
            {
                Redirect("./Error");
            }
            else 
            {
                Goal = goalResponse.Goal;
            }
        }
    }
}