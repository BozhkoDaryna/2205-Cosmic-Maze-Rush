using System;
using Application.Game;
using Runtime.Game;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Application.UI
{
    public class GameScreen : UiScreen
    {
        [SerializeField] private FireSlider _fireSlider;
        [SerializeField] private Button _pauseButton;

        public FireSlider FireSlider => _fireSlider;
        
        public event Action OnPaused;

        private void OnEnable()
        {
            _pauseButton.onClick.AddListener(Pause);
        }

        private void OnDisable()
        {
            _pauseButton.onClick.RemoveListener(Pause);
        }

        private void Pause()
        {
            OnPaused?.Invoke();
        }
    }
}