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
    public class BaseMethodViewModel : MvxViewModel
    {
        private readonly IMvxNavigationService mvxNavigationService;

        public BaseMethodViewModel(IMvxNavigationService mvxNavigationService)
        {
            this.mvxNavigationService = mvxNavigationService;

            MainMenuCommand = new MvxCommand(MainMenu);
        }

        public IMvxCommand MainMenuCommand { get; set; }


        private void MainMenu()
        {
            this.mvxNavigationService.Navigate<MainViewModel>();
        }
    }
}
