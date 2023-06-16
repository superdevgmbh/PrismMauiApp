using System.Collections.Specialized;
using PrismMauiApp.Extensions;
using static PrismMauiApp.App;

namespace PrismMauiApp.ViewModels.Devices
{
    public class AddNewDeviceViewModel : ViewModelBase, IDestructible
    {
        private readonly ILogger logger;
        private readonly INavigationService navigationService;
        private readonly INetworkService networkService;
        private readonly IWifiConnector wifiConnector;
        private readonly IPageDialogService dialogService;

        private ICommand closeCommand;
        private bool isScanning;
        private IAsyncRelayCommand scanCommand;
        private ICommand cancelScanCommand;
        private IAsyncRelayCommand continueCommand;

        public AddNewDeviceViewModel(
            ILogger<AddNewDeviceViewModel> logger,
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

            this.Devices.CollectionChanged += this.OnDevicesCollectionChanged;
        }

        private void OnDevicesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.RaisePropertyChanged(nameof(this.CanContinue));
        }

        public override async Task OnNavigatedToAsync(NavigationMode navigationMode, INavigationParameters parameters)
        {
            if (navigationMode == NavigationMode.New)
            {
                await this.InitAsync();
            }
        }

        private async Task InitAsync()
        {
            this.IsBusy = true;

            try
            {
                _ = this.ScanCommand.ExecuteAsync(null);
            }
            catch (Exception e)
            {
                this.logger.LogError(e, "InitAsync failed with exception");
            }

            this.IsBusy = false;
        }

        public IAsyncRelayCommand ScanCommand => this.scanCommand ??= new AsyncRelayCommand(
            cancelableExecute: this.ScanNetworksAsync,
            canExecute: () => !this.IsScanning);

        public bool IsScanning
        {
            get => this.isScanning;
            private set
            {
                if (this.SetProperty(ref this.isScanning, value))
                {
                    this.RaisePropertyChanged(nameof(this.CanContinue));
                }
            }
        }

        private async Task ScanNetworksAsync(CancellationToken cancellationToken)
        {
            this.logger.LogDebug("ScanNetworksAsync");
            this.IsScanning = true;

            var timeoutCts = new CancellationTokenSource();

            try
            {
                using (var cts = CancellationTokenSource.CreateLinkedTokenSource(timeoutCts.Token, cancellationToken))
                {

                    this.Devices.Clear();
                    timeoutCts.CancelAfter(TimeSpan.FromSeconds(10));

                    var ssids = await this.networkService.ScanAsync(cts.Token);

                    await Task.Delay(1000);

                    this.Devices.Add(new DeviceItemViewModel(() => this.RaisePropertyChanged(nameof(this.CanContinue)))
                    {
                        SSID = "PiWeatherDisplay_3B05CA"
                    });
                    this.Devices.Add(new DeviceItemViewModel(() => this.RaisePropertyChanged(nameof(this.CanContinue)))
                    {
                        SSID = "PiWeatherDisplay_111111"
                    });
                    this.Devices.Add(new DeviceItemViewModel(() => this.RaisePropertyChanged(nameof(this.CanContinue)))
                    {
                        SSID = "PiWeatherDisplay_2222222"
                    });
                }

                this.logger.LogDebug("ScanNetworksAsync finished successfully");
            }
            catch (OperationCanceledException)
            {
                if (timeoutCts.IsCancellationRequested)
                {
                    this.logger.LogDebug("ScanNetworksAsync cancelled after timeout");
                    _ = this.dialogService.DisplayAlertAsync("Timeout", "Reached the timeout, try again", "OK");
                }
                else if (cancellationToken.IsCancellationRequested)
                {
                    this.logger.LogDebug("ScanNetworksAsync cancelled by user");
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "ScanNetworksAsync failed with exception");
            }
            finally
            {
                timeoutCts.Dispose();
            }


            this.IsScanning = false;
        }

        public ObservableCollection<DeviceItemViewModel> Devices { get; } = new ObservableCollection<DeviceItemViewModel>();

        protected override void OnBusyChanged()
        {
            this.RaisePropertyChanged(nameof(this.CanContinue));
        }

        public bool CanContinue => !this.IsScanning && !this.IsBusy && this.Devices.Any(d => d.IsChecked);

        public ICommand ContinueCommand => this.continueCommand ??= new AsyncRelayCommand(
            execute: this.ContinueAsync,
            canExecute: () => !this.CanContinue);

        private async Task ContinueAsync()
        {
            var navigationParameters = new ConnectToDeviceViewModel.NavigationParameter
            {
                SSID = "test"
            };
            await this.navigationService.NavigateAsync(Pages.ConnectToDevicePage, navigationParameters);

            //await this.navigationService.NavigateAsync(Pages.ConnectToDevicePage, new (string, object)[] { ("device", this.Devices.SingleOrDefault(d => d.IsChecked)) });
        }

        public ICommand CloseCommand => this.closeCommand ??= new AsyncRelayCommand(
            execute: this.CloseAsync);

        public ICommand CancelScanCommand
        {
            get => this.cancelScanCommand ??= this.ScanCommand.CreateCancelCommand();
        }

        private async Task CloseAsync()
        {
            var userConsent = !this.ScanCommand.IsRunning || await this.dialogService.DisplayAlertAsync("Cancel?", "Do you really want to cancel?", "Yes", "No");
            if (userConsent)
            {
                await this.navigationService.GoBackAsync(new (string, object)[] { (KnownNavigationParameters.UseModalNavigation, true) });
            }
        }

        public void Destroy()
        {
            this.CancelScanCommand.Execute(null);
            this.Devices.CollectionChanged -= this.OnDevicesCollectionChanged;
        }
    }
}