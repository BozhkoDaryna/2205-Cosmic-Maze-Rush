using System;
using System.Threading;
using Application.EndLevelVariations;
using Application.RocketPigs;
using Application.UI;
using Core.Services.Audio;
using Core.UI;
using Cysharp.Threading.Tasks;
using Runtime.Game;

namespace Application.Game
{
    public class GameView
    {
        private readonly IUiService _uiService;
        private readonly IAudioService _audioService;

        private GameScreen _gameScreen;
        private PausePopup _pausePopup;
        private HintPopup _hintPopup;
        private FinishLevelPopup _finishLevelPopup;
        private FireSlider _fireSliderPopup;

        public event Action PauseRequestedEvent;
        public event Action HideHintPopupEvent;
        public event Action TakeUnPauseEvent;
        public event Action MenuRequestedEvent;
        public event Action RestartRequestedEvent;
        public event Action NextLevelRequestedEvent;
        public event Action ComingSoonRequestedEvent;
        public event Action<FireSlider> InitializeFireSliderEvent;

        public GameView(IUiService uiService, IAudioService audioService)
        {
            _uiService = uiService;
            _audioService = audioService;
        }

        public async UniTask ShowGameScreen(CancellationToken cancellationToken)
        {
            if (_gameScreen == null)
                _gameScreen = _uiService.GetScreen<GameScreen>(ConstScreens.GameScreenUI);

            await _gameScreen.ShowAsync(cancellationToken);
            _gameScreen.OnPaused += () => PauseRequestedEvent?.Invoke();
            InitializeFireSliderEvent?.Invoke(_gameScreen.FireSlider);
        }

        public void ShowPausePopup()
        {
            if (_pausePopup == null)
            {
                _pausePopup = _uiService.GetPopup<PausePopup>(ConstPopups.PausePopup);
                _pausePopup.Show(default);

                _pausePopup.CancelEvent += HidePausePopup;
                _pausePopup.CancelEvent += () => TakeUnPauseEvent?.Invoke();
                _pausePopup.OpenMenuEvent += () => MenuRequestedEvent?.Invoke();
                _pausePopup.RestartLevelEvent += () => RestartRequestedEvent?.Invoke();
            }
        }

        public void ShowFireSlider(bool isShow)
        {
            if (_gameScreen.FireSlider != null)
                _gameScreen.FireSlider.gameObject.SetActive(isShow);
        }

        public void ShowFinishLevelPopup(GameResult result, int nextLevel)
        {
            if (_finishLevelPopup == null)
            {
                _finishLevelPopup = _uiService.GetPopup<FinishLevelPopup>(ConstPopups.FinishLevelPopup);
                _finishLevelPopup.Show(new FinishGamePopupData {GameResult = result, NextLevel = nextLevel});

                _finishLevelPopup.RestartLevelEvent += () => RestartRequestedEvent?.Invoke();
                _finishLevelPopup.DestroyPopupEvent += TakeUnPauseEvent;
                _finishLevelPopup.DestroyPopupEvent += HideFinishLevelPopup;
                _finishLevelPopup.OpenMenuEvent += () => MenuRequestedEvent?.Invoke();
                _finishLevelPopup.OpenNextLevelEvent += () => NextLevelRequestedEvent?.Invoke();
                _finishLevelPopup.OpenComingSoonEvent += () => ComingSoonRequestedEvent?.Invoke();
            }
        }

        public void ShowHintPopup(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            GetHintPopup();

            _hintPopup.Show(default, cancellationToken);
            _hintPopup.HideHintPopupEvent += OnHideHintPopup;
        }

        private void OnHideHintPopup()
        {
            HideHintPopupEvent?.Invoke();
            _hintPopup.HideHintPopupEvent -= OnHideHintPopup;
        }

        private void HideFinishLevelPopup()
        {
            if (_finishLevelPopup != null)
            {
                _finishLevelPopup.DestroyPopupEvent -= HideFinishLevelPopup;
                _finishLevelPopup.RestartLevelEvent -= () => RestartRequestedEvent?.Invoke();
                _finishLevelPopup.OpenMenuEvent -= () => MenuRequestedEvent?.Invoke();
                _finishLevelPopup.OpenNextLevelEvent -= () => NextLevelRequestedEvent?.Invoke();
                _finishLevelPopup.OpenComingSoonEvent -= () => ComingSoonRequestedEvent?.Invoke();
            }
        }

        private void HidePausePopup()
        {
            if (_pausePopup != null)
            {
                _pausePopup.CancelEvent -= HidePausePopup;
                _pausePopup.CancelEvent -= () => TakeUnPauseEvent?.Invoke();
                _pausePopup.OpenMenuEvent -= () => MenuRequestedEvent?.Invoke();
                _pausePopup.RestartLevelEvent -= () => RestartRequestedEvent?.Invoke();

                _pausePopup?.DestroyPopup();
            }
        }

        public HintPopup GetHintPopup()
        {
            if (_hintPopup == null)
                return _hintPopup = _uiService.GetPopup<HintPopup>(ConstPopups.HintPopup);

            return _hintPopup;
        }

        public void HideUI()
        {
            _uiService.HideScreen(ConstScreens.GameScreenUI);
            _hintPopup?.Hide();
            _finishLevelPopup?.Hide();
            _pausePopup?.Hide();
        }

        public void StopAllAudio()
        {
            _audioService.StopAll();
        }

        public void ResumeSounds()
        {
            _audioService.ResumeSounds();
        }
    }
}