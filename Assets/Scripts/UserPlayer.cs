using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserPlayer:PlayerEntity
{
    private List<CardView> selectedCard = new List<CardView>();
    void SelectCard(object sender, CardView.OnCardClickedEventArgs e)
    {
        if (e.isSelected)
            selectedCard.Add(e.cardView);
        else
            selectedCard.Remove(e.cardView);
    }
    public void PlaySelectedCard()
    {
        if (selectedCard.Count < 1)
            return;

        toPlay = null;
        List<CardData> selectedData = new List<CardData>();
        foreach (CardView card in selectedCard)
        {
            selectedData.Add(card.GetData());
        }
        toPlay = RuleData.GetAvailableCombo(turnController.GetTurn(), selectedData);

        if (toPlay == null)
            print("NOT VALID");
        else if (turnController.GetTurn() == 1 || turnController.IsNewRound)
        {
            SetCardsToPlay();
        }
        else
        {
            if (RuleData.CanBeatCardOnTable(toPlay, turnController.CardOnTable))
                SetCardsToPlay();
            else
                print("NOT VALID");
        }
    }
    public override void AddCardToHand(CardView card)
    {
        card.OnCardClicked += SelectCard;
        base.AddCardToHand(card);
    }
    public override void ClearHands()
    {
        foreach (CardView card in CardsInHand)
        {
            card.OnCardClicked -= SelectCard;
        }
        base.ClearHands();
    }
    public override void PlayCards(CardView card)
    {
        base.PlayCards(card);
        card.OnCardClicked -= SelectCard;
    }
    protected override void SetCardsToPlay()
    {
        base.SetCardsToPlay();
        selectedCard.Clear();
    }
    public void PassButton()
    {
        IsPass = true;
        GameController.Instance.TurnController.NextPlayerTurn();
    }
}
