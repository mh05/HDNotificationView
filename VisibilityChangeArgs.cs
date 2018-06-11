using System;

namespace HDNotificationView
{
    public class VisibilityChangeArgs : EventArgs
    {
        public bool IsVisible { get; }

        public VisibilityChangeArgs(bool isVisible)
        {
            IsVisible = isVisible;
        }
    }
}