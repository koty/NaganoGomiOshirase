using System.Linq;
using Android.App;
using Android.Content;
using Android.Support.V4.App;
using NaganoGomiOshirase.ViewModels;
using System;
using Android.Runtime;
using Android.Util;
using Java.Util;

namespace NaganoGomiOshirase.Droid
{
	public class Alerm
	{
		public static void Register(AlarmManager alarmManager, Context context)
		{
			var intent = new Intent(context, typeof(AlarmReceiver));
			var pending = PendingIntent.GetBroadcast(context, 0, intent, PendingIntentFlags.UpdateCurrent);
			var tomorrow = DateTime.Now.AddDays(1);  // 翌日
			var exec_at = new DateTime(tomorrow.Year, tomorrow.Month, tomorrow.Day, 6, 0, 0);
			// var exec_at = DateTime.Now.AddMinutes(1);

			var calendar = Calendar.Instance;
			calendar.TimeInMillis = Java.Lang.JavaSystem.CurrentTimeMillis();
			calendar.Set(CalendarField.DayOfMonth, exec_at.Day);
			calendar.Set(CalendarField.HourOfDay, exec_at.Hour);
			calendar.Set(CalendarField.Minute, exec_at.Minute);
			alarmManager.Set(AlarmType.Rtc, calendar.TimeInMillis, pending);
		}
	}

	[BroadcastReceiver]
	[IntentFilter(new[] {
	  Android.Content.Intent.ActionBootCompleted
	})]
	public class AlarmReceiver : BroadcastReceiver
	{
		public override void OnReceive(Context context, Intent intent)
		{
			// notification
			AndroidPreference.context = context;
			var pref = new AndroidPreference();
			var recs = MainPageViewModel.GetToday(pref);
			if (recs.Length > 0)
			{
				var kinds = string.Join(", ", recs.Select(x => x.kind));
				SendNotification("ごみ収集スケジュール", kinds + "です。", context);
			}
			Log.Debug("tag", "★■□triggered!" + DateTime.Now);
			// register next alarm
			var alarmManager = context.GetSystemService(Android.Content.Context.AlarmService).JavaCast<AlarmManager>();
			Alerm.Register(alarmManager, context);
		}
		static void SendNotification(string title, string message, Context context)
		{
			// When the user clicks the notification, SecondActivity will start up.
			var resultIntent = new Intent(context, typeof(MainActivity));

			// Construct a back stack for cross-task navigation:
			var stackBuilder = Android.App.TaskStackBuilder.Create(context);
			stackBuilder.AddParentStack(Java.Lang.Class.FromType(typeof(MainActivity)));
			stackBuilder.AddNextIntent(resultIntent);

			// Create the PendingIntent with the back stack:            
			var resultPendingIntent =
				stackBuilder.GetPendingIntent(0, PendingIntentFlags.UpdateCurrent);

			// Build the notification:
			var builder = new NotificationCompat.Builder(context)
												.SetAutoCancel(true)                    // Dismiss from the notif. area when clicked
												.SetContentIntent(resultPendingIntent)  // Start 2nd activity when the intent is clicked.
												.SetContentTitle(title)      // Set its title
																			 // .SetNumber(count)                       // Display the count in the Content Info
			                                    .SetSmallIcon(Resource.Drawable.trashcan_no_bg)  // Display this icon
												.SetContentText(message); // The message to display.

			// Finally, publish the notification:
			var notificationManager = context.GetSystemService(Context.NotificationService) as NotificationManager;
			notificationManager.Notify(1001, builder.Build());
		}
	}
}