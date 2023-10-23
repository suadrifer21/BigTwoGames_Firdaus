using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Rule", menuName = "Big Two/Rule Data", order = 1)]
public class RuleData : ScriptableObject
{   
    public static bool ReshufflingByCards(List<CardData> input)
    {
        List<CardData> analyze = new List<CardData>(input);
        int numOfAll = analyze.Where(data => data.GetRank() == CardRank.Two).Count();

        if (numOfAll >= 4)
            return true;

        return false;
    }

    public static int GetWeightByCardData(CardData input)
    {
        int wRank = GetRankByRule(input);
        int wSuit = (int) input.GetSuit() + 1;
        return wRank * 10 + wSuit;
    }

    public static int GetRankByRule(CardData input)
    {
        return (int) input.GetRank() + 1;
    }

    public static ComboOutput GetAvailableCombo(int numOfTurn, List<CardData> input)
    {
        List<CardData> analyze = new List<CardData>(input);

        if (numOfTurn <= 1)
        {
            bool containDiamon3 = false;
            foreach (CardData card in analyze)
            {
                if (card == null)
                    continue;

                if (card.GetRank() == CardRank.Three && card.GetSuit() == CardSuit.Diamond)
                {
                    containDiamon3 = true;
                    break;
                }
            }

            if (!containDiamon3)
                return null;
        }

        Debug.Log(analyze.Count);

        ComboOutput output;
        output = CheckForFullHouse(analyze);
        if (output != null)
            return output;

        output = CheckForStraightFlush(analyze);
        if (output != null)
            return output;

        output = CheckForFourKind(analyze);
        if (output != null)
            return output;

        output = CheckForFlush(analyze);
        if (output != null)
            return output;

        output = CheckForStraight(analyze);
        if (output != null)
            return output;

        output = CheckForThreeKind(analyze);
        if (output != null)
            return output;

        output = CheckForPair(analyze);
        if (output != null)
            return output;

        if (analyze.Count == 1)
        {
            output = new ComboOutput();
            output.basedNum = 1;
            output.basedValue = analyze.Sum(x => GetWeightByCardData(x));
            output.availableCards = analyze;
            output.name = "Single";

            return output;
        }

        return null;
    }

    public static List<ComboOutput> GetAllAvailableCombo(int numOfTurn, List<CardData> input)
    {
        List<CardData> analyze = new List<CardData>(input);
        List<ComboOutput> result = new List<ComboOutput>();

        if (numOfTurn <= 1)
        {
            bool containDiamond3 = false;
            CardData diamond3Card = null;
            foreach (CardData card in analyze)
            {
                if (card == null)
                    continue;

                if (card.GetRank() == CardRank.Three && card.GetSuit() == CardSuit.Diamond)
                {
                    containDiamond3 = true;
                    diamond3Card = card;
                    break;
                }
            }

            if (!containDiamond3)
                return null;
            else
            {
                ComboOutput output = new ComboOutput();
                output.basedNum = 1;
                output.basedValue = GetWeightByCardData(diamond3Card);
                output.availableCards = new List<CardData> { diamond3Card };
                output.name = "Single";
                result.Add(output);

                return result;
            }
        }

        List<ComboOutput> outputFullHouse = AllFullHouse(analyze);
        if (outputFullHouse.Count > 0)
            result.AddRange(outputFullHouse);

        List<ComboOutput> outputStrFlush = AllStraightFlush(analyze);
        if (outputStrFlush.Count > 0)
            result.AddRange(outputStrFlush);

        List<ComboOutput> output4Kind = AllFourOfAKind(analyze);
        if (output4Kind.Count > 0)
            result.AddRange(output4Kind);

        List<ComboOutput> outputFlush = AllFlush(analyze);
        if (outputFlush.Count > 0)
            result.AddRange(outputFlush);

        List<ComboOutput> outputStr = AllStraight(analyze);
        if (outputStr.Count > 0)
            result.AddRange(outputStr);

        List<ComboOutput> output3Kind = AllThreeOfAKind(analyze);
        if (output3Kind.Count > 0)
            result.AddRange(output3Kind);

        List<ComboOutput> outputPair = AllPossiblePairs(analyze);
        if (outputPair.Count > 0)
            result.AddRange(outputPair);


        foreach (CardData card in analyze)
        {
            if (card == null)
                continue;

            ComboOutput outputEntry = new ComboOutput();
            outputEntry.basedNum = 1;
            outputEntry.basedValue = GetWeightByCardData(card);
            outputEntry.availableCards = new List<CardData> { card };
            outputEntry.name = "Single";
            result.Add(outputEntry);
        }

        return result;
    }

