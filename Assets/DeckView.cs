using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class DeckView : MonoBehaviour
{
    public Transform contentPanel; // Assign Content Transform in Inspector
    public GameManager gameManager; // Reference to GameManager to access deck
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
        scrollViewText.text = "";

        // For each card in the deck, add its name to the Scroll View's text
        foreach (Card card in _deck.OrderBy(card => card.cardName))
        {
            scrollViewText.text += card.cardName + " " + " Left: " + card.leftAction.Label + ", Right: " + card.rightAction.Label + "\n";
        }

        // Enable the deck view panel and close button
        deckViewPanel.SetActive(true);
        closeDeckViewButton.SetActive(true);
    }

    public void PopulateDeckViewDiscard(List<Card> _discardPile)
    {
        scrollViewText.text += "Discard:\n";

        // For each card in the discard, add its name to the Scroll View's text
        foreach (Card card in _discardPile)
        {
            scrollViewText.text += card.cardName + " " + " Left: " + card.leftAction.Label + ", Right: " + card.rightAction.Label + "\n";
        }
    }

    public void CloseDeckView()
    {
        // Disable the deck view panel and close button
        deckViewPanel.SetActive(false);
        closeDeckViewButton.SetActive(false);

    }
}
