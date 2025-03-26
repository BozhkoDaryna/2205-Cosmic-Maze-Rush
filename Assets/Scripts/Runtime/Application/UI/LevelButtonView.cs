using System;
using TMPro;
using UnityEngine;

namespace Application.UI
{
    public class LevelButtonView : SimpleButton
    {
        [SerializeField] private TextMeshProUGUI _buttonText;

        public event Action HandleClickEvent;

        public void Initialize()
        {
            _button.onClick.AddListener(HandleClick);
        }

        public void Cleanup()
        {
            _button.onClick.RemoveListener(HandleClick);
        }

        public void UpdateText(string text)
        {
            _buttonText.text = text;
        }

        private void HandleClick()
        {
            HandleClickEvent?.Invoke();
        }
    }
}