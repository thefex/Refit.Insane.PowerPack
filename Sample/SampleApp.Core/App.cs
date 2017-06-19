using System;
using System.Net.Http;
using System.Reflection;
using MvvmCross.Platform;
using MvvmCross.Platform.IoC;
using Refit.Insane.PowerPack.Services;
using SampleApp.Core.ViewModels;

namespace SampleApp.Core
{
    public class App : MvvmCross.Core.ViewModels.MvxApplication
    {
        public static Uri ApiPath => new Uri("http://apitestprezentacja.azurewebsites.net/");

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

                return restServiceBuilder.BuildRestService(() => new System.Net.Http.HttpClient(
                    new HttpClientDiagnostics.HttpClientDiagnosticsHandler(new HttpClientHandler())){
                    Timeout = TimeSpan.FromSeconds(5),
                    BaseAddress = ApiPath
                }, typeof(App).GetTypeInfo().Assembly);
            });

            RegisterAppStart<MainViewModel>();
        }
    }
}
