using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour,IGameController
{
    public static GameController Instance;

    [SerializeField]
    private List<PlayerEntity> Players;    
    private CardMaster cardMaster;
    private TurnController turnController;
    [SerializeField]
    private List<PlayerRecord> playerRecords;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        turnController = GetComponent<TurnController>();
        cardMaster = GetComponent<CardMaster>();

        turnController.SetGameController(this);
        cardMaster.SetGameController(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        StartGame();
    }
    public void StartGame()
    {
        cardMaster.CreateDeck();
    }
    public void Restartgame()
    {
        cardMaster.RetrieveAllCards();
    }
    public List<PlayerEntity> GetPlayers()
    {
        return Players;
    }
    public TurnController TurnController { get => turnController; }
    public CardMaster CardMaster { get => cardMaster; }
    public void UpdatePlayerRecords(int index, int point, int cardRemains)
    {
        playerRecords[index].thisRoundPoints = point;
        playerRecords[index].totalPoints += point;
        playerRecords[index].thisRoundRemains = cardRemains;
    }
    public PlayerRecord GetPlayerRecords(int index)
    {
        return playerRecords[index];
    }
}

public interface IGameController
{
    List<PlayerEntity> GetPlayers();
    void StartGame();
    void Restartgame();
    void UpdatePlayerRecords(int index, int point, int cardRemains);
    PlayerRecord GetPlayerRecords(int index);

}

[Serializable]
public class PlayerRecord{
    public int thisRoundRemains;
    public int thisRoundPoints;
    public int totalPoints;
}


