#if UNITY_IOS
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using System.IO;

public class PostBuildiOSPlistModifier
{
    //[PostProcessBuild]
    public static void OnPostProcessBuild(BuildTarget buildTarget, string pathToBuiltProject)
    {
        if (buildTarget != BuildTarget.iOS)
            return;

        string plistPath = Path.Combine(pathToBuiltProject, "Info.plist");
        PlistDocument plist = new PlistDocument();
        plist.ReadFromFile(plistPath);

        PlistElementDict rootDict = plist.root;

        // Додаємо UIBackgroundModes
        PlistElementArray bgModes;
        if (rootDict.values.ContainsKey("UIBackgroundModes"))
        {
            bgModes = rootDict["UIBackgroundModes"].AsArray();
        }
        else
        {
            bgModes = rootDict.CreateArray("UIBackgroundModes");
        }

        // Уникнути дублю
        void AddIfMissing(string value)
        {
            foreach (var elem in bgModes.values)
            {
                if (elem.AsString() == value) return;
            }
            bgModes.AddString(value);
        }

        //AddIfMissing("remote-notification"); // Підтримка push
        //AddIfMissing("audio"); // Якщо потрібно ще й аудіо в фоні

        plist.WriteToFile(plistPath);
    }
}
#endif