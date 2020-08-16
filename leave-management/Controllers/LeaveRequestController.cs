using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using leave_management.Contracts;
using leave_management.Data;
using leave_management.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace leave_management.Controllers
{
    [Authorize]
    public class LeaveRequestController : Controller
    {
        private readonly ILeaveRequestRepository _leaveRequestRepo;
        private readonly ILeaveTypeRepository _leaveTypeRepo;
        private readonly ILeaveAllocationRepository _leaveAllocationRepo;
        private readonly IMapper _mapper;
        private readonly UserManager<Employee> _userManager;

        public LeaveRequestController(
            ILeaveRequestRepository leaveRequestRepo,
            ILeaveTypeRepository leaveTypeRepo,
            ILeaveAllocationRepository leaveAllocationRepo,
            IMapper mapper,
            UserManager<Employee> userManager
            )
        {
            _leaveRequestRepo = leaveRequestRepo;
            _leaveTypeRepo = leaveTypeRepo;
            _leaveAllocationRepo = leaveAllocationRepo;
            _mapper = mapper;
            _userManager = userManager;
        }

        // GET: LeaveRequestController
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult> Index()
        {
            var leaveRequest = await _leaveRequestRepo.FindAll();
            var leaveRequestsModel = _mapper.Map<List<LeaveRequestVM>>(leaveRequest);

            var model = new AdminLeaveRequestViewVM
            {
                TotalRequest = leaveRequestsModel.Count,
                ApprovedRequest = leaveRequestsModel.Count(q => q.Approved == true),
                PendingRequest = leaveRequestsModel.Count(q => q.Approved == null),
                RejectedRequest = leaveRequestsModel.Count(q => q.Approved == false),
                LeaveRequests = leaveRequestsModel
            };

            return View(model);
        }

        // GET: LeaveRequestController/Details/5
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult> Details(int id)
        {
            var leaveRequest = await _leaveRequestRepo.FindById(id);
            var model = _mapper.Map<LeaveRequestVM>(leaveRequest);
            return View(model);
        }

        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult> ApproveRequest(int id)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                var leaveRequest = await _leaveRequestRepo.FindById(id);
                var employeeId = leaveRequest.RequestingEmployeeId;
                var leaveTypeId = leaveRequest.LeaveTypeId;
                var allocation = await _leaveAllocationRepo.GetLeaveAllocationsByEmployeeAndType(employeeId, leaveTypeId);
                int daysRequested = (int)(leaveRequest.EndDate - leaveRequest.StartDate).TotalDays;

                allocation.NumberOfDays -= daysRequested;

                leaveRequest.Approved = true;
                leaveRequest.ApprovedById = user.Id;
                leaveRequest.DateActioned = DateTime.Now;

                await _leaveRequestRepo.Update(leaveRequest);
                await _leaveAllocationRepo.Update(allocation);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Something Went Wrong With Submitting Your Record");
                return View();
            }
        }

        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult> RejectRequest(int id)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                var leaveRequest = await _leaveRequestRepo.FindById(id);
                leaveRequest.Approved = false;
                leaveRequest.ApprovedById = user.Id;
                leaveRequest.DateActioned = DateTime.Now;

                await _leaveRequestRepo.Update(leaveRequest);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Something Went Wrong With Submitting Your Record");
                return View();
            }
        }

        public async Task<ActionResult> MyLeave()
        {
            var employee = await _userManager.GetUserAsync(User);
            var leaveRequest = await _leaveRequestRepo.GetLeaveRequstByEmployee(employee.Id);
            var leaveAllocation = await _leaveAllocationRepo.GetLeaveAllocationsByEmployee(employee.Id);

            var leaveRequstModel = _mapper.Map<List<LeaveRequestVM>>(leaveRequest);
            var leaveAllocationModel = _mapper.Map<List<LeaveAllocationVM>>(leaveAllocation);

            var model = new EmployeeLeaveRequestViewVM
            {
                LeaveAllocations = leaveAllocationModel,
                LeaveRequests = leaveRequstModel
            };

            return View(model);
        }

        // todo!!!!
        /*public ActionResult CancelRequest(int id)
        {
            try
            {
                var employee = _userManager.GetUserAsync(User).Result;
                var leaveRequest = _leaveRequestRepo.FindById(id);
                var leaveAllocation = _leaveAllocationRepo.GetLeaveAllocationsByEmployeeAndType(employee.Id, leaveRequest.LeaveTypeId);

                var daysRequested = (int)(leaveRequest.EndDate - leaveRequest.StartDate).TotalDays;

                if(leaveRequest.Approved == true)
                {
                    leaveAllocation.NumberOfDays += daysRequested;
                }
              
                _leaveAllocationRepo.Update(leaveAllocation);
                _leaveRequestRepo.Delete(leaveRequest);

                return RedirectToAction(nameof(MyLeave));
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Something Went Wrong With Submitting Your Record");
                return View();
            }
        }*/

        public async Task<ActionResult> CancelRequest(int id)
        {
            var leaveRequest = await _leaveRequestRepo.FindById(id);
            leaveRequest.Canceled = true;
            await _leaveRequestRepo.Update(leaveRequest);
            return RedirectToAction("MyLeave");
        }

        // GET: LeaveRequestController/Create
        public async Task<ActionResult> Create()
        {
            var leaveTypes = await _leaveTypeRepo.FindAll();
            var leaveTypeItems = leaveTypes.Select(q => new SelectListItem {
                Text = q.Name,
                Value = q.Id.ToString()
            });

            var model = new CreateLeaveRequestVM
            {
                LeaveTypes = leaveTypeItems
            };

            return View(model);
        }

        // POST: LeaveRequestController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CreateLeaveRequestVM model)
        {
            try
            {
                var startDate = Convert.ToDateTime(model.StartDate);
                var endDate = Convert.ToDateTime(model.EndDate);

                var leaveTypes = await _leaveTypeRepo.FindAll();
                var leaveTypeItems = leaveTypes.Select(q => new SelectListItem
                {
                    Text = q.Name,
                    Value = q.Id.ToString()
                });

                model.LeaveTypes = leaveTypeItems;

                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                if (DateTime.Compare(startDate, endDate) > 1)
                {
                    ModelState.AddModelError("", "Start Date cannot be further in the future than the End Date");
                }

                var employee = await _userManager.GetUserAsync(User);
                var allocation = await _leaveAllocationRepo.GetLeaveAllocationsByEmployeeAndType(employee.Id, model.LeaveTypeId);
                int daysRequested = (int)(endDate - startDate).TotalDays;

                if (daysRequested > allocation.NumberOfDays)
                {
                    ModelState.AddModelError("", "You Do Not Sufficient Days For This Request");
                    return View(model);
                }

                var leaveRequestModel = new LeaveRequestVM
                {
                    RequestingEmployeeId = employee.Id,
                    StartDate = startDate,
                    EndDate = endDate,
                    Approved = null,
                    DateRequested = DateTime.Now,
                    DateActioned = DateTime.Now,
                    LeaveTypeId = model.LeaveTypeId,
                    Cancelled = false,
                    RequestComments = model.RequestComments,
                };

                var leaveRequest = _mapper.Map<LeaveRequest>(leaveRequestModel);
                var isSuccess = await _leaveRequestRepo.Create(leaveRequest);

                if(!isSuccess)
                {
                    ModelState.AddModelError("", "Something Went Wrong With Submitting Your Record");
                    return View(model);
                }

                return RedirectToAction(nameof(MyLeave));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Something Went Wrong");
                return View(model);
            }
        }

        // GET: LeaveRequestController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: LeaveRequestController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: LeaveRequestController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: LeaveRequestController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
