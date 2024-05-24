using Firebase.Auth;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ResetController : MonoBehaviour
{
    [SerializeField]
    private Button _ResetButton;
    [SerializeField]
    private GameObject _LoginPanel;
    [SerializeField]
    private GameObject _ResetPanel;
    [SerializeField]
    private TMP_InputField _emailInputField;

    void Reset()
    {
        _ResetButton = GameObject.Find("ResetButton").GetComponent<Button>();
        _emailInputField = GameObject.Find("EmailReset").GetComponent<TMP_InputField>();
        _LoginPanel = GameObject.Find("Login").GetComponent<GameObject>();
        _ResetPanel = GameObject.Find("Reset").GetComponent<GameObject>();
    }
    void Start()
    {
        _ResetButton.onClick.AddListener(HandleResetButtonClicked);
    }

    private void HandleResetButtonClicked()
    {
        string email = _emailInputField.text;

        if (string.IsNullOrEmpty(email))
        {
            Debug.LogWarning("Please enter your email address.");
            return;
        }

        var auth = FirebaseAuth.DefaultInstance;
        auth.SendPasswordResetEmailAsync(email).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("SendPasswordResetEmailAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SendPasswordResetEmailAsync encountered an error: " + task.Exception);
                return;
            }
            Debug.Log("Password reset email sent successfully.");
        });

        _ResetPanel.SetActive(false);
        _LoginPanel.SetActive(true);
    }
}
