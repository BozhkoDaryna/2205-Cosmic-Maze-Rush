import UIKit
import WebKit

final class WebSheetViewController: UIViewController {
    private let webView: WKWebView

    init(webView: WKWebView) {
        self.webView = webView
        super.init(nibName: nil, bundle: nil)
        self.modalPresentationStyle = .pageSheet //встановлює тип модального вікна у вигляді листа, що з’являється знизу.
    }

    required init?(coder: NSCoder) {
        fatalError("init(coder:) has not been implemented")
    }

    override func loadView() {
        let container = UIView()
        container.backgroundColor = .black
        webView.translatesAutoresizingMaskIntoConstraints = false
        container.addSubview(webView)

        NSLayoutConstraint.activate([
            webView.topAnchor.constraint(equalTo: container.topAnchor),
            webView.bottomAnchor.constraint(equalTo: container.bottomAnchor),
            webView.leadingAnchor.constraint(equalTo: container.leadingAnchor),
            webView.trailingAnchor.constraint(equalTo: container.trailingAnchor)
        ])
        view = container
    }

    override func viewDidLoad() {
        super.viewDidLoad()
        if let sheet = sheetPresentationController {
            sheet.detents = [.large()] // висота листа (sheet) – великий розмір
            sheet.prefersGrabberVisible = true // маленька "ручка" зверху
        }
    }
}
