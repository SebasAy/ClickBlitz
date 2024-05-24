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

    private List<string> matchmakingQueue = new List<string>();

    private bool isMatchmaking = false;

    private void Start()
    {
        _matchmakingButton.onClick.AddListener(HandleMatchmakingButtonClicked);
        mDatabaseRef = FirebaseDatabase.DefaultInstance.RootReference;

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
            // Agregar al jugador actual a la cola de matchmaking
            string userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
            matchmakingQueue.Add(userId);
            isMatchmaking = true;
            _matchmakingButton.interactable = false;

            // Mostrar que se est� buscando partida
            
            _textMP2.text = "Searching for match...";

            // Comprobar si hay suficientes jugadores en la cola
            CheckMatchmakingQueue();
        }
    }

    private void CheckMatchmakingQueue()
    {
        if (matchmakingQueue.Count >= 2)
        {
            StartCoroutine(MatchmakePlayers());
        }
        else
        {
            Debug.Log("Waiting for more players in the matchmaking queue...");
        }
    }

    private IEnumerator MatchmakePlayers()
    {
        // Emparejar jugadores (aqu� puedes implementar tu l�gica de emparejamiento)
        string player1Id = matchmakingQueue[0];
        string player2Id = matchmakingQueue[1];

        // Obtener los nombres de los jugadores emparejados
        string player1Name = "";
        string player2Name = "";

        yield return GetPlayerName(player1Id, name => player1Name = name);
        yield return GetPlayerName(player2Id, name => player2Name = name);

        // Mostrar los nombres de los jugadores emparejados
        _textMP1.text = "Match found! Player 1: " + player1Name;
        _textMP2.text = "Match found! Player 2: " + player2Name;

        // Limpiar la cola de matchmaking
        matchmakingQueue.Clear();
        isMatchmaking = false;
        _matchmakingButton.interactable = true;
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
