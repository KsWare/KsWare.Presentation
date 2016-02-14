namespace KsWare.Presentation.Core.Providers
{

	public interface ILocalizationProvider
	{
		string Prefix { get; set; }
		string Suffix { get; set; }

		string GetString(string key);

	}

}