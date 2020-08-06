using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace leave_management.Models
{
    public class LeaveTypeVM
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [Display(Name = "Default Number Of Days")]
        [Range(1, 28, ErrorMessage = "Please Enter A Valid Number (1 - 28)")]
        public int DefaultDays { get; set; }
        [Display(Name = "Date Created")]
        public DateTime? DateCreated { get; set; }
    }

}
