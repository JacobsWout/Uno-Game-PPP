using UnityEngine;

public enum CardColor
{
    Red,
    Green,
    Blue,
    Yellow,
    None, 
}


public enum CardValue
{
    Zero,
    One,
    Two,
    Three,
    Four,
    Five,
    Six,
    Seven,
    Eight,
    Nine,
    Reverse,
    Skip,
    Draw_Two,
    Wild,
    Wild_Draw_Four
}

[System.Serializable]
public class Card 
{
    public CardColor cardColor;
    public CardValue cardValue;

    public Card(CardColor color, CardValue value)
    {
        cardColor = color;
        cardValue = value;
    }
}
