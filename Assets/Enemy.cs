using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int currentHealth;

    public void Initalize(int _startingHealth)
    {
        currentHealth = _startingHealth;
    }

    public void UpdateEnemyStatus(GameManager _gameManager)
    {
        _gameManager.enemyStatusText.text = "Enemy Status:\n";
        _gameManager.enemyStatusText.text += "Current Round: " + _gameManager.currentRound + "\n";
        _gameManager.enemyStatusText.text += "Next attack: " + _gameManager.currentRound * 10 + "\n";
    }
}
