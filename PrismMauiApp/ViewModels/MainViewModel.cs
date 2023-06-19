namespace PrismMauiApp.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IWifiConnector wifiConnector;
        private readonly IIdentityService identityService;
        private readonly INetworkService networkService;
        private readonly IDisplayRepository displayRepository;
        private readonly ILogger logger;
        private readonly INavigationService navigationService;
        private readonly IPageDialogService dialogService;
        private readonly IConnectivity connectivity;

        private string text = "Click me";
        private string ssid = "PiWeatherDisplay_3B05CA";
        private string psk = "T8cvDH11";
        private readonly string username = "pi";
        private readonly string password = "raspberry";
        private DisplayConfiguration displayConfiguration;

        public MainViewModel(
            ILogger<MainViewModel> logger,
            INavigationService navigationService,
            IPageDialogService dialogService,
            IConnectivity connectivity,
            IWifiConnector wifiConnector,
            IIdentityService identityService,
            INetworkService networkService,
            IDisplayRepository displayRepository)
        {
            this.logger = logger;
            this.navigationService = navigationService;
            this.dialogService = dialogService;
            this.connectivity = connectivity;
            this.wifiConnector = wifiConnector;
            this.identityService = identityService;
            this.networkService = networkService;
            this.displayRepository = displayRepository;

            this.ConnectToDisplayCommand = new AsyncRelayCommand(this.TryConnectToDisplayAsync);
            this.DisconnectWifiCommand = new AsyncRelayCommand(this.DisconnectWifi);
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

        public override async Task InitializeAsync(INavigationParameters parameters)
        {
            this.IsBusy = true;

            try
            {
                this.logger.LogInformation("InitAsync");

                //await Task.Delay(2000);

                var displayConfigurations = await this.displayRepository.GetDisplayConfigurationsAsync();
                if (displayConfigurations.Any())
                {
                    var displayConfiguration = displayConfigurations.First();
                    //await this.ConnectToDisplayAsync(displayConfiguration.SSID, displayConfiguration.PSK, this.username, this.password);
                }
                //else
                //{
                //    this.DisplayConfiguration = null;
                //}

                this.logger.LogInformation("InitAsync finished successfully");
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "InitAsync failed with exception");
                await this.dialogService.DisplayAlertAsync("Init", "Failed to initialize", "OK");
            }

            this.IsBusy = false;
        }

        public bool HasAnyDisplayConfigurations => this.DisplayConfiguration != null;

        public DisplayConfiguration DisplayConfiguration
        {
            get => this.displayConfiguration;
            private set
            {
                if (this.SetProperty(ref this.displayConfiguration, value))
                {
                    this.RaisePropertyChanged(nameof(this.HasAnyDisplayConfigurations));
                }
            }
        }

        private async Task TryConnectToDisplayAsync()
        {
            await this.TryConnectToDisplayAsync(this.SSID, this.PSK, this.username, this.password);
        }

        private async Task TryConnectToDisplayAsync(string ssid, string psk, string username, string password)
        {
            this.IsBusy = true;
            this.logger.LogDebug("TryConnectToDisplayAsync");

            try
            {
                await this.ConnectToDisplayAsync(ssid, psk, username, password);
                await this.dialogService.DisplayAlertAsync("Wifi", "Connected successfully!", "OK");
            }
            catch (Exception e)
            {
                this.logger.LogError(e, "TryConnectToDisplayAsync failed with exception");
                await this.dialogService.DisplayAlertAsync("Wifi", "Connection failed!", "OK");
            }

            this.IsBusy = false;
        }

        public IAsyncRelayCommand ConnectToDisplayCommand { get; }

        private async Task ConnectToDisplayAsync(string ssid, string psk, string username, string password)
        {
            var success = await this.wifiConnector.ConnectToWifi(ssid, psk);
            if (success)
            {
                await this.identityService.LoginAsync(username, password);

                var displayConfiguration = new DisplayConfiguration
                {
                    DisplayName = ssid,
                    SSID = ssid,
                    PSK = psk,
                };
                await this.displayRepository.AddOrUpdateDisplayConfigurationAsync(displayConfiguration);

                var ssids = await this.networkService.ScanAsync();
                await this.dialogService.DisplayAlertAsync("Scan Result", string.Join(Environment.NewLine, ssids), "OK cool");

                this.DisplayConfiguration = displayConfiguration;
            }
            else
            {
                this.DisplayConfiguration = null;
                throw new Exception("Failed to connect");
            }
        }

        public IAsyncRelayCommand DisconnectWifiCommand { get; }

        private async Task DisconnectWifi()
        {
            this.logger.LogDebug("DisconnectWifi");

            try
            {
                var success = this.wifiConnector.DisconnectWifi(this.SSID);

                await this.displayRepository.RemoveDisplayConfigurationAsync(this.DisplayConfiguration);

                this.DisplayConfiguration = null;
            }
            catch (Exception e)
            {
                this.logger.LogError(e, "DisconnectWifi failed with exception");
                await this.dialogService.DisplayAlertAsync("Wifi", "Failed with exception", "OK");
            }
        }
    }
}