using Firebase.Database;
using TMPro;
using UnityEngine;

public class FriendEntry : MonoBehaviour
{
    [SerializeField] private TMP_Text usernameText;
    [SerializeField] private TMP_Text statusText; // Para mostrar el estado de conexión

    private string friendId;
    private DatabaseReference statusRef;

    public void SetFriendData(string friendId, string username)
    {
        this.friendId = friendId;
        usernameText.text = username;

        statusRef = FirebaseDatabase.DefaultInstance.GetReference("status").Child(friendId);
        statusRef.ValueChanged += HandleStatusChanged;
    }

    private void HandleStatusChanged(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }

        string newStatus = args.Snapshot.GetValue(true).ToString();
        statusText.text = newStatus;
    }
}
