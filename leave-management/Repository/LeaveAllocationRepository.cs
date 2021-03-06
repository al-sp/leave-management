﻿using leave_management.Contracts;
using leave_management.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace leave_management.Repository
{
    public class LeaveAllocationRepository : ILeaveAllocationRepository
    {
        private readonly ApplicationDbContext _db;
        public LeaveAllocationRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<bool> CheackAllocation(int leavetypeId, string employeeId)
        {
            var period = DateTime.Now.Year;
            var cheackAllocation = await FindAll();
            return cheackAllocation.Where(q => q.LeaveTypeId == leavetypeId && q.EmployeeId == employeeId && q.Period == period)
                .Any();
        }

        public async Task<bool> Create(LeaveAllocation entity)
        {
            await _db.LeaveAllocations.AddAsync(entity);
            return await Save();
        }

        public async Task<bool> Delete(LeaveAllocation entity)
        {
            _db.LeaveAllocations.Remove(entity);
            return await Save();
        }

        public async Task<ICollection<LeaveAllocation>> FindAll()
        {
            var leaveAllocation = await _db.LeaveAllocations
                .Include(q => q.LeaveType)
                .Include(q => q.Employee)
                .ToListAsync();
            return leaveAllocation;
        }

        public async Task<LeaveAllocation> FindById(int id)
        {
            var leaveAllocation = await _db.LeaveAllocations
                .Include(q => q.LeaveType)
                .Include(q => q.Employee)
                .FirstOrDefaultAsync(q => q.Id == id);
            return leaveAllocation;
        }

        public async Task<ICollection<LeaveAllocation>> GetLeaveAllocationsByEmployee(string employeeId)
        {
            int period = DateTime.Now.Year;
            var leaveAllocations = await FindAll();
            return leaveAllocations.Where(q => q.EmployeeId == employeeId && q.Period == period)
                .ToList();
        }

        public async Task<LeaveAllocation> GetLeaveAllocationsByEmployeeAndType(string employeeId, int leaveTypeId)
        {
            int period = DateTime.Now.Year;
            var leaveAllocations = await FindAll();
            return leaveAllocations.FirstOrDefault(q => q.EmployeeId == employeeId && q.Period == period && q.LeaveTypeId == leaveTypeId);
        }

        public async Task<bool> IsExists(int id)
        {
            var exists = await _db.LeaveAllocations.AnyAsync(q => q.Id == id);
            return exists;
        }

        public async Task<bool> Save()
        {
            var changes = await _db.SaveChangesAsync();
            return changes > 0;
        }

        public async Task<bool> Update(LeaveAllocation entity)
        {
            _db.LeaveAllocations.Update(entity);
            return await Save();
        }
    }
}
