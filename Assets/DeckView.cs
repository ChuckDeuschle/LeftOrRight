using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DeckView : MonoBehaviour
{
    public Transform contentPanel; // Assign Content Transform in Inspector
    public GameManager gameManager; // Reference to GameManager to access deck
    public GameObject deckViewPanel;
    public GameObject closeDeckViewButton;

    public void PopulateDeckView()
    {
        // Get the Scroll View's text object and clear it
        TextMeshProUGUI scrollViewText = deckViewPanel.transform.Find("Viewport/Content/DeckListText").GetComponent<TextMeshProUGUI>();
        scrollViewText.text = "";

        // For each card in the deck, add its name to the Scroll View's text
        foreach (Card card in gameManager.deck)
        {
            scrollViewText.text += card.cardName + " " + " Left: " + card.leftAction.Label + ", Right: " + card.rightAction.Label + "\n";
        }

        scrollViewText.text += "Discard:\n";

        // For each card in the discard, add its name to the Scroll View's text
        foreach (Card card in gameManager.discardPile)
        {
            scrollViewText.text += card.cardName + " " + " Left: " + card.leftAction.Label + ", Right: " + card.rightAction.Label + "\n";
        }

        // Enable the deck view panel and close button
        deckViewPanel.SetActive(true);
        closeDeckViewButton.SetActive(true);
    }

    public void CloseDeckView()
    {
        // Disable the deck view panel and close button
        deckViewPanel.SetActive(false);
        closeDeckViewButton.SetActive(false);

    }
}
