using System;
using System.Threading;
using Core.Services.Audio;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

namespace Core.UI
{
    public class BasePopup : MonoBehaviour
    {
        private const string OpenPopupSound = "OpenPopup";
        private const string ClosePopupSound = "ClosePopupSound";

        [SerializeField] protected string _id;

        protected IAudioService AudioService;

        public UnityEvent ShowEvent;
        public UnityEvent HideEvent;
        public UnityEvent HideImmediatelyEvent;

        public event Action DestroyPopupEvent;

        public string Id => _id;

        [Inject]
        public void Construct(IAudioService audioService)
        {
            AudioService = audioService;
        }

        public virtual UniTask Show(BasePopupData data, CancellationToken cancellationToken = default)
        {
            AudioService.PlaySound(OpenPopupSound);
            ShowEvent?.Invoke();
            return UniTask.CompletedTask;
        }
        
        public virtual void Hide()
        {
            HideEvent?.Invoke();
        }

        public virtual void HideImmediately()
        {
            HideImmediatelyEvent?.Invoke();
        }

        public virtual void DestroyPopup()
        {
            DestroyPopupEvent?.Invoke();
            AudioService.PlaySound(ClosePopupSound);
            Destroy(gameObject);
        }
    }
}