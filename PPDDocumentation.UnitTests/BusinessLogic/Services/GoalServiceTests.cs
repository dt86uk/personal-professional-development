using Moq;
using Microsoft.Extensions.Logging;
using PPDDocumentation.BusinessLogic;
using Shouldly;
using PPDDocumentation.Models;
using AutoMapper;
using PPDDocumentation.Models.Requests;
using PPDDocumentation.Models.Goal;
using PPDDocumentation.Models.Job;
using System.Collections.Generic;

namespace PPDDocumentation.UnitTests.BusinessLogic.Services
{
    [TestClass]
    public class GoalServiceTests : BaseServiceTests
    {
        private Mock<ILogger<GoalService>> _mockLogger;
        private Mock<IMapper> _mockMapper;
        private Mock<IMissionStatementService> _mockMissionStatementService;
        private Mock<IFileService> _mockFileService;
        private GoalService _goalService;
        private string _fileLocation;

        [TestInitialize]
        public void Init()
        {
            _mockLogger = new Mock<ILogger<GoalService>>();
            _mockMapper = new Mock<IMapper>();
            _mockMissionStatementService = new Mock<IMissionStatementService>();
            _mockFileService = new Mock<IFileService>();
            _goalService = new GoalService(_mockLogger.Object, 
                _mockMapper.Object, 
                _mockMissionStatementService.Object,
                _mockFileService.Object);
        }

        [TestMethod]
        public void GetGoals_Returns_Goals() 
        {
            _mockMissionStatementService
                .Setup(p => p.GetMissionStatement())
                .Returns(GetTestMissionStatementModel());

			var result = _goalService.GetGoals();

            result.ShouldNotBeNull();
            result.Count.ShouldBeGreaterThan(0);
        }

        [TestMethod]
        public void GetGoals_Returns_Null_When_MissionStatementService_Fails()
        {
            var missionStatementNull = (MissionStatementModel)null;
            _mockMissionStatementService
                .Setup(p => p.GetMissionStatement())
                .Returns(missionStatementNull);

            var result = _goalService.GetGoals();

            result.ShouldBeNull();
        }

        [TestMethod]
        public void GetGoalsTable_Returns_GoalsTable()
        {
            var expectedGoalsTable = new List<GoalsTableModel>
            {
                new GoalsTableModel 
                {
                    Description = "Description",
                    DueBy = 1,
                    IsComplete = true,
                    OrderId = 3,
                    ProgressHtml = "ProgressHtml",
                    Title = "Title"
                }
            };
            _mockMissionStatementService
                .Setup(p => p.GetMissionStatement())
                .Returns(GetTestMissionStatementModel());
            _mockMapper
                .Setup(p => p.Map<List<GoalsTableModel>>(It.IsAny<List<GoalModel>>()))
                .Returns(expectedGoalsTable);

            var result = _goalService.GetGoalsTable();

            var firstItem = result.First();
            result.ShouldNotBeNull();
            result.Count.ShouldBeGreaterThan(0);
            firstItem.OrderId.ShouldBe(expectedGoalsTable.First().OrderId);
            firstItem.IsComplete.ShouldBeTrue();
            firstItem.Title.ShouldBe(expectedGoalsTable.First().Title);
            firstItem.Description.ShouldBe(expectedGoalsTable.First().Description);
            firstItem.ProgressHtml.ShouldNotBeNullOrEmpty();
        }

        [TestMethod]
        public void GetGoalById_Returns_Goal()
        {
            var goals = GetTestGoals();
            var id = goals.First().Id;
            var expectedItem = goals.First();
            _mockMissionStatementService
                .Setup(p => p.GetMissionStatement())
                .Returns(GetTestMissionStatementModel());

            var result = _goalService.GetGoalById(id);

            result.ShouldNotBeNull();
            result.IsSuccess.ShouldBeTrue();
            result.Goal.ShouldNotBeNull();
            result.Goal.Id.ShouldBe(expectedItem.Id);
            result.Goal.OrderId.ShouldBe(expectedItem.OrderId);
            result.Goal.IsComplete.ShouldBeFalse();
            result.Goal.Title.ShouldBe(expectedItem.Title);
            result.Goal.Description.ShouldBe(expectedItem.Description);
        }

