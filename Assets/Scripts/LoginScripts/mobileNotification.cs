using System;
using System.Collections;
using System.Collections.Generic;

#if UNITY_ANDROID
using Unity.Notifications.Android;
# endif
using UnityEngine;
using UnityEngine.Android;


public class mobileNotification : MonoBehaviour
{

#if UNITY_ANDROID

 
    //void Start()
    //{
    //    if (!Permission.HasUserAuthorizedPermission("android.permission.POST_NOTIFICATIONS"))
    //    {
    //        Permission.RequestUserPermission("android.permission.POST_NOTIFICATIONS");
    //    }

    //}

#endif

}
