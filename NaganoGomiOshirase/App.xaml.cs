using Prism.Unity;
using NaganoGomiOshirase.Views;

namespace NaganoGomiOshirase
{
	public partial class App : PrismApplication
	{
		public App(IPlatformInitializer initializer = null) : base(initializer) { }

		protected override void OnInitialized()
		{
			InitializeComponent();

			NavigationService.NavigateAsync("MainPage?title=長野市unofficialごみカレンダー");
		}
		protected override void RegisterTypes()
		{
			Container.RegisterTypeForNavigation<MainPage>();
		}
		protected override void OnSleep()
		{
			if (MainPage == null)
			{
				return;
			}
			
			var appLifecycle = MainPage.BindingContext as IApplicationLifecycle;
			if (appLifecycle == null)
			{
				return;
			}
			appLifecycle.OnSleep();
		}

		protected override void OnResume()
		{
			if (MainPage == null)
			{
				return;
			}
			var appLifecycle = MainPage.BindingContext as IApplicationLifecycle;
			if (appLifecycle == null)
			{
				return;
			}
			appLifecycle.OnResume();
		}
	}
}
