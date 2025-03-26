using System.Threading;
using Application.RocketPigs;
using Application.UI;
using Core;
using Core.Services.Audio;
using Core.StateMachine;
using Cysharp.Threading.Tasks;

namespace Application.Game
{
    public class MenuStateController : StateController
    {
        private readonly IAudioService _audioService;
        private readonly MenuScreenController _menuScreenController;
        private readonly LevelButtonController _levelButtonController;
        private readonly IUiService _uiService;
        private readonly ConfigSettingsPopup _configSettingsPopup;

        private SettingsPopup _settingsPopup;

        public MenuStateController(ILogger logger, IAudioService audioService,
            MenuScreenController menuScreenController, IUiService uiService,
            LevelButtonController levelButtonController, ConfigSettingsPopup configSettingsPopup) : base(logger)
        {
            _configSettingsPopup = configSettingsPopup;
            _uiService = uiService;
            _levelButtonController = levelButtonController;
            _menuScreenController = menuScreenController;
            _audioService = audioService;
        }

        public override async UniTask Enter(CancellationToken cancellationToken = default)
        {
            await _menuScreenController.Run(cancellationToken);

            _menuScreenController.StartGameEvent += SwitchToGameController;
            _menuScreenController.SettingsButtonPressEvent += ShowSettingsButtonPopup;
            _menuScreenController.ComingSoonEvent += SwitchToComingSoonController;
            _menuScreenController.ExitEvent += SwitchToTitleController;

            await _menuScreenController.CreateLevels(cancellationToken);
            await _levelButtonController.Run(cancellationToken);
        }

        public override async UniTask Exit()
        {
            _menuScreenController.StartGameEvent -= SwitchToGameController;
            _menuScreenController.SettingsButtonPressEvent -= ShowSettingsButtonPopup;
            _menuScreenController.ComingSoonEvent -= SwitchToComingSoonController;
            _menuScreenController.ExitEvent -= SwitchToTitleController;
            await _levelButtonController.Stop();
            await _menuScreenController.Stop();
        }

        private void ShowSettingsButtonPopup()
        {
            _settingsPopup = _uiService.GetPopup<SettingsPopup>(ConstPopups.SettingsPopup);
            _settingsPopup.Show(_configSettingsPopup.GetConfig(_settingsPopup));
        }

        private void SwitchToGameController()
        {
            _audioService.StopMusic();
            GoTo<DarkLabyrinthsGameStateController>();
        }

        private void SwitchToTitleController()
        {
            GoTo<TitleStateController>();
        }

        private void SwitchToComingSoonController()
        {
            GoTo<ComingSoonStateController>();
        }
    }
}