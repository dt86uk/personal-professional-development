using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using PPDDocumentation.BusinessLogic;
using PPDDocumentation.Models.Job;
using PPDDocumentation.Models.Requests;
using PPDDocumentation.Models.Responses;
using Shouldly;
using System.Net;

namespace PPDDocumentation.UnitTests.BusinessLogic.Services
{
	[TestClass]
	public class JobServiceTests : BaseServiceTests
	{
		private Mock<ILogger<JobService>> _mockLogger;
		private Mock<IMapper> _mockMapper;
		private Mock<IMissionStatementService> _mockMissionStatementService;
		private Mock<IFileService> _mockFileService;
		private JobService _jobService;

		[TestInitialize]
		public void Init()
		{
			_mockLogger = new Mock<ILogger<JobService>>();
			_mockMapper = new Mock<IMapper>();
			_mockMissionStatementService = new Mock<IMissionStatementService>();
			_mockFileService = new Mock<IFileService>();
			_jobService = new JobService(_mockLogger.Object,
				_mockMapper.Object,
				_mockMissionStatementService.Object,
				_mockFileService.Object);
		}

		[TestMethod]
		public void GetJobs_Returns_Jobs()
		{
			_mockFileService
				.Setup(p => p.GetJobsJsonDataSourceFile())
				.Returns(GetTestJobsDataSource());

			var result = _jobService.GetJobs();

			result.ShouldNotBeNull();
			result.Count.ShouldBeGreaterThan(0);
			result.ShouldBeOfType<List<JobModel>>();
		}

		[TestMethod]
		public void GetJobsTable_Returns_JobsTable()
		{
			_mockFileService
				.Setup(p => p.GetJobsJsonDataSourceFile())
				.Returns(GetTestJobsDataSource());

			_mockMapper
				.Setup(p => p.Map<List<JobsTableModel>>(It.IsAny<List<JobModel>>()))
				.Returns(GetTestJobsTable());

			var result = _jobService.GetJobsTable();

			result.ShouldNotBeNull();
			result.Count.ShouldBeGreaterThan(0);
			result.ShouldBeOfType<List<JobsTableModel>>();
		}

		[TestMethod]
		public void GetJobById_Returns_Job()
		{
			var jobs = GetTestJobs();
			var firstId = jobs.First().Id;
			var expectedItem = jobs.First();
			
			_mockFileService
				.Setup(p => p.GetJobsJsonDataSourceFile())
				.Returns(GetTestJobsDataSource());

			var result = _jobService.GetJobById(JobId1);

			result.IsSuccess.ShouldBeTrue();
			result.Job.Id.ShouldBe(expectedItem.Id);
			result.Job.ShouldNotBeNull();
			result.Job.IsComplete.ShouldBeTrue();
			result.Job.IsDeleted.ShouldBeFalse();
			result.Job.Title.ShouldBe(expectedItem.Title);
			result.Job.Description.ShouldBe(expectedItem.Description);
		}

		[TestMethod]
		public void GetJobById_Returns_JobNotFound()
		{
			var id = Guid.NewGuid();
			var errorMessage = $"Job not found with ID '{id}'.";
			_mockFileService
				.Setup(p => p.GetJobsJsonDataSourceFile())
				.Returns(GetTestJobsDataSource());

			var result = _jobService.GetJobById(id);

			result.IsSuccess.ShouldBeFalse();
			result.Job.ShouldBeNull();
			result.ErrorMessages.Count.ShouldBeGreaterThan(0);
			result.ErrorMessages.First().ShouldBe(errorMessage);
		}

		[TestMethod]
		public void DeleteJob_Returns_Success()
		{
			_mockFileService
				.Setup(p => p.GetJobsJsonDataSourceFile())
				.Returns(GetTestJobsDataSource());

			_mockFileService
				.Setup(p => p.UpdateJobJsonDataSourceFile(It.IsAny<List<JobModel>>()))
				.Returns(true);

			var result = _jobService.DeleteJob(JobId2);

			result.ShouldNotBeNull();
			result.IsSuccess.ShouldBeTrue();
			result.ErrorMessages.ShouldBeEmpty();
		}

