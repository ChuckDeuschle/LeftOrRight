using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy
{
    public int currentHealth;
    public int startingHealth;
    public string name;

    // Intent / countdown (core loop)
    public Action.Actions intentAction;
    public int intentValue;
    public int intentCountdown;
    public int baseIntentValue;
    public int escalationPerCycle;
    public int countdownLength;

    public void Initalize(string _name, int _startingHealth)
    {
        name = _name;
        startingHealth = _startingHealth;
        currentHealth = _startingHealth;

        // Default intent: Attack 7 every 4 cards, +3 per cycle.
        intentAction = Action.Actions.Attack;
        baseIntentValue = 7;
        intentValue = 7;
        escalationPerCycle = 3;
        countdownLength = 4;
        intentCountdown = countdownLength;
    }

    public void Initalize(string _name, int _startingHealth, Action.Actions _intentAction, int _baseIntentValue, int _countdownLength, int _escalationPerCycle)
    {
        name = _name;
        startingHealth = _startingHealth;
        currentHealth = _startingHealth;

        intentAction = _intentAction;
        baseIntentValue = _baseIntentValue;
        intentValue = _baseIntentValue;
        escalationPerCycle = _escalationPerCycle;
        countdownLength = _countdownLength;
        intentCountdown = countdownLength;
    }

    public void UpdateEnemyStatus(GameManager _gameManager)
    {
        _gameManager.enemyStatusText.text = "Enemy Status:\n";
        _gameManager.enemyStatusText.text += $"{intentAction} {intentValue} in {intentCountdown} cards\n";
    }

    public void EnemyAction(GameManager _gameManager)
    {
        int value = _gameManager.activeRules.ModifyEnemyDamage(intentValue, _gameManager);

        switch (intentAction)
        {
            case Action.Actions.Attack:
            {
                int absorbed = value < _gameManager.player.shield ? value : _gameManager.player.shield;
                _gameManager.player.currentHealth -= value - absorbed;
                _gameManager.player.shield = 0;
                break;
            }
            case Action.Actions.Shield:
            {
                // Enemy "Shield" = self-heal capped at startingHealth.
                currentHealth += value;
                if (currentHealth > startingHealth) { currentHealth = startingHealth; }
                break;
            }
        }

        _gameManager.activeRules.OnEnemyTurnEnd(_gameManager);
    }

    public void ScheduleNextIntent(GameManager _gameManager)
    {
        baseIntentValue += escalationPerCycle;
        intentValue = baseIntentValue;
        intentCountdown = countdownLength;
    }
}
