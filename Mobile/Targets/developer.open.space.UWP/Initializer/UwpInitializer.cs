using Prism.Unity;
using Microsoft.Practices.Unity;
using developer.open.space.Clients.ViewModels.Interfaces;
using developer.open.space.DataStore.Abstractions;

namespace developer.open.space.UWP.Initializer
{
    public class UwpInitializer : IPlatformInitializer
    {
        public void RegisterTypes(IUnityContainer container)
        {
            container.RegisterType<IPushNotifications, PushNotifications>(new ContainerControlledLifetimeManager());
            container.RegisterType<IToast, Toaster>(new ContainerControlledLifetimeManager());
            container.RegisterType<IWiFiConfig, WiFiConfig>(new ContainerControlledLifetimeManager());
            container.RegisterType<ILaunchTwitter, LaunchTwitter>(new ContainerControlledLifetimeManager());
            container.RegisterType<IAuthenticate, Authenticator>(new ContainerControlledLifetimeManager());
        }
    }
}
