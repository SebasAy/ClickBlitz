using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FrienRequestManager : MonoBehaviour
{
    private DatabaseReference mDatabaseRef;
    private FirebaseAuth auth;

    [SerializeField] private GameObject friendRequestPrefab;
    [SerializeField] private Transform friendRequestParent;

    void Start()
    {
        mDatabaseRef = FirebaseDatabase.DefaultInstance.RootReference;
        auth = FirebaseAuth.DefaultInstance;
        LoadFriendRequests();
    }

    private void LoadFriendRequests()
    {
        string currentUserId = auth.CurrentUser.UserId;
        mDatabaseRef.Child("friend_requests").Child(currentUserId).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                if (task.Result.Exists)
                {
                    foreach (DataSnapshot snapshot in task.Result.Children)
                    {
                        string requesterId = snapshot.Key;
                        LoadRequesterInfo(requesterId);
                    }
                }
            }
            else
            {
                Debug.LogError("LoadFriendRequests encountered an error: " + task.Exception);
            }
        });
    }

    private void LoadRequesterInfo(string requesterId)
    {
        mDatabaseRef.Child("users").Child(requesterId).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted && task.Result.Exists)
            {
                string username = task.Result.Child("username").Value.ToString();
                DisplayFriendRequest(requesterId, username);
            }
            else
            {
                Debug.LogError("LoadRequesterInfo encountered an error: " + task.Exception);
            }
        });
    }

    private void DisplayFriendRequest(string requesterId, string username)
    {
        GameObject friendRequest = Instantiate(friendRequestPrefab, friendRequestParent);
        friendRequest.GetComponent<RequestEntry>().Initialize(requesterId, username);
    }
}
