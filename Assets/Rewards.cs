using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Rewards : MonoBehaviour
{
    public GameManager gameManager;
    public Button card1Button, card2Button, card3Button;  // Drag your Card Button UI objects in Unity inspector

    void Start()
    {
        card1Button.onClick.AddListener(delegate { SelectCard(card1Button); });
        card2Button.onClick.AddListener(delegate { SelectCard(card2Button); });
        card3Button.onClick.AddListener(delegate { SelectCard(card3Button); });
    }

    void SelectCard(Button selectedCard)
    {
        card1Button.image.color = card1Button.colors.normalColor;
        card2Button.image.color = card2Button.colors.normalColor;
        card3Button.image.color = card3Button.colors.normalColor;

        selectedCard.image.color = selectedCard.colors.highlightedColor;

        if (selectedCard == card1Button)
        {
            gameManager.awardCardSelection = 0;
        }
        else if (selectedCard == card2Button)
        {
            gameManager.awardCardSelection = 1;
        }
        else
        {
            gameManager.awardCardSelection = 2;
        }
    }

    void OnDestroy()
    {
        card1Button.onClick.RemoveAllListeners();
        card2Button.onClick.RemoveAllListeners();
        card3Button.onClick.RemoveAllListeners();
    }

    public void UpdateRewardsCards(List<Card> _rewards)
    {
        card1Button.image.sprite = _rewards[0].sprite;
        card2Button.image.sprite = _rewards[1].sprite;
        card3Button.image.sprite = _rewards[2].sprite;
    }
}
