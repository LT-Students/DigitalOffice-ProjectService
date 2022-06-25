﻿using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Models.Broker.Models.Position;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LT.DigitalOffice.ProjectService.Broker.Requests.Interfaces
{
  [AutoInject]
  public interface IPositionService
  {
    Task<List<PositionData>> GetPositionsAsync(
      List<Guid> usersIds,
      List<string> errors);
  }
}
