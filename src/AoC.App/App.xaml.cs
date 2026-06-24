using KE.AoC.App.ViewModels;
using KE.AoC.App.Views;
using KE.AoC.Core.Input;
using KE.AoC.Core.Solution;
using KE.AoC.Solutions;
using System.Windows;

namespace KE.AoC.App;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var registry = new SolutionRegistry(typeof(SolutionsAssemblyMarker).Assembly);
        var inputProvider = new FileInputProvider(FileInputProvider.LocateInputsRoot());
        var runner = new SolutionRunner();

        var mainViewModel = new MainViewModel(registry, inputProvider, runner);

        var window = new MainWindow { DataContext = mainViewModel };
        window.Show();
    }
}
