using PPDDocumentation.Models.Goal;
using PPDDocumentation.Models;
using PPDDocumentation.Models.Job;

namespace PPDDocumentation.UnitTests.BusinessLogic.Services
{
    public class BaseServiceTests
    {
        // If Unit Tests do not work, due to the Id values below
        // compare the values that are in the JSON repository files - make sure they match
        public Guid GoalId = Guid.Parse("0912beeb-e0ce-4519-b89b-e21da010ecd9");
        public Guid ActionId = Guid.Parse("7af40b05-b6bc-412e-a676-df7073b23408");

        public Guid GoalId2 = Guid.Parse("f04f730c-47a5-455a-8637-5b429eee9293");
        public Guid ActionId2 = Guid.NewGuid();

        public Guid JobId1 = Guid.Parse("002cfd62-4983-4800-a56b-cf4164741d58");
		public Guid JobId2 = Guid.Parse("0688de13-a5d5-4080-96f5-2fcbcb62334c");

		public List<GoalModel> GetTestGoals()
        {
            return new List<GoalModel>()
            {
                new GoalModel(GoalId)
                {
                    Title = "Test",
                    Description = "Test Description",
                    PercentageComplete = 40,
                    WhatILearnt = "Stuff",
                    OrderId = 1,
                    DueBy = 1,
                    Actions = new List<ActionModel>
                    {
                        new ActionModel(ActionId)
                        {
                            Title = "Title",
                            Description = $"Description",
                            IsComplete = false,
                            IsDeleted = false,
                            OrderId = 1,
                            PercentageComplete = 10,
                            WhatILearnt = "WhatILearnt",
                        }
                    }
                },
                new GoalModel(GoalId2)
                {
                    Title = "Test",
                    Description = "Test Description",
                    PercentageComplete = 60,
                    WhatILearnt = "WhatILearnt",
                    OrderId = 2,
                    DueBy = 2,
                    Actions = new List<ActionModel>()
                    {
                        new ActionModel(ActionId2)
                        {
                            Title = "Title",
                            Description = $"Description",
                            IsComplete = false,
                            IsDeleted = false,
                            OrderId = 1,
                            PercentageComplete = 30,
                            WhatILearnt = "WhatILearnt"
                        }
                    }
                }
            };
        }

        public MissionStatementModel GetTestMissionStatementModel()
        {
            return new MissionStatementModel
            {
                GoalsMe = GetTestGoals()
            };
        }

        public string GetTestGoalsDataSource()
        {
            string path = Path.Combine(Environment.CurrentDirectory, @"Repository\Goals");
            var files = Directory.GetFiles(path);
            var file = files.First();

            return file;
        }

        public string GetTestJobsDataSource()
        {
            string path = Path.Combine(Environment.CurrentDirectory, @"Repository\Jobs");
            var files = Directory.GetFiles(path);
            var file = files.First();

            return file;
        }

		public List<JobModel> GetTestJobs()
        {
            return new List<JobModel>
            {
                new JobModel(JobId1)
                {
                    Title = "Add Jobs page",
                    Description = "This lists immediate 'todos' on the site.",
					IsComplete = true,
                    IsDeleted = false
				},
				new JobModel(JobId2)
				{
					Title = "Title TEST 2",
					Description = "Description TEST 2",
					IsComplete = true,
					IsDeleted = false
				},
			}; 
		}

		public List<JobsTableModel> GetTestJobsTable()
		{
			return new List<JobsTableModel>
			{
				new JobsTableModel()
				{
					Title = "Title",
					Description = "Description",
					IsComplete = false
				},
				new JobsTableModel()
				{
					Title = "Title",
					Description = "Description",
					IsComplete = true
				}
			};
		}
	}
}
