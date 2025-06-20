using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using Core;
using Octopus.Client;
using Octopus.VerifyInternet;
using UnityEngine;
using System.Text.RegularExpressions;
using UnityEngine.Android;

public class WebViewController : MonoBehaviour
{
    [SerializeField, Header("Reference RectTransform")] private RectTransform _referenceRectTransform;
    
    [SerializeField, Header("Reload start page")] private bool canReload;
    
    private UniWebView _webView;

    private string _url;
    
    private bool _isVisible;

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

            if(!GameSettings.HasKey(Constants.IsFirstRunWebView))
            {
                GameSettings.SetFirstWebView();
                
                return GameSettings.GetValue(Constants.ReceiveUrl, "");
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

        CreateWebView();

        LoadWebView();
    }

    private void CreateWebView()
    {
        PrintMessage("### Create WebView");
        
        if (_webView != null)
            return;

        UniWebView.SetAllowAutoPlay(true);//+
        UniWebView.SetAllowInlinePlay(true);//+
        UniWebView.SetEnableKeyboardAvoidance(true);//+
        UniWebView.SetJavaScriptEnabled(true);//+
        
        UniWebView.SetAllowJavaScriptOpenWindow(true);//-
        //UniWebView.SetAllowUniversalAccessFromFileURLs(true);//-

        var webViewGameObject = new GameObject("UniWebView");
        _webView = webViewGameObject.AddComponent<UniWebView>();

        SetupWebview(_webView);

        SetUserAgent();

        RegisterShouldHandleRequest();

        SupportMultipleWindows();

        ShouldClose();

        SetFrame();

        Subscribe();
    }

    private void SetupWebview(UniWebView _view)
    {
        _webView.EmbeddedToolbar.SetPosition(UniWebViewToolbarPosition.Top);
        _webView.EmbeddedToolbar.SetDoneButtonText("\u2192");
        _webView.EmbeddedToolbar.SetGoBackButtonText("\u2190");
        _webView.EmbeddedToolbar.SetGoForwardButtonText("");
        _webView.EmbeddedToolbar.SetBackgroundColor(Color.black);
        _webView.EmbeddedToolbar.SetButtonTextColor(Color.white);
        _webView.EmbeddedToolbar.Hide();//+
        
        _webView.SetSupportMultipleWindows(true, true);//+
        _webView.SetAllowFileAccess(true);//+
        _webView.SetCalloutEnabled(true);//+
        _webView.SetBackButtonEnabled(true);//+
        _webView.SetAllowBackForwardNavigationGestures(true);//+
        _webView.SetAcceptThirdPartyCookies(true);//+
        
        //_webView.SetAllowFileAccessFromFileURLs(true);//-
        _webView.SetZoomEnabled(true);//-
        
        _webView.OnWebContentProcessTerminated += (view) =>
        {
            PrintMessage("🚨 WebView крашнувся — рестартуємо апку");

            _webView.Reload();
        };

        _webView.OnOrientationChanged += (view, orientation) =>
        {
            PrintMessage($"### 🛫 OnOrientationChanged");
            
            if (rotationCoroutine != null)
            {
                StopCoroutine(rotationCoroutine);
            }

            rotationCoroutine = StartCoroutine(DelayedAdjust(view));
        };
        
        _webView.RegisterOnRequestMediaCapturePermission((permission) =>
        {
            PrintMessage($"### 📸 RegisterOnRequestMediaCapturePermission: request={string.Join(", ", permission.Resources)}");

            PrintMessage($"### 📸 RegisterOnRequestMediaCapturePermission: request={permission.Host}");
            
            var expected = "VIDEO";

            if (permission.Resources.Contains(expected))
            {
                if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
                { 
                    Permission.RequestUserPermission(Permission.Camera);

                    return UniWebViewMediaCapturePermissionDecision.Prompt;
                }
                 
                return UniWebViewMediaCapturePermissionDecision.Grant;
            }
            
            return UniWebViewMediaCapturePermissionDecision.Grant;
        });
        
        _webView.AddUrlScheme("paytmmp");//+
        _webView.AddUrlScheme("phonepe");//+
        _webView.AddUrlScheme("bankid");//+
        _webView.AddUrlScheme("playtoupi");//+
        _webView.AddUrlScheme("mobiw");//+
        _webView.AddUrlScheme("upi");//+
        
        _webView.OnMessageReceived += (v, message) => 
        {
            PrintMessage($"@@@ ⁉️ OnMessageReceived: message={message.RawMessage}");
            var url = message.RawMessage;
            Application.OpenURL(url);
        };
    }
    
