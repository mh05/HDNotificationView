using System;
using Foundation;
using UIKit;

namespace HDNotificationView
{
    public class HDNotificationView
    {

        #region ---- Properties ----

        private static NotificationView _sharedInstance;
        private static NotificationView SharedInstance
        {
            get
            {
                if (_sharedInstance != null)
                    return _sharedInstance;

                UIApplication.EnsureUIThread();
                _sharedInstance = new NotificationView();
                _sharedInstance.OnVisibilityChanged += (sender, e) => IsVisible = e.IsVisible;
                return _sharedInstance;
            }
        }

        public static bool IsVisible { get; private set; }

        #endregion

        #region ---- Methodes -----

        public static void ShowNotificationViewWithImage(UIImage image, string title, string message, int timerLength, Action onTouch)
        {
            //Always remove auto hide when onTouch not is filled
            if (onTouch == null)
                timerLength = (int)NotificationView.NotificationViewShowingDuration;

            UIApplication.SharedApplication.InvokeOnMainThread(() => SharedInstance.ShowNotificationViewWithImage(image, title, message, timerLength, onTouch));
        }

        public static void ShowNotificationViewWithImage(UIImage image, string title, string message)
        {
            UIApplication.SharedApplication.InvokeOnMainThread(() => SharedInstance.ShowNotificationViewWithImage(image, title, message, (int)NotificationView.NotificationViewShowingDuration, Dismiss));
        }

        public static void ShowNotificationViewWithImageAndFireOnClose(UIImage image, string title, string message, int timerLength, Action onTouch, NSUrl sound)
        {
            UIApplication.SharedApplication.InvokeOnMainThread(() => SharedInstance.ShowNotificationViewWithImageAndFireOnClose(image, title, message, timerLength, onTouch, sound));
        }

        public static void ShowNotificationViewWithImageAndFireOnClose(UIImage image, string title, string message, int timerLength, Action onTouch, Action onClose, NSUrl sound)
        {
            UIApplication.SharedApplication.InvokeOnMainThread(() => SharedInstance.ShowNotificationViewWithImageAndFireOnClose(image, title, message, timerLength, onTouch, onClose, sound));
        }

        public static void Dismiss()
        {
            if (SharedInstance.IsVisible)
                UIApplication.SharedApplication.InvokeOnMainThread(SharedInstance.HideNotificationView);
        }

        public static void DismissWithAction(Action ac)
        {
            if (ac == null)
            {
                Dismiss();
                return;
            }

            if (IsVisible)
                UIApplication.SharedApplication.InvokeOnMainThread(() => SharedInstance.HideNotificationViewOnComplete(ac));
            else
                ac();
        }

        private static bool _supportPortrait = true;
        public static bool SupportPortrait
        {
            get => _supportPortrait;
            set
            {
                if (Equals(value, _supportPortrait))
                    return;
                _supportPortrait = value;
                SharedInstance.SupportPortrait = value;
            }
        }

        #endregion
    }
}