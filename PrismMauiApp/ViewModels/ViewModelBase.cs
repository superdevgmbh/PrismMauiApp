﻿using System.Runtime.CompilerServices;
using PrismMauiApp.Utils;

namespace PrismMauiApp.ViewModels
{
    public class ViewModelBase : BindableBase, INavigatedAware
    {
        private bool isInitialized = false;
        private int busyRefCount = 0;

        public bool IsBusy
        {
            get
            {
                var count = Interlocked.CompareExchange(ref this.busyRefCount, 0, 0);
                return count > 0 || !this.IsInitialized;
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

        public bool IsInitialized
        {
            get => this.isInitialized;
            private set
            {
                if (this.isInitialized != value)
                {
                    this.isInitialized = value;
                    this.RaisePropertyChanged(nameof(this.IsBusy));
                    this.OnBusyChanged();
                }
            }
        }


        async void INavigatedAware.OnNavigatedTo(INavigationParameters parameters)
        {
            var navigationMode = parameters.GetNavigationMode();
            if (navigationMode == NavigationMode.New)
            {
                this.Initialize(parameters);
                await this.InitializeAsync(parameters);
                this.IsInitialized = true;
            }

            this.OnNavigatedTo(navigationMode, parameters);
            await this.OnNavigatedToAsync(navigationMode, parameters);
        }


        public virtual void OnNavigatedTo(NavigationMode navigationMode, INavigationParameters parameters)
        {
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

        public virtual Task InitializeAsync(INavigationParameters parameters)
        {
            return Task.CompletedTask;
        }

        public virtual void Initialize(INavigationParameters parameters)
        {
        }

        protected virtual bool SetProperty(RefCountBool refCountBool, bool value, [CallerMemberName] string propertyName = null)
        {
            var hasChanged = refCountBool.SetValue(value);
            if (hasChanged)
            {
                this.RaisePropertyChanged(propertyName);
            }

            return hasChanged;
        }
    }
}