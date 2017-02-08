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
				var gomi_calendars = JsonConvert.DeserializeObject<Dictionary<string, GomiCalendar[]>>(json);
				foreach (var gomi_calendar in gomi_calendars)
				{
					System.Diagnostics.Debug.WriteLine(gomi_calendar);
				}
			}
		}
	}
	public class GomiCalendar
	{
		public DateTime date { get; private set; }
		public string kind { get; private set; }
	}
}

