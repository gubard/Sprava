using Avalonia;
using Avalonia.Controls;
using Avalonia.VisualTree;
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
        };
    }
}
