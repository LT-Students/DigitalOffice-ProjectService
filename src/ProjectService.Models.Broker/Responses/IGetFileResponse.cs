namespace LT.DigitalOffice.Broker.Responses
{
    /// <summary>
    /// Represents response for GetFileConsumer in MassTransit logic.
    /// </summary>
    public interface IGetFileResponse
    {
        string Content { get; }
        string Extension { get; }
        string Name { get; }
    }
}