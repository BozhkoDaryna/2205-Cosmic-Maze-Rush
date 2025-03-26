using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Core.UI
{
    public class ComingSoonPopup : BasePopup
    {
        [SerializeField] private Button _cancelButton;

        public event Action CancelledEvent;

        public override UniTask Show(BasePopupData data, CancellationToken cancellationToken = default)
        {
            _cancelButton.onClick.AddListener(OnCancelled);

            return base.Show(data, cancellationToken);
        }

        private void OnCancelled()
        {
            CancelledEvent?.Invoke();
            _cancelButton.onClick.RemoveListener(OnCancelled);
            DestroyPopup();    
        }
    }
}