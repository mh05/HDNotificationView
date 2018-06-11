using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using AVFoundation;
using CoreGraphics;
using Foundation;
using UIKit;

namespace HDNotificationView
{
    internal class NotificationView : UIToolbar
    {
        #region ----- Constants -----

        private const float ImageViewIconCornerRadius = 3f;
        private const float LabelTitleFontSize = 14f;
        private const float LabelMessageFontSize = 13f;
        private const float LabelMessageFrameHeight = 35f;
        private const float NotificationViewFrameHeight = 64f;

        //Showing in seconds
        public const double NotificationViewShowingDuration = 7f;
        private const double NotificationViewShowingAnimationTime = 0.5f;

        //Frames
        private readonly CGRect _imageViewIconFrame = new RectangleF(15f, 8f, 20f, 20f);
        private readonly CGRect _labelTitleFrame = new RectangleF(45f, 3f, (float)UIScreen.MainScreen.Bounds.Size.Width - 45f, 26f);
        private readonly CGRect _labelMessageFrame = new RectangleF(45.0f, 25.0f, (float)UIScreen.MainScreen.Bounds.Size.Width - 45f, LabelMessageFrameHeight);

        #endregion

        #region ----- Fields -----

        private UIImageView _imgIcon;
        private UILabel _lblTitle;
        private UILabel _lblMessage;
        private NSTimer _timerHideAuto;
        private Action _onTouch;

        #endregion

        public EventHandler<VisibilityChangeArgs> OnVisibilityChanged;

        private bool _isVisible;
        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                if (value == _isVisible)
                    return;

