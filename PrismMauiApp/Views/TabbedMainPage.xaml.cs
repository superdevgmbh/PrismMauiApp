using Prism.Common;
using PrismMauiApp.ViewModels;

namespace PrismMauiApp.Views
{
    public partial class TabbedMainPage : TabbedPage, INavigatedAware, IDestructible
    {
        public TabbedMainPage()
        {
            this.InitializeComponent();
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            var vm = this.BindingContext as TabbedMainViewModel;
        }

        public MainViewModel MainViewModel { get; }

        public void OnNavigatedTo(INavigationParameters parameters)
        {
            var currentPage = ((NavigationPage)this.CurrentPage).CurrentPage;
            this.InvokeViewAndViewModelActionOnChildren<INavigatedAware>(p => p.OnNavigatedTo(parameters), p => p.GetType() != currentPage.GetType());
        }

        public void OnNavigatedFrom(INavigationParameters parameters)
        {
            var currentPage = ((NavigationPage)this.CurrentPage).CurrentPage;
            this.InvokeViewAndViewModelActionOnChildren<INavigatedAware>(p => p.OnNavigatedFrom(parameters), p => p.GetType() != currentPage.GetType());
        }

        public void Destroy()
        {
            this.InvokeViewAndViewModelActionOnChildren<IDestructible>(p => p.Destroy());
        }

        private void InvokeViewAndViewModelActionOnChildren<T>(Action<T> action, Func<Page, bool> pageFilter = null) where T : class
        {
            foreach (var child in this.Children)
            {
                var page = ((NavigationPage)child).CurrentPage;
                if (pageFilter == null || pageFilter(page))
                {
                    try
                    {
                        MvvmHelpers.InvokeViewAndViewModelAction(page, action);
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
        }

        protected override bool OnBackButtonPressed()
        {
            //if (this.IsRootPage(App.Current.MainPage))
            //{
            //    MvvmHelpers.InvokeViewAndViewModelAction<IDestructible>(this, d => d.Destroy());
            //}

            return base.OnBackButtonPressed();
        }
    }
}