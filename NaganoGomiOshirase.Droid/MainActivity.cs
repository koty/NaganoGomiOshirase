using System;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Prism.Unity;
using Microsoft.Practices.Unity;

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
		public void RegisterTypes(IUnityContainer container)
		{

		}
	}
}
