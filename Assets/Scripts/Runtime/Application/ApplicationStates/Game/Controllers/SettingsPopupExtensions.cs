using Application.UI;

namespace Application.Game
{
    public static class SettingsPopupExtensions
    {
        public static void SubscribeToEvents(this SettingsPopup popup, ISettingsController controller)
        {
            popup.SoundVolumeChangeEvent += controller.OnChangeSoundVolume;
            popup.MusicVolumeChangeEvent += controller.OnChangeMusicVolume;
        }

        public static void UnsubscribeFromEvents(this SettingsPopup popup, ISettingsController controller)
        {
            popup.SoundVolumeChangeEvent -= controller.OnChangeSoundVolume;
            popup.MusicVolumeChangeEvent -= controller.OnChangeMusicVolume;
        }
    }
}