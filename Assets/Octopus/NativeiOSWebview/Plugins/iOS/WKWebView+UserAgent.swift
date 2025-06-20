import WebKit

extension WKWebView {
    func applyCustomUserAgent() {
        self.evaluateJavaScript("navigator.userAgent") { [weak self] result, error in
            guard let self = self else { return }
            guard var ua = result as? String, error == nil else {
                print("[WebViewPlugin] Failed to get user agent: \(error?.localizedDescription ?? "Unknown error")")
                return
            }
            print("[WebViewPlugin] User agent: \(ua)")
            // 1. Видаляємо "; wv"
            ua = ua.replacingOccurrences(of: "; wv", with: "")

            // 2. Витягуємо iOS версію
            let iosVersion: String? = {
                let pattern = #"CPU iPhone OS (\d+_\d+)"#
                if let regex = try? NSRegularExpression(pattern: pattern),
                   let match = regex.firstMatch(in: ua, range: NSRange(ua.startIndex..., in: ua)),
                   let range = Range(match.range(at: 1), in: ua) {
                    return ua[range].replacingOccurrences(of: "_", with: ".")
                }
                return nil
            }()

            // 3. Додаємо Version/XX.X перед Mobile/... якщо нема
            if let version = iosVersion,
               ua.contains("Mobile/"),
               !ua.contains("Version/") {
                if let range = ua.range(of: "Mobile/") {
                    ua.insert(contentsOf: "Version/\(version) ", at: range.lowerBound)
                }
            }

            // 4. (Опційно) додаємо Safari/604.1, якщо треба
            if !ua.contains("Safari/") {
                ua += " Safari/604.1"
            }

            self.customUserAgent = ua
            print("[WebViewPlugin] Custom user agent set: \(ua)")
        }
    }
}
