using Firebase.Auth;
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

    void Reset()
    {
        _GamePanel = GameObject.Find("Juego").GetComponent<GameObject>();
        _LoginPanel = GameObject.Find("Login").GetComponent<GameObject>();
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        FirebaseAuth.DefaultInstance.SignOut();
        _GamePanel.SetActive(false);
        _LoginPanel.SetActive(true);
    }
}
