using AutoMapper;
using Newtonsoft.Json;
using PPDDocumentation.Models;
using PPDDocumentation.Models.Goal;
using PPDDocumentation.Models.Goal.Enum;
using PPDDocumentation.Models.Requests;
using PPDDocumentation.Models.Responses;

namespace PPDDocumentation.BusinessLogic
{
    public class GoalService : IGoalService
    {
        private readonly ILogger<GoalService> _logger;
        private readonly IMapper _mapper;
        private IMissionStatementService _missionStatementService { get; set; }
        private IFileService _fileService { get; set; }

        public GoalService(ILogger<GoalService> logger, 
            IMapper mapper,
            IMissionStatementService missionStatementService,
            IFileService fileService)
        {
            _logger = logger;
            _mapper = mapper;
            _missionStatementService = missionStatementService;
            _fileService = fileService;
        }

        public List<GoalModel> GetGoals(bool includeDeleted = false)
        {
            var missionStatement = _missionStatementService.GetMissionStatement();

            if (missionStatement == null)
            {
                return null;
            }

            return missionStatement.GoalsMe.OrderBy(p => p.OrderId).Where(p => p.IsDeleted == false).ToList();
        }

        public List<GoalsTableModel> GetGoalsTable()
        {
            var goals = GetGoals();
            SetGoalsProgressHtml(goals);
            var goalsTable = _mapper.Map<List<GoalsTableModel>>(goals);
            
            return goalsTable;
        }

        public GoalResponse GetGoalById(Guid id)
        {
            var goals = GetGoals();
            var goal = goals.SingleOrDefault(p => p.Id == id);

            if (goal == null)
            {
                return new GoalResponse
                {
                    IsSuccess = false,
                    ErrorMessages = new List<string>
                    {
                        $"Goal not found with ID '{id}'."
                    }
                };
            }
            
            goal.Actions = goal.Actions == null ? 
                new List<ActionModel>() : 
                goal.Actions.Where(p => p.IsDeleted == false).ToList();

            return new GoalResponse
            {
                IsSuccess = true,
                Goal = goal
            };
        }

        public GoalResponse DeleteGoal(Guid id)
        {
            var goalResponse = GetGoalById(id);
            
            if (!goalResponse.IsSuccess)
            {
                _logger.LogInformation($"{nameof(GoalService)}.{nameof(DeleteGoal)} Info: Goal '{id}' not found.");
                return new GoalResponse
                {
                    IsSuccess = false,
                    ErrorMessages = new List<string>
                    {
                        $"Goal '{id}' not found."
                    }
                };
            }

            _logger.LogInformation($"{nameof(GoalService)}.{nameof(DeleteGoal)} Info: Goal '{id}' found.");

            var missionStatement = _missionStatementService.GetMissionStatement();
            var deleteGoal = missionStatement.GoalsMe.Single(p => p.Id == id);
            deleteGoal.IsDeleted = true;

            var isFileSaved = _fileService.UpdateGoalJsonDataSourceFile(missionStatement);
           
            _logger.LogInformation($"{nameof(GoalService)}.{nameof(DeleteGoal)} Info: Goal '{id}' deletion is {isFileSaved}");

            if (isFileSaved)
            {
                return new GoalResponse
                {
                    IsSuccess = true
                };
            }

            _logger.LogError($"Error saving JSON from deleting Goal '{deleteGoal.Title}' at {DateTime.Now.ToShortTimeString()}");
            return new GoalResponse
            {
                IsSuccess = false,
                ErrorMessages = new List<string>
                {
                    "There was a problem saving JSON: Source file not found after attempting save."
                }
            };
        }

        public GoalResponse UpdateGoal(GoalRequest request)
        {
            var goalResponse = GetGoalById(request.Goal.Id);
            
            if (!goalResponse.IsSuccess)
            {
                return new GoalResponse
                {
                    IsSuccess = false,
                    ErrorMessages = new List<string>
                    {
                        $"Goal not found with Id '{request.Goal.Id}'"
                    }
                };
            }

            var missionStatement = _missionStatementService.GetMissionStatement();
            var updatedGoal = missionStatement.GoalsMe.Single(p => p.Id == request.Goal.Id);

            updatedGoal.Title = request.Goal.Title;
            updatedGoal.Description = request.Goal.Description;
            updatedGoal.PercentageComplete = request.Goal.PercentageComplete;
            updatedGoal.DueBy = request.Goal.DueBy;
            updatedGoal.WhatILearnt = request.Goal.WhatILearnt;
            updatedGoal.IsDeleted = request.Goal.IsDeleted;

            var isFileSaved = _fileService.UpdateGoalJsonDataSourceFile(missionStatement);

            if(isFileSaved)
            {
                return new GoalResponse
                {
                    IsSuccess = true,
                    Goal = updatedGoal
                };
            }

            _logger.LogError($"Error saving JSON from updating Goal '{request.Goal.Title}' at {DateTime.Now.ToShortTimeString()}");
            return new GoalResponse
            {
                IsSuccess = false,
                ErrorMessages = new List<string>
                {
                    "There was a problem saving JSON: Source file not found after attempting save."
                }
            };
        }

