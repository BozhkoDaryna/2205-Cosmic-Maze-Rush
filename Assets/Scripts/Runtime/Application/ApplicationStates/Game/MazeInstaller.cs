using System.Collections.Generic;
using Application.Services.UserData;
using Runtime.Game;
using Runtime.MazeGenerator;
using UnityEngine;
using Zenject;

namespace Application.Game
{
    [CreateAssetMenu(fileName = "MazeInstaller", menuName = "Installers/MazeInstaller")]
    public class MazeInstaller : ScriptableObjectInstaller<MazeInstaller>
    {
        [SerializeField] private GameObject _roomPrefab;
        [SerializeField] private TriggerDetector _doorPrefab;
        [SerializeField] private FirePotion _firePotionPrefab;
        [SerializeField] private GameObject _arrowPrefab;
        [SerializeField] private List<MazeData> _mazeDatabase;
        [SerializeField] private List<TimerData> _timerDatabase;
        [SerializeField] public float _potionPower;

        public override void InstallBindings()
        {
            Container.Bind<MazeGenerator>()
                .AsSingle()
                .WithArguments(_roomPrefab, _doorPrefab, _firePotionPrefab, _arrowPrefab, _potionPower);

            Container.BindInstance(_mazeDatabase).AsSingle();
            Container.BindInstance(_timerDatabase).AsSingle();
            Container.BindInterfacesAndSelfTo<TimerController>().AsSingle();
        }
    }
}