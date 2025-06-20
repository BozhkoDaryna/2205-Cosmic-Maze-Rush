import SwiftUI
import WebKit

struct WebViewContainer: UIViewRepresentable {
    var stringURL: String
    var unityCallbackObjectName: String
    
    func makeCoordinator() -> WebViewCoordinator {
        WebViewCoordinator(callbackObjectName: unityCallbackObjectName)
    }

    func makeUIView(context: Context) -> WKWebView {
        let config = WKWebViewConfiguration()
        config.userContentController.add(context.coordinator, name: "contentLoaded")
        config.userContentController.add(context.coordinator, name: "iosListener")
        config.allowsInlineMediaPlayback = true
        config.preferences.javaScriptEnabled = true

        let webView = WKWebView(frame: .zero, configuration: config)
        webView.navigationDelegate = context.coordinator
        webView.uiDelegate = context.coordinator
        webView.applyCustomUserAgent()
        webView.allowsBackForwardNavigationGestures = true

        return webView
    }

    func updateUIView(_ uiView: WKWebView, context: Context) {
        if let url = URL(string: stringURL), stringURL.lowercased().hasPrefix("http") {
            uiView.load(URLRequest(url: url))
        } else if let localURL = Bundle.main.url(forResource: "index", withExtension: "html") {
            uiView.load(URLRequest(url: localURL))
        }
    }
}
