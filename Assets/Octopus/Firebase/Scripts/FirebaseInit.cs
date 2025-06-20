using Firebase;
using Firebase.Messaging;
using UnityEngine;

public class FirebaseInitializer : MonoBehaviour
{
    void Start()
    {
        // Ініціалізація Firebase
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            FirebaseApp app = FirebaseApp.DefaultInstance;
            InitializePushNotifications();
        });
    }

    void InitializePushNotifications()
    {
        FirebaseMessaging.TokenReceived += OnTokenReceived;
        FirebaseMessaging.MessageReceived += OnMessageReceived;

        // Запит на отримання токену push-повідомлень
        FirebaseMessaging.GetTokenAsync().ContinueWith(task => {
            if (task.IsCompleted)
            {
                Debug.Log("FCM Token: " + task.Result);
            }
        });
    }

    // Обробка отриманого токену
    void OnTokenReceived(object sender, TokenReceivedEventArgs token)
    {
        Debug.Log("Received FCM Token: " + token.Token);
    }

    // Обробка отриманого повідомлення
    void OnMessageReceived(object sender, MessageReceivedEventArgs e)
    {
        Debug.Log("Message received: " + e.Message.From);
    }
}