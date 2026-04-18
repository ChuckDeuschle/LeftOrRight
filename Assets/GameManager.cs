using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

        EnsurePrototypeUI();

        UpdateHealthDisplay();
        UpdateStatusDisplays();
        UpdateWinDisplay();
        UpdatePrototypeUI();
    }

    // Creates the prototype UI (status label, End Turn button, template win/lose panels)
    // programmatically if they were not wired up in the Inspector. This keeps the
    // GameScene scene file untouched while still giving every prototype the UI it needs.
    private void EnsurePrototypeUI()
    {
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null) { return; }

        if (prototypeStatusText == null)
        {
            prototypeStatusText = CreateStatusLabel(canvas);
        }
        if (endTurnButton == null)
        {
            endTurnButton = CreateEndTurnButton(canvas);
        }
        if (templateWinPanel == null)
        {
            templateWinPanel = CreateTemplatePanel(canvas, "Prototype Complete!");
        }
        if (templateLosePanel == null)
        {
            templateLosePanel = CreateTemplatePanel(canvas, "Defeated");
        }
    }

    private TextMeshProUGUI CreateStatusLabel(Canvas canvas)
    {
        GameObject obj = new GameObject("PrototypeStatus");
        obj.transform.SetParent(canvas.transform, false);
        RectTransform rt = obj.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 1f);
        rt.anchorMax = new Vector2(0.5f, 1f);
        rt.pivot = new Vector2(0.5f, 1f);
        rt.anchoredPosition = new Vector2(0, -50);
        rt.sizeDelta = new Vector2(500, 60);
        TextMeshProUGUI text = obj.AddComponent<TextMeshProUGUI>();
        text.alignment = TextAlignmentOptions.Center;
        text.fontSize = 32;
        text.color = Color.white;
        text.text = "";
        return text;
    }

    private GameObject CreateEndTurnButton(Canvas canvas)
    {
        GameObject obj = new GameObject("EndTurnButton");
        obj.transform.SetParent(canvas.transform, false);
        RectTransform rt = obj.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(1f, 0f);
        rt.anchorMax = new Vector2(1f, 0f);
        rt.pivot = new Vector2(1f, 0f);
        rt.anchoredPosition = new Vector2(-50, 50);
        rt.sizeDelta = new Vector2(180, 60);
        Image img = obj.AddComponent<Image>();
        img.color = new Color(0.85f, 0.3f, 0.3f, 1f);
        Button btn = obj.AddComponent<Button>();
        btn.targetGraphic = img;
        btn.onClick.AddListener(EndPlayerTurn);

        AddButtonLabel(obj, "End Turn", 24);
        return obj;
    }

    private GameObject CreateTemplatePanel(Canvas canvas, string titleText)
    {
        GameObject panel = new GameObject(titleText == "Defeated" ? "TemplateLosePanel" : "TemplateWinPanel");
        panel.transform.SetParent(canvas.transform, false);
        RectTransform rt = panel.AddComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.sizeDelta = Vector2.zero;
        rt.anchoredPosition = Vector2.zero;
        Image bg = panel.AddComponent<Image>();
        bg.color = new Color(0f, 0f, 0f, 0.75f);

        GameObject titleObj = new GameObject("Title");
        titleObj.transform.SetParent(panel.transform, false);
        RectTransform titleRt = titleObj.AddComponent<RectTransform>();
        titleRt.anchorMin = new Vector2(0.5f, 0.5f);
        titleRt.anchorMax = new Vector2(0.5f, 0.5f);
        titleRt.pivot = new Vector2(0.5f, 0.5f);
        titleRt.anchoredPosition = new Vector2(0, 100);
        titleRt.sizeDelta = new Vector2(700, 100);
        TextMeshProUGUI title = titleObj.AddComponent<TextMeshProUGUI>();
        title.text = titleText;
        title.fontSize = 56;
        title.alignment = TextAlignmentOptions.Center;
        title.color = Color.white;

        RetryButton retryComp = panel.AddComponent<RetryButton>();

        GameObject retryBtn = CreatePanelButton(panel, "Retry", new Vector2(-130, -40));
        retryBtn.GetComponent<Button>().onClick.AddListener(retryComp.RetryPrototype);

        GameObject backBtn = CreatePanelButton(panel, "Back to Menu", new Vector2(130, -40));
        backBtn.GetComponent<Button>().onClick.AddListener(retryComp.BackToMenu);

        panel.SetActive(false);
        return panel;
    }

    private GameObject CreatePanelButton(GameObject parent, string label, Vector2 pos)
    {
        GameObject obj = new GameObject(label.Replace(" ", "") + "Button");
        obj.transform.SetParent(parent.transform, false);
        RectTransform rt = obj.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = pos;
        rt.sizeDelta = new Vector2(220, 60);
        Image img = obj.AddComponent<Image>();
        img.color = new Color(0.9f, 0.9f, 0.9f, 1f);
        Button btn = obj.AddComponent<Button>();
        btn.targetGraphic = img;

        AddButtonLabel(obj, label, 22);
        return obj;
    }

    private void AddButtonLabel(GameObject parent, string label, int fontSize)
    {
        GameObject textObj = new GameObject("Label");
        textObj.transform.SetParent(parent.transform, false);
        RectTransform rt = textObj.AddComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.sizeDelta = Vector2.zero;
        rt.anchoredPosition = Vector2.zero;
        TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
        text.alignment = TextAlignmentOptions.Center;
        text.fontSize = fontSize;
        text.color = Color.black;
        text.text = label;
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
