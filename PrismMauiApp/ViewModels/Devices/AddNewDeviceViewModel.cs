﻿using System.Collections.Specialized;
using PrismMauiApp.Extensions;
using static PrismMauiApp.App;
using System.Linq;

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

        public override void Initialize(INavigationParameters parameters)
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
            timeoutCts.CancelAfter(TimeSpan.FromSeconds(20));

            try
            {
                using (var cts = CancellationTokenSource.CreateLinkedTokenSource(timeoutCts.Token, cancellationToken))
                {
                    this.Devices.Clear();

                    //var ssids = await this.wifiConnector.GetAvailableWifis(); //TODO: Pass cts.Token
                    //var deviceSSIDs2 = ssids
                    //    .Where(s => s.SSID.StartsWith(Constants.DeviceNamePrefix))
                    //    .ToArray();

                    var deviceSSIDs = new (string SSID, string PSK)[]
                    {
                        ("PiWeatherDisplay_3B05CA", "9meEAHGp")
                    };

                    await Task.Delay(2000, cts.Token);

                    foreach (var ssid in deviceSSIDs)
                    {
                        var deviceItemViewModel = new DeviceItemViewModel(() => this.RaisePropertyChanged(nameof(this.CanContinue)))
                        {
                            SSID = ssid.SSID,
                            PSK = ssid.PSK,
                        };
                        this.Devices.Add(deviceItemViewModel);
                    }
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

        public bool CanContinue
        {
            get
            {
                var canContinue = !this.IsScanning && !this.IsBusy && this.Devices.Any(d => d.IsChecked);
                return canContinue;
            }
        }

        public IAsyncRelayCommand ContinueCommand => this.continueCommand ??= new AsyncRelayCommand(
            execute: this.ContinueAsync);

        private async Task ContinueAsync()
        {
            try
            {
                var selectedDevice = this.Devices.Single(d => d.IsChecked);

                await this.wifiConnector.ConnectToWifiAsync(selectedDevice.SSID, selectedDevice.PSK);

                var navigationParameter = new ConnectToDeviceViewModel.NavigationParameter
                {
                    DeviceSSID = selectedDevice.SSID,
                };
                await this.navigationService.NavigateAsync(Pages.ConnectToDevicePage, navigationParameter);

            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "ContinueAsync failed with exception");
            }
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