		[TestMethod]
		public void DeleteJob_Returns_JobNotFound()
		{
			var id = Guid.NewGuid();
			var errorMessage = $"Job '{id}' not found.";
			_mockFileService
				.Setup(p => p.GetJobsJsonDataSourceFile())
				.Returns(GetTestJobsDataSource());

			var result = _jobService.DeleteJob(id);

			result.ShouldNotBeNull();
			result.IsSuccess.ShouldBeFalse();
			result.ErrorMessages.First().ShouldBe(errorMessage);
		}

		[TestMethod]
		public void AddJob_Returns_Success()
		{
			var jobRequest = new JobRequest
			{
				Id = Guid.NewGuid(),
				Job = new JobModel(Guid.NewGuid())
				{
					Title = "Title",
					Description = "Description",
					IsComplete = false,
					IsDeleted = false
				}
			};
            _mockFileService
                .Setup(p => p.GetJobsJsonDataSourceFile())
                .Returns(GetTestJobsDataSource());
			_mockFileService
				.Setup(p => p.UpdateJobJsonDataSourceFile(It.IsAny<List<JobModel>>()))
				.Returns(true);

            var result = _jobService.AddJob(jobRequest);

			result.ShouldNotBeNull();
			result.ShouldBeOfType<JobResponse>();
			result.IsSuccess.ShouldBeTrue();
			result.Job.ShouldNotBeNull();
			result.Job.Title.ShouldBe(jobRequest.Job.Title);
			result.Job.Description.ShouldBe(jobRequest.Job.Description);
            result.Job.IsComplete.ShouldBe(jobRequest.Job.IsComplete);
			result.Job.IsDeleted.ShouldBe(jobRequest.Job.IsDeleted);
        }

		[TestMethod]
		public void AddJob_Returns_Failure_ProblemSavingJsonFile()
		{
			var errorMessage = "There was a problem saving JSON: Source file not found after attempting save.";
            var jobRequest = new JobRequest
            {
                Id = Guid.NewGuid(),
                Job = new JobModel(Guid.NewGuid())
                {
                    Title = "Title",
                    Description = "Description",
                    IsComplete = false,
                    IsDeleted = false
                }
            };

            _mockFileService
                .Setup(p => p.GetJobsJsonDataSourceFile())
                .Returns(GetTestJobsDataSource());

            _mockFileService
                .Setup(p => p.UpdateJobJsonDataSourceFile(It.IsAny<List<JobModel>>()))
                .Returns(false);

            var result = _jobService.AddJob(jobRequest);

            result.ShouldNotBeNull();
            result.ShouldBeOfType<JobResponse>();
            result.IsSuccess.ShouldBeFalse();
            result.Job.ShouldBeNull();
			result.ErrorMessages.ShouldNotBeEmpty();
			result.ErrorMessages.First().ShouldBe(errorMessage);
        }

		[TestMethod]
		public void UpdateJob_Returns_Success()
		{
			var job = new JobModel(JobId1)
			{
				Title = "Title 2",
				Description = "Description 2",
				IsComplete = false,
				IsDeleted = false
			};

            _mockFileService
                .Setup(p => p.GetJobsJsonDataSourceFile())
                .Returns(GetTestJobsDataSource());

            _mockFileService
                .Setup(p => p.UpdateJobJsonDataSourceFile(It.IsAny<List<JobModel>>()))
                .Returns(true);

            var result = _jobService.UpdateJob(job);

			result.ShouldNotBeNull();
            result.ShouldBeOfType<JobResponse>();
            result.IsSuccess.ShouldBeTrue();
            result.Job.ShouldNotBeNull();
            result.Job.Title.ShouldBe(job.Title);
            result.Job.Description.ShouldBe(job.Description);
            result.Job.IsComplete.ShouldBe(job.IsComplete);
            result.Job.IsDeleted.ShouldBe(job.IsDeleted);
        }

