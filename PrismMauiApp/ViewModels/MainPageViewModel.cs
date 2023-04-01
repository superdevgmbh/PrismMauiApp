using Microsoft.Extensions.Logging;
using PrismMauiApp.Services;

namespace PrismMauiApp.ViewModels;
public class MainPageViewModel : BindableBase
{
    private readonly IWifiConnector wifiConnector;
    private readonly ILogger logger;
    private readonly INavigationService navigationService;
    private readonly IConnectivity connectivity;
    private string text = "Click me";

    public MainPageViewModel(
        ILogger<MainPageViewModel> logger,
        INavigationService navigationService,
        IConnectivity connectivity,
        IWifiConnector wifiConnector)
    {
        //this.screenReader = screenReader;
        //this.bluetoothService = bluetoothService;
        this.CountCommand = new DelegateCommand(this.OnCountCommandExecuted);
        this.logger = logger;
        this.navigationService = navigationService;
        this.connectivity = connectivity;
        this.wifiConnector = wifiConnector;
    }

    public string Title => "Main Page";

    public string Text
    {
        get => this.text;
        set => this.SetProperty(ref this.text, value);
    }

    public DelegateCommand CountCommand { get; }

    private async void OnCountCommandExecuted()
    {
        this.logger.LogDebug("OnCountCommandExecuted");

        try
        {
            this.wifiConnector.ConnectToWifi("testssid", "testpsk");

            this.Text = string.Join(",", this.connectivity.ConnectionProfiles);
            //Text = string.Join(",", this.connectivity.NetworkAccess);

            await this.navigationService.NavigateAsync("HomePage");
        }
        catch (Exception e)
        {
            this.logger.LogError(e, "OnCountCommandExecuted failed with exception");
        }
    }
}
