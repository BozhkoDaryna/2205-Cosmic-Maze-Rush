using UnityEngine;
using UnityEngine.UI;

namespace Core.UI
{
    public class CancelPopup : BasePopup
    {
        [SerializeField] protected Button _cancelButton;

        protected virtual void OnEnable()
        {
            _cancelButton.onClick.AddListener(DestroyPopup);
        }

        protected virtual void OnDisable()
        {
            _cancelButton.onClick.RemoveListener(DestroyPopup);
        }
    }
}