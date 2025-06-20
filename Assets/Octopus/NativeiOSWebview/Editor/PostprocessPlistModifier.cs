#if UNITY_IOS
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using System.IO;

public static class PostprocessPlistModifier
{
    [PostProcessBuild]
    public static void OnPostProcessBuild(BuildTarget buildTarget, string pathToBuiltProject)
    {
        if (buildTarget != BuildTarget.iOS) return;

        string plistPath = Path.Combine(pathToBuiltProject, "Info.plist");
        PlistDocument plist = new PlistDocument();
        plist.ReadFromFile(plistPath);

        PlistElementDict rootDict = plist.root;

        // Додай LSApplicationQueriesSchemes
        const string key = "LSApplicationQueriesSchemes";
        string[] schemes = new string[]
        {
            "tel", 
            "mailto", 
            "sms", 
            "ethereum", 
            "bitcoin", 
            "ripple", 
            "litecoin", 
            "bitcoincash", 
            "fb", 
            "viber"
        };

        PlistElementArray queriesSchemes;
        if (rootDict.values.ContainsKey(key))
        {
            queriesSchemes = rootDict[key].AsArray();
        }
        else
        {
            queriesSchemes = rootDict.CreateArray(key);
        }

        foreach (string scheme in schemes)
        {
            bool alreadyExists = false;
            foreach (var el in queriesSchemes.values)
            {
                if (el.AsString() == scheme)
                {
                    alreadyExists = true;
                    break;
                }
            }
            if (!alreadyExists)
            {
                queriesSchemes.AddString(scheme);
            }
        }

        // Запис назад у файл
        File.WriteAllText(plistPath, plist.WriteToString());
    }
}
#endif
