using System.Collections.Concurrent;
using Android.Content;
using Android.Net;
using Android.Net.Wifi;
using Java.Net;
using Javax.Net.Ssl;

namespace PrismMauiApp.Platforms.Services
{
    public class WifiConnector : IWifiConnector
    {
        private readonly ILogger<WifiConnector> logger;
        private readonly WifiManager wifiManager;
        private readonly ConnectivityManager connectivityManager;

        private readonly ConcurrentDictionary<string, NetworkCallback> networkCallbacks = new ConcurrentDictionary<string, NetworkCallback>();

        public WifiConnector(ILogger<WifiConnector> logger)
        {
            this.logger = logger;
            this.wifiManager = (WifiManager)Android.App.Application.Context.GetSystemService(Context.WifiService);
            this.connectivityManager = (ConnectivityManager)Android.App.Application.Context.GetSystemService(Context.ConnectivityService);
        }

        public async Task<List<WiFiInfo>> GetAvailableWifis(bool? getSignalStrenth = false)
        {
            List<WiFiInfo> wifiInfos = null;

            var permissionStatus = await Permissions.CheckStatusAsync<Permissions.LocationAlways>();

            if (permissionStatus != PermissionStatus.Granted)
            {
                permissionStatus = await Permissions.RequestAsync<Permissions.LocationAlways>();
            }

            if (permissionStatus == PermissionStatus.Granted)
            {
                // Get a handle to the Wifi
                var wifiReceiver = new WifiReceiver(this.wifiManager);

                await Task.Run(() =>
                {
                    var exported = true;

                    // Start a scan and register the Broadcast receiver to get the list of Wifi Networks
                    //if (Android.OS.Build.VERSION.SdkInt < Android.OS.BuildVersionCodes.UpsideDownCake)
                    //{
                    //    var flags = exported ? ReceiverFlags.Exported : ReceiverFlags.NotExported;

                    //    Android.App.Application.Context.RegisterReceiver(wifiReceiver, new IntentFilter(WifiManager.ScanResultsAvailableAction), flags);
                    //}
                    //else
                    {
                        Android.App.Application.Context.RegisterReceiver(wifiReceiver, new IntentFilter(WifiManager.ScanResultsAvailableAction));
                    }
                    
                    wifiInfos = wifiReceiver.Scan();
                });
            }

            return wifiInfos;
        }

        public async Task<bool> ConnectToWifiAsync(string ssid, string password, CancellationToken token = default)
        {
            this.logger.LogDebug($"ConnectToWifiAsync: ssid={ssid}");

            try
            {
                if (Android.OS.Build.VERSION.SdkInt < Android.OS.BuildVersionCodes.Q)
                {
                    var oldSsid = "empty";
                    var oldBsid = "empty";
                    try
                    {
                        oldSsid = this.wifiManager.ConnectionInfo.SSID.Replace("\"", "");
                        oldBsid = this.wifiManager.ConnectionInfo.BSSID.Replace("\"", "");
                    }
                    catch { }

                    var formattedSsid = $"\"{ssid}\"";
                    var formattedPassword = $"\"{password}\"";
                    var wifiConfig = new WifiConfiguration
                    {
                        Ssid = formattedSsid,
                        PreSharedKey = formattedPassword
                    };
                    this.wifiManager.AddNetwork(wifiConfig);

                    var dis = this.wifiManager.Disconnect();
                    var network = this.wifiManager.ConfiguredNetworks.FirstOrDefault(n => n.Ssid.Contains(formattedSsid));
                    var ena = this.wifiManager.EnableNetwork(network.NetworkId, true);
                    var req = this.wifiManager.Reconnect();

                    var message = "api<29, connecting to " + formattedSsid + " from " + oldSsid + ", dis = " + dis + " ena = " + ena + " req = " + req;
                    Android.Util.Log.Debug("TEST", message);
                    return true;
                }
                else
                {
                    var specifier = new WifiNetworkSpecifier.Builder()
                         .SetSsid(ssid)
                         .SetIsHiddenSsid(true)
                         .SetWpa2Passphrase(password)
                         .Build();

                    var request = new NetworkRequest.Builder()
                        .AddTransportType(TransportType.Wifi)
                        .RemoveCapability(NetCapability.Internet)
                        .SetNetworkSpecifier(specifier)
                        .Build();

                    var tcs = new TaskCompletionSource<bool>();
                    var networkCallback = new NetworkCallback(this.connectivityManager)
                    {
                        NetworkAvailable = network =>
                        {
                            tcs.TrySetResult(true);
                        },
                        NetworkUnavailable = () =>
                        {
                            tcs.TrySetResult(false);
                        },

                    };

                    var success = false;
                    var attempts = 3;
                    var timeoutMs = 15000;

                    while (attempts-- > 0)
                    {
                        this.connectivityManager.RequestNetwork(request, networkCallback, timeoutMs);

                        success = await tcs.Task;
                        if (success)
                        {
                            this.networkCallbacks.AddOrUpdate(ssid, addValue: networkCallback, updateValueFactory: (k, o) => networkCallback);
                            break;
                        }

                        timeoutMs += 1000;
                    }

                    if (!success)
                    {
                        this.DisconnectWifi(ssid);
                    }

                    this.logger.LogDebug($"ConnectToWifiAsync with ssid={ssid} -> success={success}");
                    return success;
                }
            }
            catch (Exception ex)
            {
                this.networkCallbacks.Remove(ssid, out _);

                this.logger.LogError(ex, $"ConnectToWifiAsync with ssid={ssid} failed with exception");
                return false;
            }
        }

