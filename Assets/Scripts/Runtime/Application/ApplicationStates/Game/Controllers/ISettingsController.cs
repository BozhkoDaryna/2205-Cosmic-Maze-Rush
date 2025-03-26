namespace Application.Game
{
    public interface ISettingsController
    {
        void OnChangeSoundVolume(bool isEnabled);
        void OnChangeMusicVolume(bool isEnabled);
    }
}