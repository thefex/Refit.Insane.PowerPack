using Android.App;
using Android.Content.PM;
using MvvmCross.Droid.Views;

namespace SampleApp.Droid
{
    [Activity(
        Label = "Xamarines Sample"
        , MainLauncher = true
        , Icon = "@drawable/il2"
        , Theme = "@style/Theme.Splash"
        , NoHistory = true
        , ScreenOrientation = ScreenOrientation.Portrait)]
    public class SplashScreen : MvxSplashScreenActivity
    {
        public SplashScreen()
            : base(Resource.Layout.SplashScreen)
        {
        }
    }
}
