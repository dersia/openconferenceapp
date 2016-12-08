using Prism.Unity;
using Microsoft.Practices.Unity;
using open.conference.app.Clients.ViewModels.Interfaces;
using open.conference.app.DataStore.Abstractions;

namespace open.conference.app.iOS.Initializer
{
    public class IosInitializer : IPlatformInitializer
    {
        public void RegisterTypes(IUnityContainer container)
        {
            container.RegisterType<IToast, Toaster>(new ContainerControlledLifetimeManager());
            container.RegisterType<IPushNotifications, PushNotifications>(new ContainerControlledLifetimeManager());
            container.RegisterType<ILaunchTwitter, LaunchTwitter>(new ContainerControlledLifetimeManager());
            container.RegisterType<IWiFiConfig, WiFiConfig>(new ContainerControlledLifetimeManager());
            container.RegisterType<IAuthenticate, Authenticator>(new ContainerControlledLifetimeManager());
        }
    }
}
