using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;

#pragma warning disable 1591 //TODO enable warnings

[module: SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Scope="member", Target="KsWare.Presentation.Logging.Log.#.cctor()")]
namespace KsWare.Presentation.Core.Logging 
{
	public static class Log 
	{
		
// ReSharper disable InconsistentNaming
		private static readonly IFormatProvider enUS=CultureInfo.CreateSpecificCulture("en-US");
// ReSharper restore InconsistentNaming

		public static IFormatProvider FormatProvider => enUS;

		public static bool DisableTimestamp{get;set;}

		public static ICollection<LogFilter> Include => includeFilters;

		public static ICollection<LogFilter> Exclude => excludeFilters;

		static List<LogFilter> includeFilters=new List<LogFilter>();
		static List<LogFilter> excludeFilters=new List<LogFilter>();

		static Log() {
			if(new StackTrace(false).ToString().Contains("at NUnit.Core.TestSuite.Run"))
			{
				includeFilters.Add(LogFilter.None);
			}
		}

		#region old logging

//		public static void Add(string message) {
		//			Debug.WriteLine("=>"+string.Format(FormatProvider,  "{0:yyyy-MM-dd HH:mm:ss.fff} {1}",DateTime.Now,message));
//		}
//
//		public static void AddError(string message, Exception ex, string uniqueId, params LP[] args) {
//			AddInternal(1, "ERROR",message, ex, uniqueId, args);
//		}
//
//		public static void AddInternal(int skipFrames, string level, string message, Exception ex, string uniqueId, params LP[] args) {
//			var list = new List<LP>(args);
//
//			var stackFrame = new StackFrame(skipFrames+1,true);
//			var fileColumnNumber = stackFrame.GetFileColumnNumber();
//			var fileLineNumber = stackFrame.GetFileLineNumber();
//			var fileName = stackFrame.GetFileName();
//			//			var ilOffsetstackFrame.GetILOffset();
//			list.Add(new LP("StackFrame",FirstLine(stackFrame.ToString())));
//			list.Add(new LP("Module",stackFrame.GetMethod().Module.FullyQualifiedName));
//			list.Add(new LP("ReflectedType",stackFrame.GetMethod().ReflectedType));
//			list.Add(new LP(stackFrame.GetMethod().MemberType.ToString(),stackFrame.GetMethod().ToString()));
//
//			if(!string.IsNullOrWhiteSpace(uniqueId)) list.Add(new LP("UniqueID: ",uniqueId));
//			var sb = new StringBuilder();
//			if(!DisableTimestamp) {
//				sb.AppendFormat(formatProvider, "{0:yyyy-MM-dd HH:mm:ss.fff} ", DateTime.Now);
//			}
//			if(ex!=null)sb.AppendFormat(formatProvider, "{0}: {1} {2} {3}",level, message, ex.GetType().Name, FirstLine(ex.Message));
//			else sb.AppendFormat(formatProvider, "{0}: {1}", level, message);
//			foreach (var lp in list) {
//				sb.AppendFormat(formatProvider, "\r\n\t{0}: {1}", lp.Name, lp.Value);
//			}
//			if(ex!=null) sb.Append("\r\n"+Indend(ex.ToString().Trim(),1));
//			sb.AppendFormat("\r\n\t{0}({1},{2}): at ???",fileName,fileLineNumber,fileColumnNumber);//Log.cs(58,0): at KsWare.Presentation.Logging.Log.Format(String format, Object[] args)
//			Trace.Write(sb);
//		}

		#endregion

