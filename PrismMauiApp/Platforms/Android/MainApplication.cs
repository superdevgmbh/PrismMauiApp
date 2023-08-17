using Android.App;
using Android.Runtime;

namespace PrismMauiApp;

[Application]
public class MainApplication : MauiApplication
{
    public MainApplication(IntPtr handle, JniHandleOwnership ownership)
        : base(handle, ownership)
    {
    }

    protected override MauiApp CreateMauiApp()
    {
        return MauiProgram.CreateMauiApp();
    }

    public override void OnCreate()
    {
        base.OnCreate();
    }

    public override void OnLowMemory()
    {
        base.OnLowMemory();
    }
}
