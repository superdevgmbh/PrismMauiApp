namespace PrismMauiApp.ViewModels.Devices
{
    public class DeviceItemViewModel : BindableBase
    {
        private readonly Action checkedChanged;
        private bool isChecked;
        private DelegateCommand toggleCheckedCommand;

        public DeviceItemViewModel(Action checkedChanged)
        {
            this.checkedChanged = checkedChanged;
        }

        public string SSID { get; set; }

        public bool IsChecked
        {
            get => this.isChecked;
            set
            {
                if (this.SetProperty(ref this.isChecked, value))
                {
                    this.checkedChanged();
                }
            }
        }

        public ICommand ToggleCheckedCommand => this.toggleCheckedCommand ??= new DelegateCommand(this.ToggleChecked);

        private void ToggleChecked()
        {
            this.IsChecked = true;
        }
    }
}