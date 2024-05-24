using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RequestEntry : MonoBehaviour
{
    [SerializeField] private TMP_Text usernameText;
    [SerializeField] private Button acceptButton;
    [SerializeField] private Button declineButton;

    private string requestId;
    private string senderId;
    private System.Action<string, string, string> acceptCallback;
    private System.Action<string> declineCallback;

    public void SetRequestData(string requestId, string senderId, string senderUsername, System.Action<string, string, string> acceptCallback, System.Action<string> declineCallback)
    {
        this.requestId = requestId;
        this.senderId = senderId;
        this.acceptCallback = acceptCallback;
        this.declineCallback = declineCallback;
        usernameText.text = senderUsername;

        acceptButton.onClick.AddListener(() => acceptCallback(requestId, senderId, senderUsername));
        declineButton.onClick.AddListener(() => declineCallback(requestId));
    }
}
