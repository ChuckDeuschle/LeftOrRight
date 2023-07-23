using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int currentHealth;
    public int shield;

    public void Initalize(int _startingHealth)
    {
        currentHealth = _startingHealth;
    }

    public void UpdatePlayerStatus(GameManager _gameManager)
    {
        _gameManager.playerStatusText.text = "Player Status:\n";
        _gameManager.playerStatusText.text = "Cards in Deck: " + _gameManager.deck.Count + "\n";

        if (shield > 0)
        {
            _gameManager.playerStatusText.text += "Sheild: " + shield + "\n";
        }
    }
}
