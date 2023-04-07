using Microsoft.Extensions.Logging;
using PrismMauiApp.Services;
using PrismMauiApp.Services.Login;

namespace PrismMauiApp.ViewModels;
public class MainPageViewModel : BindableBase
{
    private readonly IWifiConnector wifiConnector;
    private readonly IIdentityService identityService;
    private readonly ILogger logger;
    private readonly INavigationService navigationService;
    private readonly IPageDialogService dialogService;
    private readonly IConnectivity connectivity;
    private string text = "Click me";
    private string ssid = "PiWeatherDisplay_3B05CA";
    private string psk = "T8cvDH11";

    public MainPageViewModel(
        ILogger<MainPageViewModel> logger,
        INavigationService navigationService,
        IPageDialogService dialogService,
        IConnectivity connectivity,
        IWifiConnector wifiConnector,
        IIdentityService identityService)
    {
        this.logger = logger;
        this.navigationService = navigationService;
        this.dialogService = dialogService;
        this.connectivity = connectivity;
        this.wifiConnector = wifiConnector;
        this.identityService = identityService;

        this.ConnectToWifiCommand = new DelegateCommand(async () => await this.ConnectToWifi());
        this.DisconnectWifiCommand = new DelegateCommand(async () => await this.DisconnectWifi());
    }

    public string Title => "Main Page";

    public string Text
    {
        get => this.text;
        private set => this.SetProperty(ref this.text, value);
    }

    public string SSID
    {
        get => this.ssid;
        set => this.SetProperty(ref this.ssid, value);
    }

    public string PSK
    {
        get => this.psk;
        set => this.SetProperty(ref this.psk, value);
    }

    public DelegateCommand ConnectToWifiCommand { get; }

    public DelegateCommand DisconnectWifiCommand { get; }

    private async Task ConnectToWifi()
    {
        this.logger.LogDebug("ConnectToWifi");

        try
        {
            var success = await this.wifiConnector.ConnectToWifi(this.SSID, this.PSK);
            if (success)
            {
                var token = await this.identityService.LoginAsync("pi", "raspberry");

                await this.dialogService.DisplayAlertAsync("Wifi", "Successfully connected", "OK");
            }
            else
            {
                await this.dialogService.DisplayAlertAsync("Wifi", "Not connected", "OK");
            }

            //this.Text = string.Join(",", this.connectivity.ConnectionProfiles);
            //Text = string.Join(",", this.connectivity.NetworkAccess);

            //await this.navigationService.NavigateAsync("HomePage");
        }
        catch (Exception e)
        {
            this.logger.LogError(e, "ConnectToWifi failed with exception");
            await this.dialogService.DisplayAlertAsync("Wifi", "Failed with exception", "OK");
        }
    }

    private async Task DisconnectWifi()
    {
        this.logger.LogDebug("DisconnectWifi");

        try
        {
            var success = this.wifiConnector.DisconnectWifi(this.SSID);
            if (success)
            {
                await this.dialogService.DisplayAlertAsync("Wifi", "Successfully disconnected", "OK");
            }
            else
            {
                await this.dialogService.DisplayAlertAsync("Wifi", "Failed to disconnect", "OK");
            }
        }
        catch (Exception e)
        {
            this.logger.LogError(e, "DisconnectWifi failed with exception");
            await this.dialogService.DisplayAlertAsync("Wifi", "Failed with exception", "OK");
        }
    }
}
