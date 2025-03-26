using System;
using Runtime.Game;

namespace Application.Services.UserData
{
    [Serializable] 
    public class TimerData
    {
        public float Duration;
        public bool StartAtRuntime = true;
    }
}