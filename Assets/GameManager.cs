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

    // Prototype-specific UI — wire these up in the GameScene Inspector.
    // prototypeStatusText: shows rage meter, chain count, etc. (hidden when empty)
    // endTurnButton: voluntary end-turn for prototypes that support it (e.g. P5)
    // templateWinPanel / templateLosePanel: shown instead of the full reward panels
    //   when playing a template encounter (no MasterGameManager instance present).
    public TextMeshProUGUI prototypeStatusText;
    public GameObject endTurnButton;
    public GameObject templateWinPanel;
    public GameObject templateLosePanel;

    public GameObject GameOverPanel;
    public GameObject WinPanel;

    public enum GameState { PlayerTurn, EnemyTurn, Win, Lose };
    public GameState gameState;

    public int awardCardSelection = -1;

    public PrototypeRules activeRules;

    private void Start()
    {
        gameState = GameState.PlayerTurn;
        currentCard = GameObject.Find("Card");
        discardPile = new List<Card>();

        if (MasterGameManager.instance == null)
        {
            // Direct launch or template mode without a MasterGameManager in the scene.
            activeRules = PrototypeRules.CreateForMode(MasterGameManager.pendingPrototype);
            deck = CreateStarterDeck();
            player = new Player();
            player.Initalize(100, 0);
            currentEnemy = new Enemy();
            currentEnemy.Initalize("Training Dummy", 100);
        }
        else
        {
            activeRules = PrototypeRules.CreateForMode(MasterGameManager.instance.selectedPrototype);
            deck = MasterGameManager.instance.deck;
            player = MasterGameManager.instance.player;
            currentEnemy = MasterGameManager.instance.selectedEncounter.enemy;
        }

        deck = Shuffle(deck);

        Destroy(currentCard);

        DrawCard();

        UpdateHealthDisplay();
        UpdateStatusDisplays();
        UpdateWinDisplay();
        UpdatePrototypeUI();
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
                // Draw a card whenever the active slot is empty (covers both natural
                // deck exhaustion/reshuffle and mid-turn enemy interrupts).
                if (currentCard != null && !currentCard.activeSelf)
                {
                    DrawCard();
                }
                break;
            case GameState.EnemyTurn:
                currentEnemy.EnemyAction(this);
                gameState = GameState.PlayerTurn;
                break;
            case GameState.Win:
                if (discardPile.Count > 0)
                {
                    deck.AddRange(discardPile);
                    discardPile.Clear();
                }
                if (currentCard != null && currentCard.activeSelf)
                {
                    currentCard.SetActive(false);
                    deck.Add(currentCard.GetComponent<Card>());
                }

                if (MasterGameManager.instance != null)
                {
                    MasterGameManager.instance.deck = deck;
                    WinPanel.SetActive(true);
                }
                else
                {
                    if (templateWinPanel != null) { templateWinPanel.SetActive(true); }
                }
                break;
            case GameState.Lose:
                if (MasterGameManager.instance != null)
                {
                    GameOverPanel.SetActive(true);
                }
                else
                {
                    if (templateLosePanel != null) { templateLosePanel.SetActive(true); }
                }
                break;
        }

        UpdateHealthDisplay();
        UpdateStatusDisplays();
        UpdatePrototypeUI();
    }

    public void OnCardPlayed(Card card, DropZone.ZoneSide side)
    {
        activeRules.OnCardPlayed(card, side, this);
    }

    public void EndPlayerTurn()
    {
        gameState = GameState.EnemyTurn;
    }

    public void DrawCard()
    {
        if (deck.Count == 0)
        {
            deck = Shuffle(discardPile);
            discardPile = new List<Card>();
        }

        Card card = deck[0];
        deck.RemoveAt(0);

        currentCard = card.gameObject;

        currentCard.transform.position = new Vector3(0, 5, -2.1f);
        currentCard.SetActive(true);

        DebugCardLists();
    }

    private void UpdatePrototypeUI()
    {
        if (prototypeStatusText != null)
        {
            string statusText = activeRules.GetStatusText(this);
            prototypeStatusText.gameObject.SetActive(!string.IsNullOrEmpty(statusText));
            prototypeStatusText.text = statusText;
        }

        if (endTurnButton != null)
        {
            endTurnButton.SetActive(activeRules.ShowEndTurnButton(this) && gameState == GameState.PlayerTurn);
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
        card.gameObject.SetActive(false);

        card = Instantiate(cardPrefab).GetComponent<Card>();
        rightAction = new Action();
        rightAction.Initalize("Attack 5", Action.Actions.Attack, 5);
        leftAction = new Action();
        leftAction.Initalize("Shield 5", Action.Actions.Shield, 5);
        card.Initalize("Basic 1", leftAction, rightAction);
        newDeck.Add(card);
        card.gameObject.SetActive(false);

        card = Instantiate(cardPrefab).GetComponent<Card>();
        rightAction = new Action();
        rightAction.Initalize("Attack 5", Action.Actions.Attack, 5);
        leftAction = new Action();
        leftAction.Initalize("Shield 5", Action.Actions.Shield, 5);
        card.Initalize("Basic 1", leftAction, rightAction);
        newDeck.Add(card);
        card.gameObject.SetActive(false);

        card = Instantiate(cardPrefab).GetComponent<Card>();
        rightAction = new Action();
        rightAction.Initalize("Attack 10", Action.Actions.Attack, 10);
        leftAction = new Action();
        leftAction.Initalize("Shield 3", Action.Actions.Shield, 5);
        card.Initalize("Basic 2", leftAction, rightAction);
        newDeck.Add(card);
        card.gameObject.SetActive(false);

        card = Instantiate(cardPrefab).GetComponent<Card>();
        rightAction = new Action();
        rightAction.Initalize("Attack 10", Action.Actions.Attack, 10);
        leftAction = new Action();
        leftAction.Initalize("Shield 3", Action.Actions.Shield, 5);
        card.Initalize("Basic 2", leftAction, rightAction);
        newDeck.Add(card);
        card.gameObject.SetActive(false);

        return newDeck;
    }
}
