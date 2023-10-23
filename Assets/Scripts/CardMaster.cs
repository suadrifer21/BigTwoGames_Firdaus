using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardMaster : MonoBehaviour
{
    private IGameController gameController;
    [SerializeField]
    private GameObject cardPrefab;

    private List<PlayerEntity> players;
    List<CardData> cardDatas = new List<CardData>();
    [SerializeField]
    private List<CardView> deck;
       

    public void SetGameController(IGameController gameController)
    {
        this.gameController = gameController;
        players = gameController.GetPlayers();
    }

    public void CreateDeck()
    {
        cardDatas.AddRange(Resources.LoadAll<CardData>("Cards/Diamonds").ToList());
        cardDatas.AddRange(Resources.LoadAll<CardData>("Cards/Clubs").ToList());
        cardDatas.AddRange(Resources.LoadAll<CardData>("Cards/Hearts").ToList());
        cardDatas.AddRange(Resources.LoadAll<CardData>("Cards/Spades").ToList());

        for(int i = 0; i<cardDatas.Count;i++)
        {
            GameObject card = Instantiate(cardPrefab);
            CardView cardView = card.GetComponent<CardView>();

            cardView.InitializeView(cardDatas[i]);
            deck.Add(cardView);
        }

        ShuffleExtension.Shuffle(deck);

        DistributeCard();


        
    }
     void DistributeCard()
    {
        //PlayerEntity owner;
        List<PlayerEntity> players = gameController.GetPlayers();

        for (int i = 0; i < deck.Count; i++)
        {
            if (i < 13)
            {
                //owner = players[0];
                deck[i].transform.SetParent(players[0].HandArea);
                deck[i].transform.localRotation = Quaternion.Euler(Vector3.zero);
                deck[i].SetShowCard(true);
                deck[i].SetInteractable(true);
                players[0].AddCardToHand(deck[i]);

            }
            else if (i < 26)
            {
                //owner = players[1];
                //deck[i].owner = owner;
                deck[i].transform.SetParent(players[1].HandArea);
                deck[i].transform.localRotation = Quaternion.Euler(Vector3.zero);
                deck[i].SetShowCard(false);
                deck[i].SetInteractable(false);
                players[1].AddCardToHand(deck[i]);
            }
            else if (i < 39)
            {
               // owner = players[2];
                //deck[i].owner = owner;
                deck[i].transform.SetParent(players[2].HandArea);
                deck[i].transform.localRotation = Quaternion.Euler(Vector3.zero);
                deck[i].SetShowCard(false);
                deck[i].SetInteractable(false);
                players[2].AddCardToHand(deck[i]);
            }
            else
            {
                //owner = players[3];
                //deck[i].owner = owner;
                deck[i].transform.SetParent(players[3].HandArea);
                deck[i].transform.localRotation = Quaternion.Euler(Vector3.zero);
                deck[i].SetShowCard(false);
                deck[i].SetInteractable(false);
                players[3].AddCardToHand(deck[i]);
            }
        }

        deck.Clear();

        GameController.Instance.TurnController.InitializeGame();
    }

    public void RetrieveAllCards()
    {
        for(int i = 0; i < players.Count; i++)
        {
            deck.AddRange(players[i].GetCardsInHand());
            players[i].ClearHands();

            for(int j = 0; j < players[i].PlayArea.childCount; j++)
            {
                deck.Add(players[i].PlayArea.GetChild(j).GetComponent<CardView>());
                players[i].PlayArea.GetChild(j).gameObject.SetActive(true);
            }
        }

        print("Start New game");
        ShuffleExtension.Shuffle(deck);

        DistributeCard();
    }

}
