using System.Collections.Generic;
using System.Threading;
using Application.UI;
using Core;
using Core.StateMachine;
using Cysharp.Threading.Tasks;

namespace Application.Game
{
    public class TitleStateController : StateController
    {
        private readonly HashSet<ButtonTypes> _buttonTypes = new HashSet<ButtonTypes>();
        private readonly IUiService _uiService;
        private readonly IConfigPopup<SettingsPopup, SettingsPopupData> _configSettingsPopup;

        private CancellationTokenSource _cancellationToken = new CancellationTokenSource();
        private TitleScreen _titleScreen;

        public TitleStateController(ILogger logger, IUiService uiService, ConfigSettingsPopup configSettingsPopup) :
            base(logger)
        {
            _configSettingsPopup = configSettingsPopup;
            _uiService = uiService;
        }

        public override UniTask Enter(CancellationToken cancellationToken = default)
        {
            if (_cancellationToken.Token.IsCancellationRequested || _cancellationToken == null)
                _cancellationToken = new CancellationTokenSource();

            CreateTitleScreen();

            foreach (var buttonType in _buttonTypes)
                RestorePopup(buttonType);

            Subscribe();

            _titleScreen?.ShowAsync(_cancellationToken.Token).Forget();

            return UniTask.CompletedTask;
        }

        public override UniTask Exit()
        {
            _cancellationToken.Cancel();

            HideAndUnSubscribeTitleScreen();
            return base.Exit();
        }

        private void CreateTitleScreen()
        {
            if (_titleScreen == null)
                _titleScreen = _uiService.GetScreen<TitleScreen>(ConstScreens.TitleScreen);
        }

        private void Subscribe()
        {
            _titleScreen.PlayButtonPressEvent += PlayBallController;
            _titleScreen.SettingsButtonPressEvent += ShowSettingsButtonPopup;
            _titleScreen.InfoButtonPressEvent += ShowInfoPopup;
        }

        private void PlayBallController()
        {
            _buttonTypes.Add(ButtonTypes.Play);
            GoTo<MenuStateController>();
        }

        private void ShowInfoPopup()
        {
            var infoPopup = _uiService.GetPopup<InfoPopup>(ConstPopups.InfoPopup);
            infoPopup.Show(default).Forget();
            infoPopup.DestroyPopupEvent += RestorePopup;

            _buttonTypes.Add(ButtonTypes.Info);
        }

        private void ShowSettingsButtonPopup()
        {
            var settingsPopup = _uiService.GetPopup<SettingsPopup>(ConstPopups.SettingsPopup);
            settingsPopup.Show(_configSettingsPopup.GetConfig(settingsPopup)).Forget();
            settingsPopup.HideImmediatelyEvent += RestorePopup;

            _buttonTypes.Add(ButtonTypes.Settings);
        }

        private void RestorePopup(ButtonTypes buttonType)
        {
            _titleScreen.RestoreButton(buttonType);
        }

        private void HideAndUnSubscribeTitleScreen()
        {
            if (_titleScreen is not null)
            {
                _titleScreen.PlayButtonPressEvent -= PlayBallController;
                _titleScreen.SettingsButtonPressEvent -= ShowSettingsButtonPopup;
                _titleScreen.InfoButtonPressEvent -= ShowInfoPopup;
                _titleScreen.HideImmediately();
            }
        }
    }
}