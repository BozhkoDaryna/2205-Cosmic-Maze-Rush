using Runtime.Game;
using UnityEngine;
using Zenject;

namespace Application.Game
{
    public class PlayerConfig : MonoBehaviour
    {
        [SerializeField] private GameObject _light;
        [SerializeField] private SpriteRenderer _flameSpriteRenderer;
        [SerializeField] private Animator _animator;

        private CharacterAnimationController _characterAnimationController;
        private FireSpriteSwitcher _fireSpriteSwitcher;
        private LightScaler _lightScaler;

        public Transform PlayerTransform { get; private set; }

        [Inject]
        private void Construct(CharacterAnimationController characterAnimationController, LightScaler lightScaler,
            FireSpriteSwitcher fireSpriteSwitcher)
        {
            _lightScaler = lightScaler;
            _fireSpriteSwitcher = fireSpriteSwitcher;
            _characterAnimationController = characterAnimationController;
        }

        private void Awake()
        {
            InitializeComponents();

            SetAnimator();

            SetLight();
        }

        private void SetLight()
        {
            _fireSpriteSwitcher.SetFireGameObject(_flameSpriteRenderer);
            _lightScaler.SetLightGameObject(_light);
        }

        private void InitializeComponents()
        {
            _animator = GetComponent<Animator>();
            PlayerTransform = GetComponent<Transform>();
        }

        private void SetAnimator()
        {
            _characterAnimationController.SetAnimator(_animator);
        }
    }
}