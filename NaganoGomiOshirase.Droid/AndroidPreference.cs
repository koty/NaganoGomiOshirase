using System;
using Android.Content;
using Android.Preferences;
using NaganoGomiOshirase.Droid;
using Xamarin.Forms;

[assembly: Dependency(typeof(AndroidPreference))]
namespace NaganoGomiOshirase.Droid
{
	public class AndroidPreference: IPreference
	{
		public static Context context;

		public int GetInt(string key, int defaultValue)
		{
			var pref = context.GetSharedPreferences("pref", FileCreationMode.Private);
			var val = pref.GetInt(key, defaultValue);
			return val;
		}

		public void SetInt(string key, int value)
		{
			var pref = context.GetSharedPreferences("pref", FileCreationMode.Private);
			var editor = pref.Edit();
			editor.PutInt(key, value);
			editor.Apply();
		}
	}
}
