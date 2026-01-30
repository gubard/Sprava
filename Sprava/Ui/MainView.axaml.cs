using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Platform;
using Avalonia.VisualTree;
using Gaia.Helpers;
using Gaia.Models;
using Inanna.Services;

namespace Sprava.Ui;

public partial class MainView : UserControl
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

            var topLevel = visual.GetVisualAncestors().OfType<TopLevel>().FirstOrDefault();

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
