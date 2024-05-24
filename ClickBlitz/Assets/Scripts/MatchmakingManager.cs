using Firebase.Auth;
using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MatchmakingController : MonoBehaviour
{
    [SerializeField] private TMP_Text _textMP1;
    [SerializeField] private TMP_Text _textMP2;
    [SerializeField] private Button _matchmakingButton;

    private DatabaseReference mDatabaseRef;
    private DatabaseReference mMatchmakingQueueRef;

    private bool isMatchmaking = false;

    private void Start()
    {
        _matchmakingButton.onClick.AddListener(HandleMatchmakingButtonClicked);
        mDatabaseRef = FirebaseDatabase.DefaultInstance.RootReference;
        mMatchmakingQueueRef = mDatabaseRef.Child("matchmakingQueue");

        // Escuchar cambios en la cola de matchmaking
        mMatchmakingQueueRef.ValueChanged += HandleMatchmakingQueueChanged;

        // Obtener y mostrar el nombre de usuario al iniciar el juego
        if (FirebaseAuth.DefaultInstance.CurrentUser != null)
        {
            string userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
            StartCoroutine(GetPlayerName(userId, name => _textMP1.text = "Welcome, " + name));
        }
    }

    private void HandleMatchmakingButtonClicked()
    {
        if (!isMatchmaking)
        {
            // Agregar al jugador actual a la cola de matchmaking en Firebase
            string userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
            mMatchmakingQueueRef.Child(userId).SetValueAsync(true);
            isMatchmaking = true;
            _matchmakingButton.interactable = false;
        }
    }

    private void HandleMatchmakingQueueChanged(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError("Database error: " + args.DatabaseError.Message);
            return;
        }

        if (args.Snapshot != null && args.Snapshot.ChildrenCount >= 2)
        {
            // Hay al menos dos jugadores en la cola, iniciar emparejamiento
            StartCoroutine(MatchmakePlayers());
        }
    }

    private IEnumerator MatchmakePlayers()
    {
        // Implementa tu lógica de emparejamiento aquí

        // Una vez emparejados, limpia la cola de matchmaking en Firebase
        mMatchmakingQueueRef.RemoveValueAsync();

        yield return null;
    }

    private IEnumerator GetPlayerName(string playerId, System.Action<string> callback)
    {
        var playerDataTask = mDatabaseRef.Child("users").Child(playerId).Child("username").GetValueAsync();

        yield return new WaitUntil(() => playerDataTask.IsCompleted);

        if (playerDataTask.Exception != null)
        {
            Debug.LogError("Failed to get player data: " + playerDataTask.Exception);
        }
        else if (playerDataTask.Result.Value != null)
        {
            string playerName = playerDataTask.Result.Value.ToString();
            callback(playerName);
        }
    }
}
