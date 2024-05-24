using TMPro;
using UnityEngine;

public class FriendEntry : MonoBehaviour
{
    [SerializeField] private TMP_Text usernameText;
    [SerializeField] private TMP_Text statusText;

    public void SetFriendData(string username, bool isOnline)
    {
        {
            usernameText.text = username;
            statusText.text = isOnline ? "On" : "Off";
        }
    }
}
