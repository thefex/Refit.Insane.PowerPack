using System.Reflection;
using MvvmCross.Platform;
using MvvmCross.Platform.IoC;
using Refit.Insane.PowerPack.Services;
using SampleApp.Core.ViewModels;

namespace SampleApp.Core
{
    public class App : MvvmCross.Core.ViewModels.MvxApplication
    {
        public override void Initialize()
        {
            CreatableTypes()
                .EndingWith("Service")
                .AsInterfaces()
                .RegisterAsLazySingleton();

            Mvx.RegisterType<MainViewModel>(() => new MainViewModel(Mvx.Resolve<IRestService>()));
            Mvx.RegisterType<ClientDetailsViewModel>(() => new ClientDetailsViewModel(Mvx.Resolve<IRestService>()));

            Mvx.RegisterType<IRestService>(() =>
            {
                var restServiceBuilder = new RestServiceBuilder()
                    .WithCaching()
                    .WithAutoRetry();

                return restServiceBuilder.BuildRestService(typeof(App).GetTypeInfo().Assembly);
            });

            RegisterAppStart<MainViewModel>();
        }
    }
}
