import UIKit

extension UIViewController {
    func topMostPresentedViewController() -> UIViewController {
        var topController = self
        while let presentedVC = topController.presentedViewController {
            topController = presentedVC
        }
        return topController
    }
}
