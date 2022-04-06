using LT.DigitalOffice.Kernel.BrokerSupport.Helpers;
using LT.DigitalOffice.Kernel.RedisSupport.Constants;
using LT.DigitalOffice.Kernel.RedisSupport.Extensions;
using LT.DigitalOffice.Kernel.RedisSupport.Helpers.Interfaces;
using LT.DigitalOffice.Models.Broker.Models.Company;
using LT.DigitalOffice.Models.Broker.Requests.Company;
using LT.DigitalOffice.Models.Broker.Responses.Company;
using LT.DigitalOffice.ProjectService.Broker.Requests.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LT.DigitalOffice.ProjectService.Broker.Requests
{
  public class CompanyService : ICompanyService
  {
    private readonly IRequestClient<IGetCompaniesRequest> _rcGetCompanies;
    private readonly ILogger<CompanyService> _logger;
    private readonly IGlobalCacheRepository _globalCache;

    public CompanyService(
      IRequestClient<IGetCompaniesRequest> rcGetCompanies,
      ILogger<CompanyService> logger,
      IGlobalCacheRepository globalCache)
    {
      _rcGetCompanies = rcGetCompanies;
      _logger = logger;
      _globalCache = globalCache;
    }

    public async Task<List<CompanyData>> GetCompaniesAsync(
      List<Guid> usersIds,
      List<string> errors)
    {
      List<CompanyData> companies = await _globalCache.GetAsync<List<CompanyData>>(Cache.Companies, usersIds.GetRedisCacheHashCode());

      if (companies is not null)
      {
        _logger.LogInformation($"Companies were taken from the cache.");
      }
      else
      {
        companies = (await RequestHandler.ProcessRequest<IGetCompaniesRequest, IGetCompaniesResponse>(
            _rcGetCompanies,
            IGetCompaniesRequest.CreateObj(usersIds),
            errors,
            _logger))
          ?.Companies;
      }

      return companies;
    }
  }
}
