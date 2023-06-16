namespace PrismMauiApp.Services
{
    public interface IDisplayRepository
    {
        Task<ICollection<DisplayConfiguration>> GetDisplayConfigurationsAsync();

        Task AddOrUpdateDisplayConfigurationAsync(DisplayConfiguration displayConfiguration);
        
        Task RemoveDisplayConfigurationAsync(DisplayConfiguration displayConfiguration);
    }

    public class DisplayConfiguration
    {
        public string DisplayName { get; set; }

        public string SSID { get; set; }

        public string PSK { get; set; }
    }
}