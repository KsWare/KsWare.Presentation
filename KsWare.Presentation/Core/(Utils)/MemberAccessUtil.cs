using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using KsWare.Presentation.Core;

namespace KsWare.Presentation {

	/// <summary> Provides member access utilities
	/// </summary>
	public static class MemberAccessUtil {

		/// <summary> Demands a write once operation.
		/// </summary>
		/// <param name="canWrite">if set to <see langword="true"/> can write; else can not write.</param>
		/// <param name="message">The exception message.</param>
		/// <param name="owner">The owner (should be <c><see langword="this"/></c>).</param>
		/// <param name="propertyName">Name of the property.</param>
		/// <param name="uniqueId">The unique id.</param>
		/// <example>
		/// MemberAccessUtil.DemandWriteOnce(condition==true,"The property can only be written once!",this,"TheProperty","{619FE952-BC2C-4080-90F3-449939B9E072}")
		/// </example>
		[AssertionMethod]
		[SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
		[LocalizationRequired(false)]
		[NotifyPropertyChangedInvocator("propertyName")]
		public static void DemandWriteOnce([AssertionCondition(AssertionConditionType.IS_TRUE)] bool canWrite, string message, object owner, string propertyName, string uniqueId) { 
			//NOTE: do not change default parameter values!
			if(!canWrite) throw new InvalidOperationException(
				(message     ==null ? "Property already initialized!": DebugUtil.Indent2(message))+
				DebugUtil.P(owner       !=null,"Declaring Type", DebugUtil.FormatTypeName(owner))+
				DebugUtil.P(propertyName!=null,"Property Name", propertyName) +
				DebugUtil.P(uniqueId    !=null,"UniqueID", uniqueId)
			);
		}

		/// <summary> Demands a write operation.
		/// </summary>
		/// <param name="canWrite">if set to <see langword="true"/> can write; else can not write.</param>
		/// <param name="message">The exception message.</param>
		/// <param name="owner">The owner (should be <see langword="this"/>).</param>
		/// <param name="propertyName">Name of the property.</param>
		/// <param name="uniqueId">The unique id.</param>
		[AssertionMethod]
		[SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
		[LocalizationRequired(false)]
		[NotifyPropertyChangedInvocator("propertyName")]
		public static void DemandWrite([AssertionCondition(AssertionConditionType.IS_TRUE)] bool canWrite, string message, object owner, string propertyName, string uniqueId) { 
			if(!canWrite) throw new InvalidOperationException(
				(message     ==null ? "Property is wite protected!": DebugUtil.Indent2(message))+
				(owner       ==null ? "":"\r\n\t"+"Declaring Type: "+(owner is Type?((Type)owner).FullName:owner.GetType().FullName))+
				(propertyName==null ? "":"\r\n\t"+"Property Name: "+propertyName)+
				(uniqueId    ==null ? "":"\r\n\t"+"UniqueID: "+uniqueId)
			);
		}

		/// <summary> Demands a value is not null.
		/// </summary>
		/// <param name="value">The value to be inspect.</param>
		/// <param name="message">The exception message.</param>
		/// <param name="owner">The owner.</param>
		/// <param name="propertyName">Name of the property.</param>
		/// <param name="uniqueId">An unique id.</param>
		[AssertionMethod]
		[SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
		[LocalizationRequired(false)]
		public static void DemandNotNull([AssertionCondition(AssertionConditionType.IS_NOT_NULL)] object value, string message, object owner, string propertyName, string uniqueId) {
			if(value==null) throw new InvalidOperationException(
				(message     ==null ? "Value must not be null!": DebugUtil.Indent2(message))+
				(owner       ==null ? "":DebugUtil.P("Declaring Type",()=>(owner is Type?((Type)owner).FullName:owner.GetType().FullName)))+
				(propertyName==null ? "":DebugUtil.P("Property Name" ,propertyName))+
				(uniqueId    ==null ? "":DebugUtil.P("UniqueID"      ,uniqueId))
			);
		}

		public static void DemandNotNull([AssertionCondition(AssertionConditionType.IS_NOT_NULL)] object value, object owner, string uniqueId) {
			DemandNotNullInternal(value, null, owner, 1, uniqueId);
		}

		public static void DemandNotNull([AssertionCondition(AssertionConditionType.IS_NOT_NULL)] object value, string message, object owner, string uniqueId) {
			DemandNotNullInternal(value, message, owner, 1, uniqueId);
		}

		public static void DemandNotNull([AssertionCondition(AssertionConditionType.IS_NOT_NULL)] object value, string message, object owner, int skipStackFrames, string uniqueId) {
			DemandNotNullInternal(value, message, owner, skipStackFrames+1,uniqueId);
		}

		private static void DemandNotNullInternal([AssertionCondition(AssertionConditionType.IS_NOT_NULL)] object value, string message, object owner, int skipStackFrames, string uniqueId) {
			if(value!=null) return;

			string propertyName;
			var methodBase = new StackFrame(skipStackFrames + 1).GetMethod();
			if(methodBase.IsSpecialName && (methodBase.Name.StartsWith("Get_")||methodBase.Name.StartsWith("Get_")))
				propertyName = methodBase.Name.Substring(4);
			else
				propertyName= methodBase.Name;

			throw new InvalidOperationException(
				(message     ==null ? "Value cannot be null!": DebugUtil.Indent2(message))+
				(owner       ==null ? "":DebugUtil.P("Declaring Type: ",()=>(owner is Type?((Type)owner).FullName:owner.GetType().FullName)))+
				(propertyName==null ? "":DebugUtil.P("Property Name",propertyName))+
				(uniqueId    ==null ? "":DebugUtil.P("UniqueID",uniqueId))
			);
		}

		/// <summary> Demands the condition is <see langword="true"/>.
		/// </summary>
		/// <param name="condition">The condition</param>
		/// <param name="message">The function to create the message.</param>
		/// <param name="sender">The sender.</param>
		/// <param name="uniqueId">An unique id.</param>
		[AssertionMethod]
		[SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
		[LocalizationRequired(false)]
		public static void Demand([AssertionCondition(AssertionConditionType.IS_TRUE)] bool condition, Func<string> message, object sender, string uniqueId) {
			if (condition) return;
			string msg = null;
			if (message == null) {
				msg = "A requirement was examined and declined." +
				      DebugUtil.P("Method", new StackFrame(1).GetMethod().Name);
			} else {
				msg = message();
			}
			msg += 
				DebugUtil.P(sender   !=null, "Demanding type",DebugUtil.FormatTypeName(sender))+
				DebugUtil.P(uniqueId !=null, "UniqueID"      ,uniqueId);
			/*DEBUG*/ if(Debugger.IsAttached) {Debug.WriteLine(message);Debugger.Break();}
			throw new InvalidOperationException(msg);		
		}

		/// <summary> Demands the condition is <see langword="true"/>.
		/// </summary>
		/// <param name="condition">The condition</param>
		/// <param name="message">The message.</param>
		/// <param name="sender">The sender.</param>
		/// <param name="uniqueId">An unique id.</param>
		[AssertionMethod]
		[SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
		[LocalizationRequired(false)]
		public static void Demand([AssertionCondition(AssertionConditionType.IS_TRUE)] bool condition, [LocalizationRequired(false)] string message, object sender, string uniqueId) { 
			if (condition) return;
			if (message == null) {
				message = "A requirement was examined and declined."+
					DebugUtil.P("Method", new StackFrame(1).GetMethod().Name);
			}
			message+=
				DebugUtil.P(sender      !=null, "Demanding type",()=>(sender is Type?((Type)sender).FullName:sender.GetType().FullName))+
				DebugUtil.P(uniqueId    !=null, "UniqueID"      ,uniqueId);
			/*DEBUG*/ if(Debugger.IsAttached) {Debug.WriteLine(message);Debugger.Break();}
			throw new InvalidOperationException(message);
		}
	}
}
