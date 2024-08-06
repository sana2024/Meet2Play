using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;

public class AndroidPermissionHandler : MonoBehaviour
{
    bool isItPermissionTime = false;
    string nextPermission;
    Stack<string> permissions = new Stack<string>();

    void Start()
    {
        OpenAllPermissions();
    }

    public void OpenAllPermissions()
    {
        isItPermissionTime = true;
        CreatePermissionList();

    }
    void CreatePermissionList()
    {
        permissions = new Stack<string>();
        permissions.Push(Permission.Microphone);
        permissions.Push(Permission.Camera);
        AskForPermissions();
    }
    void AskForPermissions()
    {
        if (permissions == null || permissions.Count <= 0)
        {
            isItPermissionTime = false;
            return;
        }
        nextPermission = permissions.Pop();

        if (nextPermission == null)
        {
            isItPermissionTime = false;
            return;
        }
        if (Permission.HasUserAuthorizedPermission(nextPermission) == false)
        {
            Permission.RequestUserPermission(nextPermission);
        }
        else
        {
            if (isItPermissionTime == true)
                AskForPermissions();
        }

    }

    private void OnApplicationFocus(bool focus)
    {

        if (focus == true && isItPermissionTime == true)
        {
            AskForPermissions();
        }
    }

}