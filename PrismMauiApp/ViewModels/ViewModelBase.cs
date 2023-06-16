namespace PrismMauiApp.ViewModels
{
    public class ViewModelBase : BindableBase, INavigatedAware
    {
        private readonly bool isLoaded = false;
        private int busyRefCount = 0;

        //public bool IsBusy
        //{
        //    get => this.isBusy;
        //    set
        //    {
        //        if (this.SetProperty(ref this.isBusy, value))
        //        {
        //            this.OnBusyChanged();
        //        }
        //    }
        //}

        public bool IsBusy
        {
            get
            {
                var count = Interlocked.CompareExchange(ref this.busyRefCount, 0, 0);
                return count > 0 && !this.IsLoaded;
            }

            protected set
            {
                if (value)
                {
                    Interlocked.Increment(ref this.busyRefCount);
                }
                else
                {
                    Interlocked.Decrement(ref this.busyRefCount);
                }

                this.RaisePropertyChanged(nameof(this.IsBusy));
                this.OnBusyChanged();
            }
        }

        protected virtual void OnBusyChanged()
        {
        }

        private bool IsLoaded
        {
            get => this.isLoaded;
            set
            {
                if (this.isLoaded != value)
                {
                    this.RaisePropertyChanged(nameof(this.IsBusy));
                }
            }
        }


        async void INavigatedAware.OnNavigatedTo(INavigationParameters parameters)
        {
            var navigationMode = parameters.GetNavigationMode();
            await this.OnNavigatedToAsync(navigationMode, parameters);

            if (navigationMode == NavigationMode.New)
            {
                this.IsLoaded = true;
            }
        }

        public virtual Task OnNavigatedToAsync(NavigationMode navigationMode, INavigationParameters parameters)
        {
            return Task.CompletedTask;
        }

        async void INavigatedAware.OnNavigatedFrom(INavigationParameters parameters)
        {
            var navigationMode = parameters.GetNavigationMode();
            await this.OnNavigatedFromAsync(navigationMode, parameters);
        }

        public virtual Task OnNavigatedFromAsync(NavigationMode navigationMode, INavigationParameters parameters)
        {
            return Task.CompletedTask;
        }
    }
}