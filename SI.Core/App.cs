using MvvmCross.ViewModels;
using SI.Core.ViewModels;

namespace SI.Core
{
    public class App : MvxApplication
    {
        public override void Initialize()
        {
            RegisterAppStart<RsaViewModel>();
        }
    }
}
