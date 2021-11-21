using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SI.Core.ViewModels
{
    public enum Method
    {
        RSA = 0,
        DSA = 1,
        DES = 2
    }

    public class MainViewModel : MvxViewModel
    {
        private readonly IMvxNavigationService mvxNavigationService;
        public MainViewModel(IMvxNavigationService mvxNavigationService)
        {
            this.mvxNavigationService = mvxNavigationService;

            RsaCommand = new MvxCommand<Method>(_ => GoTo(Method.RSA));
            DsaCommand = new MvxCommand<Method>(_ => GoTo(Method.DSA));
            DesCommand = new MvxCommand<Method>(_ => GoTo(Method.DES));
        }

        public IMvxCommand RsaCommand { get; set; }
        public IMvxCommand DsaCommand { get; set; }
        public IMvxCommand DesCommand { get; set; }

        public void GoTo(Method method)
        {
            switch (method)
            {
                case Method.RSA:
                    mvxNavigationService.Navigate<RsaViewModel>();
                    break;
                case Method.DSA:
                    mvxNavigationService.Navigate<DsaViewModel>();
                    break;
                case Method.DES:
                    mvxNavigationService.Navigate<DesViewModel>();
                    break;
            }
        }

    }
}