                _isVisible = value;
                var ev = OnVisibilityChanged;
                ev?.Invoke(this, new VisibilityChangeArgs(_isVisible));
            }
        }

        public bool SupportPortrait { get; set; } = true;

        #region ---- Constructor ---

        internal NotificationView() : base(CGRect.Empty)
        {
            if (!UIDevice.CurrentDevice.GeneratesDeviceOrientationNotifications)
                UIDevice.CurrentDevice.BeginGeneratingDeviceOrientationNotifications();


            //Add to orientation switch
            NSNotificationCenter.DefaultCenter.AddObserver(UIDevice.OrientationDidChangeNotification, ch => SetupUI());

            SetupUI();
        }

        #endregion

        private void SetupUI()
        {
            Console.WriteLine($"current orientation is {UIDevice.CurrentDevice.Orientation}");

            BarTintColor = null;
            Translucent = true;
            BarStyle = UIBarStyle.Black;

            Layer.ZPosition = float.MaxValue;
            BackgroundColor = UIColor.Clear;
            MultipleTouchEnabled = false;
            ExclusiveTouch = true;

            Frame = new CGRect(0f, 0f, (float)UIScreen.MainScreen.Bounds.Size.Width, NotificationViewFrameHeight);

            Console.WriteLine($"Width = {Frame.Size.Width} , height = {Frame.Size.Height}");
            // Icon
            if (_imgIcon == null)
                _imgIcon = new UIImageView();

            _imgIcon.Frame = _imageViewIconFrame;
            _imgIcon.ContentMode = UIViewContentMode.ScaleAspectFill;
            _imgIcon.Layer.CornerRadius = ImageViewIconCornerRadius;
            _imgIcon.ClipsToBounds = true;

            if (!Subviews.Contains(_imgIcon))
                AddSubview(_imgIcon);

            // Title
            if (_lblTitle == null)
                _lblTitle = new UILabel(_labelTitleFrame);

            _lblTitle.TextColor = UIColor.White;
            _lblTitle.Font = UIFont.FromName(@"Helvetica-Bold", LabelTitleFontSize);
            _lblTitle.Lines = 1;

            if (!Subviews.Contains(_lblTitle))
                AddSubview(_lblTitle);

            //Message
            if (_lblMessage == null)
                _lblMessage = new UILabel(_labelMessageFrame);


            _lblMessage.TextColor = UIColor.White;
            _lblMessage.Font = UIFont.FromName(@"Helvetica", LabelMessageFontSize);
            _lblMessage.Lines = 2;
            _lblMessage.LineBreakMode = UILineBreakMode.TailTruncation;

            if (!Subviews.Contains(_lblMessage))
                AddSubview(_lblMessage);

            FixLabelMessageSize();

            var tapsGesture = new UITapGestureRecognizer();
            tapsGesture.AddTarget(() =>
            {
                tapsGesture.LocationInView(this);
                NotificationViewDidTap();
            });
            //set clicks
            tapsGesture.NumberOfTapsRequired = 1;
            tapsGesture.NumberOfTouchesRequired = 1;
            AddGestureRecognizer(tapsGesture);

            if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0) || SupportPortrait) return;
            switch (UIDevice.CurrentDevice.Orientation)
            {
                case UIDeviceOrientation.Unknown:
                case UIDeviceOrientation.Portrait:
                case UIDeviceOrientation.PortraitUpsideDown:
                case UIDeviceOrientation.FaceUp:
                case UIDeviceOrientation.FaceDown:

                    const double angle = 270 * Math.PI / 180;
                    Transform = CGAffineTransform.MakeRotation((nfloat)angle);
                    Frame = new CGRect(0, 95f, NotificationViewFrameHeight, UIScreen.MainScreen.Bounds.Size.Height + NotificationViewFrameHeight);

                    var tmp = _imgIcon.Frame;
                    tmp.X += NotificationViewFrameHeight;

                    _imgIcon.Frame = tmp;

                    tmp = _lblTitle.Frame;
                    tmp.X += NotificationViewFrameHeight;

                    _lblTitle.Frame = tmp;

                    tmp = _lblMessage.Frame;
                    tmp.X += NotificationViewFrameHeight;

                    _lblMessage.Frame = tmp;
                    break;
                case UIDeviceOrientation.LandscapeLeft:
                case UIDeviceOrientation.LandscapeRight:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #region ---- Class Helpers ----

        private void FixLabelMessageSize()
        {
            var size = _lblMessage.SizeThatFits(new CGSize(UIScreen.MainScreen.Bounds.Size.Width - 45f, float.MaxValue));
            var frame = _lblMessage.Frame;
            frame.Size = (size.Height <= LabelMessageFrameHeight) ? size : new CGSize(frame.Size.Width, LabelMessageFrameHeight);
            _lblMessage.Frame = frame;
        }

        private void NotificationViewDidTap()
        {
            _onTouch?.Invoke();
        }

        private void RemoveTimer()
        {
            if (_timerHideAuto == null)
                return;

            _timerHideAuto.Invalidate();
            _timerHideAuto.Dispose();
            _timerHideAuto = null;
        }

        #endregion

        #region ---- Methodes ----

        internal void ShowNotificationViewWithImage(UIImage image, string title, string message, int timerLength, Action onTouch)
        {
            ShowNotification(image, title, message, timerLength, onTouch, null, null);
        }

        internal void ShowNotificationViewWithImageAndFireOnClose(UIImage image, string title, string message, int timerLength, Action onTouch, NSUrl sound)
        {
            ShowNotificationViewWithImageAndFireOnClose(image, title, message, timerLength, onTouch, onTouch, sound);
        }

        internal void ShowNotificationViewWithImageAndFireOnClose(UIImage image, string title, string message, int timerLength, Action onTouch, Action onHide, NSUrl sound)
        {
            ShowNotification(image, title, message, timerLength, onTouch, onHide, sound);
        }

        private async void ShowNotification(UIImage image, string title, string message, int timerLength, Action onTouch, Action onClose, NSUrl url)
        {
            RemoveTimer();
            IsVisible = true;

            // onTouch
            _onTouch = onTouch;
            _imgIcon.Image = image;
            _lblTitle.Text = string.IsNullOrEmpty(title) ? string.Empty : title;
            _lblMessage.Text = string.IsNullOrEmpty(message) ? string.Empty : message;

            FixLabelMessageSize();

            // Prepare frame
            var frame = Frame;
            frame.Y = -frame.Size.Height;
            //			frame.Y = 0;
            Frame = frame;

            // Add to window
            var window = UIApplication.SharedApplication.Windows.FirstOrDefault();
            if (window == null)
                return;

            window.WindowLevel = UIWindowLevel.StatusBar;
            window.AddSubview(this);

            //Show animation
            Animate(NotificationViewShowingAnimationTime, 0d, UIViewAnimationOptions.CurveEaseOut
                , () =>
                {
                    var animationFrame = Frame;
                    animationFrame.Y += frame.Size.Height;
                    Frame = animationFrame;
                },
               () => { }
            );
            //Schedule to hide
            if (timerLength > 0)
                _timerHideAuto = NSTimer.CreateScheduledTimer(timerLength, obj => HideNotificationViewOnComplete(onClose));

            if (url != null)
                await Task.Run(() => PlaySound(url));

        }

        internal void HideNotificationView()
        {
            HideNotificationViewOnComplete(null);
        }

        internal void HideNotificationViewOnComplete(Action onComplete)
        {
            Animate(NotificationViewShowingAnimationTime, 0d, UIViewAnimationOptions.CurveEaseOut
                , () =>
                {
                    var frame = Frame;
                    frame.Y -= frame.Size.Height;
                    Frame = frame;
                },
                () =>
                {
                    RemoveFromSuperview();
                    RemoveTimer();
                    IsVisible = false;
                    var window = UIApplication.SharedApplication.Windows.FirstOrDefault();
                    if (window == null)
                        return;
                    window.WindowLevel = UIWindowLevel.Normal;

                    onComplete?.Invoke();
                }
            );
        }


        private static void PlaySound(NSUrl sound)
        {
            var player = AVAudioPlayer.FromUrl(sound);
            player.FinishedPlaying += async (sender, e) =>
            {
                // Wait till the 100 milliseconds we can dispose
                await Task.Delay(100).ConfigureAwait(false);
                player.Dispose();
            };
            player.Play();
        }
        #endregion
    }
}