        public bool DisconnectWifi(string ssid)
        {
            this.logger.LogDebug($"DisconnectWifi: ssid={ssid}");

            try
            {
                if (Android.OS.Build.VERSION.SdkInt < Android.OS.BuildVersionCodes.Q)
                {
                    throw new NotImplementedException();

                    //var oldSsid = "empty";
                    //var oldBsid = "empty";
                    //try
                    //{
                    //    oldSsid = this.wifiManager.ConnectionInfo.SSID.Replace("\"", "");
                    //    oldBsid = this.wifiManager.ConnectionInfo.BSSID.Replace("\"", "");
                    //}
                    //catch { }

                    //var formattedSsid = $"\"{ssid}\"";
                    //var wifiConfig = new WifiConfiguration
                    //{
                    //    Ssid = formattedSsid,
                    //};

                    //var dis = this.wifiManager.Disconnect();
                    //var network = this.wifiManager.ConfiguredNetworks.FirstOrDefault(n => n.Ssid.Contains(formattedSsid));
                    //var ena = this.wifiManager.EnableNetwork(network.NetworkId, true);
                    //var req = this.wifiManager.Reconnect();

                    //var message = "api<29, connecting to " + formattedSsid + " from " + oldSsid + ", dis = " + dis + " ena = " + ena + " req = " + req;
                    //Android.Util.Log.Debug("TEST", message);
                    //return true;
                }
                else
                {
                    if (this.networkCallbacks.Remove(ssid, out var networkCallback))
                    {
                        this.connectivityManager.UnregisterNetworkCallback(networkCallback);
                    }
                    else
                    {
                        this.logger.LogInformation(
                            $"{nameof(DisconnectWifi)} failed to disconnect wifi '{ssid}' " +
                            $"because it was not connected using method {nameof(ConnectToWifiAsync)}");
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"DisconnectWifi with ssid={ssid} failed with exception");
                return false;
            }
        }

        public string TestConnection()
        {
            try
            {
                if (Android.OS.Build.VERSION.SdkInt < Android.OS.BuildVersionCodes.Q)
                {
                    var ssid = this.wifiManager.ConnectionInfo.SSID.Replace("\"", "");
                    Android.Util.Log.Debug("TEST", "connected to " + ssid);
                }
                else
                {

                }
                return "";
            }
            catch (Exception ex)
            {
                var _Status = "Test WiFi error, ex=" + ex.Message;
                Android.Util.Log.Error("TEST", _Status);
                return _Status;
            }
        }

        private class NetworkCallback2 : ConnectivityManager.NetworkCallback
        {
            public Action<Network> NetworkAvailable { get; set; }

            public Action NetworkUnavailable { get; set; }

            public Action<Network> NetworkLost { get; set; }

            public override void OnAvailable(Network network)
            {
                base.OnAvailable(network);
                this.NetworkAvailable?.Invoke(network);

            }

            public override void OnUnavailable()
            {
                base.OnUnavailable();
                this.NetworkUnavailable?.Invoke();
            }
        }

        private class NetworkCallback : ConnectivityManager.NetworkCallback
        {
            private readonly ConnectivityManager connectivityManager;

            public NetworkCallback(ConnectivityManager connectivityManager)
            {
                this.connectivityManager = connectivityManager;
            }

            public Action<Network> NetworkAvailable { get; set; }

            public Action NetworkUnavailable { get; set; }

            public override void OnAvailable(Network network)
            {
                base.OnAvailable(network);
                this.connectivityManager.BindProcessToNetwork(network);
                this.NetworkAvailable?.Invoke(network);

                //var socket = network.SocketFactory.CreateSocket();
                //socket.Connect(new InetSocketAddress("192.168.10.1", 5001), 30000);
            }

            public override void OnUnavailable()
            {
                base.OnUnavailable();
                this.NetworkUnavailable?.Invoke();
            }

            public override void OnLost(Network network)
            {
                base.OnLost(network);
                this.connectivityManager.BindProcessToNetwork(null);
                this.connectivityManager.UnregisterNetworkCallback(this);
            }
        }
    }
}