    public static ComboOutput CheckForPair(List<CardData> input, bool ignoreMatchNumer = false)
    {
        List<CardData> analyze = new List<CardData>(input);

        if (analyze.Count != 2 && !ignoreMatchNumer)
            return null;

        bool fetchAnalyzer = analyze.GroupBy(data => GetRankByRule(data)).Any(group => group.Count() == 2);
        if (!fetchAnalyzer)
            return null;
        else
        {
            ComboOutput outputCombo = new ComboOutput();
            outputCombo.basedNum = 2;
            outputCombo.basedValue = analyze.Sum(x => GetWeightByCardData(x));
            outputCombo.availableCards = analyze;
            outputCombo.name = "Pair";
            return outputCombo;
        }
    }

    public static ComboOutput CheckForThreeKind(List<CardData> input, bool ignoreMatchNumer = false)
    {
        List<CardData> analyze = new List<CardData>(input);

        if (analyze.Count != 3 && !ignoreMatchNumer)
            return null;

        bool fetchAnalyzer = analyze.GroupBy(data => GetRankByRule(data)).Any(group => group.Count() == 3);
        if (!fetchAnalyzer)
            return null;
        else
        {
            ComboOutput outputCombo = new ComboOutput();
            outputCombo.basedNum = 3;
            outputCombo.basedValue = analyze.Sum(x => GetWeightByCardData(x));
            outputCombo.availableCards = analyze;
            outputCombo.name = "Three of a kind";
            return outputCombo;
        }
    }

    public static ComboOutput CheckForStraight(List<CardData> input, bool ignoreMatchNumer = false)
    {
        List<CardData> analyze = new List<CardData>(input);
        if (analyze.Count != 5 && !ignoreMatchNumer)
            return null;

        analyze = analyze.GroupBy(data => GetRankByRule(data)).Select(y => y.First()).ToList();
        analyze = analyze.OrderBy(data => GetRankByRule(data)).ToList();

        bool fetchAnalyzer = analyze.Zip(analyze.Skip(4), (x, y) => (GetRankByRule(x) + 4) == GetRankByRule(y)).Any(x => x);
        if (fetchAnalyzer)
        {
            bool checkTwo = analyze.Where(x => x.GetRank() == CardRank.Two).FirstOrDefault() != null;
            bool checkAce = analyze.Where(x => x.GetRank() == CardRank.Ace).FirstOrDefault() != null;
            bool checkKing = analyze.Where(x => x.GetRank() == CardRank.King).FirstOrDefault() != null;
            bool checkQueen = analyze.Where(x => x.GetRank() == CardRank.Queen).FirstOrDefault() != null;
            bool checkJack = analyze.Where(x => x.GetRank() == CardRank.Jack).FirstOrDefault() != null;

            if (checkTwo && checkAce && checkKing && checkQueen && checkJack)
                return null;

            ComboOutput outputCombo = new ComboOutput();
            outputCombo.basedNum = 5;
            //outputCombo.basedValue = analyze.Sum(x => this.GetWeightByCardData(x));
            outputCombo.basedValue = analyze.Sum(x => GetWeightByCardData(x));
            outputCombo.availableCards = analyze;
            outputCombo.name = "Straight";
            return outputCombo;
        }

        return null;
    }

