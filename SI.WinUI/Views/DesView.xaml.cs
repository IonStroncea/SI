using MvvmCross.Platforms.Wpf.Presenters.Attributes;
using MvvmCross.Platforms.Wpf.Views;
using MvvmCross.ViewModels;
using SI.Core.ViewModels;

namespace SI.WinUI.Views
{
    /// <summary>
    /// Interaction logic for ConnectionView.xaml
    /// </summary>
    [MvxContentPresentation]
    [MvxViewFor(typeof(DesViewModel))]
    public partial class DesView : MvxWpfView
    {
        public DesView()
        {
            InitializeComponent();
        }
    }
}
