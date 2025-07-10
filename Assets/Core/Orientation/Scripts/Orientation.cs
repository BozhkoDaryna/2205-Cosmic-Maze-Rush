using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Orientation : MonoBehaviour
{
    private static Orientation _instance = null;
    
    [SerializeField] private GameObject _hint;

    [SerializeField] private Button _fade;
    
    [SerializeField] private bool _isFadeClickable = false;
    
    [SerializeField] private bool _portrait = false;
    
    [SerializeField] private bool _landscape = true;

    [SerializeField] private bool _autoHide = true;
    
    [SerializeField] private float _hideDelay = 2;
    
    private static bool isCompleted;
    
    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
            
            return;
        }

        _instance = this;
        
        DontDestroyOnLoad(gameObject);
        
        if(_isFadeClickable)
            _fade.onClick.AddListener(HideHint);
        
        if(!isCompleted)
            ShowHint();
    }

    private void ShowHint()
    {
        _hint.SetActive(true);
        
        isCompleted = true;

        SetOrientation(_portrait, _landscape);
    }
    
    private void HideHint()
    {
        if (_autoHide)
        {
            var tween = DOVirtual.DelayedCall(_hideDelay, () =>
            {
                _hint.SetActive(false);
            }).Play();
        }
        else
        {
            _hint.SetActive(false);
        }
    }
    
    private void SetOrientation(bool portrait,bool landscape)
    {
        Screen.autorotateToPortrait = portrait;
        Screen.autorotateToPortraitUpsideDown = portrait;
        Screen.autorotateToLandscapeLeft = landscape;
        Screen.autorotateToLandscapeRight = landscape;
        
        Screen.orientation = ScreenOrientation.AutoRotation;
    }
    
    private void Update()
    {
        if(!_hint.activeSelf)
            return;
        
        if(_landscape)
        {
            if (Screen.orientation == ScreenOrientation.LandscapeLeft ||
                Screen.orientation == ScreenOrientation.LandscapeRight)
            {
                HideHint();
            }
        }
    }
}
