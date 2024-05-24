using Firebase.Auth;
using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SignUpController : MonoBehaviour
{
    [SerializeField]
    private Button _registrationButton;

    private Coroutine _signupCoroutine;

    [SerializeField]
    private TMP_InputField _usernameInputField;

    private DatabaseReference mDatabaseRef;

    [SerializeField]
    private GameObject _SignUpPanel;
    [SerializeField]
    private GameObject _GamePanel;

    void Reset()
    {
        _registrationButton = GameObject.Find("SignUpButton").GetComponent<Button>();
        _usernameInputField = GameObject.Find("UserNameSignUp").GetComponent<TMP_InputField>();
    }
    private void Start()
    {
        _registrationButton.onClick.AddListener(HandleRegisterButtonClicked);
        mDatabaseRef = FirebaseDatabase.DefaultInstance.RootReference;
    }

    private void HandleRegisterButtonClicked()
    {
        Debug.Log("HandleRegisterButtonClicked");
        string email = GameObject.Find("EmailSignUp").GetComponent<TMP_InputField>().text;
        string password = GameObject.Find("PasswordSignUp").GetComponent<TMP_InputField>().text;
        _signupCoroutine = StartCoroutine(RegisterUser(email, password));
        Debug.Log(email + " " + password);
    }
    private IEnumerator RegisterUser(string email, string password)
    {
        var auth = FirebaseAuth.DefaultInstance;
        var registerTask = auth.CreateUserWithEmailAndPasswordAsync(email, password);

        yield return new WaitUntil(() => registerTask.IsCompleted);

        if (registerTask.IsCanceled)
        {
            Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled");
        }
        else if (registerTask.IsFaulted)
        {
            Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error" + registerTask.Exception);
        }
        else
        {
            AuthResult result = registerTask.Result;
            Debug.LogFormat("Firebase user created succesfully: ´{0} ({1})", result.User.DisplayName, result.User.UserId);

            mDatabaseRef.Child("users").Child(result.User.UserId).Child("username").SetValueAsync(_usernameInputField.text);
            _SignUpPanel.SetActive(false);
            _GamePanel.SetActive(true);
        }

    }
}
