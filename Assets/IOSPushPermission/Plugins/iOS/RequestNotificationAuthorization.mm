
#import <UserNotifications/UserNotifications.h>
#import <Foundation/Foundation.h>

extern "C" {
    void RequestNotificationAuthorization()
    {
        UNUserNotificationCenter *center = [UNUserNotificationCenter currentNotificationCenter];
        [center requestAuthorizationWithOptions:(UNAuthorizationOptionAlert | UNAuthorizationOptionSound | UNAuthorizationOptionBadge)
                              completionHandler:^(BOOL granted, NSError * _Nullable error) {
            if (granted) {
                NSLog(@"✅ Notification permission granted.");
            } else {
                NSLog(@"❌ Notification permission denied.");
            }
        }];
    }
}
