namespace LT.DigitalOffice.ProjectService.Models.Dto.Configurations
{
  public record RedisConfig
  {
    public const string SectionName = "Redis";

    public double CacheLiveInMinutes { get; set; }
  }
}
