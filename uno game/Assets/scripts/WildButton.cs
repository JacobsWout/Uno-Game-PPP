using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WildButton : MonoBehaviour, IPointerClickHandler
{
    public CardColor cardColor;

    public void OnPointerClick(PointerEventData eventData)
    {
       GameManager.instance.ChoosenColor(cardColor);
    }

    public void SetImageColor(Color32 color)
    {
        GetComponent<Image>().color = color;
    }
}
