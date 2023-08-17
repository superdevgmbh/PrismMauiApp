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

                //var result = await navigationService.CreateBuilder()
                //.AddTabbedSegment(c => 
                //    c.CreateTab(App.Pages.MainPage)
                //     .CreateTab(App.Pages.AboutPage)
                //     .SelectedTab(App.Pages.AboutPage))
                //.NavigateAsync();

                await this.navigationService.NavigateAsync($"/{App.Pages.TabbedMainPage}?selectedTab={App.Pages.MainPage}");
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Initialize failed with exception");
            }

            this.IsBusy = false;
        }
    }
}
