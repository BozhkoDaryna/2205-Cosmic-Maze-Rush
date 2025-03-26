using System.Collections.Generic;
using UnityEngine;

namespace Runtime.Game
{
    public class FireSpriteSwitcher : IFireControl
    {
        private readonly List<Sprite> _sprites;

        private SpriteRenderer _light;
        private int _spriteIndex;

        public FireSpriteSwitcher(List<Sprite> sprites)
        {
            _sprites = sprites;
        }

        public void SetTime(float time)
        {
            if (time > 0.9f)
                _spriteIndex = 0;

            if (time > 0.8f && time < 0.85f)
                _spriteIndex = 1;

            if (time > 0.6f && time < 0.8f)
                _spriteIndex = 2;

            if (time > 0.4f && time < 0.6f)
                _spriteIndex = 3;

            if (time > 0.2f && time < 0.4f)
                _spriteIndex = 4;

            if (time < 0.2f)
                _spriteIndex = 5;

            BreakLight(time != 0);
        }

        public void PerformAction()
        {
            if (_spriteIndex >= 0 && _spriteIndex < _sprites.Count)
                _light.sprite = _sprites[_spriteIndex];
        }

        public void SetFireGameObject(SpriteRenderer lightSpriteRenderer)
        {
            _light = lightSpriteRenderer;
            _light.gameObject.SetActive(true);
        }

        private void BreakLight(bool isBreak)
        {
            _light.gameObject.SetActive(isBreak);
        }
    }
}