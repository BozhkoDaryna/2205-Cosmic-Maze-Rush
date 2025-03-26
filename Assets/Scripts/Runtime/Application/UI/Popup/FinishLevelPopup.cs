using System;
using System.Threading;
using Application.EndLevelVariations;
using Application.RocketPigs;
using Application.Services;
using Application.Services.Audio;
using Application.Services.UserData;
using Core.UI;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Application.UI
{
    public class FinishLevelPopup : StopPopup
    {
        [SerializeField] private Button _nextLevelButton;

        private FinishGamePopupData _data;
        private UserDataService _userDataService;
        private bool _isEndedLevels;
        private bool _isDone;

        public event Action OpenNextLevelEvent;
        public event Action OpenComingSoonEvent;

        [Inject]
        private void Construct(UserDataService userDataService)
        {
            _userDataService = userDataService;
        }

        public override UniTask Show(BasePopupData data, CancellationToken cancellationToken = default)
        {
            _data = data as FinishGamePopupData;

            SubscribeEvents();

            if (_data.GameResult == GameResult.None)
            {
                ShowOrHideButtons(true, false, true);
                BlockInput(true);
            }

            if (_data.GameResult == GameResult.Victory)
            {
                if (_data.NextLevel >= ConstGame.LevelCount - 1)
                {
                    if (_isDone == false)
                        _userDataService.GetUserData().TryAddLevel(_data.NextLevel);

                    ShowOrHideButtons(true, true, true);
                    _isEndedLevels = true;

                    _isDone = true;
                }

                else
                {
                    _userDataService.GetUserData().TryAddLevel(_data.NextLevel);
                    ShowOrHideButtons(true, true, true);
                }

                BlockInput(true);
            }

            if (_data.GameResult == GameResult.Lose)
            {
                ShowOrHideButtons(true, false, true);
                BlockInput(true);
            }

            return base.Show(data, cancellationToken);
        }

        protected override void SubscribeEvents()
        {
            _nextLevelButton.onClick.AddListener(OpenNextLevel);
            base.SubscribeEvents();
        }

        protected override void RemoveListeners()
        {
            _nextLevelButton.onClick.RemoveAllListeners();
            base.RemoveListeners();
        }

        private void ShowOrHideButtons(bool restartButton, bool nextLevelButton, bool menuButton)
        {
            _restartLevelButton.gameObject.SetActive(restartButton);
            _nextLevelButton.gameObject.SetActive(nextLevelButton);
            _menuButton.gameObject.SetActive(menuButton);
        }

        private void OpenNextLevel()
        {
            if (_data.GameResult == GameResult.Victory && _isEndedLevels == false)
            {
                OpenNextLevelEvent?.Invoke();
                AudioService.PlaySound(ConstAudio.PressButtonSound);
                RemoveListeners();
                DestroyPopup();
            }

            if (_isEndedLevels)
            {
                EndedLevels();
                DestroyPopup();
            }
        }

        private void EndedLevels()
        {
            OpenComingSoonEvent?.Invoke();
        }
    }
}