namespace PrismMauiApp.Extensions
{
    internal static class NavigationParametersExtensions
    {
        public static NavigationMode? TryGetNavigationMode(this INavigationParameters parameters)
        {
            var navigationParametersInternal = (INavigationParametersInternal)parameters;
            if (navigationParametersInternal.ContainsKey("__NavigationMode"))
            {
                return navigationParametersInternal.GetValue<NavigationMode>("__NavigationMode");
            }

            return null;
        }

        internal static void SetParameter<T>(this INavigationParameters navigationParameters, T parameter) where T : class
        {
            navigationParameters.Add("model", parameter);
        }

        internal static T GetParameter<T>(this INavigationParameters navigationParameters) where T : class
        {
            return navigationParameters.GetParameter() as T;
        }

        internal static object GetParameter(this INavigationParameters navigationParameters)
        {
            return navigationParameters["model"];
        }

        internal static void SetCanGoBack(this INavigationParameters navigationParameters, bool canGoBack)
        {
            navigationParameters.Add("canGoBack", canGoBack);
        }

        internal static bool? GetCanGoBack(this INavigationParameters navigationParameters)
        {
            return navigationParameters["canGoBack"] as bool?;
        }
    }
}