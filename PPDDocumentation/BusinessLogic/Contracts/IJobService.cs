using PPDDocumentation.Models.Job;
using PPDDocumentation.Models.Requests;
using PPDDocumentation.Models.Responses;

namespace PPDDocumentation.BusinessLogic
{
    public interface IJobService
    {
        /// <summary>
        /// Gets all Jobs in the Json data source file 
        /// </summary>
        /// <returns></returns>
        public List<JobModel> GetJobs();

        /// <summary>
        /// Gets the data to display on the main Job list page
        /// </summary>
        /// <returns></returns>
        public List<JobsTableModel> GetJobsTable();

        /// <summary>
        /// Gets Job via Guid ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JobResponse GetJobById(Guid id);

        /// <summary>
        /// Sets the IsDeleted flag on the job to true
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JobResponse DeleteJob(Guid id);

        /// <summary>
        /// Creates a new job item.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public JobResponse AddJob(JobRequest request);

        /// <summary>
        /// Saves the job - setting IsComplete flag to what was provided
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JobResponse UpdateJob(JobModel model);

		/// <summary>
		/// Sets the IsComplete flag on the job
		/// </summary>
		/// <param name="id"></param>
		/// <param name="isComplete"></param>
		/// <returns></returns>
		public JobResponse SetJobCompletion(Guid id, bool isComplete);
	}
}
