using Application.Services.UserData;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Runtime.Game
{
    public class LightScaler : IFireControl
    {
        private readonly TorchData _torchData;
        
        private GameObject _light;
        private float _time;
        private float _lightScale;
        private float _pulseTime;
        private bool _isLost;

        public LightScaler(TorchData torchData)
        {
            _torchData = torchData;
        }

        public void SetLightGameObject(GameObject light)
        {
            _light = light;
        }

        public void SetTime(float time)
        {
            _time = time;
            _isLost = _time <= 0;
        }

        public void PerformAction()
        {
            var scaleFactor = DeterminateScale();
            SetScale(scaleFactor);
        }

        public async UniTask WaitUntilLightRemoved()
        {
            await UniTask.WaitUntil(IsRemovedLight);
        }

        private bool IsRemovedLight()
        {
            return _light.transform.localScale.Equals(Vector2.zero);
        }

        private void SetScale(float scaleFactor)
        {
            _lightScale = scaleFactor;
            _light.transform.localScale = new Vector2(scaleFactor, scaleFactor);
        }

        private float DeterminateScale()
        {
            _pulseTime += Time.deltaTime * _torchData.PulsateSpeed;

            if (_isLost)
            {
                _lightScale = Mathf.Max(0, _lightScale - Time.deltaTime);
                return SetScaleFactor(_torchData.InverseFromWhenLost, _torchData.InverseTo, _lightScale);
            }

            var baseScale = SetScaleFactor(_torchData.InverseFromWhenNotLost, _torchData.InverseTo, _time *= _torchData.Multiplicator);

            var pulse = Mathf.PingPong(_pulseTime, _torchData.PulsateAmount);
            var finalScale = baseScale + pulse;
            
            return Mathf.Clamp(finalScale, _torchData.InverseFromWhenNotLost, _torchData.InverseTo);
        }

        private float SetScaleFactor(float inverseFrom, float inverseTo, float time)
        {
            var scaleFactor = Mathf.InverseLerp(inverseFrom, inverseTo, time);
            scaleFactor = Mathf.Clamp(scaleFactor, inverseFrom, _torchData.InverseTo);
            return scaleFactor;
        }
    }
}