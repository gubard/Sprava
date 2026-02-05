using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Aya.Contract.Models;
using Gaia.Helpers;
using Hestia.Contract.Models;
using Inanna.Services;
using Neotoma.Contract.Models;
using Nestor.Db.Helpers;
using Sprava.Ui;
using Turtle.Contract.Models;

namespace Sprava;

public class App : InannaApplication
{
    static App()
    {
        InsertHelper.AddDefaultInsert(
            nameof(FileEntity),
            id => new FileEntity[] { new() { Id = id } }.CreateInsertQuery()
        );

        InsertHelper.AddDefaultInsert(
            nameof(ToDoEntity),
            id => new ToDoEntity[] { new() { Id = id } }.CreateInsertQuery()
        );

        InsertHelper.AddDefaultInsert(
            nameof(CredentialEntity),
            id => new CredentialEntity[] { new() { Id = id } }.CreateInsertQuery()
        );

        InsertHelper.AddDefaultInsert(
            nameof(FileObjectEntity),
            id => new FileObjectEntity[] { new() { Id = id } }.CreateInsertQuery()
        );
    }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var viewModel = DiHelper.ServiceProvider.GetService<MainViewModel>();
        DisableAvaloniaDataAnnotationValidation();

        switch (ApplicationLifetime)
        {
            case IClassicDesktopStyleApplicationLifetime desktop:
                desktop.MainWindow = new MainWindow { DataContext = viewModel };

                break;
            case ISingleViewApplicationLifetime singleViewPlatform:
                singleViewPlatform.MainView = new MainView { DataContext = viewModel };

                break;
        }

        base.OnFrameworkInitializationCompleted();
    }
}
