namespace PrismMauiApp.ViewModels;

public class TabbedMainPageViewModel : BindableBase
{
    private readonly INavigationService navigationService;
    private DelegateCommand addCommand;

    public TabbedMainPageViewModel(INavigationService navigationService)
    {
        this.navigationService = navigationService;
    }

    public DelegateCommand AddCommand => this.addCommand ??= new DelegateCommand(async () => await this.AddAsync());

    private async Task AddAsync()
    {
        await this.navigationService.NavigateAsync(App.Pages.ViewA);
    }
}