    public static ComboOutput CheckForFourKind(List<CardData> input, bool ignoreMatchNumer = false)
    {
        List<CardData> analyze = new List<CardData>(input);

        if (analyze.Count != 5 && !ignoreMatchNumer)
            return null;

        bool fetchAnalyzer = analyze.GroupBy(data => GetRankByRule(data)).Any(group => group.Count() == 4);
        if (!fetchAnalyzer)
            return null;
        else
        {
            ComboOutput outputCombo = new ComboOutput();
            outputCombo.basedNum = 5;
            outputCombo.basedValue = analyze.Sum(x => GetWeightByCardData(x));
            outputCombo.availableCards = analyze;
            outputCombo.name = "Four of a kind & one card";
            return outputCombo;
        }
    }

    public static ComboOutput CheckForFlush(List<CardData> input, bool ignoreMatchNumer = false)
    {
        List<CardData> analyze = new List<CardData>(input);
        if (analyze.Count != 5 && !ignoreMatchNumer)
            return null;

        bool fetchAnalyzer = analyze.GroupBy(data => data.GetSuit()).Count() == 1;
        if (!fetchAnalyzer)
            return null;
        else
        {
            ComboOutput outputCombo = new ComboOutput();
            outputCombo.basedNum = 5;
            outputCombo.basedValue = analyze.Sum(x => GetWeightByCardData(x));
            outputCombo.availableCards = analyze;
            outputCombo.name = "Flush";
            return outputCombo;
        }
    }

    public static ComboOutput CheckForStraightFlush(List<CardData> input, bool ignoreMatchNumer = false)
    {
        List<CardData> analyze = new List<CardData>(input);
        if (analyze.Count != 5 && !ignoreMatchNumer)
            return null;

        ComboOutput straightflushCombo = CheckForFlush(analyze, ignoreMatchNumer);
        bool fetchFlush = (straightflushCombo != null);

        if (!fetchFlush)
            return null;

        straightflushCombo = CheckForStraight(straightflushCombo.availableCards, ignoreMatchNumer);
        bool fetchAnalyzer = (straightflushCombo != null);
        if (!fetchAnalyzer)
            return null;
        else
        {
            straightflushCombo.name = "Straight Flush";
            return straightflushCombo;
        }
    }

    public static ComboOutput CheckForFullHouse(List<CardData> input, bool ignoreMatchNumer = false)
    {
        List<CardData> analyze = new List<CardData>(input);
        if (analyze.Count != 5 && !ignoreMatchNumer)
            return null;

        bool fetchAnalyzer = analyze.GroupBy(data => GetRankByRule(data)).Count() == 2 && analyze.GroupBy(data => GetRankByRule(data)).Any(group => group.Count() == 3);
        if (!fetchAnalyzer)
            return null;
        else
        {
            ComboOutput outputCombo = new ComboOutput();
            outputCombo.basedNum = 5;
            outputCombo.basedValue = analyze.Sum(x => GetWeightByCardData(x));
            outputCombo.availableCards = analyze;
            outputCombo.name = "Full House";
            return outputCombo;
        }
    }

    public static List<ComboOutput> AllPossiblePairs(List<CardData> input)
    {
        List<ComboOutput> allPairs = new List<ComboOutput>();
        for (int i = 0; i < input.Count; i++)
        {
            CardData cardA = input[i];
            for (int j = i + 1; j < input.Count; j++)
            {
                CardData cardB = input[j];
                if (GetRankByRule(cardA) == GetRankByRule(cardB))
                {
                    ComboOutput tryInsert = new ComboOutput();
                    tryInsert.availableCards = new List<CardData>() { cardA, cardB };
                    tryInsert.basedValue = GetWeightByCardData(cardA) + GetWeightByCardData(cardB);
                    tryInsert.basedNum = 2;
                    tryInsert.name = "Pair";

                    if (!allPairs.Contains(tryInsert))
                        allPairs.Add(tryInsert);
                }
            }
        }

        return allPairs;
    }

