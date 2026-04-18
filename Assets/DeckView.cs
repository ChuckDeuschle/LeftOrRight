using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;

public class DeckView : MonoBehaviour
{
    public Transform contentPanel;
    public GameManager gameManager;
    public GameObject deckViewPanel;
    public GameObject closeDeckViewButton;
    public TextMeshProUGUI scrollViewText;

    public void PopulateDeckAndDiscard()
    {
        PopulateDeckView(gameManager.deck);
        PopulateDeckViewDiscard(gameManager.discardPile);
    }

    public void PopulateDeckView(List<Card> _deck)
    {
        var sb = new StringBuilder();
        foreach (Card card in _deck.OrderBy(c => c.cardName))
            sb.AppendLine(card.cardName + " Left: " + card.leftAction.Label + ", Right: " + card.rightAction.Label);
        scrollViewText.text = sb.ToString();

        deckViewPanel.SetActive(true);
        closeDeckViewButton.SetActive(true);
    }

    public void PopulateDeckViewDiscard(List<Card> _discardPile)
    {
        var sb = new StringBuilder(scrollViewText.text);
        sb.AppendLine("Discard:");
        foreach (Card card in _discardPile)
            sb.AppendLine(card.cardName + " Left: " + card.leftAction.Label + ", Right: " + card.rightAction.Label);
        scrollViewText.text = sb.ToString();
    }

    public void CloseDeckView()
    {
        deckViewPanel.SetActive(false);
        closeDeckViewButton.SetActive(false);
    }
}
