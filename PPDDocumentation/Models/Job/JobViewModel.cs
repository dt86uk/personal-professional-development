using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PPDDocumentation.Models.Job
{
    public class JobViewModel
    {
        public Guid Id { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "* Required")]
        public string Title { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "* Required")]
        public string Description { get; set; }

        [BindProperty]
        [DisplayName("Completed?")]
        public bool IsComplete { get; set; }

        [BindProperty]
        [DisplayName("Deleted?")]
        public bool IsDeleted { get; set; }
    }
}
