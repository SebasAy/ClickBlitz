using TMPro;
using UnityEngine;

public class FriendEntry : MonoBehaviour
{
    [SerializeField] private TMP_Text usernameText;

    private string friendId;

    public void SetFriendData(string friendId, string username)
    {
        this.friendId = friendId;
        usernameText.text = username;
    }
}
