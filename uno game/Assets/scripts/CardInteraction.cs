using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardInteraction : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    CardDisplay cardDisplay;
    Vector3 originalPosition;

    float liftAmount = 30f;

    // Start is called before the first frame update
    void Start()
    {
        cardDisplay = GetComponent<CardDisplay>();
        originalPosition = transform.localPosition;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(cardDisplay.Owner.IsHuman && GameManager.instance.humanHasTurn)
        {
            LiftCard(false);
            GameManager.instance.PlayCard(cardDisplay);
            Debug.Log("Clicked on card: " + cardDisplay.MyCard.cardColor.ToString() + cardDisplay.MyCard.cardValue.ToString());
        }
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
       LiftCard(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        LiftCard(false);
    }
    
    void LiftCard(bool lift)
    {
        if(lift && cardDisplay.Owner.IsHuman)
        {
            transform.localPosition = originalPosition + new Vector3(0, liftAmount, 0);
        }
        else
        {
            transform.localPosition = originalPosition;
        }
    }

    
}
