using PPDDocumentation.Models;

namespace PPDDocumentation.BusinessLogic
{
    /// <summary>
    /// Mission Statement operations.
    /// </summary>
    public interface IMissionStatementService
    {
        /// <summary>
        /// Gets the Mission Statement (top-level) and it's child properties.
        /// </summary>
        /// <returns></returns>
        public MissionStatementModel GetMissionStatement();
    }
}
