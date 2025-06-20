import Foundation
import WebKit
import UIKit
import SwiftUI

// ✅ Тут додаємо enum для ключів UserDefaults
enum WebViewStorageKeys {
    static let startUrl = "WebViewStartUrl"
}

@objc public class WebViewPlugin: NSObject {
    
    @objc public static let shared = WebViewPlugin()
    private var rootView: UIViewController?
    
    @objc public func showWebView(_ urlString: NSString, callbackObject: NSString) {
        DispatchQueue.main.async {
            guard let window = UIApplication.shared.windows.first(where: { $0.isKeyWindow }) else { return }
            self.rootView = window.rootViewController
            
            let defaults = UserDefaults.standard

            // Якщо вже є збережений стартовий URL — використовуємо його.
            // Інакше використовуємо той, що прийшов із Unity (urlString).
            let initialUrl: String
            if let savedUrl = defaults.string(forKey: WebViewStorageKeys.startUrl), !savedUrl.isEmpty {
                initialUrl = savedUrl
                print("[WebViewPlugin] ✅ Використано збережений стартовий URL: \(savedUrl)")
            } else {
                initialUrl = urlString as String
                print("[WebViewPlugin] 🔰 Використано стартовий URL із Unity: \(initialUrl)")
            }
            
            let container = UIHostingController(
                rootView: WebViewContainer(
                    stringURL: initialUrl,
                    unityCallbackObjectName: callbackObject as String
                )
            )
            container.view.backgroundColor = .black
            container.modalPresentationStyle = .fullScreen
            self.rootView?.present(container, animated: true)
        }
    }
    
    @objc public func closeWebView() {
        DispatchQueue.main.async {
            self.rootView?.dismiss(animated: true)
        }
    }
}

@_cdecl("showWebView")
public func showWebView(urlPtr: UnsafePointer<CChar>, callbackPtr: UnsafePointer<CChar>) {
    let urlString = String(cString: urlPtr)
    let callbackObject = String(cString: callbackPtr)
    WebViewPlugin.shared.showWebView(urlString as NSString, callbackObject: callbackObject as NSString)
}

@_cdecl("closeWebView")
public func closeWebView() {
    WebViewPlugin.shared.closeWebView()
}
