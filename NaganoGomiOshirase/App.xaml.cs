using NaganoGomiOshirase.Views;
using Prism.Ioc;
using Prism;

namespace NaganoGomiOshirase
{
    public partial class App : Prism.Unity.PrismApplication
	{
		public App(IPlatformInitializer initializer = null) : base(initializer) { }

		protected override async void OnInitialized()
		{
			InitializeComponent();

			await NavigationService.NavigateAsync("MainPage?title=長野市unofficialごみカレンダー");
		}
        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<MainPage>();
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
