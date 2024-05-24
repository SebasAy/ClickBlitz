using Firebase.Auth;
using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class LogOutButton : MonoBehaviour , IPointerClickHandler
{
    [SerializeField]
    private GameObject _LoginPanel;
    [SerializeField]
    private GameObject _GamePanel;

    public void OnPointerClick(PointerEventData eventData)
    {
        string userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        UpdateUserStatus(userId, "offline");
        FirebaseAuth.DefaultInstance.SignOut();
        _GamePanel.SetActive(false);
        _LoginPanel.SetActive(true);
    }
    private void UpdateUserStatus(string userId, string status)
    {
        DatabaseReference statusRef = FirebaseDatabase.DefaultInstance.GetReference("status").Child(userId);
        statusRef.SetValueAsync(status);
    }
}
