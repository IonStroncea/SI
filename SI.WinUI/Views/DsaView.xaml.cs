using Microsoft.Win32;
using MvvmCross.Platforms.Wpf.Presenters.Attributes;
using MvvmCross.Platforms.Wpf.Views;
using MvvmCross.ViewModels;
using SI.Core.ViewModels;
using System;

namespace SI.WinUI.Views
{
    /// <summary>
    /// Interaction logic for ConnectionView.xaml
    /// </summary>
    [MvxContentPresentation]
    [MvxViewFor(typeof(DsaViewModel))]
    public partial class DsaView : MvxWpfView
    {
        public DsaView()
        {
            InitializeComponent();
        }
    }
}
