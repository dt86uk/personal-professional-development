using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PPDDocumentation.BusinessLogic;
using PPDDocumentation.Models.Goal;
using PPDDocumentation.Models.Requests;

namespace PPDDocumentation.Pages
{
    public class EditGoalModel : PageModel
    {
        private readonly ILogger<EditGoalModel> _logger;
        private IGoalService _goalService;

        public EditGoalModel(ILogger<EditGoalModel> logger, IGoalService goalService)
        {
            _logger = logger;
            _goalService = goalService;
        }

        public void OnGet(Guid id)
        {
            _logger.LogInformation($"Edit Goal Info: Request to get Goal '{id}' at {DateTime.Now.ToShortDateString()} {DateTime.Now.ToShortTimeString()}");
            var goalResponse = _goalService.GetGoalById(id);

            if (EditModel == null)
            {
                EditModel = new GoalViewModel();
            }

            EditModel.Id = id;
            EditModel.Title = goalResponse.Goal.Title;
            EditModel.Description = goalResponse.Goal.Description;
            EditModel.DueBy = goalResponse.Goal.DueBy;
            EditModel.PercentageComplete = goalResponse.Goal.PercentageComplete;
            EditModel.WhatILearnt = goalResponse.Goal.WhatILearnt;
        }

        [BindProperty]
        public GoalViewModel? EditModel { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "There was a validation error. Please check the fields and try again.");
                return Page();
            }

            var request = new GoalRequest
            {
                Goal = new GoalModel(EditModel.Id)
                {
                    Title = EditModel.Title,
                    Description = EditModel.Description,
                    PercentageComplete = !EditModel.PercentageComplete.HasValue ? 0 : EditModel.PercentageComplete.Value,
                    DueBy = !EditModel.DueBy.HasValue ? 0 : EditModel.DueBy.Value,
                    WhatILearnt = EditModel.WhatILearnt  == null ? string.Empty : EditModel.WhatILearnt
                } 
            };

            _logger.LogInformation($"Edit Goal Info: Request to update Goal ('{EditModel.Title}') for Goal ID '{EditModel.Id}' was called.");
            var response = _goalService.UpdateGoal(request);

            if (!response.IsSuccess)
            {
                _logger.LogError($"Edit Goal Error: Request to update Goal ('{EditModel.Title}') for Goal ID '{EditModel.Id}' failed.");
                foreach (var error in response.ErrorMessages)
                {
                    ModelState.AddModelError(string.Empty, error);
                }

                return Page();
            }

            _logger.LogInformation($"Edit Goal Info: Request to update Goal ('{EditModel.Title}') for Goal ID '{EditModel.Id}' completed.");
            return RedirectToPage("./view-goal", new { id = EditModel.Id });
        }
    }
}
