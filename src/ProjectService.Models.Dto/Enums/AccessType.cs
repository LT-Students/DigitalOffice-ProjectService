using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LT.DigitalOffice.ProjectService.Models.Dto.Enums
{
  [JsonConverter(typeof(StringEnumConverter))]
  public enum AccessType
  {
    Manager,
    Team,
    Public
  }
}
