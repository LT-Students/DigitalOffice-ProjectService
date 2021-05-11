using System;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.ProjectService.Models.Db;

namespace LT.DigitalOffice.ProjectService.Data.Interfaces
{
    [AutoInject]
    public interface ITaskPropertyRepository
    {
        bool AreExist(params Guid[] ids);
        DbTaskProperty Get(Guid propertyId);
    }
}
