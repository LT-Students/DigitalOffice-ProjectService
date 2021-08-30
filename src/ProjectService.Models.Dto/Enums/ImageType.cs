using Newtonsoft.Json.Converters;
using System.Text.Json.Serialization;
namespace LT.DigitalOffice.ProjectService.Models.Dto.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ImageType
    {
        Project,
        Task
    }
}
