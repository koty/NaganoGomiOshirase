using System;
namespace NaganoGomiOshirase
{
	public interface IApplicationLifecycle
	{
		void OnSleep();
		void OnResume();
	}
}
