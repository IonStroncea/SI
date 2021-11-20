using MvvmCross.ViewModels;
using SI.Core.ViewModels;

namespace SI.Core
{
    public class App : MvxApplication
    {
        public override void Initialize()
        {
            /* Update View to change the encryption */
            RegisterAppStart<DsaViewModel>();
            //RegisterAppStart<DesViewModel>();
            //RegisterAppStart<RsaViewModel>();
        }
    }
}
