//using System.Reflection;
//using Prism.Ioc;
//using PrismMauiApp.Platforms.Services;
//using PrismMauiApp.Services;
//using PrismMauiApp.Services.Login;
//using PrismMauiApp.ViewModels;
//using PrismMauiApp.Views;
//using PrismMauiApp.Extensions;
//using System.Globalization;
//using PrismMauiApp.ViewModels.Devices;
//using PrismMauiApp.Services.Http;
//using PrismMauiApp.Platforms;

//namespace PrismMauiApp;

//internal static class PrismStartup
//{
//    public static void Configure(PrismAppBuilder builder)
//    {
//        ViewModelLocationProvider.SetDefaultViewTypeToViewModelTypeResolver((viewType) =>
//        {
//            var viewName = viewType.FullName.ReplaceLastOccurrence(".Views.", ".ViewModels.");
//            var viewAssemblyName = viewType.GetTypeInfo().Assembly.FullName;

//            var viewModelName = viewName.ReplaceLastOccurrence("Page", viewName.EndsWith("View") ? "Model" : "ViewModel");
//            var viewModelFullName = string.Format(CultureInfo.InvariantCulture, "{0}, {1}", viewModelName, viewAssemblyName);
//            var type = Type.GetType(viewModelFullName);
//            return type;
//        });

//        builder.RegisterTypes(PlatformInitializer.RegisterTypes);
//        //builder.RegisterTypes(RegisterTypes);
//        builder.OnInitialized((IContainerProvider c) =>
//        {

//        });
//        builder.OnAppStart(async (c, navigationService) =>
//        {
//            //var result = await navigationService.CreateBuilder()
//            //.AddTabbedSegment(c => 
//            //    c.CreateTab(App.Pages.MainPage)
//            //     .CreateTab(App.Pages.AboutPage)
//            //     .SelectedTab(App.Pages.AboutPage))
//            //.NavigateAsync();
//            var result = await navigationService.NavigateAsync($"/{App.Pages.TabbedMainPage}?selectedTab={App.Pages.MainPage}");
//            //var result = await navigationService.NavigateAsync($"/{App.Pages.TabbedMainPage}");
//            if (!result.Success)
//            {
//                System.Diagnostics.Debugger.Break();
//            }
//        });
//    }

//}
