using System;
using Core.UI;

namespace Application.UI
{
    public class SimpleDecisionPopupData : BasePopupData
    {
        public Action PressOkEvent;
        public string Message;
    }
}