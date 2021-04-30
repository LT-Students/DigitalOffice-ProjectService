using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.Broker.Responses
{
    public interface IFindDepartmentsResponse
    {
        IDictionary<Guid, string> IdNamePairs { get; }

        static object CreateObj(IDictionary<Guid, string> idNamePairs)
        {
            return new
            {
                IdNamePairs = idNamePairs
            };
        }
    }
}