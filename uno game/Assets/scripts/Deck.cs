using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Deck : MonoBehaviour, IPointerClickHandler
{
    public List<Card> cardDeck = new List<Card>();
    public List<Card> usedCardDeck = new List<Card>();
    // Start is called before the first frame update
    // void Start()
    // {
    //     InitializeDeck();
    // }

    // Update is called once per frame
   public void InitializeDeck()
    {
    cardDeck.Clear();

    foreach (CardColor color in System.Enum.GetValues(typeof(CardColor)))
    {
        if (color == CardColor.None) continue; // Sla "None" over voor normale kaarten

        foreach (CardValue cardValue in System.Enum.GetValues(typeof(CardValue)))
        {
            if (cardValue == CardValue.Wild || cardValue == CardValue.Wild_Draw_Four) continue; // Sla wild cards hier over

            // Voeg 1 exemplaar toe voor "0", maar 2 exemplaren voor andere kaarten
            int count = (cardValue == CardValue.Zero) ? 1 : 2;
            for (int i = 0; i < count; i++)
            {
                cardDeck.Add(new Card(color, cardValue));
            }
        }
    }

    // Voeg de vier Wild- en Wild Draw Four-kaarten toe
    for (int i = 0; i < 4; i++)
    {
        cardDeck.Add(new Card(CardColor.None, CardValue.Wild));
        cardDeck.Add(new Card(CardColor.None, CardValue.Wild_Draw_Four));
    }

    ShuffleCardDeck();
    }


    public void ShuffleCardDeck()

        {
            for (int i = 0; i < cardDeck.Count; i++)
            {
                Card temp = cardDeck[i];
                int randomIndex = Random.Range(i, cardDeck.Count);
                cardDeck[i] = cardDeck[randomIndex];
                cardDeck[randomIndex] = temp;
            }
        }


    public Card DrawCard()
    {
        if(cardDeck.Count == 0)
        {
            cardDeck.AddRange(usedCardDeck);
            usedCardDeck.Clear();
            ShuffleCardDeck();
            if(cardDeck.Count == 0)
            {
                return null;
            }
            return null;
        }

        Card drawnCard = cardDeck[0];
        cardDeck.RemoveAt(0);
        return drawnCard;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(GameManager.instance.humanHasTurn && !GameManager.instance.CanPlayAnyCard())
        {
            GameManager.instance.DrawCardFromDeck();
        }
        
    }

    public void AddUsedCard(Card card)
    {
        usedCardDeck.Add(card);
    }
}
