using System;

namespace Application.Services.UserData
{
    [Serializable]
    public class TorchData
    {
        public int InverseTo;
        public int InverseFromWhenLost;
        public float InverseFromWhenNotLost;
        public float Multiplicator;
        public float PulsateSpeed;
        public float PulsateAmount;
    }
}