using PPDDocumentation.Models;
using PPDDocumentation.Models.Job;

namespace PPDDocumentation.BusinessLogic
{
    /// <summary>
    /// Accesses the local machine's file system to retrieve the source file.
    /// </summary>
    public interface IFileService
    {
        /// <summary>
        /// Returns the JSON file containing the goals with their actions
        /// </summary>
        /// <returns></returns>
        public string GetGoalJsonDataSourceFile();

        /// <summary>
        /// Saves the updated Goal JSON to the local disk.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public bool UpdateGoalJsonDataSourceFile(MissionStatementModel missionStatementModel);

        /// <summary>
        /// Returns the JSON file containing the jobs 
        /// </summary>
        /// <returns></returns>
        public string GetJobsJsonDataSourceFile();

        /// <summary>
        /// Saves the updated Job JSON to the local disk.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        bool UpdateJobJsonDataSourceFile(List<JobModel> jobs);
    }
}
