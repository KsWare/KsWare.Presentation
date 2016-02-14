using System;
using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;

namespace KsWare.Presentation.Core.Diagnostics {

	/// <summary> Provides information about a stack frame
	/// </summary>
	public class StackFrameInfo {

		private int m_FileColumnNumber;
		private int m_FileLineNumber;
		private string m_FileName;
		//private string methodSig;

		/// <summary> Initializes a new instance of the <see cref="StackFrameInfo"/> class.
		/// </summary>
		public StackFrameInfo(): this(new StackFrame(true)){}

		/// <summary> Initializes a new instance of the <see cref="StackFrameInfo"/> class.
		/// </summary>
		/// <param name="stackFrame">The stack frame.</param>
		public StackFrameInfo(StackFrame stackFrame) {
			m_FileColumnNumber = stackFrame.GetFileColumnNumber();
			m_FileLineNumber   = stackFrame.GetFileLineNumber();
			m_FileName         = stackFrame.GetFileName();

			//var m=stackFrame.GetMethod().ToString();
			//var s=string.Format(CultureInfo.InvariantCulture, "at {0} in {1}:line {2}",m, fileName,fileLineNumber);
			//...
		}

		/// <summary> Initializes a new instance of the <see cref="StackFrameInfo"/> class that corresponds to a frame above the current stack frame.
		/// </summary>
		/// <param name="stackTrace">The stack trace presentation from an exception </param>
		/// <param name="skipFrames">The number of frames up the stack to skip.</param>
		public StackFrameInfo (string stackTrace,int skipFrames) {
			Parse(stackTrace, skipFrames);
		}

		/// <summary> Initializes a new instance of the <see cref="StackFrameInfo"/> class using a frame presentation from an exception 
		/// </summary>
		/// <param name="stackFrame">The stack frame presentation from an exception</param>
		public StackFrameInfo(string stackFrame) { Parse(stackFrame); }

		/// <summary> Parses the specified stack trace.
		/// </summary>
		/// <param name="stackTrace">The stack trace presentation from an exception</param>
		/// <param name="skipFrames">The number of frames up the stack to skip.</param>
		private void Parse(string stackTrace, int skipFrames) { 
			var lines = stackTrace.Split(new[] {"\r\n", "\r", "\n"}, StringSplitOptions.None);
			int frameNr = -1;
			for (int i = 0; i < lines.Length; i++) {
				if(lines[i].Trim().StartsWith("at ",StringComparison.Ordinal)) {
					frameNr++;
					if(frameNr==skipFrames) {
						Parse(lines[i]);
						return;
					}
				}
			}
		}

		/// <summary> Parses the specified stack frame.
		/// </summary>
		/// <param name="stackFrame">The stack frame presentation from an exception</param>
		private void Parse(string stackFrame) { 
			const string pattern = @"(at\s)(?<Method>.*?)(\sin\s)(?<File>.*?)(:line\s)(?<Line>\d*)";
			var match = Regex.Match(stackFrame,pattern,RegexOptions.Compiled);
			if(!match.Success) {
				throw new NotImplementedException("{421CCA27-AF2F-43BF-8A04-D13028D1F69B}");
			} else {
				//
				//var method     = match.Groups["Method"].Value;
				m_FileColumnNumber = 0;
				m_FileLineNumber   = string.IsNullOrWhiteSpace(match.Groups["Line"].Value)?0:int.Parse(match.Groups["Line"].Value,CultureInfo.InvariantCulture);
				m_FileName         = match.Groups["File"].Value;
			}
		}

		/// <summary> Gets the column number in the file that contains the code that is executing. 
		/// This information is typically extracted from the debugging symbols for the executable.
		/// </summary>
		public int FileColumnNumber {get {return m_FileColumnNumber;}}

		/// <summary> Gets the line number in the file that contains the code that is executing. 
		/// This information is typically extracted from the debugging symbols for the executable.
		/// </summary>
		public int FileLineNumber {get {return m_FileLineNumber;}}

		/// <summary> Gets the file name that contains the code that is executing. 
		/// This information is typically extracted from the debugging symbols for the executable.
		/// </summary>
		public string FileName {get {return m_FileName;}}

		///// <summary> [PRELIMINARY] Get the method signature.
		///// </summary>
		//public string MethodSig {get {return methodSig;}}


//		private static StackFrameInfo QuickTest() {
//			return new StackFrameInfo();
//		}
	}
}
