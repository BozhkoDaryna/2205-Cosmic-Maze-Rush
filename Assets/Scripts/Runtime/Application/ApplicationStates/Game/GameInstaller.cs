using Application.EndLevelVariations;
using Application.Level;
using Application.RocketPigs;
using Application.UI;
using Runtime.Game;
using UnityEngine;
using Zenject;

namespace Application.Game
{
    [CreateAssetMenu(fileName = "GameInstaller", menuName = "Installers/GameInstaller")]
    public class GameInstaller : ScriptableObjectInstaller<GameInstaller>
    {
        public override void InstallBindings()
        {
            Container.Bind<MenuStateController>().AsSingle();
            Container.Bind<StartSettingsController>().AsSingle();
            Container.Bind<TitleStateController>().AsSingle();
            Container.Bind<SettingsPopupData>().AsSingle();
            Container.Bind<VictoryLevelVariation>().AsSingle();
            Container.Bind<LoseLevelVariation>().AsSingle();
            Container.Bind<LevelLoaderController>().AsSingle();
            Container.Bind<MenuScreenController>().AsSingle();
            Container.Bind<LevelButtonController>().AsSingle();
            Container.Bind<CharacterAnimationController>().AsSingle();
            Container.Bind<ISettingsController>().To<SettingsController>().AsSingle();
            Container.Bind<DarkLabyrinthsGameStateController>().AsCached();
            Container.Bind<GameView>().AsSingle();
            Container.Bind<ComingSoonStateController>().AsSingle();
            Container.BindInterfacesAndSelfTo<GameModel>().AsSingle();
            Container.Bind<PlayerSpawner>().AsSingle();
            Container.BindInterfacesAndSelfTo<PlayerMovement>().AsCached();
            Container.BindInterfacesAndSelfTo<InputReaderController>().AsSingle().NonLazy();
        }
    }
}