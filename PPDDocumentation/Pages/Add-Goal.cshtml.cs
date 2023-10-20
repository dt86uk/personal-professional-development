using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PPDDocumentation.BusinessLogic;
using PPDDocumentation.Models.Goal;
using PPDDocumentation.Models.Requests;

namespace PPDDocumentation.Pages
{
    public class AddGoalModel : PageModel
    {
        private readonly ILogger<AddGoalModel> _logger;
        private IGoalService _goalService;

        public AddGoalModel(ILogger<AddGoalModel> logger, IGoalService goalService)
        {
            _logger = logger;
            _goalService = goalService;
        }

        public void OnGet()
        {
        }

        [BindProperty]
        public GoalViewModel? TaskViewModel { get; set; }
        
        public IActionResult OnPostAddGoal()
        {
            if (!ModelState.IsValid || TaskViewModel == null)
            {
                return Page();
            }

            var request = new GoalRequest
            {
                Goal = new GoalModel(Guid.NewGuid())
                {
                    Title = TaskViewModel.Title,
                    Description = TaskViewModel.Description,
                    DueBy = TaskViewModel.DueBy ?? 0 ,
                    PercentageComplete = TaskViewModel.PercentageComplete ?? 0 ,
                } 
            };

            _logger.LogInformation($"Add Goal Info: Request to add Goal ('{TaskViewModel.Title}') was called.");
            var response = _goalService.AddGoal(request);

            if (!response.IsSuccess)
            {
                _logger.LogError($"Add Goal Error: Request to add Goal ('{TaskViewModel.Title}') failed.");
                foreach (var error in response.ErrorMessages)
                {
                    ModelState.AddModelError(string.Empty, error);
                }

                return Page();
            }

            _logger.LogInformation($"Add Goal Info: Request to add Goal ('{TaskViewModel.Title}') completed.");
            return RedirectToPage("./view-goal", new { id = response.Goal.Id });
        }
    }
}
