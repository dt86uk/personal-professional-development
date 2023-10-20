using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PPDDocumentation.Models
{
    public class TaskViewModelBase
    {
        public Guid ParentId { get; set; }
        public Guid Id { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "* Required")]
        public string Title { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "* Required")]
        public string Description { get; set; }

        [DisplayName("Percentage Complete")]
        [Range(0, 100, ErrorMessage = "* Choose Between 1-100")]
        public int? PercentageComplete { get; set; }

        [DisplayName("What did I learn?")]
        public string? WhatILearnt { get; set; }

        [DisplayName("Deleted?")]
        public bool IsDeleted { get; set; }
    }
}