        public GoalResponse AddGoal(GoalRequest request)
        {
            _logger.Log(LogLevel.Information, $"New Action added: '{request.Goal.Title}' at {DateTime.Now.ToShortTimeString()}");

            var jsonDataSourceFile = _fileService.GetGoalJsonDataSourceFile();
            var json = File.ReadAllText(jsonDataSourceFile);
            var missionStatement = JsonConvert.DeserializeObject<MissionStatementModel>(json);

            var goalId = Guid.NewGuid();
            var goal = new GoalModel(goalId)
            {
                Title = request.Goal.Title,
                Description = request.Goal.Description,
                PercentageComplete = request.Goal.PercentageComplete,
                IsComplete = request.Goal.PercentageComplete == 100,
                DueBy = request.Goal.DueBy,
                WhatILearnt = !string.IsNullOrEmpty(request.Goal.WhatILearnt) ? request.Goal.WhatILearnt : string.Empty
            };

            if (missionStatement.GoalsMe == null)
            {
                missionStatement.GoalsMe = new List<GoalModel>();
            }

            missionStatement.GoalsMe.Add(goal);

            var isFileSaved = _fileService.UpdateGoalJsonDataSourceFile(missionStatement);

            if (isFileSaved)
            {
                return new GoalResponse
                {
                    IsSuccess = true,
                    Goal = goal
                };
            }

            _logger.LogError($"Error saving JSON from new Goal '{request.Goal.Title}' at {DateTime.Now.ToShortTimeString()}");
            return new GoalResponse
            {
                IsSuccess = false,
                ErrorMessages = new List<string>
                {
                    "There was a problem saving JSON: Source file not found after attempting save."
                }
            };
        }

        public void SetGoalsProgressHtml(List<GoalModel> goals)
        {
            foreach (var goal in goals)
            {
                var percentage = goal.PercentageComplete;
                var progressHtml = "<div class='{0} {1}' title='{2}%'></div>";

                if (IsWithinRange(0, 10, percentage))
                {
                    goal.ProgressHtml = string.Format(progressHtml, GoalProgressEnum.progressCircle, GoalProgressEnum.progress10percent, goal.PercentageComplete);
                    continue;
                }

                if (IsWithinRange(11, 20, percentage))
                {
                    goal.ProgressHtml = string.Format(progressHtml, GoalProgressEnum.progressCircle, GoalProgressEnum.progress20percent, goal.PercentageComplete);
                    continue;
                }

                if (IsWithinRange(21, 30, percentage))
                {
                    goal.ProgressHtml = string.Format(progressHtml, GoalProgressEnum.progressCircle, GoalProgressEnum.progress30percent, goal.PercentageComplete);
                    continue;
                }

                if (IsWithinRange(31, 40, percentage))
                {
                    goal.ProgressHtml = string.Format(progressHtml, GoalProgressEnum.progressCircle, GoalProgressEnum.progress40percent, goal.PercentageComplete);
                    continue;
                }

                if (IsWithinRange(41, 50, percentage))
                {
                    goal.ProgressHtml = string.Format(progressHtml, GoalProgressEnum.progressCircle, GoalProgressEnum.progress50percent, goal.PercentageComplete);
                    continue;
                }

                if (IsWithinRange(51, 60, percentage))
                {
                    goal.ProgressHtml = string.Format(progressHtml, GoalProgressEnum.progressCircle, GoalProgressEnum.progress60percent, goal.PercentageComplete);
                    continue;
                }

                if (IsWithinRange(61, 70, percentage))
                {
                    goal.ProgressHtml = string.Format(progressHtml, GoalProgressEnum.progressCircle, GoalProgressEnum.progress70percent, goal.PercentageComplete);
                    continue;
                }

                if (IsWithinRange(71, 80, percentage))
                {
                    goal.ProgressHtml = string.Format(progressHtml, GoalProgressEnum.progressCircle, GoalProgressEnum.progress80percent, goal.PercentageComplete);
                    continue;
                }

                if (IsWithinRange(81, 90, percentage))
                {
                    goal.ProgressHtml = string.Format(progressHtml, GoalProgressEnum.progressCircle, GoalProgressEnum.progress90percent, goal.PercentageComplete);
                    continue;
                }

                if (IsWithinRange(91, 100, percentage))
                {
                    goal.ProgressHtml = string.Format(progressHtml, GoalProgressEnum.progressCircle, GoalProgressEnum.progress100percent, goal.PercentageComplete);
                    continue;
                }

                goal.ProgressHtml = "<div>N/A</div>";
            }
        }

        private bool IsWithinRange(int startRange, int endRange, int actualValue)
        {
            return actualValue >= startRange && actualValue <= endRange;
        }
    }
}
