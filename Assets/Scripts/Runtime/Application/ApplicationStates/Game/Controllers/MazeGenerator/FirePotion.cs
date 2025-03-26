using UnityEngine;

namespace Runtime.MazeGenerator
{
    public class FirePotion : TriggerDetector
    {
        protected override void OnTriggerEnter2D(Collider2D other)
        {
            base.OnTriggerEnter2D(other);
            
            Destroy(gameObject);
        }
    }
}