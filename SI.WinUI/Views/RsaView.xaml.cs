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
    [MvxViewFor(typeof(RsaViewModel))]
    public partial class RsaView : MvxWpfView
    {
        public RsaView()
        {
            InitializeComponent();
        }
    }
}
