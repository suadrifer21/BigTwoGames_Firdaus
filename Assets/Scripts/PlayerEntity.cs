using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEntity : MonoBehaviour
{
    /*
    [SerializeField]
    private bool isUser;
    */
    [SerializeField]
    private Transform handArea;
    [SerializeField]
    protected Transform playArea;
    [SerializeField]
    protected List<CardView> CardsInHand = new List<CardView>();
    protected List<CardData> HandData = new List<CardData>();
    
    [SerializeField]
    protected ComboOutput toPlay = null;
    private bool isPass;
    protected TurnController turnController;
    /*
    public bool IsUser { get => isUser; set => isUser = value; }
    */
    public Transform HandArea { get => handArea; }
    public Transform PlayArea { get => playArea; }
    public bool IsPass { get => isPass; set => isPass = value; }
    private void Start()
    {
        turnController = GameController.Instance.TurnController;
    }
    public List<CardView> GetCardsInHand()
    {
        return CardsInHand;
    }
    public List<CardData> GetHandData()
    {
        return HandData;
    }

    public virtual void AddCardToHand(CardView card)
    {
        CardsInHand.Add(card);
        HandData.Add(card.GetData());             
    }
    public virtual void ClearHands()
    {      
        CardsInHand.Clear();
        HandData.Clear();        
    }
    public virtual void PlayCards(CardView card)
    {
        CardsInHand.Remove(card);
        HandData.Remove(card.GetData());
        card.transform.SetParent(playArea);
        card.transform.localScale = Vector3.one;
        card.SetShowCard(true);
        card.SetInteractable(false);
    }
    protected virtual void SetCardsToPlay()
    {
        if (toPlay== null)
        {
            isPass = true;
            turnController.NextPlayerTurn();
            return;
        }

        turnController.CardOnTable = toPlay;

        print(toPlay.name + " " + toPlay.availableCards[0].name);

        for (int i = 0; i < toPlay.availableCards.Count; i++)
        {
            for (int j = 0; j < CardsInHand.Count; j++)
            {
                if (toPlay.availableCards[i].GetCardName().Equals(CardsInHand[j].gameObject.name))
                {
                    PlayCards(CardsInHand[j]);
                    j--;
                }
            }
        }

        turnController.EndPlayerTurn();
    }    
}
