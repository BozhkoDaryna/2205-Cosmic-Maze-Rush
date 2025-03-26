using UnityEngine;

namespace Core.Services.ScreenOrientation
{
    [CreateAssetMenu(fileName = "ScreenOrientationConfig", menuName = "Config/ScreenOrientationConfig")]
    public sealed class ScreenOrientationConfig : BaseSettings
    {
        public UnityEngine.ScreenOrientation AllowedScreenMode;
        public bool EnableScreenOrientationPopup;
    }
}