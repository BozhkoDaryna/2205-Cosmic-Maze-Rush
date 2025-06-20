using Permissions;
using UnityEngine;

public class NotificationPermission : MonoBehaviour
{
    private void Start()
    {
        PermissionManager.AskPermission("android.permission.POST_NOTIFICATIONS");
        
        //if (GetAndroidSDKVersion() >= 13)
        //{
        //    PermissionManager.AskPermission("android.permission.POST_NOTIFICATIONS");    
        //}
    }
    
    private int GetAndroidSDKVersion()
    {
        using var androidVersion = new AndroidJavaClass("android.os.Build$VERSION");
        return androidVersion.GetStatic<int>("SDK_INT");
        
    }
}