    private void ShouldClose()
    {
        _webView.OnShouldClose += (view) =>
        {
            PrintMessage($"@@@ ⏪ OnShouldClose: url = {view.Url}");

            if (_webView.CanGoForward)
            {
                _webView.GoForward();
            }
            
            return false;
            
            if (GameSettings.GetValue(Constants.LastUrl) != GameSettings.GetValue(Constants.StartUrl))
            {
                _webView.Load(GameSettings.GetValue(Constants.StartUrl));
            }
            else
            {
                if(canReload)
                {
                    _webView.Reload();
                }
            }

            return false;
        };
    }
    
    public void SetFrame()
    {
        _webView.Frame = FlipRectY(Screen.safeArea);
    }

    private void RegisterShouldHandleRequest()
    {
        _webView.RegisterShouldHandleRequest(request => {

            PrintMessage($"@@@ 👁️RegisterShouldHandleRequest: request.Url={request.Url}");
            
            CultureInfo ci = new CultureInfo("en-US");
            
            string[] allowedPrefixes = { "http", "about:blank", "intent", "about:srcdoc" };

            if (!allowedPrefixes.Any(prefix => request.Url.StartsWith(prefix, true, ci)))
            {
                PrintMessage($"⁉️ ️RegisterShouldHandleRequest");
            
                Application.OpenURL(request.Url);
            
                return false;
            }

            if (!IsBlockedUrl(request.Url)) return true;
            
            PrintMessage($"### 🔒Blocked download files: {request.Url}");
            
            _webView.GoBack();
                
            return false;
        });
    }

    private void SupportMultipleWindows()
    {
        _webView.OnMultipleWindowOpened += (view, windowId) => {
            PrintMessage($"📫 @@@ OnMultipleWindowOpened");
            PrintMessage($"        view.Url {view.Url}");
            PrintMessage($"        A new window with identifier '{windowId}' is opened");

            _webView.EmbeddedToolbar.Show();
            
            if (rotationCoroutine != null)
            {
                StopCoroutine(rotationCoroutine);
            }
            
            rotationCoroutine = StartCoroutine(DelayedAdjust(view));
            
            view.ScrollTo(0, 0, false);
            
            //_swipeGameObject = new GameObject("swipeGameObject");
            //var swipeHandler = _swipeGameObject.AddComponent<SmoothSwipeNavigation>();
            //swipeHandler.webView = view;
        };
        
        _webView.OnMultipleWindowClosed += (view, windowId) => {
            PrintMessage($"📪 @@@ OnMultipleWindowClosed");
            PrintMessage($"        view.Url {view.Url}");
            PrintMessage($"        A new window with identifier '{windowId}' is closed");
            
            _webView.EmbeddedToolbar.Hide();
            
            //if (_swipeGameObject != null)
            //{
                //Destroy(_swipeGameObject);
            //}
        };
    }
    
    private Coroutine rotationCoroutine;
    private IEnumerator DelayedAdjust(UniWebView webView)
    {
        //yield return new WaitForSeconds(0.33f);
        
        yield return new WaitForEndOfFrame(); 
        
        PrintMessage($"🪃 @@@ DelayedAdjust");
        
        SetFrame();
      
        //webView.Frame = FlipRectY(Screen.safeArea);
    }

    private static Rect FlipRectY(Rect rect)
    {
        return new Rect(rect.x, Screen.height - rect.yMax, rect.width, rect.height);
    }
    
    private void Subscribe()
    {
        PrintMessage($"📥Subscribe");
        
        _webView.OnPageStarted += OnPageStarted;
            
        _webView.OnPageFinished += OnPageFinished;

        _webView.OnLoadingErrorReceived += OnLoadingErrorReceived;
    }

    private void SetUserAgent()
    {
        var agent = _webView.GetUserAgent();

        // 1. Очистка агенту (твоя функція)
        agent = ClearAgent(agent);

        // 2. Видалити стару версію Safari (якщо є)
        agent = Regex.Replace(agent, @"Version/\d+\.\d+", "");

        // 3. Отримати iOS версію з User-Agent (напр. 17_6_1 → 17.6)
        var match = Regex.Match(agent, @"CPU iPhone OS (\d+)_?(\d+)?_?\d*");
        if (match.Success)
        {
            var major = match.Groups[1].Value;
            var minor = match.Groups[2].Success ? match.Groups[2].Value : "0";
            var iosVersion = $"{major}.{minor}";

            // 4. Вставити Version/X.X перед Mobile/
            var mobileMatch = Regex.Match(agent, @"\sMobile/");
            if (mobileMatch.Success)
            {
                var insertIndex = mobileMatch.Index;
                agent = agent.Insert(insertIndex, $" Version/{iosVersion}");
            }
        }

        // 5. Додати Safari/604.1, якщо його немає
        if (!agent.Contains("Safari/"))
        {
            agent += " Safari/604.1";
        }

        _webView.SetUserAgent(agent);
    }

    
    private string ClearAgent(string agent)
    {
        return agent.Replace("; wv", "");
    }
    
