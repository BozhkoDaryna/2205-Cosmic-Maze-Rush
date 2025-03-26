using System;
using System.Threading;
using Application.EndLevelVariations;
using Application.GameStateMachine;
using Application.RocketPigs;
using Application.Services.Audio;
using Core;
using Core.Services.Audio;
using Core.StateMachine;
using Core.UI;
using Cysharp.Threading.Tasks;
using Runtime.Game;
using Runtime.MazeGenerator;

namespace Application.Game
{
    public class DarkLabyrinthsGameStateController : StateController
    {
        private readonly GameModel _model;
        private readonly GameView _view;
        private readonly PlayerSpawner _playerSpawner;
        private readonly PlayerMovement _playerMovement;
        private readonly InputReaderController _inputReaderController;
        private readonly AudioSettingsBootstrapController _audioSettingsBootstrapController;
        private readonly BaseLevelVariation _victoryLevelVariation;
        private readonly BaseLevelVariation _loseLevelVariation;
        private readonly MazeGenerator _mazeGenerator;
        private readonly TimerController _timerController;
        private readonly IAudioService _audioService;

        private CancellationTokenSource _cancellationTokenSource;
        private CancellationTokenSource _gameCancellationTokenSource;
        private HintPopup _hintPopup;
        private bool _isHintPopupShow;

        public DarkLabyrinthsGameStateController(ILogger logger, GameModel model, GameView view,
            PlayerSpawner playerSpawner, PlayerMovement playerMovement, InputReaderController inputReaderController,
            AudioSettingsBootstrapController audioSettingsBootstrapController,
            VictoryLevelVariation victoryLevelVariation, LoseLevelVariation loseLevelVariation,
            MazeGenerator mazeGenerator, TimerController timerController, IAudioService audioService) : base(logger)
        {
            _audioService = audioService;
            _timerController = timerController;
            _model = model;
            _view = view;
            _playerSpawner = playerSpawner;
            _playerMovement = playerMovement;
            _inputReaderController = inputReaderController;
            _audioSettingsBootstrapController = audioSettingsBootstrapController;
            _victoryLevelVariation = victoryLevelVariation;
            _loseLevelVariation = loseLevelVariation;
            _mazeGenerator = mazeGenerator;
        }

        public override async UniTask Enter(CancellationToken cancellationToken = default)
        {
            _hintPopup = _view.GetHintPopup();
            CreateCancellationToken();

            PlayGameMusic(_cancellationTokenSource.Token).Forget();

            SubscribeToViewEvents();

            _mazeGenerator.CreateHintPopupEvent += OnShowHintPopup;
            _mazeGenerator.LoadNextLevelEvent += OnLevelVariationRequested;
            _mazeGenerator.UsedPotionEvent += UsePotion;
            _timerController.TimerEndEvent += OnLevelVariationRequested;
            _victoryLevelVariation.FinishedLevel += OnLevelFinished;
            _loseLevelVariation.FinishedLevel += OnLevelFinished;
            await _view.ShowGameScreen(cancellationToken);
            await InitializeGameSystems(cancellationToken);

            _mazeGenerator.Config(_playerSpawner.GetPlayer().gameObject, _hintPopup);
            _model.LoadCurrentMaze();
        }

        public override async UniTask Exit()
        {
            _audioService.PlayMusic(ConstAudio.TitleMusic);
            
            ResetMaze();

            await CleanupGameSystems();
            UnsubscribeFromEvents();
            TakeUnPause();
            _view.HideUI();
            _isHintPopupShow = false;
            _gameCancellationTokenSource.Cancel();
            _cancellationTokenSource.Cancel();
        }

        private async UniTask PlayGameMusic(CancellationToken cancellationToken)
        {
            if (_audioService.IsPlaying(ConstAudio.TitleMusic))
                _audioService.StopMusic();
            
            while (cancellationToken.IsCancellationRequested == false)
            {
                if (_audioService.IsPlaying(ConstAudio.GameMusic) == false)
                    _audioService.PlayMusic(ConstAudio.GameMusic);

                await UniTask.Delay(TimeSpan.FromSeconds(0.2f), cancellationToken: cancellationToken);
            }
        }

        private void UsePotion(float time)
        {
            _timerController.AddTime(time);
        }

        private void RunTimerController()
        {
            if (_timerController.CurrentControllerState != ControllerState.Run)
                _timerController.Run(_gameCancellationTokenSource.Token).Forget();
        }

        private void ResetMaze()
        {
            _mazeGenerator.Reset();
        }

        private void SubscribeToViewEvents()
        {
            _view.HideHintPopupEvent += RunTimerController;
            _view.InitializeFireSliderEvent += InitializeFireSlider;
            _view.TakeUnPauseEvent += TakeUnPause;
            _view.PauseRequestedEvent += PauseRequestedEvent;
            _view.MenuRequestedEvent += MenuRequestedEvent;
            _view.RestartRequestedEvent += RestartRequestedEvent;
            _view.NextLevelRequestedEvent += NextLevelRequestedEvent;
            _view.ComingSoonRequestedEvent += ComingSoonRequestedEvent;
        }

