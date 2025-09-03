using Superdev.Maui.Mvvm;

namespace PrismMauiApp.ViewModels
{
    public class ViewModelBase : BaseViewModel, INavigatedAware
    {
        async void INavigatedAware.OnNavigatedTo(INavigationParameters parameters)
        {
            var navigationMode = parameters.GetNavigationMode();
            if (navigationMode == NavigationMode.New && !this.IsInitialized)
            {
                this.Initialize(parameters);
                await this.InitializeAsync(parameters);
                this.IsInitialized = true;
            }

            this.OnNavigatedTo(navigationMode, parameters);
            await this.OnNavigatedToAsync(navigationMode, parameters);
        }

        async void INavigatedAware.OnNavigatedFrom(INavigationParameters parameters)
        {
            var navigationMode = parameters.GetNavigationMode();
            await this.OnNavigatedFromAsync(navigationMode, parameters);
        }


        public virtual void OnNavigatedTo(NavigationMode navigationMode, INavigationParameters parameters)
        {
        }

        public virtual Task OnNavigatedToAsync(NavigationMode navigationMode, INavigationParameters parameters)
        {
            return Task.CompletedTask;
        }

        public virtual Task OnNavigatedFromAsync(NavigationMode navigationMode, INavigationParameters parameters)
        {
            return Task.CompletedTask;
        }

        public virtual Task InitializeAsync(INavigationParameters parameters)
        {
            return Task.CompletedTask;
        }

        public virtual void Initialize(INavigationParameters parameters)
        {
        }
    }
}