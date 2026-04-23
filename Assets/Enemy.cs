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

    // Optional compound intent modifier: when true, EnemyAction also shuffles
    // a trap card into the deck each time it fires. Flipped on by
    // InfiltratorRules.Initialize.
    public bool addsTrap;

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
        addsTrap = false;
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
        addsTrap = false;
    }

    public void UpdateEnemyStatus(GameManager _gameManager)
    {
        string action = intentAction.ToString().ToLower();
        string intent = $"Intends to {action} {intentValue}";
        if (addsTrap) { intent += " and add trap"; }
        intent += $" in {intentCountdown} cards";
        _gameManager.enemyStatusText.text = "Enemy Status:\n" + intent + "\n";
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

        if (addsTrap)
        {
            SpawnTrap(_gameManager);
        }

        _gameManager.activeRules.OnEnemyTurnEnd(_gameManager);
    }

    public void ScheduleNextIntent(GameManager _gameManager)
    {
        baseIntentValue += escalationPerCycle;
        intentValue = baseIntentValue;
        intentCountdown = countdownLength;
    }

    // Shuffles a fresh trap card into the deck. Trap cards have no-op left/right
    // actions; InfiltratorRules.OnCardPlayed applies the 5 damage when a trap is
    // played (regardless of side).
    public void SpawnTrap(GameManager _gameManager)
    {
        GameObject prefab = _gameManager.cardPrefab;
        if (prefab == null && MasterGameManager.instance != null)
        {
            prefab = MasterGameManager.instance.cardPrefab;
        }
        if (prefab == null)
        {
            Debug.LogWarning("Enemy.SpawnTrap: GameManager.cardPrefab is not assigned — cannot create trap card.");
            return;
        }

        GameObject trapObj = Object.Instantiate(prefab);
        Card trapCard = trapObj.GetComponent<Card>();
        if (trapCard == null)
        {
            Object.Destroy(trapObj);
            return;
        }

        Action leftNoop = new Action();
        leftNoop.Initalize("Trap!", Action.Actions.Attack, 0);
        Action rightNoop = new Action();
        rightNoop.Initalize("Trap!", Action.Actions.Attack, 0);
        trapCard.Initalize("Trap", leftNoop, rightNoop);
        trapCard.isTrap = true;
        trapObj.SetActive(false);

        // Insert at a random position in the deck (never at position 0 so it
        // cannot displace a card being drawn this frame).
        int insertAt = _gameManager.deck.Count == 0 ? 0 : Random.Range(1, _gameManager.deck.Count + 1);
        _gameManager.deck.Insert(insertAt, trapCard);

        Debug.Log($"[Infiltrator] Trap added to deck at index {insertAt}. Deck size: {_gameManager.deck.Count}");
    }
}
