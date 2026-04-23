using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Card : MonoBehaviour
{
    // You can add card-specific variables here later
    public string cardName;
    public Action leftAction;
    public Action rightAction;
    public Sprite sprite;

    // Marks cards that trigger archetype-specific side effects when played
    // (e.g. Infiltrator traps). Normal starter-deck cards leave this false.
    public bool isTrap;

    [SerializeField]
    private TextMeshPro cardText; // Reference to the text object

    public void SetText()
    {
        cardText.text = cardName + "\nLeft: " + leftAction.Label + "\nRight: " + rightAction.Label;
    }

    public void Initalize(string _cardName, Action _leftAction, Action _rightAction)
    {
        cardName = _cardName;
        leftAction = _leftAction;
        rightAction = _rightAction;

        SetText();
    }
}
