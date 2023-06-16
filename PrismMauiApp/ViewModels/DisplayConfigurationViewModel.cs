using PrismMauiApp.Services;

namespace PrismMauiApp.ViewModels
{
    public class DisplayConfigurationViewModel
    {
        public DisplayConfigurationViewModel(DisplayConfiguration displayConfiguration)
        {
            this.DisplayName = displayConfiguration.DisplayName;
        }

        public string DisplayName { get; }
    }
}