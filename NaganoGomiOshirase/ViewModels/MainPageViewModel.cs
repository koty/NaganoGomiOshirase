using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.IO;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace NaganoGomiOshirase.ViewModels
{
	public class MainPageViewModel : BindableBase, INavigationAware, IApplicationLifecycle
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
		static T ReadJson<T>(string path)
		{
			var assembly = typeof(MainPageViewModel).GetTypeInfo().Assembly;
			using (var stream = assembly.GetManifestResourceStream(path))
			using (var reader = new StreamReader(stream))
			{
				var json = reader.ReadToEnd();
				return JsonConvert.DeserializeObject<T>(json);
			}
		}

		internal static HashSet<DateTime> _holidays;
		internal static HashSet<DateTime> holidays { get { return _holidays; } }
		static MainPageViewModel()
		{
			var __holidays = ReadJson<Dictionary<int, string>>("NaganoGomiOshirase.holiday_list.json")
				.Select(x => FromUnixTime(x.Key));
			_holidays = new HashSet<DateTime>(__holidays);

			var gomiCalendarDic = ReadJson<Dictionary<string, GomiCalendarRec[]>>("NaganoGomiOshirase.gomi_calendar.json");
			_gomi_calendar = new GomiCalendar(gomiCalendarDic);

			_calendar_no_list = ReadJson<Dictionary<int, string>>("NaganoGomiOshirase.calendar_no_list.json")
				.Select(x => new KeyValuePair<int, string>(x.Key, x.Key + " " + x.Value)).ToList();

		}
		public MainPageViewModel()
		{
			var pref = DependencyService.Get<IPreference>();
			var saved_selected_calendar_no = pref.GetInt("selected_calendar_no", 1);
			if (saved_selected_calendar_no > 0)
			{
				_selected_calendar_no = calendar_no_list.First(x => x.Key == saved_selected_calendar_no);
			}
		}
		public static GomiCalendarRec[] GetToday(IPreference pref = null)
		{
			if (pref == null)
			{
				pref = DependencyService.Get<IPreference>();
			}
			var saved_selected_calendar_no = pref.GetInt("selected_calendar_no", 0);
			var _selected_calendar_no = _calendar_no_list.Where(x => x.Key == saved_selected_calendar_no).FirstOrDefault();
			if (_selected_calendar_no.Equals(default(KeyValuePair<int, string>)))
			{
				return new GomiCalendarRec[] { };
			}
			var today = DateTime.Today;
			var recs = _gomi_calendar.GetCalendar(_selected_calendar_no.Key).OrderBy(x => x.date).Where(x => x.date == today).ToArray();
			return recs;
			
		}
		static GomiCalendar _gomi_calendar;
		static List<KeyValuePair<int, string>> _calendar_no_list;
		public List<KeyValuePair<int, string>> calendar_no_list { get { return _calendar_no_list; } }

		KeyValuePair<int, string> _selected_calendar_no;
		public KeyValuePair<int, string> selected_calendar_no
		{
			get { return _selected_calendar_no; }
			set
			{
				SetProperty(ref _selected_calendar_no, value);
				RaisePropertyChanged(nameof(RecentCalendarRec));
				var pref = DependencyService.Get<IPreference>();
				pref.SetInt("selected_calendar_no", selected_calendar_no.Key);
			}
		}
		public GomiCalendarRec[] RecentCalendarRec
		{
			get
			{
				var now = DateTime.Now;
				var recs = _gomi_calendar.GetCalendar(selected_calendar_no.Key).OrderBy(x => x.date).Where(x => x.date >= now.Date).ToArray();
				return recs;
			}
		}

		public void OnNavigatedFrom(NavigationParameters parameters)
		{

		}
		public void OnSleep()
		{
			// do nothing
		}

		public void OnResume()
		{
			RaisePropertyChanged(nameof(RecentCalendarRec));
		}

        public void OnNavigatingTo(NavigationParameters parameters)
        {
            if (parameters.ContainsKey("title"))
                Title = (string)parameters["title"];
        }

        public void OnNavigatedTo(NavigationParameters parameters)
        {
            
        }
    }

	public class GomiCalendarRec
	{
		public DateTime date { get; set; }
		public string date_formated { get { return date.ToString("M/d(ddd)"); } }
		public string kind { get; set; }
		public Color color
		{
			get
			{
				if (MainPageViewModel.holidays.Any(d => d == date))
				{
					return Color.Pink;
				}
				else {
					return Color.Default;
				}
			}
		}
	}
	public class GomiCalendar
	{
		readonly Dictionary<string, GomiCalendarRec[]> _gomiCalendarDic;
		public GomiCalendar(Dictionary<string, GomiCalendarRec[]> gomiCalendarDic)
		{
			_gomiCalendarDic = gomiCalendarDic;
		}

		public GomiCalendarRec[] GetCalendar(int calendar_no)
		{
			if (!_gomiCalendarDic.ContainsKey(calendar_no.ToString()))
			{
				return new GomiCalendarRec[] { };
			}
			return _gomiCalendarDic[calendar_no.ToString()];
		}
	}
}