        [TestMethod]
        public void GetGoalById_NotFound_Returns_Null()
        {
            _mockMissionStatementService
                .Setup(p => p.GetMissionStatement())
                .Returns(GetTestMissionStatementModel());
            var id = Guid.NewGuid();

            var result = _goalService.GetGoalById(id);

            result.ShouldNotBeNull();
            result.IsSuccess.ShouldBeFalse();
            result.Goal.ShouldBeNull();
        }

        [TestMethod]
        public void DeleteGoal_Returns_Success_NoErrors()
        {
            var missionStatement = GetTestMissionStatementModel();
            var goal = missionStatement.GoalsMe.Single(p => p.Id == Guid.Parse("f04f730c-47a5-455a-8637-5b429eee9293"));
            _mockMissionStatementService
                .SetupSequence(p => p.GetMissionStatement())
                .Returns(missionStatement)
                .Returns(missionStatement);
            _mockFileService
                .Setup(p => p.UpdateGoalJsonDataSourceFile(It.IsAny<MissionStatementModel>()))
                .Returns(true);

            var result = _goalService.DeleteGoal(goal.Id);

            result.IsSuccess.ShouldBeTrue();
        }

        [TestMethod]
        public void DeleteGoal_Returns_Failure_NotFound()
        {
            var goalId = Guid.NewGuid();
            _mockMissionStatementService
                .Setup(p => p.GetMissionStatement())
                .Returns(GetTestMissionStatementModel());

            var result = _goalService.DeleteGoal(goalId);

            result.IsSuccess.ShouldBeFalse();
            result.ErrorMessages.Count.ShouldBe(1);
            result.ErrorMessages.First().ShouldContain($"Goal '{goalId}' not found.");
        }

        [TestMethod]
        public void AddGoal_Returns_Success_With_Goal()
        {
            GetFileLocation();

            _mockFileService
                .Setup(p => p.GetGoalJsonDataSourceFile())
                .Returns(_fileLocation);
            _mockFileService
                .Setup(p => p.UpdateGoalJsonDataSourceFile(It.IsAny<MissionStatementModel>()))
                .Returns(true);
            var goalRequest = new GoalRequest
            {
                Id = Guid.NewGuid(),
                Goal = new Models.Goal.GoalModel()
                {
                    Actions = new List<ActionModel>(),
                    IsComplete = false,
                    DueBy = 4,
                    Title = "Test"
                }
            };

            var result = _goalService.AddGoal(goalRequest);

            result.IsSuccess.ShouldBeTrue();
            result.Goal.Id.ShouldNotBe(Guid.Empty);
        }

        [TestMethod]
        public void AddGoal_Returns_Failure_FileNotSaved()
        {
            GetFileLocation();

            _mockFileService
                .Setup(p => p.GetGoalJsonDataSourceFile())
                .Returns(_fileLocation);
            _mockFileService
                .Setup(p => p.UpdateGoalJsonDataSourceFile(It.IsAny<MissionStatementModel>()))
                .Returns(false);
            var goalRequest = new GoalRequest
            {
                Id = Guid.NewGuid(),
                Goal = new Models.Goal.GoalModel()
                {
                    Actions = new List<ActionModel>(),
                    IsComplete = false,
                    DueBy = 4,
                    Title = "Test"
                }
            };

            var result = _goalService.AddGoal(goalRequest);

            result.IsSuccess.ShouldBeFalse();
            result.ErrorMessages.Count.ShouldBeGreaterThan(0);
            result.ErrorMessages.First().ShouldContain("There was a problem saving JSON: Source file not found after attempting save.");
        }

