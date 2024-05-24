using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FriendsManager : MonoBehaviour
{
    private FirebaseAuth auth;
    private DatabaseReference databaseRef;

    [Header("UI References")]
    public GameObject friendEntryPrefab;
    public GameObject requestEntryPrefab;
    public Transform friendsContainer;
    public Transform requestsContainer;
    public GameObject requestsPanel;
    public GameObject addFriendPanel;
    public TMP_InputField addFriendInputField;
    public Button sendRequestButton;
    public Button showRequestsButton;
    public Button showAddFriendButton;

    private void Start()
    {
        auth = FirebaseAuth.DefaultInstance;
        databaseRef = FirebaseDatabase.DefaultInstance.RootReference;

        sendRequestButton.onClick.AddListener(SendFriendRequest);
        showRequestsButton.onClick.AddListener(() => TogglePanel(requestsPanel));
        showAddFriendButton.onClick.AddListener(() => TogglePanel(addFriendPanel));

        InitializeListeners();
    }

    private void InitializeListeners()
    {
        if (auth.CurrentUser != null)
        {
            // Listen for changes in the friends list
            databaseRef.Child("users").Child(auth.CurrentUser.UserId).Child("friends").ValueChanged += HandleFriendsChanged;

            // Listen for changes in the friend requests
            databaseRef.Child("friendRequests").OrderByChild("receiverId").EqualTo(auth.CurrentUser.UserId).ValueChanged += HandleFriendRequestsChanged;
        }
        else
        {
            Debug.LogError("FirebaseAuth current user is null.");
        }
    }

    private void HandleFriendsChanged(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }

        foreach (Transform child in friendsContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (DataSnapshot snapshot in args.Snapshot.Children)
        {
            string friendId = snapshot.Key;
            bool isOnline = bool.Parse(snapshot.Child("isOnline").Value.ToString());
            string username = snapshot.Child("username").Value.ToString();

            GameObject friendEntry = Instantiate(friendEntryPrefab, friendsContainer);
            friendEntry.GetComponent<FriendEntry>().SetFriendData(username, isOnline);
        }
    }

    private void HandleFriendRequestsChanged(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }

        foreach (Transform child in requestsContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (DataSnapshot snapshot in args.Snapshot.Children)
        {
            string requestId = snapshot.Key;
            string senderId = snapshot.Child("senderId").Value.ToString();
            string senderUsername = snapshot.Child("senderUsername").Value.ToString();

            GameObject requestEntry = Instantiate(requestEntryPrefab, requestsContainer);
            requestEntry.GetComponent<RequestEntry>().SetRequestData(requestId, senderId, senderUsername, AcceptFriendRequest, DeclineFriendRequest);
        }
    }

    private void SendFriendRequest()
    {
        string receiverUsername = addFriendInputField.text;
        Debug.Log("Attempting to send friend request to " + receiverUsername);

        // Buscar el ID del usuario utilizando el nombre de usuario
        databaseRef.Child("usernames").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                if (task.Result.Exists)
                {
                    foreach (DataSnapshot snapshot in task.Result.Children)
                    {
                        if (snapshot.Child("username").Value.ToString() == receiverUsername)
                        {
                            string receiverId = snapshot.Key; // Obtener el ID del usuario
                            string senderId = auth.CurrentUser.UserId;
                            string senderUsername = auth.CurrentUser.DisplayName;

                            string requestId = databaseRef.Child("friendRequests").Push().Key;
                            FriendRequest request = new FriendRequest(senderId, receiverId, senderUsername);
                            databaseRef.Child("friendRequests").Child(requestId).SetValueAsync(request).ContinueWithOnMainThread(requestTask =>
                            {
                                if (requestTask.IsCompleted)
                                {
                                    Debug.Log("Friend request sent successfully.");
                                }
                                else
                                {
                                    Debug.LogError("Failed to send friend request: " + requestTask.Exception);
                                }
                            });
                            return;
                        }
                    }
                    Debug.LogWarning("User not found.");
                }
                else
                {
                    Debug.LogWarning("No usernames found in the database.");
                }
            }
            else
            {
                Debug.LogError("Failed to get usernames: " + task.Exception);
            }
        });
    }

    private void AcceptFriendRequest(string requestId, string senderId, string senderUsername)
    {
        string receiverId = auth.CurrentUser.UserId;
        string receiverUsername = auth.CurrentUser.DisplayName;

        databaseRef.Child("users").Child(receiverId).Child("friends").Child(senderId).SetValueAsync(new Friend(senderUsername, true));
        databaseRef.Child("users").Child(senderId).Child("friends").Child(receiverId).SetValueAsync(new Friend(receiverUsername, true));

        databaseRef.Child("friendRequests").Child(requestId).RemoveValueAsync();
    }

    private void DeclineFriendRequest(string requestId)
    {
        databaseRef.Child("friendRequests").Child(requestId).RemoveValueAsync();
    }

    private void TogglePanel(GameObject panel)
    {
        panel.SetActive(!panel.activeSelf);
    }
}

public class FriendRequest
{
    public string senderId;
    public string receiverId;
    public string senderUsername;

    public FriendRequest() { }

    public FriendRequest(string senderId, string receiverId, string senderUsername)
    {
        this.senderId = senderId;
        this.receiverId = receiverId;
        this.senderUsername = senderUsername;
    }
}

public class Friend
{
    public string username;
    public bool isOnline;

    public Friend() { }

    public Friend(string username, bool isOnline)
    {
        this.username = username;
        this.isOnline = isOnline;
    }
}