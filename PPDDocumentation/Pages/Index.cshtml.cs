using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PPDDocumentation.BusinessLogic;
using PPDDocumentation.Models.Goal;

namespace PPDDocumentation.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IMissionStatementService _missionStatementService;
        private readonly IGoalService _goalService;

        public string MissionStatementTitle { get; set; }
        public string MissionStatementDescription { get; set; }
        public List<GoalsTableModel> Goals { get; set; }

        [TempData]
        public string ActionResponseMessage { get; set; }

        public IndexModel(IMissionStatementService missionStatementService, IGoalService goalService)
        {
            _missionStatementService = missionStatementService;
            _goalService = goalService;
        }

        public void OnGet()
        {
            var missionStatement = _missionStatementService.GetMissionStatement();
            MissionStatementTitle = missionStatement.Title;
            MissionStatementDescription = missionStatement.Description;

            Goals = _goalService.GetGoalsTable();

            ActionResponseMessage = TempData?.Peek("ActionResponseMessage") == null ? 
                string.Empty : 
                TempData?.Peek("ActionResponseMessage").ToString();
        }
    }
}