namespace PrismMauiApp.ViewModels
{
    public class AppStartViewModel : ViewModelBase
    {
        private readonly ILogger logger;
        private readonly INavigationService navigationService;

        public AppStartViewModel(
            ILogger<AppStartViewModel> logger,
            INavigationService navigationService)
        {
            this.logger = logger;
            this.navigationService = navigationService;
        }

        public override async void Initialize(INavigationParameters parameters)
        {
            // The first navigation call to the very first page must not be awaited.
            // That's why we use async void here.

            this.IsBusy = true;

            try
            {
                await Task.Delay(1000);

                // HACK: For some Prism-reasons we have to set the selected tab to 'NavigationPage' - not MainPage or something similar.
                var result = await this.navigationService.CreateBuilder()
                    .AddTabbedSegment(App.Pages.TabbedMainPage, c => c.SelectedTab(App.Pages.NavigationPage))
                    .NavigateAsync();
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Initialize failed with exception");
            }

            this.IsBusy = false;
        }
    }
}
