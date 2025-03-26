using System;
using Core.UI;

namespace Application.UI
{
    public class InfoPopup : CancelPopup
    {
        public new event Action<ButtonTypes> DestroyPopupEvent;

        public override void DestroyPopup()
        {
            DestroyPopupEvent?.Invoke(ButtonTypes.Info);

            base.DestroyPopup();
        }
    }
}