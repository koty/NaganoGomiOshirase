
using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using Unity;
using Prism;
using Prism.Ioc;

namespace NaganoGomiOshirase.Droid
{
    [Activity(Label = "長野市unofficialごみカレンダー", Icon = "@drawable/trashcan", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsApplicationActivity
	{
		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);

			global::Xamarin.Forms.Forms.Init(this, bundle);

			AndroidPreference.context = BaseContext;
			LoadApplication(new App(new AndroidInitializer()));

			var alarmManager = GetSystemService(AlarmService).JavaCast<AlarmManager>();
			Alerm.Register(alarmManager, this);
		}
	}

	public class AndroidInitializer : IPlatformInitializer
	{
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            
        }
    }
}
