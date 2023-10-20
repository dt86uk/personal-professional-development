using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PPDDocumentation.BusinessLogic;
using PPDDocumentation.Models;
using PPDDocumentation.Models.Requests;

namespace PPDDocumentation.Pages
{
    public class AddActionModel : PageModel
    {
        private readonly ILogger<AddActionModel> _logger;
        private IActionService _actionService;

        public Guid GoalId { get; set; }

        [TempData]
        public List<string> ErrorMessages { get; set; }

        [TempData]
        public bool PageHasErrorMessages { get; set; }

        public AddActionModel(ILogger<AddActionModel> logger, IActionService actionService)
        {
            _logger = logger;
            _actionService = actionService;
        }

        public void OnGet(Guid? id)
        {
            if (!id.HasValue || id == null)
            {
                _logger.LogError($"Add Action Error: The ID querystring value was null");
                Redirect("./Error");
            }

            GoalId = id.Value;

            PageHasErrorMessages = TempData["PageHasErrorMessages"] != null ?
                    Convert.ToBoolean(TempData["PageHasErrorMessages"]) :
                    false;

            ErrorMessages = TempData["ErrorMessages"] != null ?
                    (List<string>)TempData["ErrorMessages"] :
                    null;
        }

        [BindProperty]
        public TaskViewModelBase? TaskViewModel { get; set; }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid || TaskViewModel == null)
            {
                return Page();
            }

            var request = new ActionRequest
            {
                Id = TaskViewModel.ParentId,
                NewTaskViewModel = TaskViewModel,
                Action = new ActionModel
                {
                    Title = TaskViewModel.Title,
                    Description = TaskViewModel.Description,
                    PercentageComplete = !TaskViewModel.PercentageComplete.HasValue ? 0 : TaskViewModel.PercentageComplete.Value,
                    WhatILearnt = TaskViewModel.WhatILearnt
                }
            };

            _logger.LogInformation($"Add Action Info: Request to add Action ('{TaskViewModel.Title}') for Goal ID '{TaskViewModel.ParentId}' was called.");
            var response = _actionService.AddAction(request);

            if (!response.IsSuccess)
            {
                _logger.LogError($"Add Action Error: Request to add Action ('{TaskViewModel.Title}') for Goal ID '{TaskViewModel.ParentId}' failed.");
                foreach (var error in response.ErrorMessages)
                {
                    ModelState.AddModelError(string.Empty, error);
                }

                return Page();
            }

            _logger.LogInformation($"Add Action Info: Request to add action ('{TaskViewModel.Title}') for Goal ID '{TaskViewModel.ParentId}' completed.");
            return RedirectToPage("./view-goal", new { id = TaskViewModel.ParentId });
        }
    }
}