        [TestMethod]
        public void UpdateGoal_Returns_Failure_NotFound()
        {
            _mockMissionStatementService
                .Setup(p => p.GetMissionStatement())
                .Returns(GetTestMissionStatementModel());
            var goalId = Guid.NewGuid();
            var goalRequest = new GoalRequest
            {
                Id = Guid.NewGuid(),
                Goal = new Models.Goal.GoalModel(goalId)
                {
                    Actions = new List<ActionModel>(),
                    IsComplete = false,
                    DueBy = 4,
                    Title = "Test"
                }
            };

            var result = _goalService.UpdateGoal(goalRequest);

            result.IsSuccess.ShouldBeFalse();
            result.ErrorMessages.Count.ShouldBeGreaterThan(0);
            result.ErrorMessages.First().ShouldContain($"Goal not found with Id '{goalRequest.Goal.Id}'");
        }

        [TestMethod]
        public void UpdateGoal_Returns_Success_With_Goal()
        {
            var missionStatement = GetTestMissionStatementModel();
            var goal = missionStatement.GoalsMe.Single(p => p.Id == Guid.Parse("f04f730c-47a5-455a-8637-5b429eee9293"));
            _mockMissionStatementService
                .Setup(p => p.GetMissionStatement())
                .Returns(missionStatement);
            GetFileLocation();
            _mockFileService
                .Setup(p => p.GetGoalJsonDataSourceFile())
                .Returns(_fileLocation);
            _mockFileService
                .Setup(p => p.UpdateGoalJsonDataSourceFile(It.IsAny<MissionStatementModel>()))
                .Returns(true);
            goal.PercentageComplete = 27;
            var goalRequest = new GoalRequest
            {
                Id = Guid.NewGuid(),
                Goal = goal
            };

            var result = _goalService.UpdateGoal(goalRequest);

            result.IsSuccess.ShouldBeTrue();
            result.Goal.PercentageComplete.ShouldBe(27);
        }

        [TestMethod]
        public void UpdateGoal_Returns_Failure_FileNotSaved()
        {
            var missionStatement = GetTestMissionStatementModel();
            var goal = missionStatement.GoalsMe.Single(p => p.Id == Guid.Parse("f04f730c-47a5-455a-8637-5b429eee9293"));
            _mockMissionStatementService
                .Setup(p => p.GetMissionStatement())
                .Returns(missionStatement);
            GetFileLocation();
            _mockFileService
                .Setup(p => p.GetGoalJsonDataSourceFile())
                .Returns(_fileLocation);
            _mockFileService
                .Setup(p => p.UpdateGoalJsonDataSourceFile(It.IsAny<MissionStatementModel>()))
                .Returns(false);
            var goalRequest = new GoalRequest
            {
                Id = Guid.NewGuid(),
                Goal = goal
            };

            var result = _goalService.UpdateGoal(goalRequest);

            result.IsSuccess.ShouldBeFalse();
            result.ErrorMessages.Count.ShouldBeGreaterThan(0);
            result.ErrorMessages.First().ShouldContain("There was a problem saving JSON: Source file not found after attempting save.");
        }

        [TestMethod]
        public void SetGoalsProgressHtml_Returns_UpdatedProgessHtml()
        {
            var goals = GetTestGoals();
            var expectedGoalPercentages = goals.Select(p => p.PercentageComplete).ToList();

            Should.NotThrow(() => _goalService.SetGoalsProgressHtml(goals));

            for (int i = 0; i < goals.Count; i++)
            {
                goals[i].ProgressHtml.ShouldContain(expectedGoalPercentages[i].ToString());
            }
        }

        private void GetFileLocation()
        {
            string path = Path.Combine(Environment.CurrentDirectory, @"Repository\Goals");
            var files = Directory.GetFiles(path);
            _fileLocation = files.SingleOrDefault();

            if (string.IsNullOrEmpty(_fileLocation))
            {
                throw new FileNotFoundException("JSON Data Source file not found");
            }
        }
    }
}
