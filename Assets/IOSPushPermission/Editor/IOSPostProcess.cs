
#if UNITY_IOS
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using System.IO;

public static class IOSPostProcess
{
    [PostProcessBuild]
    public static void OnPostProcessBuild(BuildTarget target, string pathToBuiltProject)
    {
        if (target != BuildTarget.iOS) return;

        var plistPath = Path.Combine(pathToBuiltProject, "Info.plist");
        var plist = new PlistDocument();
        plist.ReadFromFile(plistPath);
        var rootDict = plist.root;

        rootDict.SetString("NSUserNotificationUsageDescription", "Ми хочемо надсилати вам важливі сповіщення.");
        plist.WriteToFile(plistPath);
    }
}
#endif
