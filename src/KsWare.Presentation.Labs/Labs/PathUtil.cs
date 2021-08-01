using System;
using System.IO;
using System.Reflection;

// ReSharper disable once CheckNamespace
namespace KsWare.Presentation 
{
	/// <summary> Provides path utilities
	/// </summary>
	public static class PathUtil 
	{

		/// <summary> Gets the application path.
		/// </summary>
		/// <value>The application path.</value>
		public static string ApplicationPath {
			get {
				//FIXED: ReSharper/NUnit returns 'D:\Temp\KayS\Temp\bgffmecr.w3n\KsWare.Presentation.BusinessModel.Test\assembly\dl3\1c38d14f\aa83a394_b313cb01'
				//FIXED: IDE returns 'C:\Program Files\Microsoft Visual Studio 10.0\Common7\IDE'
				//REVIEW: not really perfect but at present it works

				

				var callingAssembly = Assembly.GetCallingAssembly();

				if(Assembly.GetEntryAssembly()==null) {
					//if entry assembly is null return the path of calling assembly
					var path = Path.GetDirectoryName(new Uri(callingAssembly.Location, UriKind.Absolute).LocalPath);
					return path;
				}
#if !NETCOREAPP3_0_OR_GREATER 
				if(callingAssembly.GlobalAssemblyCache) {
					//throw new NotImplementedException("{4357DA2E-8836-4350-8AB8-53FF8A75D662}");
					throw new InvalidOperationException("LibraryPath couldn't be resolved!");
				}				
#endif

				{
					var path = Path.GetDirectoryName(callingAssembly.Location);
					return path;
				}
			}
		}
		//////////////////////////////////////////////////////////////
		//All other special folders are founded in BusinessApplication
		//////////////////////////////////////////////////////////////
	}
}
