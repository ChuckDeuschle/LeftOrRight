using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    public int currentHealth;
    public int shield = 0;
    public int goldValue = 0;

    public void Initalize(int _startingHealth, int _goldValue)
    {
        currentHealth = _startingHealth;
        goldValue = _goldValue;
    }

    public void UpdatePlayerStatus(GameManager _gameManager)
    {
        _gameManager.playerStatusText.text = "Player Status:\n";
        _gameManager.playerStatusText.text = "Cards in Deck: " + _gameManager.deck.Count + "\n";

        if (shield > 0)
        {
            _gameManager.playerStatusText.text += "Shield: " + shield + "\n";
        }
    }
}
