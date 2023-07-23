using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Player player;
    public Enemy currentEnemy;
    public GameObject cardPrefab; // This should be your Card prefab

    public List<Card> deck;
    public List<Card> discardPile;

    public GameObject currentCard;
    public int currentRound = 0;

    public TextMeshProUGUI playerHealthText;
    public TextMeshProUGUI enemyHealthText;
    public TextMeshProUGUI playerStatusText;
    public TextMeshProUGUI enemyStatusText;

    public GameObject GameOverPanel;
    public GameObject WinPanel;


    enum GameState { PlayerTurn, EnemyTurn, Win, Lose };
    GameState gameState;

    private void Start()
    {
        // Initialize the game
        gameState = GameState.PlayerTurn;
        currentCard = GameObject.Find("Card");
        discardPile = new List<Card>();
        deck = CreateStarterDeck();
        Destroy(currentCard);

        // ... initialize player and first enemy
        player = new Player();
        player.Initalize(100); // set appropriate starting values
        currentEnemy = new Enemy();
        currentEnemy.Initalize(50);

        // Shuffle the deck
        deck = Shuffle(deck);

        // Draw the top card
        DrawCard();

        UpdateHealthDisplay();
        UpdateStatusDisplays();
    }

    private void Update()
    {
        switch (gameState)
        {
            case GameState.PlayerTurn:
                // Handle player's turn logic
                // If player ends turn or runs out of actions, set gameState to EnemyTurn
                break;
            case GameState.EnemyTurn:
                // Handle enemy's turn logic
                // Attack player 
                int damage = (currentRound * 10);
                player.currentHealth -= damage < player.shield ? 0 : damage - player.shield;
                // Set player shield to zero
                player.shield = 0;
                // If enemy ends turn or runs out of actions, set gameState to PlayerTurn
                UpdateHealthDisplay();
                UpdateStatusDisplays();
                gameState = GameState.PlayerTurn;
                break;
            case GameState.Win:
                // Handle victory
                // Could show a victory screen, start a new game, etc.
                WinPanel.SetActive(true);
                break;
            case GameState.Lose:
                // Handle game over
                // Could show a game over screen, offer to restart, etc.
                GameOverPanel.SetActive(true);
                break;
        }
    }

    public void DrawCard()
    {
        // If the deck is now empty, reshuffle the discard pile into a new deck
        if (deck.Count == 0)
        {
            deck = Shuffle(discardPile);
            discardPile = new List<Card>();
            gameState = GameState.EnemyTurn;
        }

        // Move the top card to the current card
        Card card = deck[0];
        deck.RemoveAt(0);

        currentCard = card.gameObject;

        // Activate the card object and place it in the starting position
        currentCard.transform.position = new Vector3(0, 5, -2.1f);
        currentCard.SetActive(true);
        // currentCard.GetComponent<MeshRenderer>().material.color = Color.red;

        DebugCardLists();
    }

    public void CheckEndConditions()
    {
        if (player.currentHealth <= 0)
        {
            gameState = GameState.Lose;
        }
        else if (currentEnemy.currentHealth <= 0)
        {
            gameState = GameState.Win;
        }
    }

    private List<Card> Shuffle(List<Card> cards)
    {
        System.Random rng = new System.Random();
        int n = cards.Count;

        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            Card value = cards[k];
            cards[k] = cards[n];
            cards[n] = value;
        }

        currentRound++;

        return cards;
    }

    public void UpdateHealthDisplay()
    {
        playerHealthText.text = "Player Health: " + player.currentHealth;
        enemyHealthText.text = "Enemy Health: " + currentEnemy.currentHealth;
    }

    public void UpdateStatusDisplays()
    {
        player.UpdatePlayerStatus(this);
        currentEnemy.UpdateEnemyStatus(this);
    }

    public void DebugCardLists()
    {
        Debug.Log("Current Card: " + currentCard.GetComponent<Card>().cardName);
        Debug.Log("Deck: " + string.Join(", ", deck.Select(c => c.cardName)));
        Debug.Log("Discard Pile: " + string.Join(", ", discardPile.Select(c => c.cardName)));
    }

    private List<Card> CreateStarterDeck()
    {
        List<Card> newDeck = new List<Card>();
        Card card;
        Action leftAction, rightAction;

        // 10 starter Attack/Shield cards
        card = Instantiate(cardPrefab).GetComponent<Card>();
        rightAction = new Action();
        rightAction.Initalize("Attack 10", Action.Actions.Attack, 10);
        leftAction = new Action();
        leftAction.Initalize("Shield 1", Action.Actions.Shield, 1);
        card.Initalize("Starter 0", leftAction, rightAction);
        newDeck.Add(card);
        // Initially, we'll deactivate the card objects
        card.gameObject.SetActive(false);

        card = Instantiate(cardPrefab).GetComponent<Card>();
        rightAction = new Action();
        rightAction.Initalize("Attack 9", Action.Actions.Attack, 9);
        leftAction = new Action();
        leftAction.Initalize("Shield 2", Action.Actions.Shield, 2);
        card.Initalize("Starter 1", leftAction, rightAction);
        newDeck.Add(card);
        // Initially, we'll deactivate the card objects
        card.gameObject.SetActive(false);

        card = Instantiate(cardPrefab).GetComponent<Card>();
        rightAction = new Action();
        rightAction.Initalize("Attack 8", Action.Actions.Attack, 8);
        leftAction = new Action();
        leftAction.Initalize("Shield 3", Action.Actions.Shield, 3);
        card.Initalize("Starter 2", leftAction, rightAction);
        newDeck.Add(card);
        // Initially, we'll deactivate the card objects
        card.gameObject.SetActive(false);

        card = Instantiate(cardPrefab).GetComponent<Card>();
        rightAction = new Action();
        rightAction.Initalize("Attack 7", Action.Actions.Attack, 7);
        leftAction = new Action();
        leftAction.Initalize("Shield 4", Action.Actions.Shield, 4);
        card.Initalize("Starter 3", leftAction, rightAction);
        newDeck.Add(card);
        // Initially, we'll deactivate the card objects
        card.gameObject.SetActive(false);

        card = Instantiate(cardPrefab).GetComponent<Card>();
        rightAction = new Action();
        rightAction.Initalize("Attack 6", Action.Actions.Attack, 6);
        leftAction = new Action();
        leftAction.Initalize("Shield 5", Action.Actions.Shield, 5);
        card.Initalize("Starter 4", leftAction, rightAction);
        newDeck.Add(card);
        // Initially, we'll deactivate the card objects
        card.gameObject.SetActive(false);

        card = Instantiate(cardPrefab).GetComponent<Card>();
        rightAction = new Action();
        rightAction.Initalize("Attack 5", Action.Actions.Attack, 5);
        leftAction = new Action();
        leftAction.Initalize("Shield 6", Action.Actions.Shield, 6);
        card.Initalize("Starter 5", leftAction, rightAction);
        newDeck.Add(card);
        // Initially, we'll deactivate the card objects
        card.gameObject.SetActive(false);

        card = Instantiate(cardPrefab).GetComponent<Card>();
        rightAction = new Action();
        rightAction.Initalize("Attack 4", Action.Actions.Attack, 4);
        leftAction = new Action();
        leftAction.Initalize("Shield 7", Action.Actions.Shield, 7);
        card.Initalize("Starter 6", leftAction, rightAction);
        newDeck.Add(card);
        // Initially, we'll deactivate the card objects
        card.gameObject.SetActive(false);

        card = Instantiate(cardPrefab).GetComponent<Card>();
        rightAction = new Action();
        rightAction.Initalize("Attack 3", Action.Actions.Attack, 3);
        leftAction = new Action();
        leftAction.Initalize("Shield 8", Action.Actions.Shield, 8);
        card.Initalize("Starter 7", leftAction, rightAction);
        newDeck.Add(card);
        // Initially, we'll deactivate the card objects
        card.gameObject.SetActive(false);

        card = Instantiate(cardPrefab).GetComponent<Card>();
        rightAction = new Action();
        rightAction.Initalize("Attack 2", Action.Actions.Attack, 2);
        leftAction = new Action();
        leftAction.Initalize("Shield 9", Action.Actions.Shield, 9);
        card.Initalize("Starter 8", leftAction, rightAction);
        newDeck.Add(card);
        // Initially, we'll deactivate the card objects
        card.gameObject.SetActive(false);

        card = Instantiate(cardPrefab).GetComponent<Card>();
        rightAction = new Action();
        rightAction.Initalize("Attack 1", Action.Actions.Attack, 1);
        leftAction = new Action();
        leftAction.Initalize("Shield 10", Action.Actions.Shield, 10);
        card.Initalize("Starter 9", leftAction, rightAction);
        newDeck.Add(card);
        // Initially, we'll deactivate the card objects
        card.gameObject.SetActive(false);

        return newDeck;
    }
}