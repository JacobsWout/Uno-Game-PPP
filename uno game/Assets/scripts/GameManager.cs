using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;


public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public List<Player> players = new List<Player>();
    [SerializeField] Deck deck;

    [SerializeField] Transform playerHandTransform;
    [SerializeField] List<Transform> aiHandTransforms = new List<Transform>();
    [SerializeField] GameObject cardPrefab;

    [SerializeField] int numberOfAiPlayers = 3;
    [SerializeField] int startingHandSize = 7;
    int currentPlayer = 0;
    int playDirection = 1; //1 // -1
    [Header("Gameplay")]
    [SerializeField] Transform discardPileTransform;
    [SerializeField] CardDisplay topCard;
    [SerializeField] Transform directionArrow;
    [SerializeField] GameObject wildPanel;
    [SerializeField] WildButton redButton;
    [SerializeField] WildButton greenButton;
    [SerializeField] WildButton blueButton;
    [SerializeField] WildButton yellowButton;
    CardColor topColor = CardColor.None;
    bool unoCalled;
    [Header("Colors")]
    [SerializeField] Color32 red;
    [SerializeField] Color32 green;
    [SerializeField] Color32 blue;
    [SerializeField] Color32 yellow;
    [SerializeField] Color32 black;
    [Header("UI")]
    [SerializeField] GameObject winPanel;
    [SerializeField] TMP_Text winningText;
    [SerializeField] List<Image> PlayerHighlights = new List<Image>();
    [SerializeField] List<TMP_Text> playedCardCounts = new List<TMP_Text>();

    public bool humanHasTurn{get; private set; }

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        wildPanel.SetActive(false);
        redButton.SetImageColor(red);
        greenButton.SetImageColor(green);
        blueButton.SetImageColor(blue);
        yellowButton.SetImageColor(yellow);
        winPanel.SetActive(false);
        //INIT NEW DECK
        deck.InitializeDeck();


        //INIT PLAYERS

        InitializePlayers();

        //DEAL CARDS

        StartCoroutine(DealStartingCards());

        //START GAME
    }

    void InitializePlayers()
    {
        players.Clear();
        players.Add(new Player("Player", true));

        for (int i = 0; i < numberOfAiPlayers; i++)
        {
            players.Add(new AiPlayer("AI " + (i+1)));
        }
    }

    void DealCards()
    {
        for (int i = 0; i < startingHandSize; i++)
        {
            foreach (Player player in players)
            {
                player.DrawCard(deck.DrawCard());
            }
        }
    }

    IEnumerator DealStartingCards()
    {
    for (int i = 0; i < startingHandSize; i++)
    {
        foreach (Player player in players)
        {
            Card drawnCard = deck.DrawCard();
            player.DrawCard(drawnCard);

            // Zorg ervoor dat de kaart in de juiste hand wordt geplaatst
            Transform hand = player.IsHuman ? playerHandTransform : aiHandTransforms[Mathf.Max(players.IndexOf(player) - 1, 0)];
            
            GameObject card = Instantiate(cardPrefab, hand, false);
            CardDisplay cardDisplay = card.GetComponentInChildren<CardDisplay>();
            cardDisplay.setCard(drawnCard, player);

            if (player.IsHuman)
            {
                cardDisplay.ShowCard();
            }

            yield return new WaitForSeconds(0.1f);
        }
    }
    //hand out top card
    Card pileCard = deck.DrawCard();
    GameObject newCard = Instantiate(cardPrefab, discardPileTransform, false);
    CardDisplay display = newCard.GetComponentInChildren<CardDisplay>();
    display.setCard(pileCard, null);
    display.ShowCard();
    newCard.GetComponentInChildren<CardInteraction>().enabled = false;
    deck.AddUsedCard(pileCard);

    // **Fix de grootte van de kaart**
    RectTransform cardRect = newCard.GetComponent<RectTransform>();
    RectTransform pileRect = discardPileTransform.GetComponent<RectTransform>();

    // Reset schaal om te voorkomen dat de kaart misvormd is
    newCard.transform.localScale = Vector3.one;

    // Pas de grootte van de kaart aan aan de discard pile
    cardRect.anchorMin = pileRect.anchorMin;
    cardRect.anchorMax = pileRect.anchorMax;
    cardRect.anchoredPosition = Vector2.zero;
    cardRect.sizeDelta = pileRect.sizeDelta;

    //set top card
    topCard = display;
    topColor = pileCard.cardColor;
    if(topColor == CardColor.None)
    {
        topColor = PickRandomColor();
    }
    TintArrow();
    //Start game
    Debug.Log("The game can start");
    humanHasTurn = true;
    UpdatePlayerUI();
    }

    CardColor PickRandomColor()
    {
        CardColor[] colors = (CardColor[])Enum.GetValues(typeof(CardColor));
        int randomIndex = UnityEngine.Random.Range(0, colors.Length - 1);
        return colors[randomIndex];
    }

   public void PlayCard(CardDisplay cardDisplay = null, Card card = null)
    {
        Card cardToPlay = cardDisplay?.MyCard ?? card;

        if(cardDisplay == null && card != null)
        {
            cardDisplay = FindCardDisplayForCard(card);
        }

        if(!IsPlayable(cardDisplay.MyCard))
        {
            Debug.Log("Card is not playable");
            return;
        }

        players[currentPlayer].PlayCard(cardToPlay);
        cardDisplay.transform.parent.SetParent(discardPileTransform);
        MoveCardToPile(cardDisplay.transform.parent.gameObject);
        topCard = cardDisplay;
        topColor = cardToPlay.cardColor;
        TintArrow();
        OnCardPlayed(topCard.MyCard);

        cardDisplay.ShowCard();
        cardDisplay.GetComponent<CardInteraction>().enabled = false;

        deck.AddUsedCard(cardToPlay);
    }


    CardDisplay FindCardDisplayForCard(Card card)
    {
        Player player = players[currentPlayer];
        Transform hand = player.IsHuman ? playerHandTransform : aiHandTransforms[players.IndexOf(player) - 1];
        foreach(Transform cardTransform in hand)
        {
            CardDisplay tempDisplay = cardTransform.GetComponentInChildren<CardDisplay>();
            if(tempDisplay.MyCard == card)
            {
                return tempDisplay;
            }
        }
        return null;
    }

    void MoveCardToPile(GameObject currentCard)
    {
        currentCard.transform.SetParent(discardPileTransform);
        currentCard.transform.localPosition = Vector3.zero;
        currentCard.transform.localScale = Vector3.one;

        RectTransform cardRect = currentCard.GetComponent<RectTransform>();
        RectTransform pileRect = discardPileTransform.GetComponent<RectTransform>();

        // Past de grootte aan, ongeacht anchors
        cardRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, pileRect.rect.width);
        cardRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, pileRect.rect.height);

    }

    
    public void DrawCardFromDeck()
    {
        Card drawnCard = deck.DrawCard();
        Player player = players[currentPlayer];

        if(drawnCard != null) 
        {
            player.DrawCard(drawnCard);

            // Zorg ervoor dat de kaart in de juiste hand wordt geplaatst
            Transform hand = player.IsHuman ? playerHandTransform : aiHandTransforms[players.IndexOf(player) - 1];
            
            GameObject card = Instantiate(cardPrefab, hand, false);
            CardDisplay cardDisplay = card.GetComponentInChildren<CardDisplay>();
            cardDisplay.setCard(drawnCard, player);

            if (player.IsHuman)
            {
                cardDisplay.ShowCard();
            }

            if(!CanPlayAnyCard() && player.IsHuman)
            {
                Debug.Log("No card to play, switching player");
                SwitchPlayer();
            }
        }
        
    }

    void OnCardPlayed(Card playedCard)
    {
        ApplyCardEffects(playedCard);

        // Controleer of iemand gewonnen heeft
        if(players.Any(p => p.playerHand.Count == 0))
        {
            Debug.Log("Speler heeft gewonnen!");
            return;
        }

        // Controleer of de kaart een effect heeft zoals Skip of Reverse
        if(playedCard.cardValue == CardValue.Skip)
        {
            return;
        }

        // Voor Wild-kaarten moet een nieuwe kleur worden gekozen
        if(playedCard.cardColor == CardColor.None && players[currentPlayer].IsHuman)
        {
            Debug.Log("Wacht op speler om kleur te kiezen.");
            return;
        }
        if(players[currentPlayer].IsHuman)
        {
            SwitchPlayer();
        }
        
        // Standaard wissel van speler
        
    }



    public void SwitchPlayer(bool skipNext = false)
    {
        humanHasTurn = false;
        int numberOfPlayers = players.Count;

        if(players[currentPlayer].playerHand.Count == 1 && !unoCalled)
        {
            // Speler vergat UNO te zeggen → 2 kaarten trekken
            for (int i = 0; i < 2; i++)
            {
                DrawCardFromDeck();
            }
        }

        if(CheckWinCondition())
        {
            winPanel.SetActive(true);
            winningText.text = players[currentPlayer].playerName + " has won!";
            return;
        }

        if(skipNext)
        {
            currentPlayer = (currentPlayer + 2 * playDirection + numberOfPlayers) % numberOfPlayers;
        }
        else
        {
            currentPlayer = (currentPlayer + playDirection + numberOfPlayers) % numberOfPlayers;
        }

        UpdatePlayerUI();

        // <-- Deze regel toevoegen

        unoCalled = false;

        if (players[currentPlayer].IsHuman)
        {
            humanHasTurn = true;
        }
        else
        {
        StartCoroutine(HandleAiTurn());
        }
    }


    public bool CanPlayAnyCard()
    {
        foreach(Card card in players[currentPlayer].playerHand)
        {
            if(IsPlayable(card))
            {
                return true;
            }
        }
        return false;

    }

    bool IsPlayable(Card card)
    {
        return card.cardColor == topColor || card.cardValue == topCard.MyCard.cardValue || card.cardColor == CardColor.None;
    }

    void ApplyCardEffects(Card playedCard)
    {
        switch(playedCard.cardValue)
        {
            case CardValue.Skip:
                SkipPlayer(); // Hier wordt al een speler overgeslagen
                return; // Stop hier om extra switch te voorkomen
            case CardValue.Reverse:
                ReversePlayOrder();
                break;
            case CardValue.Draw_Two:
                MakeNextPlayerDrawcards(2);
                break;
            case CardValue.Wild:
                ChooseNewColor();
                break;
            case CardValue.Wild_Draw_Four:
                ChooseNewColor();
                MakeNextPlayerDrawcards(4);
                break;
            default:
                // Er is geen speciale actie, gewoon de beurt wisselen 
                break;
        }
    }

    bool CheckWinCondition()
    {
        if(players[currentPlayer].playerHand.Count == 0)
        {
            return true;
        }
        return false;
    }

    void SkipPlayer()
    {
        SwitchPlayer(true);


    }

    void ReversePlayOrder()
    {
        playDirection *= -1;

        Vector3 scale = directionArrow.localScale;

        scale.x = playDirection;
        directionArrow.localScale = scale;

    }

    void MakeNextPlayerDrawcards(int cardAmount)
    {
        int numberOfPlayer = players.Count;
        int nextPlayerIndex = (currentPlayer + playDirection + numberOfPlayer) % numberOfPlayer;
        for (int i = 0; i < cardAmount; i++)
        {
            Card drawnCard = deck.DrawCard();
            Player player = players[nextPlayerIndex];

            if(drawnCard!=null)
            {
                 player.DrawCard(drawnCard);

                // Zorg ervoor dat de kaart in de juiste hand wordt geplaatst
                Transform hand = player.IsHuman ? playerHandTransform : aiHandTransforms[players.IndexOf(player) - 1];
            
                GameObject card = Instantiate(cardPrefab, hand, false);
                CardDisplay cardDisplay = card.GetComponentInChildren<CardDisplay>();
                cardDisplay.setCard(drawnCard, player);

                if (player.IsHuman)
                {
                    cardDisplay.ShowCard();
                }
            }
        }
    }

    void ChooseNewColor()
    {
        if(players[currentPlayer].IsHuman)
        {
            wildPanel.SetActive(true);
            return;
        }
    }

    public (Color32 red, Color32 green, Color32 yellow, Color32 blue, Color32 black) GetColors()
    {
        return (red, green, yellow, blue, black);
    }

    void TintArrow()
    {
        switch(topColor)
        {
            case CardColor.Red:
                directionArrow.GetComponent<Image>().color = red;
            break;
            case CardColor.Green:
                directionArrow.GetComponent<Image>().color = green;
            break;
            case CardColor.Blue:
                directionArrow.GetComponent<Image>().color = blue;
            break;
            case CardColor.Yellow:
                directionArrow.GetComponent<Image>().color = yellow;
            break;
            case CardColor.None:
                directionArrow.GetComponent<Image>().color = black;
            break;
        }
    }

    public void ChoosenColor(CardColor newColor)
    {
        topColor = newColor;
        wildPanel.SetActive(false);
        TintArrow();
        if(players[currentPlayer].IsHuman)
        {
            SwitchPlayer();
        }
    }

    IEnumerator HandleAiTurn()
    {
        yield return new WaitForSeconds(1f);
        players[currentPlayer].TakeTurn(topCard.MyCard, topColor);

        //SwitchPlayer();
    }

    public int GetNextPlayerHandSize()
    {
        int numberOfPlayer = players.Count;
        int nextPlayerIndex = (currentPlayer + playDirection + numberOfPlayer) % numberOfPlayer;
        int nextPlayerHandSize = players[nextPlayerIndex].playerHand.Count;
        return nextPlayerHandSize;
    }

    public void UnoButton()
    {
        if(players[currentPlayer].playerHand.Count == 2 && !unoCalled)
        {
            unoCalled = true;
        }
        else
        {
            Debug.Log("Uno Button clicked but not correctly ");
        }
    }

    public void SetUnoByAi()
    {
        unoCalled = true;
    }

    void UpdatePlayerUI()
    {
        for (int i = 0; i < players.Count; i++)
        {
            if(i == currentPlayer)
            {
                PlayerHighlights[i].color = yellow;
            }
            else
            {
                PlayerHighlights[i].color = Color.white;
            }

            playedCardCounts[i].text = players[i].playerHand.Count.ToString();
        }
    }

    
}
