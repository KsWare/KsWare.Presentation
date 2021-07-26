//using System;
//using System.Diagnostics;
//using System.Reflection;
//using System.Runtime.CompilerServices;
//using System.Threading;
//using NUnit.Framework;
//
//namespace KsWare.Test.Presentation.KsWare.Presentation.ViewFramework {
//
//	[TestFixture]
//	public class AppDomainTests:MarshalByRefObject {
//
//		private void Log(string s) {
//			Debug.WriteLine(string.Format("{0:HH:mm:ss,fff} {1,-10} {2,-4} {3}",DateTime.Now, AppDomain.CurrentDomain.FriendlyName,  Thread.CurrentThread.ManagedThreadId,s));
//		}
//		private void Log(string format, params object[] args) {
//			Log(string.Format(format,args));
//		}
//
//		private bool RunInDomain([CallerMemberName] string method=null) {
//			if (AppDomain.CurrentDomain.FriendlyName != "DomainRunner") {
//				var domaininfo = new AppDomainSetup();
//				domaininfo.ApplicationBase = Environment.CurrentDirectory;
//				var appDomain = AppDomain.CreateDomain("DomainRunner",null,domaininfo);
//				var test=(AppDomainTests) appDomain.CreateInstanceAndUnwrap(Assembly.GetExecutingAssembly().FullName, GetType().FullName);
//				//test.TestMethod1();
//				var testMethod = test.GetType().GetMethod(method);
//				testMethod.Invoke(test, null);
//				return false;
//			}
//			return true;
//		}
//
//		[Test,Ignore("TODO")]
//		public void TestMethod1() {
//			if(!RunInDomain()) return;
//			
//			Log("Real test");
//			Assert.Fail("Simple Fail");
//		}
//
//
//	}
//}
