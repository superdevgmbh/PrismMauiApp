using PrismMauiApp.Extensions;
using static PrismMauiApp.App;

namespace PrismMauiApp.ViewModels.Devices
{
    public class ConnectToDeviceViewModel : ViewModelBase, INavigatedAware
    {
        private readonly ILogger logger;
        private readonly INavigationService navigationService;
        private readonly INetworkService networkService;
        private readonly IWifiConnector wifiConnector;
        private readonly IPageDialogService dialogService;
        private readonly IIdentityService identityService;
        private bool isClosing;
        private IAsyncRelayCommand cancelCommand;
        private IAsyncRelayCommand continueCommand;
        private WifiItemViewModel[] ssids = Array.Empty<WifiItemViewModel>();

        public ConnectToDeviceViewModel(
            ILogger<ConnectToDeviceViewModel> logger,
            INavigationService navigationService,
            INetworkService networkService,
            IWifiConnector wifiConnector,
            IPageDialogService dialogService,
            IIdentityService identityService)
        {
            this.logger = logger;
            this.navigationService = navigationService;
            this.networkService = networkService;
            this.wifiConnector = wifiConnector;
            this.dialogService = dialogService;
            this.identityService = identityService;
        }

        public override async Task InitializeAsync(INavigationParameters parameters)
        {
            this.IsBusy = true;

            try
            {
                var navigationParameter = parameters.GetParameter<NavigationParameter>();

                var username = "pi";
                var password = "raspberry";
                await this.identityService.LoginAsync(username, password);

                var ssids = await this.networkService.ScanAsync();
                this.SSIDs = ssids
                    .Where(s => !s.StartsWith(Constants.DeviceNamePrefix, StringComparison.InvariantCultureIgnoreCase) &&
                                !string.Equals(s, navigationParameter.DeviceSSID, StringComparison.InvariantCultureIgnoreCase))
                    .Select(s => new WifiItemViewModel(s, () => this.RaisePropertyChanged(nameof(this.CanContinue))))
                    .ToArray();
            }
            catch (Exception e)
            {
                this.logger.LogError(e, "InitializeAsync failed with exception");
            }

            this.IsBusy = false;
        }

        public WifiItemViewModel[] SSIDs
        {
            get => this.ssids;
            private set => this.SetProperty(ref this.ssids, value);
        }

        protected override void OnBusyChanged()
        {
            this.RaisePropertyChanged(nameof(this.CanContinue));
        }

        public bool CanContinue => !this.IsBusy && this.SSIDs.Any(d => d.IsChecked);

        public IAsyncRelayCommand ContinueCommand => this.continueCommand ??= new AsyncRelayCommand(
            execute: this.ContinueAsync);

        private async Task ContinueAsync()
        {
            try
            {
                var selectedSSID = this.SSIDs.Single(d => d.IsChecked);

                // TODO: Enter wifi password here!
                await this.networkService.ConnectToWifiAsync(selectedSSID.SSID, "abcdefg12345678");

                // TODO: Store device in repository
                await this.navigationService.NavigateAsync(Pages.AboutPage);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "ContinueAsync failed with exception");
            }
        }

        public IAsyncRelayCommand CancelCommand => this.cancelCommand ??= new AsyncRelayCommand(
            execute: this.CancelAsync,
            canExecute: () => !this.IsClosing);

        public bool IsClosing
        {
            get => this.isClosing;
            private set => this.SetProperty(ref this.isClosing, value);
        }

        private async Task CancelAsync()
        {
            var userConsent = await this.dialogService.DisplayAlertAsync("Cancel?", "Do you really want to cancel?", "Yes", "No");
            if (userConsent)
            {
                await this.navigationService.GoBackAsync(new (string, object)[] { (KnownNavigationParameters.UseModalNavigation, true) });
            }
        }

        public class NavigationParameter
        {
            public string DeviceSSID { get; set; }
        }
    }
}