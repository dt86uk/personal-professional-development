using Moq;
using Microsoft.Extensions.Logging;
using PPDDocumentation.BusinessLogic;
using Shouldly;
using PPDDocumentation.Models;

namespace PPDDocumentation.UnitTests.BusinessLogic.Services
{
    [TestClass]
    public class FileServiceTests : BaseServiceTests
    {
        private Mock<ILogger<FileService>> _mockLogger;
        private FileService _fileService;

        [TestInitialize]
        public void Init()
        {
            _mockLogger = new Mock<ILogger<FileService>>();
            _fileService = new FileService(_mockLogger.Object);
        }

        [TestMethod]
        public void GetGoalJsonDataSourceFile_Returns_FileLocation()
        {
            var result = _fileService.GetGoalJsonDataSourceFile();

            result.ShouldNotBeNullOrEmpty();
            result.Contains("C:\\");
        }

        [TestMethod]
        public void UpdateGoalJsonDataSourceFile_Returns_True()
        {
            var missionStatement = new MissionStatementModel
            {
                GoalsMe = GetTestGoals()
            };

            var result = _fileService.UpdateGoalJsonDataSourceFile(missionStatement);

            result.ShouldBeTrue();
        }

        [TestMethod]
        public void GetJobsJsonDataSourceFile_Returns_FileLocation()
        {
			var result = _fileService.GetGoalJsonDataSourceFile();

			result.ShouldNotBeNullOrEmpty();
			result.Contains("C:\\");
		}

        [TestMethod]
        public void UpdateJobJsonDataSourceFile_Returns_True()
        {
            var jobs = GetTestJobs();

			var result = _fileService.UpdateJobJsonDataSourceFile(jobs);

			result.ShouldBeTrue();
		}
	}
}
