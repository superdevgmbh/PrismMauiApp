using static PrismMauiApp.App;

namespace PrismMauiApp.ViewModels;

public class TabbedMainViewModel : BindableBase
{
    private readonly INavigationService navigationService;

    private IAsyncRelayCommand addNewDeviceCommand;

    public TabbedMainViewModel(INavigationService navigationService)
    {
        this.navigationService = navigationService;
    }

    public IAsyncRelayCommand AddNewDeviceCommand => this.addNewDeviceCommand ??= new AsyncRelayCommand(
        execute: this.AddNewDeviceAsync);

    private async Task AddNewDeviceAsync()
    {
        //await this.navigationService.NavigateAsync(App.Pages.AboutPage);
        //var request = this.navigationService
        //     .CreateBuilder()

        //     //.UseRelativeNavigation()
        //     .AddNavigationPage().AddParameter(KnownNavigationParameters.UseModalNavigation, true)
        //     .AddSegment(Pages.AddNewDevicePage).AddParameter(KnownNavigationParameters.UseModalNavigation, true)
        //    ;


        //var result = await request.NavigateAsync();


        var result = await this.navigationService.CreateBuilder()
           .AddNavigationPage(useModalNavigation: true)
            .AddSegment(Pages.AddNewDevicePage, s => s.UseModalNavigation())
            .NavigateAsync();

    }
}