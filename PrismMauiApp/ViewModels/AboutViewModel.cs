namespace PrismMauiApp.ViewModels
{
    public class AboutViewModel : ViewModelBase
    {
        private readonly IDeviceInfo deviceInfo;
        private readonly IAppInfo appInfo;

        private string packageName;
        private string deviceModel;

        public AboutViewModel(
            IDeviceInfo deviceInfo,
            IAppInfo appInfo)
        {
            this.deviceInfo = deviceInfo;
            this.appInfo = appInfo;
        }

        public override void Initialize(INavigationParameters parameters)
        {
            this.PackageName = this.appInfo.PackageName;
            this.DeviceModel = this.deviceInfo.Model;
        }

        public string PackageName
        {
            get => this.packageName;
            private set => this.SetProperty(ref this.packageName, value);
        }

        public string DeviceModel
        {
            get => this.deviceModel;
            private set => this.SetProperty(ref this.deviceModel, value);
        }
    }
}