    public static List<ComboOutput> AllThreeOfAKind(List<CardData> input)
    {
        List<ComboOutput> allThreeOfAKind = new List<ComboOutput>();
        for (int i = 0; i < input.Count; i++)
        {
            CardData cardA = input[i];
            int count = 0;
            for (int j = 0; j < input.Count; j++)
            {
                CardData cardB = input[j];
                if (GetWeightByCardData(cardA) != GetWeightByCardData(cardB) &&
                    GetRankByRule(cardA) == GetRankByRule(cardB))
                    count++;
            }

            if (count >= 2)
            {
                ComboOutput tryInsert = new ComboOutput();
                tryInsert.availableCards = new List<CardData>() { cardA };
                tryInsert.basedValue = GetWeightByCardData(cardA);
                tryInsert.basedNum = 3;
                tryInsert.name = "Three of a kind";
                for (int j = 0; j < input.Count; j++)
                {
                    CardData cardB = input[j];
                    if (GetWeightByCardData(cardA) != GetWeightByCardData(cardB) &&
                        GetRankByRule(cardA) == GetRankByRule(cardB))
                    {
                        tryInsert.availableCards.Add(cardB);
                        tryInsert.basedValue += GetWeightByCardData(cardB);
                    }
                }
                allThreeOfAKind.Add(tryInsert);
                break;
            }
        }

        return allThreeOfAKind;
    }

    public static List<ComboOutput> AllStraight(List<CardData> input)
    {
        List<CardData> analyze = new List<CardData>(input);
        List<ComboOutput> allStraight = new List<ComboOutput>();
        analyze = analyze.OrderBy(x => GetRankByRule(x)).ToList();

        for (int i = 0; i < analyze.Count - 3; i++)
        {
            List<CardData> straight = new List<CardData>() { analyze[i] };
            for (int j = 1; j < 4; j++)
            {
                if (GetRankByRule(analyze[i + j]) == (GetRankByRule(analyze[i + j - 1]) + 1))
                {
                    straight.Add(analyze[i + j]);
                    if (straight.Count >= 5)
                        break;
                }
                else
                    break;
            }

            if (straight.Count >= 5)
            {
                bool checkTwo = straight.Where(x => x.GetRank() == CardRank.Two).FirstOrDefault() != null;
                bool checkAce = straight.Where(x => x.GetRank() == CardRank.Ace).FirstOrDefault() != null;
                bool checkKing = straight.Where(x => x.GetRank() == CardRank.King).FirstOrDefault() != null;
                bool checkQueen = straight.Where(x => x.GetRank() == CardRank.Queen).FirstOrDefault() != null;
                bool checkJack = straight.Where(x => x.GetRank() == CardRank.Jack).FirstOrDefault() != null;

                if (checkTwo && checkAce && checkKing && checkQueen && checkJack)
                    continue;

                ComboOutput tryInsert = new ComboOutput();
                tryInsert.availableCards = straight;
                tryInsert.basedValue = straight.Sum(card => GetWeightByCardData(card));
                tryInsert.basedNum = 5;
                tryInsert.name = "Straight";

                allStraight.Add(tryInsert);
            }
        }

        return allStraight;
    }

    public static List<ComboOutput> AllFlush(List<CardData> input)
    {
        List<ComboOutput> allFlush = new List<ComboOutput>();

        for (int i = 0; i < input.Count; i++)
        {
            CardData cardA = input[i];
            List<CardData> flush = new List<CardData>() { cardA };

            for (int j = 0; j < input.Count; j++)
            {
                CardData cardB = input[j];
                if (GetWeightByCardData(cardA) != GetWeightByCardData(cardB) &&
                    cardA.GetSuit() == cardB.GetSuit())
                {
                    flush.Add(cardB);
                    if (flush.Count >= 5)
                        break;
                }
            }

            if (flush.Count >= 5)
            {
                ComboOutput tryInsert = new ComboOutput();
                tryInsert.availableCards = flush;
                tryInsert.basedValue = flush.Sum(card => GetWeightByCardData(card));
                tryInsert.basedNum = 5;
                tryInsert.name = "Flush";

                allFlush.Add(tryInsert);
            }
        }

        return allFlush;
    }

