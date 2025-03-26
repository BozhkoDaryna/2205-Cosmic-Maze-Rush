using System.Threading;
using Application.Services.UserData;
using Application.UI;
using Core;
using Core.Services.ScreenOrientation;
using Core.StateMachine;
using Cysharp.Threading.Tasks;

namespace Application.GameStateMachine
{
    public class BootstrapState : StateController
    {
        private readonly IAssetProvider _assetProvider;
        private readonly IUiService _uiService;
        private readonly ISettingProvider _settingProvider;
        private readonly UserDataService _userDataService;
        private readonly AudioSettingsBootstrapController _audioSettingsBootstrapController;
        private readonly ScreenOrientationAlertController _screenOrientationAlertController;

        public BootstrapState(IAssetProvider assetProvider, IUiService uiService, ILogger logger,
            ISettingProvider settingProvider, UserDataService userDataService,
            AudioSettingsBootstrapController audioSettingsBootstrapController,
            ScreenOrientationAlertController screenOrientationAlertController) : base(logger)
        {
            _assetProvider = assetProvider;
            _uiService = uiService;
            _settingProvider = settingProvider;
            _userDataService = userDataService;
            _audioSettingsBootstrapController = audioSettingsBootstrapController;
            _screenOrientationAlertController = screenOrientationAlertController;
        }

        public override async UniTask Enter(CancellationToken cancellationToken = default)
        {
            _userDataService.Initialize();
            await _assetProvider.Initialize();
            await _uiService.Initialize();
            await _settingProvider.Initialize();
            await _screenOrientationAlertController.Run(default);
            _uiService.ShowScreen(ConstScreens.SplashScreen, cancellationToken).Forget();
            await _audioSettingsBootstrapController.Run(default);
            UpdateSession();

            GoTo<GameState>(cancellationToken).Forget();
        }

        public override async UniTask Exit()
        {
            await _uiService.HideScreen(ConstScreens.SplashScreen);
        }

        private void UpdateSession()
        {
            _userDataService.GetUserData().GameData.SessionNumber++;
            _userDataService.SaveUserData();
        }
    }
}