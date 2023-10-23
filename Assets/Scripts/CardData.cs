using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Big Two/Card Data", order = 0)]
public class CardData : ScriptableObject
{
  

    [SerializeField] private CardSuit suit = CardSuit.Diamond;
    [SerializeField] private CardRank rank = CardRank.Three;
    [SerializeField] private Sprite sprite = null;

    public CardRank GetRank()
    {
        return rank;
    }

    public CardSuit GetSuit()
    {
        return suit;
    }

    public string GetCardName()
    {
        return rank.ToString() + " " + suit.ToString();
    }

    public Sprite GetSprite()
    {
        return sprite;
    }
}

public enum CardSuit : int
{
    Diamond = 0,
    Club = 1,
    Heart = 2,
    Spade = 3
}
public enum CardRank : int
{
    Three = 0,
    Four = 1,
    Five = 2,
    Six = 3,
    Seven = 4,
    Eight = 5,
    Nine = 6,
    Ten = 7,
    Jack = 8,
    Queen = 9,
    King = 10,
    Ace = 11,
    Two = 12
}