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
    public TextMeshProUGUI winText;

    public GameObject GameOverPanel;
    public GameObject WinPanel;

    public enum GameState { PlayerTurn, EnemyTurn, Win, Lose };
    public GameState gameState;

    public int awardCardSelection = -1;

    private void Start()
    {
        // Initialize the game
        gameState = GameState.PlayerTurn;
        currentCard = GameObject.Find("Card");
        discardPile = new List<Card>();
        
        // If we're not coming from the encounter select screen, initalize with some defaults
        if (MasterGameManager.instance == null)
        {
            deck = CreateStarterDeck();
            player = new Player();
            player.Initalize(100, 0);
            currentEnemy = new Enemy();
            currentEnemy.Initalize("Enemy 1", 50);
        }
        // If we have a MasterGameManager, pull from that instead
        else
        {
            deck = MasterGameManager.instance.deck;
            player = MasterGameManager.instance.player;
            currentEnemy = MasterGameManager.instance.selectedEncounter.enemy;
        }

        // Shuffle the deck
        deck = Shuffle(deck);

        Destroy(currentCard);

        // Draw the top card
        DrawCard();

        UpdateHealthDisplay();
        UpdateStatusDisplays();
        UpdateWinDisplay();
    }

    public void Update()
    {
        if (player.currentHealth <= 0)
        {
            gameState = GameState.Lose;
        }
        else if (currentEnemy.currentHealth <= 0)
        {
            gameState = GameState.Win;
        }

        switch (gameState)
        {
            case GameState.PlayerTurn:
                // Handle player's turn logic
                // If player ends turn or runs out of actions, set gameState to EnemyTurn
                if (deck.Count == 0 && currentCard.activeSelf == false)
                {
                    DrawCard();
                }
                break;
            case GameState.EnemyTurn:
                // Handle enemy's turn logic
                // If enemy ends turn or runs out of actions, set gameState to PlayerTurn
                currentEnemy.EnemyAction(this);
                gameState = GameState.PlayerTurn;
                break;
            case GameState.Win:
                // Handle victory
                // Could show a victory screen, put discarded cards back into deck and current card as well
                if (discardPile.Count > 0)
                {
                    deck.AddRange(discardPile);
                    discardPile.Clear();
                }
                if (currentCard.activeSelf)
                {
                    currentCard.SetActive(false);
                    deck.Add(currentCard.GetComponent<Card>());
                }

                if (MasterGameManager.instance != null)
                    MasterGameManager.instance.deck = deck;

                WinPanel.SetActive(true);
                break;
            case GameState.Lose:
                // Handle game over
                // Could show a game over screen, offer to restart, etc.
                GameOverPanel.SetActive(true);
                break;
        }

        UpdateHealthDisplay();
        UpdateStatusDisplays();
    }

    public void DrawCard()
    {
        // If the deck is now empty, reshuffle the discard pile into a new deck
        if (deck.Count == 0)
        {
            deck = Shuffle(discardPile);
            discardPile = new List<Card>();
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

    public void UpdateWinDisplay()
    {
        string goldText = MasterGameManager.instance != null
            ? MasterGameManager.instance.selectedEncounter.goldAward + " gold"
            : "some gold";
        winText.text = "You have defeated " + currentEnemy.name + "!\nYou gain " + goldText + ".\nChoose a card to add to your deck:";
    }

    public void DebugCardLists()
    {
        Debug.Log("Current Card: " + currentCard.GetComponent<Card>().cardName);
        Debug.Log("Deck: " + string.Join(", ", deck.Select(c => c.cardName)));
        Debug.Log("Discard Pile: " + string.Join(", ", discardPile.Select(c => c.cardName)));
    }

    public List<Card> CreateStarterDeck()
    {
        List<Card> newDeck = new List<Card>();
        Card card;
        Action leftAction, rightAction;

        // 10 starter Attack/Shield cards
        card = Instantiate(cardPrefab).GetComponent<Card>();
        rightAction = new Action();
        rightAction.Initalize("Attack 5", Action.Actions.Attack, 10);
        leftAction = new Action();
        leftAction.Initalize("Shield 5", Action.Actions.Shield, 5);
        card.Initalize("Basic 1", leftAction, rightAction);
        newDeck.Add(card);
        // Initially, we'll deactivate the card objects
        card.gameObject.SetActive(false);

        card = Instantiate(cardPrefab).GetComponent<Card>();
        rightAction = new Action();
        rightAction.Initalize("Attack 5", Action.Actions.Attack, 5);
        leftAction = new Action();
        leftAction.Initalize("Shield 5", Action.Actions.Shield, 5);
        card.Initalize("Basic 1", leftAction, rightAction);
        newDeck.Add(card);
        // Initially, we'll deactivate the card objects
        card.gameObject.SetActive(false);

        card = Instantiate(cardPrefab).GetComponent<Card>();
        rightAction = new Action();
        rightAction.Initalize("Attack 5", Action.Actions.Attack, 5);
        leftAction = new Action();
        leftAction.Initalize("Shield 5", Action.Actions.Shield, 5);
        card.Initalize("Basic 1", leftAction, rightAction);
        newDeck.Add(card);
        // Initially, we'll deactivate the card objects
        card.gameObject.SetActive(false);

        card = Instantiate(cardPrefab).GetComponent<Card>();
        rightAction = new Action();
        rightAction.Initalize("Attack 10", Action.Actions.Attack, 10);
        leftAction = new Action();
        leftAction.Initalize("Shield 3", Action.Actions.Shield, 5);
        card.Initalize("Basic 2", leftAction, rightAction);
        newDeck.Add(card);
        // Initially, we'll deactivate the card objects
        card.gameObject.SetActive(false);

        card = Instantiate(cardPrefab).GetComponent<Card>();
        rightAction = new Action();
        rightAction.Initalize("Attack 10", Action.Actions.Attack, 10);
        leftAction = new Action();
        leftAction.Initalize("Shield 3", Action.Actions.Shield, 5);
        card.Initalize("Basic 2", leftAction, rightAction);
        newDeck.Add(card);
        // Initially, we'll deactivate the card objects
        card.gameObject.SetActive(false);

        return newDeck;
    }
}