using Newtonsoft.Json;
using PPDDocumentation.Models;
using PPDDocumentation.Models.Job;

namespace PPDDocumentation.BusinessLogic
{
    public class MissionStatementService : IMissionStatementService
    {
        private IFileService _fileService;

        public MissionStatementService(IFileService fileService)
        {
            _fileService = fileService;
        }

        public MissionStatementModel GetMissionStatement()
        {
            var jsonDataSourceFile = _fileService.GetGoalJsonDataSourceFile();
            var json = File.ReadAllText(jsonDataSourceFile);
            var missionStatement = JsonConvert.DeserializeObject<MissionStatementModel>(json);

            return missionStatement;
        }
    }
}
