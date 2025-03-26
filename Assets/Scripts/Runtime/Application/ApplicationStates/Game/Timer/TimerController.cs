using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.EndLevelVariations;
using Application.Services.Audio;
using Application.Services.UserData;
using Core;
using Core.Services.Audio;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Runtime.Game
{
    public class TimerController : BaseController, ITickable
    {
        private const float WarningThreshold = 0.3f;

        private readonly List<IFireControl> _fireControls = new List<IFireControl>();
        private readonly IAudioService _audioService;

        private float _currentTime;
        private bool _isBroken;
        private TimerData _timerData;
        private FireSlider _fireSlider;
        private CancellationTokenSource _cancellationToken;

        public Action<GameResult> TimerEndEvent;

        public TimerController(IAudioService audioService, params IFireControl[] fireControls)
        {
            _audioService = audioService;
            _fireControls.AddRange(fireControls);
        }

        public override UniTask Run(CancellationToken cancellationToken)
        {
            base.Run(cancellationToken);
            _isBroken = false;
            _cancellationToken = new CancellationTokenSource();

            PlayFlameBurningSound(_cancellationToken.Token).Forget();

            _fireSlider.gameObject.SetActive(true);

            if (_timerData.StartAtRuntime)
                StartTimer();

            return UniTask.CompletedTask;
        }

        public override UniTask Stop()
        {
            _cancellationToken.Cancel();
            _isBroken = false;
            return base.Stop();
        }

        public void Tick()
        {
            if (CurrentState == ControllerState.Run && _cancellationToken.IsCancellationRequested == false)
            {
                CountDown();

                UpdateSlider();
            }
        }

        private async UniTask PlayFlameBurningSound(CancellationToken cancellationToken)
        {
            var timeToEndOfClip = _audioService.GetClipLength(ConstAudio.FlameBurningSound);

            while (cancellationToken.IsCancellationRequested == false)
            {
                _audioService.PlaySound(ConstAudio.FlameBurningSound);

                await UniTask.Delay(TimeSpan.FromSeconds(timeToEndOfClip), cancellationToken: cancellationToken);
            }
        }

        public void AddTime(float time)
        {
            _currentTime = Mathf.Clamp(_currentTime + time, 0, _timerData.Duration);
        }

        public void SetTimerData(TimerData timerData)
        {
            _timerData = timerData;
            _currentTime = _timerData.Duration;
        }

        public void SetSlider(FireSlider fireSlider)
        {
            _fireSlider = fireSlider;
        }

        private void UpdateSlider()
        {
            var percentComplete = GetPercentComplete();
            _fireSlider.SetTimeInPercents(percentComplete);

            var shouldShowWarning = percentComplete <= WarningThreshold;

            SetWarning(shouldShowWarning);

            foreach (var fireControl in _fireControls)
            {
                fireControl.SetTime(percentComplete);
                fireControl.PerformAction();
            }

            if (percentComplete <= 0 && _isBroken == false)
                PlayBreakingSound();
        }

        private void PlayBreakingSound()
        {
            _isBroken = true;
            _audioService.PlaySound(ConstAudio.TorchBreakingSound);
        }

        private void SetWarning(bool shouldShowWarning)
        {
            if (shouldShowWarning)
            {
                _audioService.PlaySound(ConstAudio.FlameDangerSound, 0.3f);
            }

            _fireSlider.ShowHurryUpText(shouldShowWarning);
        }

        private float GetPercentComplete()
        {
            return _currentTime / _timerData.Duration;
        }

        private void CountDown()
        {
            if (_currentTime > 0)
            {
                _currentTime -= Time.deltaTime;
                if (_currentTime <= 0)
                    TimerComplete();
            }
        }

        private void TimerComplete()
        {
            _currentTime = 0;
            TimerEndEvent?.Invoke(GameResult.Lose);
        }

        private void StartTimer()
        {
            _fireSlider.ShowHurryUpText(false);
            _currentTime = _timerData.Duration;
            UpdateSlider();
        }

        private void StopTimer()
        {
            _currentTime = _timerData.Duration;
            UpdateSlider();
        }

        public void RestartTimer()
        {
            StopTimer();
            StartTimer();
        }
    }
}