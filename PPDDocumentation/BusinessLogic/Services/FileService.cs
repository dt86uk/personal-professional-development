using Newtonsoft.Json;
using PPDDocumentation.Models;
using PPDDocumentation.Models.Job;

namespace PPDDocumentation.BusinessLogic
{
    public class FileService : IFileService
    {
        private readonly ILogger<FileService> _logger;

        public FileService(ILogger<FileService> logger)
        {
            _logger = logger;
        }

        public string GetGoalJsonDataSourceFile()
        {
            _logger.LogError($"Goals JSON data source file acquired at {DateTime.Now.ToShortTimeString()}");

            // get all json files from Goals folder
            string path = Path.Combine(Environment.CurrentDirectory, @"Repository\Goals");
            var files = Directory.GetFiles(path);
            var file = files.SingleOrDefault();

            if (file == null)
            {
                throw new Exception("No json file found in Repository file system.");
            }

            return file;
        }

        public bool UpdateGoalJsonDataSourceFile(MissionStatementModel missionStatementModel)
        {
            try
            {
                string jsonDataSourceFile = GetGoalJsonDataSourceFile();
                string output = JsonConvert.SerializeObject(missionStatementModel, Formatting.Indented);
                File.Delete(jsonDataSourceFile);
                File.WriteAllText($"{jsonDataSourceFile}", output);
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error saving JSON file: {ex.Message}");
                return false;
            }
        }

        public string GetJobsJsonDataSourceFile()
        {
            _logger.LogError($"Jobs JSON data source file acquired at {DateTime.Now.ToShortTimeString()}");

            // get all json files from Goals folder
            string path = Path.Combine(Environment.CurrentDirectory, @"Repository\Jobs");
            var files = Directory.GetFiles(path);
            var file = files.SingleOrDefault();

            if (file == null)
            {
                throw new Exception("No json file found in Repository file system.");
            }

            return file;
        }

        public bool UpdateJobJsonDataSourceFile(List<JobModel> jobs)
        {
            try
            {
                string jsonDataSourceFile = GetJobsJsonDataSourceFile();
                string output = JsonConvert.SerializeObject(jobs, Formatting.Indented);
                File.Delete(jsonDataSourceFile);
                File.WriteAllText($"{jsonDataSourceFile}", output);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error saving JSON file: {ex.Message}");
                return false;
            }
        }
    }
}
