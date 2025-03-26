using Application.Game;
using Core;
using Application.IAP;
using Application.Services.Audio;
using Application.UI;
using Application.Services.Network;
using Application.Services.ApplicationState;
using Application.Services.UserData;
using Core.Compressor;
using Core.Factory;
using Core.Services.Audio;
using Core.Services.ScreenOrientation;
using Core.UI;
using UnityEngine;
using Zenject;
using ILogger = Core.ILogger;

namespace Application.Services
{
    [CreateAssetMenu(fileName = "ServicesInstaller", menuName = "Installers/ServicesInstaller")]
    public class ServicesInstaller : ScriptableObjectInstaller<ServicesInstaller>
    {
        public override void InstallBindings()
        {
            Container.Bind<IUiService>().To<UiService>().AsSingle();
            Container.Bind<IAssetProvider>().To<AssetProvider>().AsSingle();
            Container.Bind<IPersistentDataProvider>().To<PersistantDataProvider>().AsSingle();
            Container.Bind<ISettingProvider>().To<SettingsProvider>().AsSingle();
            Container.Bind<ILogger>().To<SimpleLogger>().AsSingle();
            Container.Bind<IFileStorageService>().To<PersistentFileStorageService>().AsSingle();
            Container.Bind<IFileCleaner>().To<FileCleaner>().AsSingle();
            Container.Bind<ISerializationProvider>().To<JsonSerializationProvider>().AsSingle();
            Container.Bind<IAudioService>().To<AudioService>().AsSingle();
            Container.Bind<INetworkConnectionService>().To<NetworkConnectionService>().AsSingle();
            Container.Bind<BaseCompressor>().To<ZipCompressor>().AsSingle();
            Container.Bind<IIAPService>().To<IAPServiceMock>().AsSingle();
            Container.Bind<GameObjectFactory>().AsSingle();
            Container.Bind<ConfigSettingsPopup>().AsSingle();
            Container.Bind<ApplicationStateService>().AsSingle();
            Container.Bind<UserDataService>().AsSingle();
            Container.Bind<ServerProvider>().AsSingle();
            Container.BindInterfacesAndSelfTo<ScreenOrientationAlertController>().AsSingle();
        }
    }
}