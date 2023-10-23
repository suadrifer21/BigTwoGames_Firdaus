using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnController : MonoBehaviour
{
    private IGameController gameController;
    private GameState currentState;
    private int currentPlayerIndex;
    private int starterPlayerIndex;
    private int turn;
    private bool isNewRound;
    [SerializeField]
    private ComboOutput cardOnTable = null;
    private List<PlayerEntity> players;
    public ComboOutput CardOnTable { get => cardOnTable; set => cardOnTable = value; }
    public bool IsNewRound { get => isNewRound; }
    public event EventHandler<OnTurnChangedEventArgs> OnTurnChanged;
    public event EventHandler<OnComboSetEventArgs> OnComboSet;
    public event EventHandler<OnPlayerPassEventArgs> OnPlayerPass;
    public event GameEndDelegate GameEndEvent;
    public class OnTurnChangedEventArgs
    {
        public int playerIndex;
    }
    public class OnComboSetEventArgs
    {
        public string comboName;
    }
    public class OnPlayerPassEventArgs
    {
        public int playerIndex;
        public bool isPass;
    }
    public int GetTurn()
    {
        return turn;
    }

    public void SetGameController(IGameController gameController)
    {
        this.gameController = gameController;
    }

    public void InitializeGame()
    {
        players = gameController.GetPlayers();

        // Set initial state and player
        currentState = GameState.InitializingGame;
        currentPlayerIndex = 0;
        starterPlayerIndex = 0;
        turn = 1;
        isNewRound = false;

        for (int i = 0; i< players.Count; i++)
        {
            List<CardData> playersHand = players[i].GetHandData();
            foreach(CardData card in playersHand)
            {
                if (card.GetRank() == CardRank.Three && card.GetSuit() == CardSuit.Diamond)
                {
                    currentPlayerIndex = i;
                    starterPlayerIndex = i;
                    break;
                }
            }
        }


        Invoke("StartPlayerTurn",1f);
    }

    void RestartedRound()
    {
        OnComboSet?.Invoke(this, new OnComboSetEventArgs { comboName = "" });
        currentState = GameState.StartRound;
        CardOnTable = null;
        isNewRound = true;
        for(int i= 0; i< players.Count;i++)
        {
            players[i].IsPass = false;
            OnPlayerPass?.Invoke(this, new OnPlayerPassEventArgs { playerIndex = i, isPass = false });
            for (int j = 0; j< players[i].PlayArea.childCount; j++)
            {
                players[i].PlayArea.GetChild(j).gameObject.SetActive(false);
            }
        }
        StartPlayerTurn();
    }
    void StartPlayerTurn()
    {
        currentState = GameState.PlayerTurn;
        OnTurnChanged?.Invoke(this, new OnTurnChangedEventArgs { playerIndex = currentPlayerIndex+1 });
        Debug.Log($"Player {currentPlayerIndex + 1}'s turn");

        //players[currentPlayerIndex].IsPlayer;


        if (players[currentPlayerIndex] is AIPlayer)
        {
            AIPlayer ai = (AIPlayer)players[currentPlayerIndex];
            ai.CheckCombo(turn);
            if (turn == 1 || IsNewRound)
                ai.DecideCombo(turn);
            else
                ai.MatchComboInTable();
        }

    }

    public void EndPlayerTurn()
    {
        currentState = GameState.WaitForCardPlay;
        OnComboSet?.Invoke(this, new OnComboSetEventArgs { comboName = cardOnTable.name });
        starterPlayerIndex = currentPlayerIndex;

        if (players[currentPlayerIndex].GetCardsInHand().Count > 0)
            NextPlayerTurn();
        else
            SettlesRound();
    }

    public void NextPlayerTurn()
    {       
        currentState = GameState.NextPlayer;
        OnPlayerPass?.Invoke(this, new OnPlayerPassEventArgs { playerIndex = currentPlayerIndex, isPass = players[currentPlayerIndex].IsPass});
        Debug.Log($"Moving to the next player");
        turn++;
        isNewRound = false;

        currentPlayerIndex = (currentPlayerIndex + 1) % players.Count;
        if (players[currentPlayerIndex].IsPass)
        {
            NextPlayerTurn();
        }else if (currentPlayerIndex == starterPlayerIndex)
        {
            RestartedRound();
        } else        
            Invoke("StartPlayerTurn", 1f);
    }
    void SettlesRound()
    {
        currentState = GameState.GameOver;
        CardOnTable = null;
        Debug.Log($"Player {currentPlayerIndex + 1}' WIN");

        for(int i = 0; i < players.Count; i++)
        {
            int point = CalculatePoint(i);
            gameController.UpdatePlayerRecords(i, point, players[i].GetCardsInHand().Count);
        }

        GameEndEvent?.Invoke();
        //GameController.Instance.CardMaster.RetrieveAllCards();
    }
    int CalculatePoint(int playerIndex)
    {
        var cardLeft = players[playerIndex].GetCardsInHand().Count;
        if (cardLeft >= 13)
        {
            return cardLeft * -3;
        }
        else if (cardLeft >= 10)
        {
            return cardLeft * -2;
        }
        else if (cardLeft > 0)
        {
            return cardLeft * -1;
        }
        else
        {
            var point = 0;
            for (int i = 0; i < players.Count; ++i)
            {
                if (i != playerIndex)
                    point += CalculatePoint(i);
            }
            return Mathf.Abs(point);
        }
    }

}

public delegate void GameEndDelegate();

public enum GameState
{
    InitializingGame,
    StartRound,
    PlayerTurn,
    WaitForCardPlay,
    NextPlayer,
    GameOver
}