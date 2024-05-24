using Firebase.Auth;
using Firebase.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginController : MonoBehaviour
{
    [SerializeField]
    private Button _loginButton;
    [SerializeField]
    private Button _SignUpButton;
    [SerializeField]
    private GameObject _LoginPanel;
    [SerializeField]
    private GameObject _SignUpPanel;
    [SerializeField]
    private GameObject _ResetPanel;
    [SerializeField]
    private GameObject _GamePanel;
    [SerializeField]
    private TMP_InputField _emailInputField;
    [SerializeField]
    private TMP_InputField _passwordInputField;

    void Reset()
    {
        _loginButton = GameObject.Find("LoginButton").GetComponent<Button>();
        _SignUpButton = GameObject.Find("SignUpLButton").GetComponent<Button>();
        _emailInputField = GameObject.Find("EmailLogin").GetComponent<TMP_InputField>();
        _passwordInputField = GameObject.Find("PasswordLogin").GetComponent<TMP_InputField>();
        _LoginPanel = GameObject.Find("Login").GetComponent<GameObject>();
        _SignUpPanel = GameObject.Find("SignUp").GetComponent<GameObject>();
        _GamePanel = GameObject.Find("GamePanel").GetComponent<GameObject>();
    }
    void Start()
    {
        _loginButton.onClick.AddListener(HandleLoginButtonClicked);
        _SignUpButton.onClick.AddListener(HandleSignUpButtonClicked);
        FirebaseAuth.DefaultInstance.StateChanged += HandleAuthStateChange;
    }

    private void HandleLoginButtonClicked()
    {
        var auth = FirebaseAuth.DefaultInstance;
        auth.SignInWithEmailAndPasswordAsync(_emailInputField.text, _passwordInputField.text).ContinueWith(task => {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                return;
            }


            Firebase.Auth.AuthResult result = task.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})",
                result.User.DisplayName, result.User.UserId);

        });
    }
    private void HandleSignUpButtonClicked()
    {
        _LoginPanel.SetActive(false);
        _SignUpPanel.SetActive(true);
    }
    private void HandleAuthStateChange(object sender, EventArgs e)
    {
        if (FirebaseAuth.DefaultInstance.CurrentUser != null)
        {
            _LoginPanel.SetActive(false);
            _GamePanel.SetActive(true);
            string userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
            UpdateUserStatus(userId, "online");
        }
    }
    private void UpdateUserStatus(string userId, string status)
    {
        DatabaseReference statusRef = FirebaseDatabase.DefaultInstance.GetReference("status").Child(userId);
        statusRef.SetValueAsync(status);
    }
    void OnDestroy()
    {
        FirebaseAuth.DefaultInstance.StateChanged -= HandleAuthStateChange;
    }

}
