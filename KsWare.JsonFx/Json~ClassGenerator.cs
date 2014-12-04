using System;
using System.Collections.Generic;

#if(DYNAMIC_SUPPORT)
using System.Web.Script.Serialization;
#endif

namespace KsWare.JsonFx {

	public partial class Json {

		public string ToClassDefinition(string className, int ind) {
			var  dic=ToDictionary();
			var sCH = string.Format(new string('\t',ind)+"public class {0} {{",className); Console.WriteLine(sCH);
			ind++;
			foreach (var kv in dic) {
				if(kv.Value==null) {
					var s = string.Format(new string('\t',ind)+"public {0} {1} {{get;set;}}","string",kv.Key);
					Console.WriteLine(s);
				} else if(kv.Value is object[]) {
					var s = string.Format(new string('\t',ind)+"public {0} {1} {{get;set;}}\t//{2}", "object[]", kv.Key, new Json(kv.Value,false,true).ToString());
					Console.WriteLine(s);
				} else if(kv.Value is Dictionary<string,object>) {
					//var s = string.Format(new string('\t',ind)+"public {0} {1} {{get;set;}}","Dictionary<string,object>",kv.Key);
					var subClassName="GeneratedClass_"+kv.Key+"";
					var s = string.Format(new string('\t',ind)+"public {0} {1} {{get;set;}}",subClassName,kv.Key);
					Console.WriteLine(s);
					var subJson = new Json(kv.Value, false, true);
					s=subJson.ToClassDefinition(subClassName,ind);
				} else if(kv.Value is string || kv.Value is long || kv.Value is int || kv.Value is bool || kv.Value is decimal || kv.Value is double || kv.Value is float) {
					var s = string.Format(new string('\t',ind)+"public {0} {1} {{get;set;}}\t//{2}","string",kv.Key,kv.Value);
					Console.WriteLine(s);
				} else {
					var s = string.Format(new string('\t',ind)+"public {0} {1} {{get;set;}}\t//{2}",kv.Value.GetType().Name,kv.Key,new Json(kv.Value,false,true).ToString());
					Console.WriteLine(s);
				}
			}
			ind--;
			var sCF = string.Format(new string('\t',ind)+"}}",""); Console.WriteLine(sCF);
			return "";
		}
	}
}