        private void InitializeFireSlider(FireSlider fireSlider)
        {
            _timerController.SetSlider(fireSlider);
            _model.LoadTimerData();
        }

        private async UniTask InitializeGameSystems(CancellationToken cancellationToken)
        {
            await _audioSettingsBootstrapController.Stop();
            await _playerSpawner.Run(cancellationToken);
            await _playerMovement.Run(cancellationToken);
            await _inputReaderController.Run(cancellationToken);
        }

        private void OnLevelVariationRequested(GameResult gameResult)
        {
            if (gameResult == GameResult.Victory)
            {
                if (_victoryLevelVariation.CurrentControllerState != ControllerState.Run &&
                    _loseLevelVariation.CurrentControllerState != ControllerState.Run)
                    _victoryLevelVariation.Run(_gameCancellationTokenSource.Token);
            }
            else if (gameResult == GameResult.Lose)
            {
                if (_loseLevelVariation.CurrentControllerState != ControllerState.Run &&
                    _victoryLevelVariation.CurrentControllerState != ControllerState.Run)
                    _loseLevelVariation.Run(_gameCancellationTokenSource.Token);
            }
        }

        private void OnLevelFinished(GameResult result)
        {
            StopTimerController();
            _view.ShowFinishLevelPopup(result, _model.GetNextLevel());
            _model.SetPauseState(true);
        }

        private void StopTimerController()
        {
            if (_timerController.CurrentControllerState == ControllerState.Run)
            {
                _timerController.Stop();
                _view.ShowFireSlider(false);
            }
        }

        private void TakeUnPause()
        {
            _model.SetPauseState(false);
        }

        private async UniTask CleanupGameSystems()
        {
            await _audioSettingsBootstrapController.Stop();
            await _playerSpawner.Stop();
            await _playerMovement.Stop();
            await _inputReaderController.Stop();
            StopGameResults();

            StopTimerController();
        }

        private void StopGameResults()
        {
            if (_loseLevelVariation.CurrentControllerState == ControllerState.Run)
                _loseLevelVariation.Stop();

            if (_victoryLevelVariation.CurrentControllerState == ControllerState.Run)
                _victoryLevelVariation.Stop();
        }

        private void UnsubscribeFromEvents()
        {
            _mazeGenerator.CreateHintPopupEvent -= OnShowHintPopup;
            _mazeGenerator.LoadNextLevelEvent -= OnLevelVariationRequested;
            _mazeGenerator.UsedPotionEvent -= UsePotion;
            _timerController.TimerEndEvent -= OnLevelVariationRequested;
            _victoryLevelVariation.FinishedLevel -= OnLevelFinished;
            _loseLevelVariation.FinishedLevel -= OnLevelFinished;

            _view.HideHintPopupEvent -= RunTimerController;
            _view.InitializeFireSliderEvent -= InitializeFireSlider;
            _view.TakeUnPauseEvent -= TakeUnPause;
            _view.PauseRequestedEvent -= PauseRequestedEvent;
            _view.MenuRequestedEvent -= MenuRequestedEvent;
            _view.RestartRequestedEvent -= RestartRequestedEvent;
            _view.NextLevelRequestedEvent -= NextLevelRequestedEvent;
            _view.ComingSoonRequestedEvent -= ComingSoonRequestedEvent;
        }

        private void CreateCancellationToken()
        {
            _gameCancellationTokenSource = new CancellationTokenSource();
            _cancellationTokenSource = new CancellationTokenSource();
        }

        private void PauseRequestedEvent()
        {
            var newPauseState = !_model.IsPaused;
            _model.SetPauseState(newPauseState);

            _view.ShowPausePopup();
            _view.ResumeSounds();
        }

        private void MenuRequestedEvent()
        {
            _view.StopAllAudio();
            TakeUnPause();
            GoTo<MenuStateController>().Forget();
        }

        private void ComingSoonRequestedEvent()
        {
            _view.StopAllAudio();
            TakeUnPause();
            GoTo<ComingSoonStateController>();
        }

        private void RestartRequestedEvent()
        {
            TakeUnPause();
            ResetMaze();
            StopTimerController();
            _timerController.RestartTimer();
            OnConfigureMazeGenerator();
            _model.LoadTimerData();
            StopGameResults();
        }

        private void NextLevelRequestedEvent()
        {
            TakeUnPause();
            ResetMaze();
            StopTimerController();
            _model.IncrementLevel();
            OnConfigureMazeGenerator();
            _model.LoadTimerData();
            StopGameResults();
        }

        private void OnConfigureMazeGenerator()
        {
            if (_hintPopup is not null)
            {
                _mazeGenerator.Config(_playerSpawner.GetPlayer().gameObject, _hintPopup);
                _model.LoadCurrentMaze();
            }
        }

        private void OnShowHintPopup()
        {
            if (_isHintPopupShow)
                _gameCancellationTokenSource.Cancel();

            TakeUnPause();
            _gameCancellationTokenSource = new CancellationTokenSource();
            _view.ShowHintPopup(_gameCancellationTokenSource.Token);
            _isHintPopupShow = true;
        }
    }
}