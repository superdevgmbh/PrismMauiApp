using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;

namespace PrismMauiApp;

[Activity(
    Theme = "@style/Maui.SplashTheme",
    MainLauncher = true,
    ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        Platform.Init(this, savedInstanceState);
    }

    public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
    {
        Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

        base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
    }

    protected override void OnNewIntent(Intent intent)
    {
        base.OnNewIntent(intent);
        Platform.OnNewIntent(intent);
    }

    protected override void OnResume()
    {
        base.OnResume();
        Platform.OnResume(this);
    }

    public override void OnUserInteraction()
    {
        base.OnUserInteraction();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }
}