    public static List<ComboOutput> AllFourOfAKind(List<CardData> input)
    {
        List<ComboOutput> allFourOfAKind = new List<ComboOutput>();
        for (int i = 0; i < input.Count; i++)
        {
            CardData cardA = input[i];
            int count = 0;
            for (int j = 0; j < input.Count; j++)
            {
                CardData cardB = input[j];
                if (GetWeightByCardData(cardA) != GetWeightByCardData(cardB) &&
                    GetRankByRule(cardA) == GetRankByRule(cardB))
                    count++;
            }

            if (count >= 3)
            {
                ComboOutput tryInsert = new ComboOutput();
                tryInsert.availableCards = new List<CardData>() { cardA };
                tryInsert.basedValue = GetWeightByCardData(cardA);
                tryInsert.basedNum = 5;
                tryInsert.name = "Four of a kind & one card";
                for (int j = 0; j < input.Count; j++)
                {
                    CardData cardB = input[j];
                    if (GetWeightByCardData(cardA) != GetWeightByCardData(cardB) &&
                        GetRankByRule(cardA) == GetRankByRule(cardB))
                    {
                        tryInsert.availableCards.Add(cardB);
                        tryInsert.basedValue += GetWeightByCardData(cardB);
                    }
                }
                for (int j = 0; j < input.Count; j++)
                {
                    CardData cardB = input[j];
                    if (!tryInsert.availableCards.Contains(cardB))
                    {
                        tryInsert.availableCards.Add(cardB);
                        tryInsert.basedValue += GetWeightByCardData(cardB);
                        if (tryInsert.availableCards.Count == 5)
                            break;
                    }
                }

                if (tryInsert.availableCards.Count >= 5)
                    allFourOfAKind.Add(tryInsert);

                break;
            }
        }

        return allFourOfAKind;
    }

    public static List<ComboOutput> AllStraightFlush(List<CardData> input)
    {
        List<ComboOutput> result = new List<ComboOutput>();
        List<ComboOutput> allStraight = AllStraight(input);

        foreach (ComboOutput straight in allStraight)
        {
            result.AddRange(AllFlush(straight.availableCards));

        }

        return result;
    }

    public static List<ComboOutput> AllFullHouse(List<CardData> input)
    {
        List<ComboOutput> result = new List<ComboOutput>();
        List<CardData> analyze = new List<CardData>(input);

        foreach (CardRank rank in Enum.GetValues(typeof(CardRank)))
        {
            int countRank = analyze.Count(x => x.GetRank() == rank);

            if (countRank == 3)
            {
                foreach (CardRank otherRank in Enum.GetValues(typeof(CardRank)))
                {
                    if (otherRank != rank)
                    {
                        int countOtherRank = analyze.Count(x => x.GetRank() == otherRank);
                        if (countOtherRank == 2)
                        {
                            ComboOutput fullHouse = new ComboOutput();
                            fullHouse.availableCards = new List<CardData>();
                            fullHouse.basedNum = 5;
                            fullHouse.name = "Full House";

                            fullHouse.availableCards.AddRange(analyze.Where(x => x.GetRank() == rank).ToList());
                            fullHouse.availableCards.AddRange(analyze.Where(x => x.GetRank() == otherRank).ToList());
                            fullHouse.basedValue = fullHouse.availableCards.Sum(x => GetWeightByCardData(x));
                            result.Add(fullHouse);
                        }
                    }
                }
            }
        }

        return result;
    }

    public static bool CanBeatCardOnTable(ComboOutput input, ComboOutput compareTable)
    {
        if (input.name != compareTable.name || input.basedValue <= compareTable.basedValue)
        {
            if (input.basedNum == 5)
            {
                if (input.name == "Full House" && compareTable.name.Contains("Straight"))
                    return true;
                else if (compareTable.name == "Straight Flush" || compareTable.name == "Four of a kind & one card")
                    return true;
                else
                    return false;
            }
            else
                return false;
        }
        else
            return true;
    }
}

[System.Serializable]
public class ComboOutput
{
    public string name = "Single";
    public int basedNum = 1;
    public int basedValue = 1;
    public List<CardData> availableCards = new List<CardData>();

    public ComboOutput()
    {
        name = "Single";
        basedNum = 1;
        basedValue = 1;
    }
}