using System.Threading;
using Application.Game;
using Application.Level;
using Application.RocketPigs;
using Application.Services.UserData;
using Core;
using Core.StateMachine;
using Cysharp.Threading.Tasks;

namespace Application.GameStateMachine
{
    public class GameState : StateController
    {
        private readonly StateMachine _stateMachine;

        private readonly MenuStateController _menuStateController;
        private readonly UserDataStateChangeController _userDataStateChangeController;
        private readonly TitleStateController _titleStateController;
        private readonly DarkLabyrinthsGameStateController _darkLabyrinthsGameStateController;
        private readonly LevelLoaderController _levelLoaderController;
        private readonly ComingSoonStateController _comingSoonStateController;
        private readonly UserDataService _userDataService;

        public GameState(ILogger logger, MenuStateController menuStateController, StateMachine stateMachine,
            UserDataStateChangeController userDataStateChangeController, TitleStateController titleStateController,
            DarkLabyrinthsGameStateController darkLabyrinthsGameStateController,
            LevelLoaderController levelLoaderController, ComingSoonStateController comingSoonStateController, 
            UserDataService userDataService) : 
            base(logger)
        {
            _userDataService = userDataService;
            _comingSoonStateController = comingSoonStateController;
            _levelLoaderController = levelLoaderController;
            _darkLabyrinthsGameStateController = darkLabyrinthsGameStateController;
            _titleStateController = titleStateController;
            _stateMachine = stateMachine;
            _menuStateController = menuStateController;
            _userDataStateChangeController = userDataStateChangeController;
        }

        public override async UniTask Enter(CancellationToken cancellationToken = default)
        {
            await _userDataStateChangeController.Run(default);
            await _levelLoaderController.Run(cancellationToken);
            _userDataService.GetUserData().TryAddLevel(0);

            _stateMachine.Initialize(_menuStateController, _titleStateController, _darkLabyrinthsGameStateController, _comingSoonStateController);
            _stateMachine.GoTo<TitleStateController>(cancellationToken).Forget();
        }
    }
}