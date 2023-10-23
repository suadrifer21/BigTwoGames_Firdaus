using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardView : MonoBehaviour
{
    [SerializeField] private CardData cardData = null;
    [SerializeField] private Image image = null;
    [SerializeField] private Sprite faceCard= null;
    [SerializeField] private Sprite backCard = null;
    [SerializeField] private Button button = null;
    //public PlayerEntity owner;
    private bool isSelected = false;
    public event EventHandler<OnCardClickedEventArgs> OnCardClicked;
    public class OnCardClickedEventArgs
    {
        public CardView cardView;
        public bool isSelected;
    }
    private void Awake()
    {
        image = GetComponent<Image>();
        button = GetComponent<Button>();
        image.sprite = backCard;
    }
    public void InitializeView(CardData inputData)// System.Action<CardView> onClick = null)
    {
        cardData = inputData;
        faceCard = cardData.GetSprite();
        gameObject.name = cardData.GetCardName();

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(CardSelected);
    }
    public CardData GetData()
    {
        return cardData;
    }
    void CardSelected()
    {
        isSelected = !isSelected;
        if (isSelected)
            transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
        else
            transform.localScale = Vector3.one;

        OnCardClicked?.Invoke(this, new OnCardClickedEventArgs { cardView = this, isSelected = isSelected });
    }
    public void SetShowCard(bool isShow)
    {
       if(isShow) 
            image.sprite = faceCard; 
       else 
            image.sprite = backCard;       
    }
    public void SetInteractable(bool isInteractable)
    {
        button.interactable = isInteractable;
    }

    
}
