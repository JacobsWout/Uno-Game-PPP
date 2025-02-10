using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class CardDisplay : MonoBehaviour
{
    
    Color32 red;
    Color32 green;
    Color32 blue;
    Color32 yellow;
    Color32 black;
    [Header("Sprites")]
    [SerializeField] Sprite reverse;
    [SerializeField] Sprite skip;
    [SerializeField] Sprite plusTwo;
    [SerializeField] Sprite plusFour;
    [Header("Center Card")]
    [SerializeField] Image baseCardColor;
    [SerializeField] Image imageCenter;
    [SerializeField] Image valueImage;
    [SerializeField] TMP_Text valueTextCenter;
    [SerializeField] GameObject wildImageCenter;
    [SerializeField] Image topLeftCenter;
    [SerializeField] Image bottomRightCenter;
    [SerializeField] Image bottomLeftCenter;
    [SerializeField] Image topRightCenter;
    [Header("Top Left Corner")]
    [SerializeField] Image valueImageTL;
    [SerializeField] TMP_Text valueTextCenterTL;
    [SerializeField] GameObject wildImageCenterTL;
    [SerializeField] Image topLeftCenterTL;
    [SerializeField] Image bottomRightCenterTL;
    [SerializeField] Image bottomLeftCenterTL;
    [SerializeField] Image topRightCenterTL;
    [Header("Bottom Right Corner")]
    [SerializeField] Image valueImageBR;
    [SerializeField] TMP_Text valueTextCenterBR;
    [SerializeField] GameObject wildImageCenterBR;
    [SerializeField] Image topLeftCenterBR;
    [SerializeField] Image bottomRightCenterBR;
    [SerializeField] Image bottomLeftCenterBR;
    [SerializeField] Image topRightCenterBR;
    [SerializeField] GameObject cardBack;

    Card myCard;
    public Card MyCard => myCard;
    Player cardOwner;
    public Player Owner => cardOwner;

    public void setCard(Card card, Player owner)
    {
        var Colors = GameManager.instance.GetColors();
        red = Colors.red;
        green = Colors.green;
        blue = Colors.blue;
        yellow = Colors.yellow;
        black = Colors.black;
        myCard = card;
        SetAllColors(card.cardColor);
        SetValue(card.cardValue);
        cardOwner = owner;
    }

    void SetAllColors(CardColor cardColor)
    {
        if (baseCardColor == null) return;
        if (imageCenter == null) return;

        switch (cardColor)
        {
            case CardColor.Red:
                baseCardColor.color = red;
                imageCenter.color = red;
                break;
            case CardColor.Green:
                baseCardColor.color = green;
                imageCenter.color = green;
                break;
            case CardColor.Blue:
                baseCardColor.color = blue;
                imageCenter.color = blue;
                break;
            case CardColor.Yellow:
                baseCardColor.color = yellow;
                imageCenter.color = yellow;
                break;
            case CardColor.None:
                baseCardColor.color = black;
                imageCenter.color = black;

                topLeftCenter.color = red;
                topRightCenter.color = blue;
                bottomLeftCenter.color = green;
                bottomRightCenter.color = yellow;

                topLeftCenterTL.color = red;
                topRightCenterTL.color = blue;
                bottomLeftCenterTL.color = green;
                bottomRightCenterTL.color = yellow;

                topLeftCenterBR.color = red;
                topRightCenterBR.color = blue;
                bottomLeftCenterBR.color = green;
                bottomRightCenterBR.color = yellow;
                break;
        }
    }

    void SetValue(CardValue cardValue)
    {

        wildImageCenter.SetActive(false);
        wildImageCenterBR.SetActive(false);
        wildImageCenterTL.SetActive(false);
        valueImage.gameObject.SetActive(false);
        valueImageBR.gameObject.SetActive(false);
        valueImageTL.gameObject.SetActive(false);

        switch (cardValue)
        {
            case CardValue.Skip:
                valueImage.sprite = skip;
                valueImage.gameObject.SetActive(true);
                valueImageBR.sprite = skip;
                valueImageBR.gameObject.SetActive(true);
                valueImageTL.sprite = skip;
                valueImageTL.gameObject.SetActive(true);
                valueTextCenter.text = "";
                valueTextCenterTL.text = "";
                valueTextCenterBR.text = "";
                break;
            case CardValue.Reverse:
                valueImage.sprite = reverse;
                valueImage.gameObject.SetActive(true);
                valueImageBR.sprite = reverse;
                valueImageBR.gameObject.SetActive(true);
                valueImageTL.sprite = reverse;
                valueImageTL.gameObject.SetActive(true);
                valueTextCenter.text = "";
                valueTextCenterTL.text = "";
                valueTextCenterBR.text = "";
                break;
            case CardValue.Draw_Two:
                valueImage.sprite = plusTwo;
                valueImage.gameObject.SetActive(true);
                valueTextCenter.text = "";
                valueTextCenterTL.text = "+2";
                valueTextCenterBR.text = "+2";
                break;
            case CardValue.Wild_Draw_Four:
                valueImage.sprite = plusFour;
                valueImage.gameObject.SetActive(true);
                valueTextCenter.text = "";
                valueTextCenterTL.text = "+4";
                valueTextCenterBR.text = "+4";
                break;
            case CardValue.Wild:
                wildImageCenter.SetActive(true);
                wildImageCenterBR.SetActive(true);
                wildImageCenterTL.SetActive(true);
                valueTextCenter.text = "";
                valueTextCenterTL.text = "";
                valueTextCenterBR.text = "";
                break;
            default:
                valueTextCenter.text = ((int)cardValue).ToString();
                valueTextCenterTL.text = ((int)cardValue).ToString();
                valueTextCenterBR.text = ((int)cardValue).ToString();
                break;
        }
    }

    public void ShowCard()
    {
        cardBack.SetActive(false);
    }

    public void HideCard()
    {
        cardBack.SetActive(true);
    }
}
