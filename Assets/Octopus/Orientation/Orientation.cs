using UnityEngine;

namespace Orientation
{
    public class Orientation : MonoBehaviour
    {
        public enum OrientationMode
        {
            Portrait,
            Landscape,
            Both
        }

        [SerializeField]
        private OrientationMode orientationMode = OrientationMode.Both;

        protected void Awake()
        {
            SetAutoRotate();
        }

        private void SetAutoRotate()
        {
            switch (orientationMode)
            {
                case OrientationMode.Portrait:
                    Screen.autorotateToPortrait = true;
                    Screen.autorotateToPortraitUpsideDown = true;
                    Screen.autorotateToLandscapeLeft = false;
                    Screen.autorotateToLandscapeRight = false;
                    break;

                case OrientationMode.Landscape:
                    Screen.autorotateToPortrait = false;
                    Screen.autorotateToPortraitUpsideDown = false;
                    Screen.autorotateToLandscapeLeft = true;
                    Screen.autorotateToLandscapeRight = true;
                    break;

                case OrientationMode.Both:
                    Screen.autorotateToPortrait = true;
                    Screen.autorotateToPortraitUpsideDown = true;
                    Screen.autorotateToLandscapeLeft = true;
                    Screen.autorotateToLandscapeRight = true;
                    break;
            }

            Screen.orientation = ScreenOrientation.AutoRotation;
        }
    }
}