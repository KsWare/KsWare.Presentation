﻿using System;
using NUnit.Framework;
using KsWare.Presentation.Core.Logging;
using Assert=NUnit.Framework.Assert;

namespace KsWare.Presentation.Tests.Core
{
	// ReSharper disable InconsistentNaming

	/// <summary> Test the <see cref="Log"/>-class
	/// </summary>
	[TestFixture]
	public class LogTests {

		/// <summary> Setup this instance.
		/// </summary>
		[SetUp]
		public void Setup() { }

		/// <summary> Teardowns this instance.
		/// </summary>
		[TearDown]
		public void Teardown() { }

		/// <summary> 
		/// </summary>
		[Test]
		public void Common() {
			Log.Write(this, Flow.Enter.EventHandler.Name("my").Parameter("P1","test").Parameter("P2","test"));
			Log.Write(this, Flow.Enter.EventHandler.Current.Parameter("P1", "test"));
			this.Log(Flow.Enter.EventHandler.Current.Parameter("P1", "test"));
		}

		/// <summary> TestDescription
		/// </summary>
		[Test,Ignore("TODO")]
		public void UseUsingThrowsException() {
			Assert.Throws<InvalidOperationException>(delegate {
				//LOG: using(this.LogBlock(Flow.Enter)) 
				{
					this.Log(Flow.Output("say hello"));
					try {
						this.Log(Flow.Throws(new InvalidOperationException("Test")));
						throw Flow.LoggedException;
					} catch (InvalidOperationException ex) {
						this.Log(Flow.Catch(ex));
						this.Log(Flow.Output("something wrong here"));
						this.Log(Flow.Retrow());
						throw;
					}
				}
			});

		}

		/// <summary> Filter test
		/// </summary>
		[Test]
		public void FilterTest() {
			this.Log(Flow.Output("ERROR: You should not see this!"));
			Log.Include.Clear();
			Log.Include.Add(new LogFilter {Module = this.GetType().Module});
			this.Log(Flow.Output("INFO: You should see this!"));

			//restore the log filter
			Log.Include.Clear();Log.Include.Add(LogFilter.None);

		}

	}
	// ReSharper restore InconsistentNaming
}