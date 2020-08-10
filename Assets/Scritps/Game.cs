using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TVD;

public class Game : MonoBehaviourPun, IPunObservable
{
    public Card_Animator cardAnimator;
    [SerializeField]
    ProtectedData protectedData;
    [SerializeField]
    TVD.Player localPlayer;
    [SerializeField]
    TVD.Player remotePlayer;
    [SerializeField]
    TVD.Player currentTurnPlayer;

    public List<Transform> PlayerPositions = new List<Transform>();

    public enum GameState
    {
        Idle,
        GameStarted,
        TurnStarted,
        GameFinished
    }

    [SerializeField]
    GameState gameState = GameState.Idle;

    public GameObject panelStart = null;

    public static Game GameInstance;
    public static Game Instance
    {
        get
        {
            if (GameInstance == null)
            {
                GameInstance = FindObjectOfType<Game>();
                if (GameInstance == null)
                {
                    GameObject obj = new GameObject();
                    GameInstance = obj.AddComponent<Game>();
                }
            }
            return GameInstance;
        }
    }

    void Awake()
    {
        GameObject.DontDestroyOnLoad(gameObject);

        foreach (Photon.Realtime.Player PTPlayer in PhotonNetwork.PlayerList)
        {
            string playerName = PTPlayer.NickName;
            string playerId = PTPlayer.UserId;

            if (playerId == PhotonNetwork.LocalPlayer.UserId)
            {
                localPlayer.PlayerId = playerId;
                localPlayer.PlayerName = playerName;
                localPlayer.Position = PlayerPositions[0].position;
            }
            else
            {
                remotePlayer.PlayerId = playerId;
                remotePlayer.PlayerName = playerName;
                remotePlayer.Position = PlayerPositions[1].position;
            }

        }

        protectedData = new ProtectedData(localPlayer.PlayerId, remotePlayer.PlayerId);

        cardAnimator = FindObjectOfType<Card_Animator>();
    }

    void Start()
    {
        gameState = GameState.GameStarted;
        GameFlow();
    }

    
    private void GameFlow()
    {
        switch (gameState)
        {
            case GameState.Idle:
                break;
            case GameState.GameStarted:
                OnGameStarted();
                break;
            case GameState.TurnStarted:
                break;
            case GameState.GameFinished:
                break;
        }
    }

    public void OnGameStarted()
    {
        Shuffle();

        List<TVD.Player> players = new List<TVD.Player>();
        players.Add(localPlayer);
        players.Add(remotePlayer);

        DealCardValuesToPlayers(players, Constants.PLAYER_INITIAL_CARDS);
        cardAnimator.DealDisplayingCards(players, Constants.PLAYER_INITIAL_CARDS);
    }

    public void DealCardValuesToPlayers(List<TVD.Player> players, int numberOfCards)
    {
        List<byte> poolOfCards = protectedData.GetPoolOfCards();

    }

    public void Shuffle()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            List<byte> cardValues = new List<byte>();

            for (byte i = 0; i < 52; i++)
            {
                cardValues.Add(i);
            }

            int minPos;
            int maxPos = cardValues.Count - 1;
            int swapPos;

            for (int i = 0; i < maxPos; i++)
            {
                minPos = i + 1;
                swapPos = Random.Range(minPos, maxPos);

                byte temp = cardValues[i];
                cardValues[i] = cardValues[swapPos];
                cardValues[swapPos] = temp;
            }

            protectedData.SetPoolOfCards(cardValues.ToArray());

        }
    }

    public void OnCardSelected(Card card)
    {
        if (localPlayer.PlayerId == card.OwnerId)
        {
            card.OnSelected();
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(protectedData.GetPoolOfCards().ToArray());
        }
        else if (stream.IsReading)
        {
            protectedData.SetPoolOfCards((byte[])stream.ReceiveNext());
        }
    }

    public void ShowAndHidePlayersDisplayingCards()
    {
        protectedData.AddCardValuesToPlayer(localPlayer);
        protectedData.AddCardValuesToPlayer(remotePlayer);
        localPlayer.ShowCardValues();
        remotePlayer.HideCardValues();
    }
}
