using System;
using UnityEngine;

namespace Runtime.MazeGenerator
{
    public class TriggerDetector : MonoBehaviour
    {
        public event Action DetectedEvent;

        protected virtual void OnTriggerEnter2D(Collider2D other)
        {
            DetectedEvent?.Invoke();
        }
    }
}