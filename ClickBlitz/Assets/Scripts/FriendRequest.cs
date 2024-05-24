using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FriendRequest : MonoBehaviour
{
    public TMP_InputField usernameInputField;
    public Button sendRequestButton;

    private FirebaseAuth auth;
    private DatabaseReference mDatabaseRef;

    private void Start()
    {
        auth = FirebaseAuth.DefaultInstance;
        mDatabaseRef = FirebaseDatabase.DefaultInstance.RootReference;

        sendRequestButton.onClick.AddListener(SendFriendRequest);
    }

    private void SendFriendRequest()
    {
        string username = usernameInputField.text.Trim();

        if (string.IsNullOrEmpty(username))
        {
            Debug.LogWarning("Please enter a username.");
            return;
        }

        mDatabaseRef.Child("users").OrderByChild("username").EqualTo(username).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Error searching for user: " + task.Exception);
                return;
            }

            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                if (snapshot.HasChildren)
                {
                    foreach (DataSnapshot userSnapshot in snapshot.Children)
                    {
                        string userId = userSnapshot.Key;
                        SendFriendRequestToUser(userId);
                        return;
                    }
                }
                else
                {
                    Debug.LogWarning("No user found with the provided username.");
                }
            }
        });
    }

    private void SendFriendRequestToUser(string userId)
    {
        string currentUserId = auth.CurrentUser.UserId;
        mDatabaseRef.Child("friend_requests").Child(userId).Child(currentUserId).SetValueAsync(true).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("Friend request sent successfully.");
            }
            else
            {
                Debug.LogError("Failed to send friend request: " + task.Exception);
            }
        });
    }
}
