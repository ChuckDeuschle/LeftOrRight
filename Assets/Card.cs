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
