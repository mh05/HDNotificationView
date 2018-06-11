using System;

using UIKit;

namespace HDNotificationView.Example
{
    public partial class ViewController : UIViewController
    {
        protected ViewController(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            btnTest.TouchUpInside += BtnTest_TouchUpInside;
            // Perform any additional setup after loading the view, typically from a nib.
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }


        void BtnTest_TouchUpInside(object sender, EventArgs e)
        {
            HDNotificationView.ShowNotificationViewWithImage(UIImage.FromFile("Icons/icon.png"), "Title", "Message");
        }

    }

}
