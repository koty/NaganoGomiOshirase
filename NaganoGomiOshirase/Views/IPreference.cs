using System;
namespace NaganoGomiOshirase
{
	public interface IPreference
	{
		int GetInt(string key, int defaultValue);
		void SetInt(string key, int value);
	}
}
