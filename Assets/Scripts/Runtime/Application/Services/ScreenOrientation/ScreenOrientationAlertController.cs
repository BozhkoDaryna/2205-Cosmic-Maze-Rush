using UnityEngine;
using Application.UI;
using Zenject;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace Core.Services.ScreenOrientation
{
    public class ScreenOrientationAlertController : BaseController, ITickable
    {
        private readonly IUiService _uiService;
        private readonly ISettingProvider _settingProvider;
        private ScreenOrientationAlertPopup _alertPopup;
        private ScreenOrientationConfig _config;
        private bool _isInitialized;

        public ScreenOrientationAlertController(IUiService uiService, ISettingProvider settingProvider)
        {
            _uiService = uiService;
            _settingProvider = settingProvider;
        }

        public override UniTask Run(CancellationToken cancellationToken)
        {
            base.Run(cancellationToken);
            Init();

            return UniTask.CompletedTask;
        }

        public void Tick()
        {
            if (!_isInitialized)
                return;

            if (_config == null || !_config.EnableScreenOrientationPopup)
                return;

            CheckScreenOrientation();
        }

        private void CheckScreenOrientation()
        {
            UnityEngine.ScreenOrientation currentScreenMode = Screen.orientation;

            if (currentScreenMode == _config.AllowedScreenMode)
            {
                if (_alertPopup.gameObject.activeSelf)
                    _alertPopup.Hide();

                return;
            }

            if (!_alertPopup.gameObject.activeSelf)
                _alertPopup.Show(default);
        }

        private UniTask Init()
        {
            _config = _settingProvider.Get<ScreenOrientationConfig>();

            if (_config == null || !_config.EnableScreenOrientationPopup)
                return UniTask.CompletedTask;

            _alertPopup = _uiService.GetPopup<ScreenOrientationAlertPopup>(ConstPopups.ScreenOrientationAlertPopup);
            _alertPopup.Hide();

            _isInitialized = true;

            return UniTask.CompletedTask;
        }
    }
}