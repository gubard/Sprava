using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Platform;
using Gaia.Helpers;
using Inanna.Services;

namespace Sprava.Ui;

public sealed partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();

        Loaded += (_, e) =>
        {
            if (e.Source is not Visual visual)
            {
                return;
            }

            var topLevel = TopLevel.GetTopLevel(visual);

            if (topLevel is null)
            {
                return;
            }

            InannaApplication.UpdateMaterialDesignSizeType(topLevel);

            if (!OsHelper.IsMobile || topLevel.InputPane is null)
            {
                return;
            }

            topLevel.InputPane.StateChanged += (_, i) =>
                MobileBottomRectangle.Height =
                    i.NewState == InputPaneState.Closed ? 0 : i.EndRect.Height;
        };
    }
}
