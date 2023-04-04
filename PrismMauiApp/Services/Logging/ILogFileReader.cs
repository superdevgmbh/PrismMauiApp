namespace PrismMauiApp.Services.Logging
{
    public interface ILogFileReader
    {
        string FilePath { get; }

        Task<string> ReadLogFileAsync();

        Task FlushLogFileAsync();
    }
}