		[SuppressMessage("Microsoft.Naming", "CA1719:ParameterNamesShouldNotMatchMemberNames", MessageId="0#")]
		public static string Format(string format, params object[] args) { return string.Format(enUS, format, args); }
		
		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		private static string Indend(string s, int count) {
			var strings = s.Split(new[] {"\r\n", "\r", "\n"}, StringSplitOptions.None);
			for (int i = 0; i < strings.Length; i++) {
				strings[i]=new string('\t',count)+strings[i];
			}
			return string.Join("\r\n", strings);
		}

//RESERVED
//		private static string FirstLine(string s) {
//			if(string.IsNullOrWhiteSpace(s)) return s;
//			var strings = s.Split(new[] {"\r\n", "\r", "\n"}, 2,StringSplitOptions.None);
//			return strings[0];
//		}



		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity"), SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "dummy")]
		[SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]//REVIEW too complex! rewrite/split this method
		[Conditional("DEBUG")]
		public static void Write(object instanceOrType, Flow.FlowExpression flow) {
			var expressionResult = ((Flow.IExpressionResult)flow).Result;
			expressionResult.Type = instanceOrType as Type;
			expressionResult.Instance = (expressionResult.Type == null ? instanceOrType : null);
			if(expressionResult.Instance!=null) expressionResult.Type = expressionResult.Instance.GetType();

			if(expressionResult.MethodDescriptor==null) {var dummy=new Flow.MethodTypeExpression(expressionResult).Current;}

//REVIEW	//if(expressionResult.Position==Flow.FlowMemberPosition.ExitMethod) return;
		
			var subjectName = expressionResult.Instance==null
						? expressionResult.Type.Name
						: expressionResult.Type.Name+" #"+expressionResult.Instance.TypeInstanceId();
			string memberName = expressionResult.MethodDescriptor;
			string sMemberType = Flow.Visualize(expressionResult.MethodType);
			string sPosition = Flow.Visualize(expressionResult.Position);

			var parameterInfo = new StringBuilder();
			foreach (var parameter in expressionResult.Parameters) {
				parameterInfo.Append(parameterInfo.Length == 0 ? " " : ", ");
				parameterInfo.AppendFormat(FormatProvider, "{0}: {1}", parameter.Name, parameter.Value);
			}


			if(includeFilters.Count==1 && includeFilters[0].IsNone) return;
			if(includeFilters.Count>0) {
				#region Assembly
				bool assemblyMatch = false;
				if(includeFilters.Any(f=>f.Assembly!=null && f.Module==null && f.Type==null && f.Instance==null)) {
					foreach (var filter in includeFilters) {
						if(
							filter.Assembly == expressionResult.Assembly && 
							filter.Module   == null                      &&
							filter.Type     == null                      &&
							filter.Instance == null
						){assemblyMatch=true; break;}
					}
					if (assemblyMatch==false) return;					
				}
				#endregion
				#region Module
				if(includeFilters.Any(f=>f.Assembly==null && f.Module!=null && f.Type==null && f.Instance==null)) {
					bool moduleMatch = false;
					foreach (var filter in includeFilters) {
						if(
							filter.Module   == expressionResult.Module   && 
							filter.Type     == null                      &&
							filter.Instance == null
						){moduleMatch=true; break;}
					}
					if (moduleMatch==false) return;
				}
				#endregion
				#region Type
				if(includeFilters.Any(f=>f.Assembly==null && f.Module==null && f.Type!=null && f.Instance==null)) {
					bool typeMatch = false;
					foreach (var filter in includeFilters) {
						if(
							filter.Type     == expressionResult.Type     &&
							filter.Instance == null
						){typeMatch=true; break;}
					}
					if (typeMatch==false) return;
				}
				#endregion
				#region Instance
				if(includeFilters.Any(f=>f.Assembly==null && f.Module==null && f.Type==null && f.Instance!=null)) {
					bool instanceMatch = false;
					foreach (var filter in includeFilters) {
						if(
							filter.Instance == null && expressionResult.Instance!=null
						){instanceMatch=true; break;}
					}
					if (instanceMatch==false) return;
				}
				#endregion
			}

			if(excludeFilters.Count>0) {
				#region Assembly
				if(excludeFilters.Any(f=> f.Assembly==expressionResult.Assembly && f.Module==null && f.Type==null && f.Instance==null)) 
					return;
				#endregion
				#region Type
				if(excludeFilters.Any(f=> f.Assembly==null && f.Module==null && f.Type==expressionResult.Type && f.Instance==null)) 
					return;
				#endregion
				#region Instance
				if(excludeFilters.Any(f=> f.Assembly==null && f.Module==null && f.Type==null && f.Instance==expressionResult.Instance)) 
					return;
				#endregion
			}

			WriteFlowInternal(subjectName, sPosition+" "+sMemberType+" "+memberName, parameterInfo.ToString(), expressionResult.Message);
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2241:Provide correct arguments to formatting methods")]
		private static void WriteFlowInternal(string subjectName,string memberName,string parameterInfo,string additionalInfo) {
			long threadId = Thread.CurrentThread.ManagedThreadId;
			Debug.WriteLine("=>" + string.Format(FormatProvider, "{0} {3,4}{4} {5,-24}: {6,-8}: {7}{8}{9}",
				DateTime.Now.ToString("HH:mm:ss.fff",FormatProvider),
				AppDomain.CurrentDomain.Id, " ",
				threadId, " ",
				subjectName,
				"FLOW",
				memberName,
				(string.IsNullOrWhiteSpace(parameterInfo)?"":" "+parameterInfo),
				(string.IsNullOrWhiteSpace(additionalInfo)?"":" "+additionalInfo)));
		}

		public static string GetTypeName(object value) { 
			if(value==null) return "{NULL}";
			return "{" + value.GetType().Name + "}";
		}

		/// <summary> Formats an object
		/// </summary>
		/// <param name="obj">The object to format</param>
		/// <param name="format">one or more of "GgTtIi"</param>
		/// <returns>A string representation of the <paramref name="obj"/></returns>
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId="obj")]
		public static string FormatParameter(object obj, string format) {
			// GgTtIi
			string s="";
			if(format==null || format.Contains("t") || format.Contains("T") || format.Contains("g")|| format.Contains("G")) {
				if(obj==null) s= "NULL";
				else          s = obj.GetType().ToString(format);
			} else {
				//uups
				s = "???";
			}

			//envelope with '{' '}'
			if(!s.StartsWith("{",StringComparison.Ordinal) && format!=null && format.Contains("{") && format.Contains("}")) {
				s = string.Format(enUS, "{{{0}}}", s);
			}

			if(format!=null && (format.Contains("i") || format.Contains("I"))) {
				if(obj==null){}
				else s += " #" + obj.TypeInstanceId();
			}

			return s;
		}
	}
}
