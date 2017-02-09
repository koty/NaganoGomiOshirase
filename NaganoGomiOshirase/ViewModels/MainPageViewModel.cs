using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace NaganoGomiOshirase.ViewModels
{
	public class MainPageViewModel : BindableBase, INavigationAware
	{
		string _title;
		public string Title
		{
			get { return _title; }
			set { SetProperty(ref _title, value); }
		}

		static readonly DateTime UNIX_EPOCH = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

		static DateTime FromUnixTime(long unixTime)
		{
			return UNIX_EPOCH.AddSeconds(unixTime).ToLocalTime();
		}
		static MainPageViewModel()
		{
			var assembly = typeof(MainPageViewModel).GetTypeInfo().Assembly;
			using (var stream = assembly.GetManifestResourceStream("NaganoGomiOshirase.holiday_list.json"))
			using (var reader = new StreamReader(stream))
			{
				var json = reader.ReadToEnd();
				var holidays = JsonConvert.DeserializeObject<Dictionary<int, string>>(json).Select(x => FromUnixTime(x.Key));
				_holidays = new HashSet<DateTime>(holidays);
			}
		}
		public MainPageViewModel()
		{
			var assembly = typeof(MainPageViewModel).GetTypeInfo().Assembly;
			using (var stream = assembly.GetManifestResourceStream("NaganoGomiOshirase.gomi_calendar.json"))
			using (var reader = new StreamReader(stream))
			{
				var json = reader.ReadToEnd();
				var gomiCalendarDic = JsonConvert.DeserializeObject<Dictionary<string, GomiCalendarRec[]>>(json);
				_gomi_calendar = new GomiCalendar(gomiCalendarDic);
			}
		}
		GomiCalendar _gomi_calendar;
		internal static HashSet<DateTime> _holidays;
		internal static HashSet<DateTime> holidays { get { return _holidays; } }
		int _selected_calendar_no = 5; // とりあえず 5固定

		public GomiCalendarRec[] RecentCalendarRec
		{
			get
			{
				var now = DateTime.Now;
				var recs = _gomi_calendar.GetCalendar(_selected_calendar_no).OrderBy(x => x.date).Where(x => x.date >= now.Date).ToArray();
				return recs;
			}
		}

		public void OnNavigatedFrom(NavigationParameters parameters)
		{

		}
		public void OnNavigatedTo(NavigationParameters parameters)
		{
			if (parameters.ContainsKey("title"))
				Title = (string)parameters["title"];
		}
	}
	public class GomiCalendarRec
	{
		public DateTime date { get; set; }
		public string date_formated { get { return date.ToString("M/d(ddd)"); }}
		public string kind { get; set; }
		public Color color
		{
			get
			{
				if (MainPageViewModel.holidays.Any(d => d == date))
				{
					return Color.Pink;
				} else {
					return Color.Default;
				}
			}
		}
	}
	public class GomiCalendar
	{
		Dictionary<string, GomiCalendarRec[]> _gomiCalendarDic;
		public GomiCalendar(Dictionary<string, GomiCalendarRec[]> gomiCalendarDic)
		{
			_gomiCalendarDic = gomiCalendarDic;
		}

		public GomiCalendarRec[] GetCalendar(int calendar_no)
		{
			return _gomiCalendarDic[calendar_no.ToString()];
		}
	}
}
