---------------------------------------------In-Game Notify System Readme--------------------------------------------------

<---/________________________________________________Description_______________________________________________________/--->
This is a package, which allows you to inform user by notifications.
Notification system use Unity UI to display notifications.
Includes NotifyManager, which needs to be the Canvas or other UI element child and a Notify template, used in the demo scene.
You can change them ass you like.


<---/__________________________________________________API & Usage_____________________________________________________/--->
There`s simple API: to add a new notify in queue, u need only one command:

NotifyData.AddNew(notifyText, notifyIcon, notifyAction);

Also there are other overloads:

NotifyData.AddNew(notifyText);
NotifyData.AddNew(notifyText, notifyAction);
NotifyData.AddNew(notifyText, notifyAction(onShow/onHide/both), notifyAction(both/hide));
NotifyData.AddNew(notifyText, notifyIcon);
NotifyData.AddNew(notifyText, notifyIcon, notifyAction);
NotifyData.AddNew(notifyText, notifyIcon, notifyAction, notifyAction);

If you want show only image, you need only change your notify text to "":
NotifyData.AddNew("", yourImage);

Similarly with actions.


<---/______________________________________________Limitations_______________________________________________________/--->
Action needs to be a simple function without parameters, like void MyVoid(){ ... }