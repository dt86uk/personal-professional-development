using Microsoft.Extensions.Logging;
using Moq;
using PPDDocumentation.BusinessLogic;
using PPDDocumentation.Models;
using PPDDocumentation.Models.Requests;
using PPDDocumentation.UnitTests.BusinessLogic.Services;

namespace PPDDocumentation.UnitTests.BusinessLogic
{
    [TestClass]
    public class ActionServiceTests : BaseServiceTests
    {
        private Mock<ILogger<ActionService>> _mockLogger;
        private Mock<IFileService> _mockFileService; 
        private Mock<IMissionStatementService> _mockMissionStatementService { get; set; }
        private ActionService _actionService;

        [TestInitialize]
        public void Init()
        {
            _mockLogger = new Mock<ILogger<ActionService>>();
            _mockFileService = new Mock<IFileService>();
            _mockMissionStatementService = new Mock<IMissionStatementService>();
            _actionService = new ActionService(_mockLogger.Object, _mockFileService.Object, _mockMissionStatementService.Object);
        }

        [TestMethod]
        public void GetActionById_Returns_Data()
        {
            var missionStatement = GetTestMissionStatementModel();
            _mockMissionStatementService
                .Setup(p => p.GetMissionStatement())
                .Returns(missionStatement);

            var goal = missionStatement.GoalsMe.First();
            var action = missionStatement.GoalsMe.First().Actions.First();
            
            var result = _actionService.GetActionById(action.Id);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(result.GoalId, goal.Id);
            Assert.AreEqual(result.Action.Id, action.Id);
            Assert.AreEqual(result.Action.Title, action.Title);
            Assert.AreEqual(result.Action.Description, action.Description);
            Assert.AreEqual(result.Action.IsComplete, action.IsComplete);
            Assert.AreEqual(result.Action.IsDeleted, action.IsDeleted);
            Assert.AreEqual(result.Action.OrderId, action.OrderId);
            Assert.AreEqual(result.Action.PercentageComplete, action.PercentageComplete);
            Assert.AreEqual(result.Action.WhatILearnt, action.WhatILearnt);
        }

        [TestMethod]
        public void GetActionById_Returns_Error()
        {
            var guidId = Guid.NewGuid();
            _mockMissionStatementService
                .Setup(p => p.GetMissionStatement())
                .Returns(new MissionStatementModel
                {
                    GoalsMe = new List<Models.Goal.GoalModel>()
                });

            var result = _actionService.GetActionById(guidId);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual($"Action with Id '{guidId}' not found.", result.ErrorMessages.First());
        }

        [TestMethod]
        public void DeleteAction_Returns_ActionIdNotFound()
        {
            var goals = GetTestGoals();
            _mockMissionStatementService
                .Setup(p => p.GetMissionStatement())
                .Returns(new MissionStatementModel
                {
                    GoalsMe = goals
                });

            var result = _actionService.DeleteAction(Guid.Empty);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(result.ErrorMessages.First(), $"Action '{Guid.Empty}' not found.");
        }

        [TestMethod]
        public void DeleteAction_Returns_ProblemSavingJson()
        {
            var missionStatement = GetTestMissionStatementModel();
            var jsonDataSource = GetTestGoalsDataSource();
            _mockMissionStatementService
                .Setup(p => p.GetMissionStatement())
                .Returns(missionStatement);
            _mockFileService
                .Setup(p => p.GetGoalJsonDataSourceFile())
                .Returns(jsonDataSource);
            _mockFileService
                .Setup(p => p.UpdateGoalJsonDataSourceFile(It.IsAny<MissionStatementModel>()))
                .Returns(false);

            var result = _actionService.DeleteAction(missionStatement.GoalsMe.First().Actions.First().Id);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("There was a problem saving JSON: Source file not found after attempting deleting the action.", result.ErrorMessages.First());
        }

        [TestMethod]
        public void DeleteAction_Returns_IsSuccess_True()
        {
            var missionStatement = GetTestMissionStatementModel();
            var jsonDataSource = GetTestGoalsDataSource();
            _mockMissionStatementService
                .Setup(p => p.GetMissionStatement())
                .Returns(missionStatement);
            _mockFileService
                .Setup(p => p.GetGoalJsonDataSourceFile())
                .Returns(jsonDataSource);
            _mockFileService
                .Setup(p => p.UpdateGoalJsonDataSourceFile(It.IsAny<MissionStatementModel>()))
                .Returns(true);

            var result = _actionService.DeleteAction(missionStatement.GoalsMe.First().Actions.First().Id);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsSuccess);
        }

        [TestMethod]
        public void UpdateAction_Returns_IsSuccess_False()
        {
            _mockMissionStatementService
                .Setup(p => p.GetMissionStatement())
                .Returns(new MissionStatementModel
                {
                    GoalsMe = GetTestGoals()
                });
            var actionId = Guid.Empty;
            var actionRequest = new ActionRequest
            {
                Action = new ActionModel(actionId),
                NewTaskViewModel = new TaskViewModelBase
                {
                    Id = actionId
                }
            };

            var result = _actionService.UpdateAction(actionRequest);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsSuccess);
        }

        [TestMethod]
        public void UpdateAction_Returns_IsSuccess_True()
        {
            var goals = GetTestGoals();
            _mockMissionStatementService
                .Setup(p => p.GetMissionStatement())
                .Returns(new MissionStatementModel
                {
                    GoalsMe = goals
                });
            _mockFileService
                .Setup(p => p.GetGoalJsonDataSourceFile())
                .Returns(@"C:\Users\danie\source\repos\PPDDocumentation\PPDDocumentation.UnitTests\Repository\Goals\a38bb0f0-c270-4727-9979-841e5bd495dc.json");
            _mockFileService
                .Setup(p => p.UpdateGoalJsonDataSourceFile(It.IsAny<MissionStatementModel>()))
                .Returns(true);
            var actionId = goals.First().Actions.First().Id;
            var actionRequest = new ActionRequest
            {
                Action = new ActionModel(actionId),
                NewTaskViewModel = new TaskViewModelBase
                {
                    Id = actionId,
                    ParentId = Guid.Parse("63e5a07c-c502-4cd4-8ff3-6ffed5f38af6"),
                    Description = "Updated",
                    Title = "Updated",
                    IsDeleted = false,
                    PercentageComplete = 100,
                    WhatILearnt = "Updated"
                }
            };

            var result = _actionService.UpdateAction(actionRequest);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsSuccess);
        }
    }
}
