using Core;
using Octopus.Client;
using Octopus.VerifyInternet;
using UnityEngine;
using System.Runtime.InteropServices;

public class NativeWebView : MonoBehaviour
{
#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void showWebView(string url, string callbackObject);

    [DllImport("__Internal")]
    private static extern void closeWebView();
#else
    private static extern void showWebView(string url, string callbackObject);
    private static extern void closeWebView();
#endif
    
    public void ShowWeb(string url)
    {
#if UNITY_IOS && !UNITY_EDITOR
        showWebView(url, gameObject.name);
#endif
    }

    public void CloseWeb()
    {
#if UNITY_IOS && !UNITY_EDITOR
        closeWebView();
#endif
    }

    // Callback з Swift
    public void OnWebViewContentLoaded()
    {
        Debug.Log("✅ Контент WebView завантажено");
    }
    
    private string UrlB
    {
        get
        { 
            //return "https://fatelioa.fun/testsetstse.json/";
            //return "https://balloonswinner.life/privacypolicy/?3ve1kbqs8h=e9fecaf8-1376-4017-9115-e95a08119218&huax91ae4p=0&6x13a9aqh2=cmpgn=trident-dev-test_TEST-Deeplink_test1_%D1%82%D0%B5%D1%81%D1%822_TEST3_%D0%A2%D0%95%D0%A1%D0%A24_s%20p%20a%20c%20e";//✔
            //return "https://betking.com.ua/";//✔
            //return "https://winboss.ua";//✔
            //return "http://www.http2demo.io/";//✔
            //return "https://slotscity.ua/";//✔
            //return "https://www.whatismybrowser.com/detect/are-third-party-cookies-enabled/";//✔
            //return "https://betoholictrack.com/wKWSmlPF?sub_id=23uejou1gt25f"; //✔

            if(!GameSettings.HasKey(Constants.IsFirstRunWebView)) // 1 запуск
            {
                GameSettings.SetFirstWebView();
                
                //return GameSettings.GetValue(Constants.ReceiveUrl, "");
                return GameSettings.GetValue(Constants.SecondRedirectUrl, "");
            }
            else
            {
                var url = GameSettings.GetValue(Constants.StartUrl, "");
                
                if(!GameSettings.HasKey(Constants.StartUrl))
                    return GameSettings.GetValue(Constants.ReceiveUrl, "");
                
                if (!GameSettings.HasKey(Constants.LastUrl))
                {
                    GameSettings.SetValue(Constants.LastUrl, url);
                }
                else
                {
                    //Only start offer url
                    //url = GameSettings.GetValue(Constants.LastUrl, "");
                }

                return url;
            }
        }
        set 
        {
            if(!GameSettings.HasKey(Constants.StartUrl))
            {
                GameSettings.SetValue(Constants.StartUrl, value);
            }
            
            GameSettings.SetValue(Constants.LastUrl, value);
        }
    }
    
    private void Start()
    {
        InitializeWebView();
    }

    private void OnInitialize(bool? isConnection)
    {
        PrintMessage("### OnInitialize");
        
        CheckConnection(isConnection);
    }
    
    private void CheckConnection(bool? isConnection)
    {
        PrintMessage($"### CheckConnection: isConnection={isConnection}");
        
        if (isConnection != true) return;
        
        if(ConnectivityManager.Instance)
            ConnectivityManager.Instance.OnChangedInternetConnection.AddListener(OnInitialize);
            
        InitializeWebView();
    }

    private void InitializeWebView()
    {
        PrintMessage("### Initialize Webview");

        //createWebView(0, 0, Screen.width, Screen.height);
        
        //loadUrl(UrlB);
        
        ShowWeb(UrlB);
    }

    public void CloseWebView()
    {
        //removeWebView();
    }

    public void RunJavaScript(string js)
    {
        //evaluateJS(js);
    }
    
    private void PrintMessage(string message)
    {
        Debugger.Log($"@@@ WebViewController ->: {message}", new Color(0.2f, 0.9f, 0.2f));
    }
}