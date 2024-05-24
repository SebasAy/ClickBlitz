using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class RequestEntry : MonoBehaviour
{
    [SerializeField] private TMP_Text usernameText;
    [SerializeField] private Button acceptButton;
    [SerializeField] private Button declineButton;

    private string requesterId;
    private string username;

    private void Start()
    {
        acceptButton.onClick.AddListener(AcceptFriendRequest);
        declineButton.onClick.AddListener(DeclineFriendRequest);
    }

    public void Initialize(string requesterId, string username)
    {
        this.requesterId = requesterId;
        this.username = username;
        usernameText.text = username;
    }

    private void AcceptFriendRequest()
    {
        string currentUserId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;

        // Agregar al usuario solicitante como amigo
        FirebaseDatabase.DefaultInstance.GetReference("friends").Child(currentUserId).Child(requesterId).SetValueAsync(true).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("Friend request accepted from: " + username);

                Destroy(gameObject);
                FirebaseDatabase.DefaultInstance.GetReference("friend_requests").Child(currentUserId).Child(requesterId).RemoveValueAsync();
            }
            else
            {
                Debug.LogError("Failed to accept friend request: " + task.Exception);
            }
        });
    }

    private void DeclineFriendRequest()
    {
        string currentUserId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;

        // Rechazar la solicitud de amistad
        FirebaseDatabase.DefaultInstance.GetReference("friend_requests").Child(currentUserId).Child(requesterId).RemoveValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                Destroy(gameObject);
                Debug.Log("Friend request declined from: " + username);
            }
            else
            {
                Debug.LogError("Failed to decline friend request: " + task.Exception);
            }
        });
    }
}