    private void UnSubscribe()
    {
        PrintMessage($"📤UnSubscribe");
        
        _webView.OnPageStarted -= OnPageStarted;
        
        _webView.OnPageFinished -= OnPageFinished;
        
        _webView.OnLoadingErrorReceived -= OnLoadingErrorReceived;
            
        _webView.UnregisterShouldHandleRequest();
    }
    
    private void LoadWebView()
    {
        PrintMessage($"LoadUrl: _webView = {_webView}");

        _url = UrlB;
       
        AddPermissionTrustDomain("forms.kycaid.com");
        
        _webView.Load(_url);
    }
    
    private void AddPermissionTrustDomain(string domain)
    {
        _webView.AddPermissionTrustDomain(domain);
    }

    private void OnLoadingErrorReceived(UniWebView view, int errorCode, string errorMessage, UniWebViewNativeResultPayload payload)
    {
        PrintMessage($"### 💀OnLoadingErrorReceived: errorCode={errorCode}, _webView.Url={_webView.Url}, errorMessage={errorMessage}");
        
        //if (errorCode is not (-1007 or -9 or 0)) return;
        //if (payload.Extra != null &&
        //    payload.Extra.TryGetValue(UniWebViewNativeResultPayload.ExtraFailingURLKey, out var value))
        //    view.Load((string)value);
        
        ShowWebView();
    }
    
    private void OnPageStarted(UniWebView view, string url)
    {
        PrintMessage($"### 🎬OnPageStarted UniWebView: url={url}");

        CultureInfo ci = new CultureInfo("en-US");
        
        if (!url.StartsWith("http", true, ci) && !url.StartsWith("about:blank", true, ci))
        {
            PrintMessage($"⁉️ OnPageStarted");
            
            Application.OpenURL(url);
            
            if (_webView.CanGoBack) 
                _webView.GoBack();
        }
    }
    
    private void OnPageFinished(UniWebView view, int statusCode, string url)
    {
        PrintMessage($"### 🏁OnPageFinished: url={url}");
        
        if(url != "about:blank")
        {
            _url = url;

            UrlB = url;
        }
        else
        {
            PrintMessage($"⁉️ url == about:blank");
        }
        
        if (rotationCoroutine != null)
        {
            StopCoroutine(rotationCoroutine);
        }

        rotationCoroutine = StartCoroutine(DelayedAdjust(view));
            
        ShowWebView();
        
        view.EvaluateJavaScript("window.lazySizes && lazySizes.init();", (payload) => {
            if (payload.resultCode == "0") {
                Debug.Log("lazySizes перезапущено.");
            }
            else {
                Debug.LogWarning($"Помилка при ініціалізації lazySizes: {payload.resultCode}, data: {payload.data}");
            }
        });
    }
    
    private void HideWebView()
    {
        if(_webView == null) return;
        
        if (!_isVisible) return;

        _isVisible = false;
        
        _webView.Hide();
        
        if(ConnectivityManager.Instance)
            ConnectivityManager.Instance.CheckErrorReceived();
        
        if(ConnectivityManager.Instance)
            ConnectivityManager.Instance.OnChangedInternetConnection.AddListener(CheckConnection);
    }

    private void ShowWebView()
    {
        if(_webView == null) return;
        
        if (_isVisible) return;

        _isVisible = true;
        
        _webView.Show();
    }
    
    private bool IsBlockedUrl(string url)
    {
        string[] blockedExtensions = { ".zip", ".rar", ".apk", ".pdf", ".exe", ".aab", ".bin" };
        
        foreach (var ext in blockedExtensions)
        {
            if (url.EndsWith(ext))
                return true;
        }
        return false;
    }

    private void PrintMessage(string message)
    {
        Debugger.Log($"@@@ WebViewController ->: {message}", new Color(0.2f, 0.9f, 0.2f));
    }
}
