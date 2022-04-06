using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Models.Broker.Models.Company;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LT.DigitalOffice.ProjectService.Broker.Requests.Interfaces
{
  [AutoInject]
  public interface ICompanyService
  {
    Task<List<CompanyData>> GetCompaniesAsync(List<Guid> usersIds, List<string> errors);
  }
}
