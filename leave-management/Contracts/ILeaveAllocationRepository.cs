using leave_management.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace leave_management.Contracts
{
    public interface ILeaveAllocationRepository : IRepositoryBase<LeaveAllocation>
    {
        Task<bool> CheackAllocation(int leavetypeId, string employeeId);
        Task<ICollection<LeaveAllocation>> GetLeaveAllocationsByEmployee(string employeeId);
        Task<LeaveAllocation> GetLeaveAllocationsByEmployeeAndType(string employeeId, int leaveTypeId);
    }
}
