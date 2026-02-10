using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using Aya.Contract.Models;
using Gaia.Helpers;
using Gaia.Services;
using Hestia.Contract.Models;
using Inanna.Models;
using Inanna.Services;
using Neotoma.Contract.Models;
using Nestor.Db.Helpers;
using Rooster.Contract.Models;
using Sprava.Models;
using Sprava.Ui;
using Turtle.Contract.Models;

namespace Sprava;

public sealed class App : InannaApplication
{
    static App()
    {
        InsertHelper.AddDefaultInsert(
            nameof(FileEntity),
            i => new FileEntity[] { new() { Id = i } }.CreateInsertQuery()
        );

        InsertHelper.AddDefaultInsert(
            nameof(ToDoEntity),
            i => new ToDoEntity[] { new() { Id = i } }.CreateInsertQuery()
        );

        InsertHelper.AddDefaultInsert(
            nameof(CredentialEntity),
            i => new CredentialEntity[] { new() { Id = i } }.CreateInsertQuery()
        );

        InsertHelper.AddDefaultInsert(
            nameof(FileObjectEntity),
            i => new FileObjectEntity[] { new() { Id = i } }.CreateInsertQuery()
        );

        InsertHelper.AddDefaultInsert(
            nameof(AlarmEntity),
            i => new AlarmEntity[] { new() { Id = i } }.CreateInsertQuery()
        );
    }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override async void OnFrameworkInitializationCompleted()
    {
        await LoadSettingAsync();
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

    private async ValueTask LoadSettingAsync()
    {
        var objectStorage = DiHelper.ServiceProvider.GetService<IObjectStorage>();
        var settings = await objectStorage.LoadAsync<InannaSettings>(CancellationToken.None);
        var langResource = Styles.OfType<LangResource>().First();
        langResource.Lang = settings.Lang;

        RequestedThemeVariant = settings.Theme switch
        {
            ThemeVariantType.System => ThemeVariant.Default,
            ThemeVariantType.Dark => ThemeVariant.Dark,
            ThemeVariantType.Light => ThemeVariant.Light,
            _ => throw new ArgumentOutOfRangeException(),
        };
    }
}
