using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendList : MonoBehaviour
{
    [SerializeField] private GameObject _friendsListPanel;
    [SerializeField] private GameObject _friendPrefab; // Prefab para mostrar un amigo en la lista

    private DatabaseReference mDatabaseRef;
    private FirebaseAuth auth;

    void Start()
    {
        mDatabaseRef = FirebaseDatabase.DefaultInstance.RootReference;
        auth = FirebaseAuth.DefaultInstance;
        LoadFriendsList();
    }

    private void LoadFriendsList()
    {
        string currentUserId = auth.CurrentUser.UserId;
        mDatabaseRef.Child("friends").Child(currentUserId).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                if (task.Result.Exists)
                {
                    foreach (DataSnapshot snapshot in task.Result.Children)
                    {
                        string friendId = snapshot.Key;
                        LoadFriendInfo(friendId);
                    }
                }
            }
            else
            {
                Debug.LogError("LoadFriendsList encountered an error: " + task.Exception);
            }
        });
    }

    private void LoadFriendInfo(string friendId)
    {
        mDatabaseRef.Child("users").Child(friendId).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted && task.Result.Exists)
            {
                string username = task.Result.Child("username").Value.ToString();

                GameObject friend = Instantiate(_friendPrefab, _friendsListPanel.transform);
                friend.GetComponent<FriendEntry>().SetFriendData(friendId, username);
            }
            else
            {
                Debug.LogError("LoadFriendInfo encountered an error: " + task.Exception);
            }
        });
    }
}
