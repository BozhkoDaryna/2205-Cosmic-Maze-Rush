using UnityEngine;

namespace IOSPushPermission
{
    public class IOSPermissionRequester : MonoBehaviour
    {
        void Start()
        {
            // Запит на дозвіл на отримання push-повідомлень
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                Debug.Log("Запит на дозвіл на отримання push-повідомлень");
                //Firebase.Messaging.FirebaseMessaging.RequestPermissionAsync();
            }
        }
    }
}
