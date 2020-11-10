using System;

namespace LT.DigitalOffice.Broker.Requests
{
    public interface IGetDepartmentRequest
    {
        Guid DepartmentId { get; }

        static object CreateObj(Guid departmentId)
        {
            return new
            {
                DepartmentId = departmentId
            };
        }
    }
}
