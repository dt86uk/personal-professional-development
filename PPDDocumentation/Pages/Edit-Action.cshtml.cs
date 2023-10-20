using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PPDDocumentation.BusinessLogic;
using PPDDocumentation.Models;
using PPDDocumentation.Models.Requests;

namespace PPDDocumentation.Pages
{
    public class EditActionModel : PageModel
    {
        private readonly ILogger<EditActionModel> _logger;
        private IActionService _actionService;

        public EditActionModel(ILogger<EditActionModel> logger, IActionService actionService)
        {
            _logger = logger;
            _actionService = actionService;
        }

        public void OnGet(Guid id)
        {
            _logger.LogInformation($"Edit Action Info: Request to get Action '{id}' at {DateTime.Now.ToShortDateString()} {DateTime.Now.ToShortTimeString()}");
            var actionResponse = _actionService.GetActionById(id);

            if (EditModel == null)
            {
                EditModel = new TaskViewModelBase();
            }

            EditModel.Id = id;
            EditModel.ParentId = actionResponse.GoalId;
            EditModel.Title = actionResponse.Action.Title;
            EditModel.Description = actionResponse.Action.Description;
            EditModel.PercentageComplete = actionResponse.Action.PercentageComplete;
            EditModel.WhatILearnt = actionResponse.Action.WhatILearnt;
        }

        [BindProperty]
        public TaskViewModelBase? EditModel { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var request = new ActionRequest
            {
                NewTaskViewModel = EditModel
            };

            _logger.LogInformation($"Edit Action Info: Request to update Action ('{EditModel.Title}') for Goal ID '{EditModel.ParentId}' was called.");
            var response = _actionService.UpdateAction(request);

            if (!response.IsSuccess)
            {
                _logger.LogError($"Edit Action Error: Request to update Action ('{EditModel.Title}') for Goal ID '{EditModel.ParentId}' failed.");
                foreach (var error in response.ErrorMessages)
                {
                    ModelState.AddModelError(string.Empty, error);
                }

                return Page();
            }

            _logger.LogInformation($"Edit Action Info: Request to update Action ('{EditModel.Title}') for Action ID '{EditModel.Id}' completed.");
            return RedirectToPage("./view-action", new { id = EditModel.Id });
        }
    }
}
