using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy
{
    public int currentHealth;
    public string name;

    public void Initalize(string _name, int _startingHealth)
    {
        name = _name;
        currentHealth = _startingHealth;
    }

    public void UpdateEnemyStatus(GameManager _gameManager)
    {
        _gameManager.enemyStatusText.text = "Enemy Status:\n";
        _gameManager.enemyStatusText.text += "Current Round: " + _gameManager.currentRound + "\n";
        _gameManager.enemyStatusText.text += "Next attack: " + _gameManager.currentRound * 10 + "\n";
    }

    public void EnemyAction(GameManager _gameManager)
    {
        // Attack player 
        int damage = (_gameManager.currentRound * 10);
        _gameManager.player.currentHealth -= damage < _gameManager.player.shield ? 0 : damage - _gameManager.player.shield;
        // Set player shield to zero
        _gameManager.player.shield = 0;
    }
}
