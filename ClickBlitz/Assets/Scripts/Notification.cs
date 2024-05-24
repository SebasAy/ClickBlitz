using Firebase.Database;
using Firebase.Auth;
using UnityEngine;
using TMPro;
using Firebase.Extensions;
using System.Collections;

public class Notification : MonoBehaviour
{
    [SerializeField]
    private GameObject notificationPanel; // Panel que se activará temporalmente
    [SerializeField]
    private GameObject friendEntryPrefab; // Prefab para mostrar la información del amigo en la notificación
    [SerializeField]
    private Transform notificationPanelTransform; // Lugar donde se instanciará el prefab del amigo

    private DatabaseReference databaseRef;
    private FirebaseAuth auth;

    void Start()
    {
        databaseRef = FirebaseDatabase.DefaultInstance.RootReference;
        auth = FirebaseAuth.DefaultInstance;

        ListenToFriendsStatus();
    }

    private void ListenToFriendsStatus()
    {
        string currentUserId = auth.CurrentUser.UserId;
        DatabaseReference friendsRef = databaseRef.Child("friends").Child(currentUserId);

        friendsRef.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted && task.Result.Exists)
            {
                foreach (DataSnapshot snapshot in task.Result.Children)
                {
                    string friendId = snapshot.Key;
                    DatabaseReference friendStatusRef = databaseRef.Child("status").Child(friendId);
                    friendStatusRef.ValueChanged += (sender, args) =>
                    {
                        HandleFriendStatusChanged(friendId, args.Snapshot.Value.ToString());
                    };
                }
            }
            else
            {
                Debug.LogError("Error al cargar la lista de amigos: " + task.Exception);
            }
        });
    }

    private void HandleFriendStatusChanged(string friendId, string status)
    {
        LoadFriendInfo(friendId, status);
        StartCoroutine(ShowNotificationPanel());
    }

    private void LoadFriendInfo(string friendId, string status)
    {
        databaseRef.Child("users").Child(friendId).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted && task.Result.Exists)
            {
                string username = task.Result.Child("username").Value.ToString();
                GameObject friendEntry = Instantiate(friendEntryPrefab, notificationPanelTransform);
                FriendEntry friendEntryScript = friendEntry.GetComponent<FriendEntry>();
                friendEntryScript.SetFriendData(friendId, username);
            }
            else
            {
                Debug.LogError("Error al cargar la información del amigo: " + task.Exception);
            }
        });
    }

    private IEnumerator ShowNotificationPanel()
    {
        notificationPanel.SetActive(true);
        yield return new WaitForSeconds(2);
        notificationPanel.SetActive(false);

        // Eliminar los hijos del panel para preparar la siguiente notificación
        foreach (Transform child in notificationPanelTransform)
        {
            Destroy(child.gameObject);
        }
    }
}
