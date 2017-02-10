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
		readonly ISettings _settings;
		static MainPageViewModel()
		{
			var __holidays = ReadJson<Dictionary<int, string>>("NaganoGomiOshirase.holiday_list.json")
				.Select(x => FromUnixTime(x.Key));
			_holidays = new HashSet<DateTime>(__holidays);

		}
		public MainPageViewModel()
		{
			var gomiCalendarDic = ReadJson<Dictionary<string, GomiCalendarRec[]>>("NaganoGomiOshirase.gomi_calendar.json");
			_gomi_calendar = new GomiCalendar(gomiCalendarDic);

			_calendar_no_list = ReadJson<Dictionary<int, string>>("NaganoGomiOshirase.calendar_no_list.json")
				.Select(x => new KeyValuePair<int, string>(x.Key, x.Key + " " + x.Value)).ToList();
			_settings = DependencyService.Get<ISettings>();
			// var saved_selected_calendar_no = _settings.GetValue<int>("SelectedCalendarNo", 1);
			if (!Application.Current.Properties.ContainsKey("selected_calendar_no"))
			{
				Application.Current.Properties["selected_calendar_no"] = 1;
			}
			var saved_selected_calendar_no = int.Parse(Application.Current.Properties["selected_calendar_no"].ToString());
			_selected_calendar_no = calendar_no_list.First(x => x.Key == saved_selected_calendar_no);
		}
		GomiCalendar _gomi_calendar;
		List<KeyValuePair<int, string>> _calendar_no_list;
		public List<KeyValuePair<int, string>> calendar_no_list { get { return _calendar_no_list; } }

		KeyValuePair<int, string> _selected_calendar_no;
		public KeyValuePair<int, string> selected_calendar_no
		{
			get { return _selected_calendar_no; }
			set
			{
				SetProperty(ref _selected_calendar_no, value);
				OnPropertyChanged(nameof(RecentCalendarRec));
				Application.Current.Properties["selected_calendar_no"] = selected_calendar_no.Key;
				//_settings.SetValue<int>("SelectCalendarNo", selected_calendar_no.Key);
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
		public void OnNavigatedTo(NavigationParameters parameters)
		{
			if (parameters.ContainsKey("title"))
				Title = (string)parameters["title"];
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
			return _gomiCalendarDic[calendar_no.ToString()];
		}
	}
}
