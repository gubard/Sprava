using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Aya.Contract.Models;
using Gaia.Helpers;
using Hestia.Contract.Models;
using Inanna.Services;
using Neotoma.Contract.Models;
using Nestor.Db.Helpers;
using Rooster.Contract.Models;
using Sprava.Ui;
using Turtle.Contract.Models;

namespace Sprava;

public class App : InannaApplication
{
    static App()
    {
        InsertHelper.AddDefaultInsert(
            nameof(FileEntity),
            (i, s) => new FileEntity[] { new() { Id = i } }.CreateInsertQuery(s)
        );

        InsertHelper.AddDefaultInsert(
            nameof(ToDoEntity),
            (i, s) => new ToDoEntity[] { new() { Id = i } }.CreateInsertQuery(s)
        );

        InsertHelper.AddDefaultInsert(
            nameof(CredentialEntity),
            (i, s) => new CredentialEntity[] { new() { Id = i } }.CreateInsertQuery(s)
        );

        InsertHelper.AddDefaultInsert(
            nameof(FileObjectEntity),
            (i, s) => new FileObjectEntity[] { new() { Id = i } }.CreateInsertQuery(s)
        );

        InsertHelper.AddDefaultInsert(
            nameof(AlarmEntity),
            (i, s) => new AlarmEntity[] { new() { Id = i } }.CreateInsertQuery(s)
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
