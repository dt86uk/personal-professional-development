using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PPDDocumentation.Models.Goal
{
    public class GoalViewModel : TaskViewModelBase
    {
        [BindProperty]
        [Required(ErrorMessage = "* Required")]
        [Range(1, 4, ErrorMessage = "* Choose Between 1-4")]
        [DisplayName("Due By (Qtr)")]
        public int? DueBy { get; set; }
    }
}
