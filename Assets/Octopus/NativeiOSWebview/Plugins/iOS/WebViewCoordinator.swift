import Foundation
import WebKit
import UIKit

class WebViewCoordinator: NSObject, WKNavigationDelegate, WKUIDelegate, WKScriptMessageHandler {
    private let callbackObjectName: String
    
    init(callbackObjectName: String) {
        self.callbackObjectName = callbackObjectName
    }
    
    func userContentController(_ userContentController: WKUserContentController, didReceive message: WKScriptMessage) {
        if message.name == "contentLoaded" {
            //UnitySendMessage(callbackObjectName, "OnWebViewContentLoaded", "")
        }
        if message.name == "iosListener",
           let value = message.body as? String {
            UserDefaults.standard.set(value, forKey: "stringURL")
        }
    }
    
    func webView(
        _ webView: WKWebView,
        createWebViewWith configuration: WKWebViewConfiguration,
        for navigationAction: WKNavigationAction,
        windowFeatures: WKWindowFeatures
    ) -> WKWebView? {
        let newWebView = WKWebView(frame: .zero, configuration: configuration)
        newWebView.navigationDelegate = self
        newWebView.uiDelegate = self
        newWebView.applyCustomUserAgent()
        newWebView.allowsBackForwardNavigationGestures = true
        
        let sheetVC = WebSheetViewController(webView: newWebView)
        
        if let topVC = UIApplication.shared.windows.first(where: { $0.isKeyWindow })?.rootViewController?.topMostPresentedViewController() {
            topVC.present(sheetVC, animated: true)
        }
        return newWebView
    }
    
    func webView(_ webView: WKWebView, decidePolicyFor navigationAction: WKNavigationAction,
                     decisionHandler: @escaping (WKNavigationActionPolicy) -> Void) {

            if let url = navigationAction.request.url, url.scheme != "http", url.scheme != "https" {
                if UIApplication.shared.canOpenURL(url) {
                    UIApplication.shared.open(url, options: [:], completionHandler: nil)
                    unityLog("[WebViewPlugin] Opened external URL: \(url.absoluteString)")
                    decisionHandler(.cancel)
                    return
                } else {
                    unityLog("[WebViewPlugin] Can't open URL with scheme: \(url.scheme ?? "unknown")")
                }
            }

            decisionHandler(.allow)
        }

        func webView(_ webView: WKWebView, didStartProvisionalNavigation navigation: WKNavigation!) {
            let url = webView.url?.absoluteString ?? "unknown URL"
            unityLog("[WebViewPlugin] Page started loading: \(url)")
        }

        func webView(_ webView: WKWebView, didFinish navigation: WKNavigation!) {
            let url = webView.url?.absoluteString ?? "unknown URL"
            unityLog("[WebViewPlugin] Page finished loading: \(url)")
            
            // Зберігаємо тільки якщо ще не збережено
                let defaults = UserDefaults.standard
                if defaults.string(forKey: WebViewStorageKeys.startUrl) == nil {
                    defaults.set(url, forKey: WebViewStorageKeys.startUrl)
                    unityLog("[WebViewPlugin] ✅ Start URL saved: \(url)")
                }
        }

        func webView(_ webView: WKWebView, didFail navigation: WKNavigation!, withError error: Error) {
            unityLog("[WebViewPlugin] Page failed loading with error: \(error.localizedDescription)")
        }

        func webView(_ webView: WKWebView, didFailProvisionalNavigation navigation: WKNavigation!, withError error: Error) {
            unityLog("[WebViewPlugin] Provisional navigation failed: \(error.localizedDescription)")
        }
}
