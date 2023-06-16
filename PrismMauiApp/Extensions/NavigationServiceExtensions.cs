using PrismMauiApp.Extensions;

namespace PrismMauiApp.Extensions
{
    internal static class NavigationServiceExtensions
    {
        internal static Task<INavigationResult> NavigateAsync<T>(this INavigationService navigationService, string name, T parameter) where T : class
        {
            var navigationParameters = new NavigationParameters();
            navigationParameters.SetParameter(parameter);
            return navigationService.NavigateAsync(name: name, parameters: navigationParameters);
        }

        internal static Task<INavigationResult> GoBackAsync<T>(this INavigationService navigationService, T parameter) where T : class
        {
            var navigationParameters = new NavigationParameters();
            navigationParameters.SetParameter(parameter);
            return navigationService.GoBackAsync(parameters: navigationParameters);
        }

        public static INavigationParameters CreateNavigationParametersInternal(NavigationMode navigationMode, INavigationParameters parameters)
        {
            var internalParameters = (INavigationParametersInternal)new NavigationParameters();
            internalParameters.Add("__NavigationMode", navigationMode);

            if (parameters != null)
            {
                foreach (var item in parameters)
                {
                    internalParameters.Add(item.Key, item.Value);
                }
            }

            return (INavigationParameters)internalParameters;
        }
    }
}