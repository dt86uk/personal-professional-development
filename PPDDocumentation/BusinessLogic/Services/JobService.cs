using AutoMapper;
using Newtonsoft.Json;
using PPDDocumentation.Models.Job;
using PPDDocumentation.Models.Requests;
using PPDDocumentation.Models.Responses;

namespace PPDDocumentation.BusinessLogic
{
    public class JobService : IJobService
    {
        private readonly ILogger<JobService> _logger;
        private readonly IMapper _mapper;
        private IMissionStatementService _missionStatementService { get; set; }
        private IFileService _fileService { get; set; }

        public JobService(ILogger<JobService> logger, 
            IMapper mapper, 
            IMissionStatementService missionStatementService, 
            IFileService fileService)
        {
            _logger = logger;
            _mapper = mapper;
            _missionStatementService = missionStatementService;
            _fileService = fileService;
        }

        public List<JobModel> GetJobs()
        {
            var jobsDataSource = _fileService.GetJobsJsonDataSourceFile();
            var json = File.ReadAllText(jobsDataSource);
            var jobs = JsonConvert.DeserializeObject<List<JobModel>>(json);

            return jobs.OrderBy(p => p.Id).Where(p => p.IsDeleted == false).ToList();
        }

        public List<JobsTableModel> GetJobsTable()
        {
            var jobs = GetJobs();
            var jobsTable = _mapper.Map<List<JobsTableModel>>(jobs);

            return jobsTable;
        }

        public JobResponse GetJobById(Guid id)
        {
            var jobs = GetJobs();
            var job = jobs.SingleOrDefault(p => p.Id == id);

            if (job == null)
            {
                return new JobResponse
                {
                    IsSuccess = false,
                    ErrorMessages = new List<string>
                    {
                        $"Job not found with ID '{id}'."
                    }
                };
            }

            return new JobResponse
            {
                IsSuccess = true,
                Job = job
            };
        }

        public JobResponse DeleteJob(Guid id)
        {
            var jobs = GetJobs();
            var job = jobs.SingleOrDefault(p => p.Id == id);

            if (job == null)
            {
                return JobNotFound(id);
            }

            _logger.LogInformation($"{nameof(JobService)}.{nameof(DeleteJob)} Info: Job '{id}' found.");

            job.IsDeleted = true;

            var isFileSaved = _fileService.UpdateJobJsonDataSourceFile(jobs);

            _logger.LogInformation($"{nameof(JobService)}.{nameof(DeleteJob)} Info: Job '{id}' deletion is {isFileSaved}");

            return new JobResponse
            {
                IsSuccess = isFileSaved,
                ErrorMessages = isFileSaved ? 
                    new List<string>() : 
                    new List<string> { "Job could not be saved." }
            };
        }

        public JobResponse AddJob(JobRequest request)
        {
            var jobs = GetJobs();

            jobs.Add(request.Job);

            var isFileSaved = _fileService.UpdateJobJsonDataSourceFile(jobs);

            if (isFileSaved)
            {
                return new JobResponse
                {
                    IsSuccess = isFileSaved,
                    Job = request.Job
                };
            }

            _logger.LogError($"Error saving JSON from updating Job '{request.Job.Title}' at {DateTime.Now.ToShortTimeString()}");
            return new JobResponse
            {
                IsSuccess = false,
                ErrorMessages = new List<string>
                {
                    "There was a problem saving JSON: Source file not found after attempting save."
                }
            };
        }

		public JobResponse UpdateJob(JobModel model)
		{
			var jobs = GetJobs();
			var job = jobs.SingleOrDefault(p => p.Id == model.Id);

			if (job == null)
			{
				return JobNotFound(model.Id);
			}

			_logger.LogInformation($"{nameof(JobService)}.{nameof(CompleteJob)} Info: Job '{model.Id}' found.");

			job.Title = model.Title;
			job.Description = model.Description;
			job.IsComplete = model.IsComplete;
			job.IsDeleted = model.IsDeleted;

			var isFileSaved = _fileService.UpdateJobJsonDataSourceFile(jobs);

			if (isFileSaved)
			{
				_logger.LogInformation($"{nameof(JobService)}.{nameof(CompleteJob)} Info: Job '{model.Id}' completion is {isFileSaved}");

				return new JobResponse
				{
					IsSuccess = true,
					Job = job
				};
			}

			_logger.LogError($"Error saving JSON from updating Goal '{job.Title}' at {DateTime.Now.ToShortTimeString()}");
			return new JobResponse
			{
				IsSuccess = false,
				ErrorMessages = new List<string>
				{
					"There was a problem saving JSON: Source file not found after attempting save."
				}
			};
		}

		public JobResponse CompleteJob(Guid id)
        {
            return SetJobCompletion(id, true);
        }

        public JobResponse IncompleteJob(Guid id)
        {
            return SetJobCompletion(id, false);
        }

		public JobResponse SetJobCompletion(Guid id, bool isComplete)
		{
			var jobs = GetJobs();
			var job = jobs.SingleOrDefault(p => p.Id == id);

			if (job == null)
			{
				return JobNotFound(id);
			}

			_logger.LogInformation($"{nameof(JobService)}.{nameof(CompleteJob)} Info: Job '{id}' found.");

			job.IsComplete = isComplete;

			var isFileSaved = _fileService.UpdateJobJsonDataSourceFile(jobs);

			if (isFileSaved)
			{
				_logger.LogInformation($"{nameof(JobService)}.{nameof(CompleteJob)} Info: Job '{id}' completion is {isFileSaved}");

				return new JobResponse
				{
					IsSuccess = true,
					Job = job
				};
			}

			_logger.LogError($"Error saving JSON from updating Goal '{job.Title}' at {DateTime.Now.ToShortTimeString()}");
			return new JobResponse
			{
				IsSuccess = false,
				ErrorMessages = new List<string>
				{
					"There was a problem saving JSON: Source file not found after attempting save."
				}
			};
		}

		private JobResponse JobNotFound(Guid id)
        {
            _logger.LogInformation($"{nameof(JobService)}.{nameof(CompleteJob)} Info: Job '{id}' not found.");
            return new JobResponse
            {
                IsSuccess = false,
                ErrorMessages = new List<string>
                {
                    $"Job '{id}' not found."
                }
            };
        }
    }
}
