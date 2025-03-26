using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Core.UI
{
    public class HintPopup : CancelPopup
    {
        private readonly List<GameObject> _arrows = new List<GameObject>();

        [SerializeField] private TextMeshProUGUI _timerText;
        [SerializeField] private RectTransform _context;
        [SerializeField] private int _timeToFade;

        public event Action HideHintPopupEvent;

        protected override void OnEnable()
        {
            _cancelButton.onClick.AddListener(OnHideHintPopup);
        }

        protected override void OnDisable()
        {
            _cancelButton.onClick.RemoveListener(OnHideHintPopup);
        }

        public override async UniTask Show(BasePopupData data, CancellationToken cancellationToken = default)
        {
            if (_arrows != null)
            {
                foreach (var arrow in _arrows)
                {
                    if (arrow != null)
                    {
                        Destroy(arrow);
                    }
                }

                _arrows.Clear();
            }

            base.Show(data, cancellationToken);

            float remainingTime = _timeToFade;
            while (remainingTime > 0)
            {
                if (cancellationToken.IsCancellationRequested)
                    return;

                _timerText.text = remainingTime.ToString("F1");
                await UniTask.Delay(TimeSpan.FromSeconds(0.1f), cancellationToken: cancellationToken);
                remainingTime -= 0.1f;
            }

            _timerText.text = "0.0";

            if (cancellationToken.IsCancellationRequested)
                return;

            OnHideHintPopup();
        }

        public void AddArrow(GameObject arrowPrefab, float angle, string arrowName)
        {
            var newArrow = Instantiate(arrowPrefab, Vector3.zero, Quaternion.Euler(0, 0, angle), _context);
            newArrow.name = arrowName;
            _arrows.Add(newArrow);
        }

        private void OnHideHintPopup()
        {
            HideHintPopupEvent?.Invoke();

            Hide();
        }
    }
}