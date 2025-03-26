using System.Collections.Generic;
using Application.Services.UserData;
using Runtime.Game;
using UnityEngine;
using Zenject;

namespace Application.Game
{
    public class LightInstaller : ScriptableObjectInstaller<MazeInstaller>
    {
        [SerializeField] private TorchData _torchData;
        [SerializeField] private List<Sprite> _fireSprites;

        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<LightScaler>().AsSingle().WithArguments(_torchData);
            Container.BindInterfacesAndSelfTo<FireSpriteSwitcher>().AsSingle().WithArguments(_fireSprites);
        }
    }
}