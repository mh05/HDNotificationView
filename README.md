# HDNotificationView
  
  Xamarin iOS port of [HDNotificationView
 ](https://github.com/nhdang103/HDNotificationView) from [nhdang103](https://github.com/nhdang103)
  
  HDNotificationView appears notification view like system.

## Requirement
- iOS 8.0+

## Screenshots
![alt tag](./Assets/screen_portrait.gif) ![alt tag](./Assets/screen_landscape.gif)

## Usage
This API has multiple methodes starting with shortest:
This will shown a notification view with **image** **title** and **message** for 7 seconds.
`HDNotificationView.ShowNotificationViewWithImage(UIImage.FromFile("Icons/icon.png"), "Title", "Message");`

if you would like to specify your own time use.
`HDNotificationView.ShowNotificationViewWithImage(UIImage.FromFile("Icons/icon.png"), "Title", "Message",7,()=>{});`

There is also an methode during start of the Notification view the create a sound when displaying
`HDNotificationView.ShowNotificationViewWithImageAndFireOnClose(UIImage image, string title, string message, int timerLength, Action onTouch, NSUrl sound)`

Note: HDNotificationView class is wrapper around NotificationView class for displaying.

## License
HDNotificationView is available under the [MIT License](https://en.wikipedia.org/wiki/MIT_License). See the [LICENSE](./LICENSE) for details.