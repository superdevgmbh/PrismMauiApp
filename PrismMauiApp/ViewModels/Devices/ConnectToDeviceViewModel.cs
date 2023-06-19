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

        private bool isClosing;
        private bool isConnecting;
        private IAsyncRelayCommand cancelCommand;
        private IAsyncRelayCommand continueCommand;
        private IRelayCommand connectCommand;

        public ConnectToDeviceViewModel(
            ILogger<ConnectToDeviceViewModel> logger,
            INavigationService navigationService,
            INetworkService networkService,
            IWifiConnector wifiConnector,
            IPageDialogService dialogService)
        {
            this.logger = logger;
            this.navigationService = navigationService;
            this.networkService = networkService;
            this.wifiConnector = wifiConnector;
            this.dialogService = dialogService;
        }

        public override void Initialize(INavigationParameters parameters)
        {
            this.IsBusy = true;

            try
            {
                var navigationParameters = parameters.GetParameter<NavigationParameter>();
                this.ConnectToDevice();
            }
            catch (Exception e)
            {
                this.logger.LogError(e, "InitAsync failed with exception");
            }

            this.IsBusy = false;
        }

        public IRelayCommand ConnectCommand => this.connectCommand ??= new RelayCommand(
            execute: this.ConnectToDevice,
            canExecute: () => !this.IsConnecting);

        public bool IsConnecting
        {
            get => this.isConnecting;
            private set
            {
                if (this.SetProperty(ref this.isConnecting, value))
                {
                    this.RaisePropertyChanged(nameof(this.CanContinue));
                }
            }
        }

        private void ConnectToDevice()
        {
            this.IsConnecting = true;


            try
            {

            }
            catch (Exception)
            {

            }


            this.IsConnecting = false;
        }

        protected override void OnBusyChanged()
        {
            this.RaisePropertyChanged(nameof(this.CanContinue));
        }

        public bool CanContinue => !this.IsConnecting && !this.IsBusy;

        public IAsyncRelayCommand ContinueCommand => this.continueCommand ??= new AsyncRelayCommand(
            execute: this.ContinueAsync,
            canExecute: () => !this.CanContinue);

        private async Task ContinueAsync()
        {
            await this.navigationService.NavigateAsync(Pages.AboutPage, new (string, object)[] { ("bla", "bla") });
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
            public string SSID { get; set; }
        }
    }
}