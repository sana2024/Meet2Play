
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_IOS
using Unity.Notifications.iOS;
#endif

using System;

public class mobileNotificationIos : MonoBehaviour
{
#if UNITY_IOS
    public string notificationId = "tes_Push_Notification";

    // Start is called before the first frame update
    private void Start()
    {

 

    }

 

    public void OnClick(string subtitle , string Body)
    {
        iOSNotificationTimeIntervalTrigger timeTrigger = new iOSNotificationTimeIntervalTrigger()
        {

            TimeInterval = new TimeSpan(0, 0, 1),
            Repeats = true,

        };

        iOSNotificationCalendarTrigger clalendarTrigger = new iOSNotificationCalendarTrigger()
        {
            Hour = 12,
            Minute = 0,
            Repeats = true,



        };

        iOSNotificationLocationTrigger locationTrigger = new iOSNotificationLocationTrigger()
        {
            Center = new Vector3(2.294498f, 48, 858263f),
            Radius = 250f,
            NotifyOnEntry = true,
            NotifyOnExit = false,

        };

        iOSNotification notification = new iOSNotification()
        {

            Identifier = "test_notfication",
            Title = "Meet2Play",
            Subtitle = subtitle,
            Body = Body,
            ShowInForeground = true,
            ForegroundPresentationOption = (PresentationOption.Alert | PresentationOption.Sound),
            CategoryIdentifier = "category_a",
            ThreadIdentifier = "thread1",
            Trigger = timeTrigger,

        };

        iOSNotificationCenter.ScheduleNotification(notification);

        iOSNotificationCenter.OnRemoteNotificationReceived += recivedNotification => {

            Debug.Log("Received notification " + notification.Identifier + "!");
        };

        iOSNotification notificationIntentData = iOSNotificationCenter.GetLastRespondedNotification();
        if (notificationIntentData != null)
        {

            Debug.Log("App was opened with notification!");
        }
    }
    private void OnApplicationPause(bool pause)
    {

        //  iOSNotificationCenter.RemoveScheduledNotification(notificationId);
        // iOSNotificationCenter.RemoveDeliveredNotification(notificationId);

    }


#endif
}



