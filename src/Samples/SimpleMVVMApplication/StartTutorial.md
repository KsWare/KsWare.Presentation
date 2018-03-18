# Start-Tutorial
Create a simple MVVM Application using KsWare.Presentation.ViewModelFramework

1. Create new project using template "WPF App (.NET Framework)"
2. Add class AppVM and derive from ApplicationVM
3. Modify app.xml 
   - derive from ViewModelApplication
   - remove StartUpUri
4. Create MainWindow.cs and derive from WindowVM  

### 2. AppVM

```C#
public class AppVM : ApplicationVM {

	public AppVM() {
        RegisterChildren(() => this);
        StartupUri = typeof(MainWindowVM);
	}
}
````
### 3. App.xaml
```XML
<ksv:ViewModelApplication 
	x:TypeArguments="local:AppVM" 
	x:Class="MyApp.App"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:ksv="http://ksware.de/Presentation/ViewFramework"
    xmlns:local="clr-namespace:MyApp">
    <Application.Resources>
         
    </Application.Resources>
</ksv:ViewModelApplication>
```
### 4. MainWindowVM.cs
```
public class MainWindowVM : WindowVM {

	public MainWindowVM() {
		RegisterChildren(() => this);
	}
}
```
## Ready to Start

**Now you are ready to start**

- Add some Controls for a simple sample.
- A Button, a TextBlock and a TextBox. 
- The click will fill the textblock with actual date.
- The TextBox will add some days to actual date.

### Why this works so easy?

**The trick is auto-wire with naming conventions**

The MainWindow.xaml is used automatically because the view-model is named **MainWindow**VM. 
All views (xaml files) ending with **Window** or **View** are auto-wired. 
- **XyzWindow** or **XyzWindow**View to **YxzWindow**VM
- **Sample**View is auto-wired to **Sample**VM

Same for Actions

- **FillDate**Action is wired to Do**FillDate** method

View-Model properties are are alo wired
- use RegisterChildren and all Properties with a View-Model type are wired. e.g. the DayInput and the FillDateAction.

**You may use auto-wireing but you can also explicit declare the "wires".**

## Sample
```C#
public class MainWindowVM : WindowVM {

	public MainWindowVM() {
		RegisterChildren(() => this);

		DayInput.ValueChangedEvent.add = (sender, args) => {
			Date = DateTime.Today.AddDays(DayInput.Value);
		};
	}

	public DateTime Date { get => Fields.GetValue<DateTime>(); private set => Fields.SetValue(value); }

	public Int32VM DayInput { get; [UsedImplicitly] private set; }

	public ActionVM FillDateAction { get; [UsedImplicitly] private set; }

	[UsedImplicitly /*by FillDateAction*/]
	private void DoFillDate(object parameter) {
		var days = (int) Convert.ChangeType(parameter ?? 0, typeof(int));
		Date = DateTime.Today.AddDays(days);
	}
}
```
```XML
<Window
	x:Class="MyApp.MainWindow"
	xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
	xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
	xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
	xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
	xmlns:local="clr-namespace:MyApp"
	mc:Ignorable="d"
	d:DataContext="{d:DesignInstance Type=local:MainWindowVM}"
	Title="MainWindow" Height="140" Width="301">
	<StackPanel Margin="10">
		<TextBlock Text="{Binding Date, StringFormat=yyyy-MM-dd}" FontSize="18" HorizontalAlignment="Center"/>
		<UniformGrid Rows="1" Margin="0 8 0 0">
			<Button Content="-1" Command="{Binding FillDateAction}" CommandParameter="-1"/>
			<Button Content="Today" Command="{Binding FillDateAction}"/>
			<Button Content="+1" Command="{Binding FillDateAction}" CommandParameter="+1"/>
		</UniformGrid>
		<DockPanel Margin="0 8 0 0">
			<TextBlock Text="Days: "/>
			<TextBox Text="{Binding DayInput.EditValueProvider.String, UpdateSourceTrigger=PropertyChanged}" />
		</DockPanel>
	</StackPanel>
</Window>
```