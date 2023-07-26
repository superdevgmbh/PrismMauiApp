using System.Runtime.CompilerServices;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]

[assembly: InternalsVisibleTo("PrismMauiApp.Tests")]

[assembly: Preserve]

[assembly: XmlnsDefinition("http://prismmauiapp", "PrismMauiApp")]
[assembly: XmlnsDefinition("http://prismmauiapp", "PrismMauiApp.Controls")]
[assembly: XmlnsDefinition("http://prismmauiapp", "PrismMauiApp.Views")]
[assembly: XmlnsDefinition("http://prismmauiapp", "PrismMauiApp.Views.Devices")]
[assembly: XmlnsDefinition("http://prismmauiapp", "PrismMauiApp.ViewModels")]
[assembly: XmlnsDefinition("http://prismmauiapp", "PrismMauiApp.ViewModels.Devices")]