		[TestMethod]
		public void UpdateJob_Returns_Failure_JobNotFound()
		{
            var id = Guid.NewGuid();
			var errorMessage = $"Job '{id}' not found.";
            var job = new JobModel(id)
            {
                Title = "Title",
                Description = "Description",
                IsComplete = false,
                IsDeleted = false
            };

            _mockFileService
                .Setup(p => p.GetJobsJsonDataSourceFile())
                .Returns(GetTestJobsDataSource());

            var result = _jobService.UpdateJob(job);

            result.ShouldNotBeNull();
            result.ShouldBeOfType<JobResponse>();
            result.IsSuccess.ShouldBeFalse();
            result.ErrorMessages.ShouldNotBeEmpty();
			result.ErrorMessages.First().ShouldBe(errorMessage);
        }

		[TestMethod]
		public void UpdateJob_Returns_Failure_ProblemSavingJsonFile()
		{
			var errorMessage = "There was a problem saving JSON: Source file not found after attempting save.";
            var job = new JobModel(JobId1)
            {
                Title = "Title",
                Description = "Description",
                IsComplete = false,
                IsDeleted = false
            };

            _mockFileService
                .Setup(p => p.GetJobsJsonDataSourceFile())
                .Returns(GetTestJobsDataSource());

            _mockFileService
                .Setup(p => p.UpdateJobJsonDataSourceFile(It.IsAny<List<JobModel>>()))
                .Returns(false);

            var result = _jobService.UpdateJob(job);

            result.ShouldNotBeNull();
            result.ShouldBeOfType<JobResponse>();
            result.IsSuccess.ShouldBeFalse();
            result.ErrorMessages.ShouldNotBeEmpty();
            result.ErrorMessages.First().ShouldBe(errorMessage);
        }

		[TestMethod]
		public void SetJobCompletion_Returns_Success()
		{
            _mockFileService
                .Setup(p => p.GetJobsJsonDataSourceFile())
                .Returns(GetTestJobsDataSource());

            _mockFileService
                .Setup(p => p.UpdateJobJsonDataSourceFile(It.IsAny<List<JobModel>>()))
                .Returns(true);

            var result = _jobService.SetJobCompletion(JobId1, true);

            result.IsSuccess.ShouldBeTrue();
            result.Job.ShouldNotBeNull();
            result.Job.IsComplete.ShouldBeTrue();
        }

		[TestMethod]
		public void SetJobCompletion_Returns_Failure_JobNotFound()
		{
            var id = Guid.NewGuid();
            var errorMessage = $"Job '{id}' not found.";

            _mockFileService
                .Setup(p => p.GetJobsJsonDataSourceFile())
                .Returns(GetTestJobsDataSource());

            var result = _jobService.SetJobCompletion(id, true);

            result.ShouldNotBeNull();
            result.ShouldBeOfType<JobResponse>();
            result.IsSuccess.ShouldBeFalse();
            result.ErrorMessages.ShouldNotBeEmpty();
            result.ErrorMessages.First().ShouldBe(errorMessage);
        }

		[TestMethod]
		public void SetJobCompletion_Returns_Failure_ProblemSavingJsonFile()
		{
            var errorMessage = "There was a problem saving JSON: Source file not found after attempting save.";

            _mockFileService
                .Setup(p => p.GetJobsJsonDataSourceFile())
                .Returns(GetTestJobsDataSource());

            _mockFileService
                .Setup(p => p.UpdateJobJsonDataSourceFile(It.IsAny<List<JobModel>>()))
                .Returns(false);

            var result = _jobService.SetJobCompletion(JobId1, true);

            result.ShouldNotBeNull();
			result.IsSuccess.ShouldBeFalse();
            result.ErrorMessages.ShouldNotBeEmpty();
            result.ErrorMessages.First().ShouldBe(errorMessage);
        }
    }
}
