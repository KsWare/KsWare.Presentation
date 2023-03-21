# 0.41 [2023-03-21]
- ActionVM.Implicit for suppressing the CS8618 warning.
- FIX usage of HierarchyAttribute.CreateInstance

# 0.40 [2023-03-12]
- drop support for net5.0
- drop support for net452, replaced with net462

# 0.32
- add TargetFramework net48

# 0.31
- BackingFieldsStore SetValue enhanced with return value and optional changeHandler
- BackingFieldsStore SetAndRaise is now obsolete, Use the new SetValue.

# 0.30
- add TargetFrameworks: net6.0-windows, net452
- remove TargetFrameworks: net45

# 0.21
- EventManager.Raise add overload with lazy event args
- FIX ActionVM.RegisterActionMethod
- FIX do not RegisterActionMethod if Metadata.ActionProvider.ExecutedCallback is already set
- add ButtonActionVM. derived from ActionVM supporting common button properties
- add some UIProperties (ObjectVM.UI)
- FIX ListVM clear item.Parent when removed

# 0.20
- BREAKING CHANGE ValueChangedEventArgs swap arguments and rename PreviousValue to OldValue, so that you get a compiler error on event handlers. If you create ValueChangedEventArgs you have to swap the arguments manually. No Warning in this case!
- add SimpleCommand.RaiseCanExecuteChanged
- ValueChangedEventArgs no longer returns DependencyProperty.UnsetValue, so this is consistent to ValueChangedEventArgs<T>
- ActionVM supports async methods  Task DoAnything(), Task DoAnything(object)

# 0.19
- convert to SDK format
- TargetFrameworks: net5.0-windows, netcoreapp3.1, net45
- add WindowVM.Owner
- add MenuItemVM.Command
- add MenuItemVM.CommandParameter
- add WindowVM.ClosingEvent
- DataTypeConverter.Convert: fix System.InvalidCastException
- DebugType=embedded and DebugSymbols=true for each configuration 

# 0.18
- split KsWare.Presentation.Behavior
- split KsWare.Presentation.Behavior.Common

# 0.18.40 (2019-09-04)
 - Use nuget Microsoft.Xaml.Behaviors.Wpf instead the System.Windows.Interactivity
   - Read more: https://devblogs.microsoft.com/dotnet/open-sourcing-xaml-behaviors-for-wpf/
- remove nuget System.Windows.Interactivity.WPF-2.0.20525
# 0.18.36 (2019-09-03)
- use nuget KsWare.Presentation.Converters
- use nuget KsWare.Presentation.ViewFramework.Common
- change namespace of some converters (KsWare.Presentation.Converters)
# 0.18.22
- add ViewModelStylesResources
- add UIElementControllerVM<T>
- add ViewModelControllerBehavior
- add FrameControllerVM
# 0.18.15 (2018-03-30)
- add MenuItemVM
- add MenuItemVMStyle
- add DesigntimeResourceDictionary
# 0.18.11
- BackingFieldStore extended
- add ObservableDictionary
- Sample application moved
- add SharedResourceDictionary
- add FieldBindings
# 0.18.5
- remove KsWare.JsonFx
- remove KsWare.Presentation.Compatibility40
- add KsWare.Presentation.Themes to NuGet package
- add KsWare.Presentation.Themes.Aero2Fix to NuGet package
# 0.18.2
- cleanup old methods
- Remove support for .NET Framework 4.0
- Minimum supported .NET Framework 4.5
- Minimum Visual Studio 2017
- Using C# 7.2
- configure continuous integration for NuGet and GitHub Releases
# 0.18.1 (2018-03-11)
- moved to GitGub
- license changed to MIT
- change nuget package name and owner "KsWare.Presentation" owner KsWare
# [2017-07-18]
- UPD WindowVM action for Close, Minimize, Maximize
- ADD WindowVM IsOpenProperty
- BETA WindowVM IsFullScreen, 
- UPD SpecialBooleanConverter default instances for compare operations