using Core.UI;

namespace Application.Game
{
    public interface IConfigPopup<in TPopup, out TData> where TPopup : BasePopup where TData : BasePopupData
    {
        TData GetConfig(TPopup popup);
    }
}