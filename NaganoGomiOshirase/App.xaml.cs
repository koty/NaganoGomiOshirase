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
			(MainPage.BindingContext as IApplicationLifecycle)?.OnSleep();
		}

		protected override void OnResume()
		{
			(MainPage.BindingContext as IApplicationLifecycle)?.OnResume();
		}
	}
}
