using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiPlayer : Player
{
    public AiPlayer(string name) : base(name, false)
    {

    }

    public override void TakeTurn(Card topCard, CardColor topColor)
    {
       Card cardToPlay = null;
       Debug.Log( playerName + " take TURN");
       List<Card> playableCards = GetPlayableCards(topCard, topColor);

       if(playableCards.Count > 0)
       {
           cardToPlay = ChooseBestCard(playableCards);
       }
       else
       {
           GameManager.instance.DrawCardFromDeck();
           playableCards = GetPlayableCards(topCard, topColor);
           if(playableCards.Count > 0)
           {
               cardToPlay = ChooseBestCard(playableCards);
           }   
       }
       if(cardToPlay == null)
       {
           GameManager.instance.SwitchPlayer();
       }
       else
       {
           if(playerHand.Count == 2)
           {
                GameManager.instance.SetUnoByAi();
                Debug.Log(playerName + " says UNO");
           }
           GameManager.instance.PlayCard(null,cardToPlay);
           //if wild card choose best color
           if(cardToPlay.cardColor == CardColor.None)
           {
               GameManager.instance.ChoosenColor(SelectBestColor());
           }

           if(cardToPlay.cardValue == CardValue.Skip)
           {
              return;
           }

           GameManager.instance.SwitchPlayer();
       }
    }

    List<Card> GetPlayableCards(Card topCard, CardColor topColor)
    {
        List<Card> playableCards = new List<Card>();

        foreach(Card card in playerHand)
        {
            if(card.cardColor == topColor || card.cardValue == topCard.cardValue || card.cardColor == CardColor.None)
            {
                playableCards.Add(card);
            }
        }

        return playableCards;
    }

    Card ChooseBestCard(List<Card> playableCards)
    {
        Card bestActionCard = null;
        Card bestRegularCard = null;
        Card bestWildCard = null;
        int nextPlayerHandSize = GameManager.instance.GetNextPlayerHandSize();

        foreach (Card card in playableCards)
        {
            if(card.cardValue == CardValue.Wild_Draw_Four)
            {
                if(nextPlayerHandSize <= 2 || bestActionCard == null)
                {
                    bestActionCard = card;
                }
            }
            else if(card.cardValue == CardValue.Draw_Two)
            {
                if(nextPlayerHandSize <= 2 || bestActionCard == null)
                {
                    bestActionCard = card;
                }
            }
            else if(card.cardValue == CardValue.Skip)
            {
                if(nextPlayerHandSize <= 2 || bestActionCard == null)
                {
                    bestActionCard = card;
                }
            }
            else if(card.cardValue == CardValue.Reverse)
            {
                if(nextPlayerHandSize <= 2 || bestActionCard == null)
                {
                    bestActionCard = card;
                }
            }
            else if(card.cardValue == CardValue.Wild)
            {
                if(nextPlayerHandSize <= 2 || bestActionCard == null)
                {
                    bestWildCard = card;
                }
            }
        }
        foreach (Card card in playableCards)
        {
            if(bestRegularCard == null || card.cardValue > bestRegularCard.cardValue)
            {
                bestRegularCard = card;
            }
        }
        if(bestActionCard == null && bestWildCard != null)
        {
            bestActionCard = bestWildCard;
        }
        if(bestActionCard != null)
        {
            return bestActionCard;
        }
        if(bestRegularCard != null)
        {
            return bestRegularCard;
        }
        return playableCards[0];
    }

    CardColor SelectBestColor()
    {
        Dictionary<CardColor, int> colorCounts = new Dictionary<CardColor, int>()
        {
            {CardColor.Red, 0},
            {CardColor.Blue, 0},
            {CardColor.Green, 0},
            {CardColor.Yellow, 0}
        };

        foreach(Card card in playerHand)
        {
            if(card.cardColor != CardColor.None)
            {
                colorCounts[card.cardColor]++;
            }
        }

        CardColor bestColor = CardColor.Red;
        int MaxCount = 0;
        foreach(var color in colorCounts)
        {
            if(color.Value > MaxCount)
            {
                bestColor = color.Key;
                MaxCount = color.Value;
            }
        }

        return bestColor;
    }
}
