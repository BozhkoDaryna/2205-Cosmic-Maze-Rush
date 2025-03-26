using UnityEngine;
using UnityEngine.UI;

namespace Runtime.Game
{
    public class FireSlider : MonoBehaviour
    {
        [SerializeField] private GameObject _hurryUpText;

        private Slider _slider;

        public Slider Slider
        {
            get => _slider;
            private set => _slider = value;
        }

        private void OnEnable()
        {
            Slider = GetComponent<Slider>();

            ShowHurryUpText(false);
        }

        public void SetTimeInPercents(float time)
        {
            if (Slider is not null)
                Slider.value = time;
        }
        
        public void ShowHurryUpText(bool isShow)
        {
            _hurryUpText.gameObject.SetActive(isShow);
        }
    }
}