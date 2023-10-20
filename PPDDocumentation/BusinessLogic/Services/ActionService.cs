using Newtonsoft.Json;
using PPDDocumentation.Helpers;
using PPDDocumentation.Models;
using PPDDocumentation.Models.Requests;
using PPDDocumentation.Models.Responses;

namespace PPDDocumentation.BusinessLogic
{
    public class ActionService : IActionService
    {
        private readonly ILogger<ActionService> _logger;
        private IFileService _fileService { get; set; }
        private IMissionStatementService _missionStatementService { get; set; }

        public ActionService(ILogger<ActionService> logger, 
            IFileService fileService,
            IMissionStatementService missionStatementService)
        {
            _logger = logger;
            _fileService = fileService;
            _missionStatementService = missionStatementService;
        }

        public ActionResponse AddAction(ActionRequest request)
        {
            _logger.Log(LogLevel.Information, $"New Action added: '{request.Action.Title}' at {DateTime.Now.ToShortTimeString()}");

            var jsonDataSourceFile = _fileService.GetGoalJsonDataSourceFile();
            var json = File.ReadAllText(jsonDataSourceFile);
            var missionStatement = JsonConvert.DeserializeObject<MissionStatementModel>(json);

            var actionId = Guid.NewGuid();
            var action = new ActionModel(actionId)
            {
                Title = request.NewTaskViewModel.Title,
                Description = request.NewTaskViewModel.Description,
                PercentageComplete = request.NewTaskViewModel.PercentageComplete ?? 0 ,
                WhatILearnt = !string.IsNullOrEmpty(request.NewTaskViewModel.WhatILearnt) ? request.NewTaskViewModel.WhatILearnt : string.Empty
            };

            var goal = missionStatement.GoalsMe.Single(p => p.Id == request.Id);
            
            if (goal.Actions == null)
            {
                goal.Actions = new List<ActionModel>();
            }
            
            goal.Actions.Add(action);

            missionStatement.GoalsMe.Remove(goal);
            missionStatement.GoalsMe.Add(goal);

            goal.PercentageComplete = GoalHelper.CalculatePercentageComplete(missionStatement, goal.Id);
            goal.IsComplete = request.NewTaskViewModel.PercentageComplete == 100;

            var isFileSaved = _fileService.UpdateGoalJsonDataSourceFile(missionStatement);

            if (isFileSaved)
            {
                return new ActionResponse
                {
                    IsSuccess = true,
                    Action = action
                };
            }

            _logger.LogError($"Error saving JSON from new Action '{request.Action.Title}' at {DateTime.Now.ToShortTimeString()}");
            return new ActionResponse
            {
                IsSuccess = false,
                ErrorMessages = new List<string>
                {
                    "There was a problem saving JSON: Source file not found."
                }
            };
        }

        public ActionResponse UpdateAction(ActionRequest request)
        {
            var actionResponse = GetActionById(request.NewTaskViewModel.Id);

            if (!actionResponse.IsSuccess)
            {
                return new ActionResponse
                {
                    IsSuccess = false,
                    ErrorMessages = new List<string>
                    {
                        $"Action not found with Id '{request.Action.Id}'"
                    }
                };
            }

            var jsonDataSourceFile = _fileService.GetGoalJsonDataSourceFile();
            var json = File.ReadAllText(jsonDataSourceFile);
            var missionStatement = JsonConvert.DeserializeObject<MissionStatementModel>(json);

            var goal = missionStatement.GoalsMe.Single(p => p.Id == request.NewTaskViewModel.ParentId);
            var updatedAction = goal.Actions.Single(p => p.Id == request.NewTaskViewModel.Id);

            updatedAction.Title = request.NewTaskViewModel.Title;
            updatedAction.Description = request.NewTaskViewModel.Description;
            updatedAction.PercentageComplete = request.NewTaskViewModel.PercentageComplete ?? 0  ;
            updatedAction.WhatILearnt = !string.IsNullOrEmpty(request.NewTaskViewModel.WhatILearnt) ? request.NewTaskViewModel.WhatILearnt : string.Empty;
            updatedAction.IsComplete = request.NewTaskViewModel.PercentageComplete == 100;

            goal.PercentageComplete = GoalHelper.CalculatePercentageComplete(missionStatement, goal.Id);
            goal.IsComplete = request.NewTaskViewModel.PercentageComplete == 100;

            var isFileSaved = _fileService.UpdateGoalJsonDataSourceFile(missionStatement);

            if (isFileSaved)
            {
                return new ActionResponse
                {
                    IsSuccess = true
                };
            }

            _logger.LogError($"Error saving JSON from Action '{request.NewTaskViewModel.Title}' at {DateTime.Now.ToShortTimeString()}");
            return new ActionResponse
            {
                IsSuccess = false,
                ErrorMessages = new List<string>
                {
                    "There was a problem saving JSON: Source file not found after attempting save."
                }
            };
        }

        public ActionResponse GetActionById(Guid id)
        {
            var missionStatement = _missionStatementService.GetMissionStatement();
            var goals = missionStatement.GoalsMe.Where(p => p.IsDeleted == false).ToList();

            ActionModel action = null;
            foreach (var goal in goals)
            {
                foreach (var a in goal.Actions)
                {
                    if (a.Id == id)
                    {
                        action = a;
                        break;
                    }
                }

                if (action != null)
                {
                    break;
                }
            }

            if (action == null)
            {
                return new ActionResponse
                {
                    IsSuccess = false,
                    ErrorMessages = new List<string>
                    {
                        $"Action with Id '{id}' not found."
                    }
                };
            }

            var goalId = goals.Where(p => p.Actions.Contains(action)).Single().Id;

            return new ActionResponse
            {
                IsSuccess = action != null,
                Action = action,
                GoalId = goalId
            };
        }

        public ActionResponse DeleteAction(Guid id)
        {
            var actionResponse = GetActionById(id);

            if (!actionResponse.IsSuccess)
            {
                _logger.LogInformation($"{nameof(GoalService)}.{nameof(DeleteAction)} Info: Goal '{id}' not found.");
                return new ActionResponse
                {
                    IsSuccess = false,
                    ErrorMessages = new List<string>
                    {
                        $"Action '{id}' not found."
                    }
                };
            }

            _logger.LogInformation($"{nameof(GoalService)}.{nameof(DeleteAction)} Info: Goal '{id}' found.");

            var jsonDataSourceFile = _fileService.GetGoalJsonDataSourceFile();
            var json = File.ReadAllText(jsonDataSourceFile);
            var missionStatement = JsonConvert.DeserializeObject<MissionStatementModel>(json);

            foreach (var goal in missionStatement.GoalsMe)
            {
                if (goal.Actions == null)
                {
                    continue;    
                }

                foreach (var action in goal.Actions)
                {
                    if (action.Id == id)
                    {
                        action.IsDeleted = true;
                    }
                }
            }

            var isFileSaved = _fileService.UpdateGoalJsonDataSourceFile(missionStatement);

            _logger.LogInformation($"{nameof(GoalService)}.{nameof(DeleteAction)} Info: Action '{id}' deletion is {isFileSaved}");

            if (isFileSaved)
            {
                return new ActionResponse
                {
                    IsSuccess = isFileSaved
                };
            }

            return new ActionResponse
            {
                IsSuccess = isFileSaved,
                ErrorMessages = new List<string>
                {
                    "There was a problem saving JSON: Source file not found after attempting deleting the action."
                }
            };
        }
    }
}
