using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPlayer : PlayerEntity
{
    [SerializeField]
    private List<ComboOutput> combo = new List<ComboOutput>();
    
    public void CheckCombo(int turn)
    {
        combo = RuleData.GetAllAvailableCombo(turn, HandData);
    }
    public void DecideCombo(int turn)
    {
        toPlay = null;

        if (turn <= 1)
        {
            int value = combo[0].basedValue;
            int index = 0;
            for (int i = 0; i < combo.Count; i++)
            {
                if (combo[i].basedValue < value)
                {
                    value = combo[i].basedValue;
                    index = i;
                }
            }
            /*
            for (int i = 0; i < combo.Count; i++){
                for(int j = 0; j < combo[i].availableCards.Count; j++)
                {
                    if(combo[i].availableCards[j].GetRank() == CardRank.Three && combo[i].availableCards[j].GetSuit() == CardSuit.Diamond)
                    {
                        print(combo[i].basedValue);
                        if (combo[i].basedValue > value)
                        {
                            index = i;
                            value = combo[i].basedValue;
                        }
                        break;
                    }
                }
            }
            */
            toPlay = combo[index];
        }
        else
        {
            //lowest value non single, klo g ada baru single
            int valueSingle = combo[0].basedValue;
            int valueNonSingle = 0;
            int indexSingle = 0;
            int indexNonSingle = 0;
            for (int i = 0; i < combo.Count; i++)
            {
                if (combo[i].name.Equals("Single"))
                {
                    if (combo[i].basedValue < valueSingle)
                    {
                        indexSingle = i;
                    }
                }
                else
                {
                    if (valueNonSingle == 0 || combo[i].basedValue < valueNonSingle)
                    {
                        valueNonSingle = combo[i].basedValue;
                    }
                }
            }

            if (valueNonSingle != 0)
                toPlay = combo[indexNonSingle];
            else
                toPlay = combo[indexSingle];
        }
        SetCardsToPlay();
    }
    public void MatchComboInTable()
    {
        toPlay = null;
        for (int i = 0; i < combo.Count; i++)
        {
            if (RuleData.CanBeatCardOnTable(combo[i], turnController.CardOnTable))
                toPlay = combo[i];
        }
        SetCardsToPlay();
    }
}
