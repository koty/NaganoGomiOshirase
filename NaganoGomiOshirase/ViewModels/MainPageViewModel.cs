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


		public MainPageViewModel()
		{

		}
		GomiCalendar _gomi_calendar;

		public void OnNavigatedFrom(NavigationParameters parameters)
		{

		}
		public void OnNavigatedTo(NavigationParameters parameters)
		{
			if (parameters.ContainsKey("title"))
				Title = (string)parameters["title"] + " and Prism";


			var assembly = typeof(MainPageViewModel).GetTypeInfo().Assembly;
			using (var stream = assembly.GetManifestResourceStream("NaganoGomiOshirase.gomi_calendar.json"))
			using (var reader = new StreamReader(stream))
			{
				var json = reader.ReadToEnd();
				// dynamic gomi_calendars = JObject.Parse(json);
				var gomiCalendarDic = JsonConvert.DeserializeObject<Dictionary<string, GomiCalendarRec[]>>(json);
				_gomi_calendar = new GomiCalendar(gomiCalendarDic);
			}
		}
	}
	public class GomiCalendarRec
	{
		public DateTime date { get; set; }
		public string kind { get; set; }
	}
	public class GomiCalendar
	{
		Dictionary<string, GomiCalendarRec[]> _gomiCalendarDic;
		public GomiCalendar(Dictionary<string, GomiCalendarRec[]> gomiCalendarDic)
		{
			_gomiCalendarDic = gomiCalendarDic;
		}
	}
}
