﻿using leave_management.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace leave_management.Models
{
    public class LeaveRequestVM
    {
        public int Id { get; set; }

        [Display(Name = "Employee Name")]
        public Employee RequestingEmployee { get; set; }

        [Display(Name = "Employee Name")]
        public string RequestingEmployeeId { get; set; }

        [Display(Name = "Start Date")]
        [Required]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Display(Name = "End Date")]
        [Required]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        public LeaveType LeaveType { get; set; }

        public int LeaveTypeId { get; set; }

        [Display(Name = "Date Requested")]
        public DateTime DateRequested { get; set; }

        [Display(Name = "Date Actioned")]
        public DateTime DateActioned { get; set; }

        [Display(Name = "Approval State")]
        public bool? Approved { get; set; }

        public Employee ApprovedBy { get; set; }

        [Display(Name = "Approver Name")]
        public string ApprovedById { get; set; }

        public bool Cancelled { get; set; }

        [Display(Name = "Empoloyee Comments")]
        [MaxLength(300)]
        public string RequestComments { get; set; }
    }

    public class AdminLeaveRequestViewVM
    {
        [Display(Name = "Total Request")]
        public int TotalRequest { get; set; }

        [Display(Name = "Approved Request")]
        public int ApprovedRequest { get; set; }

        [Display(Name = "Pending Request")]
        public int PendingRequest { get; set; }

        [Display(Name = "Rejected Request")]
        public int RejectedRequest { get; set; }

        public List<LeaveRequestVM> LeaveRequests { get; set; }
    }

    public class CreateLeaveRequestVM
    {
        [Display(Name = "Start Date")]
        [Required]
        public string StartDate { get; set; }

        [Display(Name = "End Date")]
        [Required]
        public string EndDate { get; set; }

        [Display(Name = "Empoloyee Comments")]
        [MaxLength(300)]
        public string RequestComments { get; set; }

        public IEnumerable<SelectListItem> LeaveTypes { get; set; }

        [Display(Name = "Leave Type")]
        public int LeaveTypeId { get; set; }
    }

    public class EmployeeLeaveRequestViewVM
    {
        public List<LeaveAllocationVM> LeaveAllocations { get; set; }
        public List<LeaveRequestVM> LeaveRequests { get; set; }
    }
}
