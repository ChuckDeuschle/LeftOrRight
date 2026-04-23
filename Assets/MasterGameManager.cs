using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MasterGameManager : MonoBehaviour
{
    private static MasterGameManager _instance;
    public static MasterGameManager instance => _instance;

    public enum PrototypeMode { CoreLoop, RequeueEnemy, Infiltrator, Mirror, EnragedBoss }

    // Set by PrototypeButton before scene load; read by GameManager when MGM instance is absent.
    public static PrototypeMode pendingPrototype = PrototypeMode.CoreLoop;

    public PrototypeMode selectedPrototype;

    // Game Data
    public Player player;

    public List<Card> deck;
    public List<Encounter> encounterList;
    public GameObject cardPrefab; // This should be your Card prefab
    public Encounter selectedEncounter;
    // You can add other game data that you need to pass across scenes here...

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);

        player = new Player();
        player.Initalize(100, 0);

        deck = CreateStarterDeck();

        encounterList = CreateEncounterPath();
        selectedEncounter = encounterList[0];
    }

    void Start()
    {
        AssignEncountersToButtons();
    }

    void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
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
        DontDestroyOnLoad(card.gameObject);

        card = Instantiate(cardPrefab).GetComponent<Card>();
        rightAction = new Action();
        rightAction.Initalize("Attack 5", Action.Actions.Attack, 5);
        leftAction = new Action();
        leftAction.Initalize("Shield 5", Action.Actions.Shield, 5);
        card.Initalize("Basic 1", leftAction, rightAction);
        newDeck.Add(card);
        // Initially, we'll deactivate the card objects
        card.gameObject.SetActive(false);
        DontDestroyOnLoad(card.gameObject);

        card = Instantiate(cardPrefab).GetComponent<Card>();
        rightAction = new Action();
        rightAction.Initalize("Attack 5", Action.Actions.Attack, 5);
        leftAction = new Action();
        leftAction.Initalize("Shield 5", Action.Actions.Shield, 5);
        card.Initalize("Basic 1", leftAction, rightAction);
        newDeck.Add(card);
        // Initially, we'll deactivate the card objects
        card.gameObject.SetActive(false);
        DontDestroyOnLoad(card.gameObject);

        card = Instantiate(cardPrefab).GetComponent<Card>();
        rightAction = new Action();
        rightAction.Initalize("Attack 10", Action.Actions.Attack, 10);
        leftAction = new Action();
        leftAction.Initalize("Shield 3", Action.Actions.Shield, 5);
        card.Initalize("Basic 2", leftAction, rightAction);
        newDeck.Add(card);
        // Initially, we'll deactivate the card objects
        card.gameObject.SetActive(false);
        DontDestroyOnLoad(card.gameObject);

        card = Instantiate(cardPrefab).GetComponent<Card>();
        rightAction = new Action();
        rightAction.Initalize("Attack 10", Action.Actions.Attack, 10);
        leftAction = new Action();
        leftAction.Initalize("Shield 3", Action.Actions.Shield, 5);
        card.Initalize("Basic 2", leftAction, rightAction);
        newDeck.Add(card);
        // Initially, we'll deactivate the card objects
        card.gameObject.SetActive(false);
        DontDestroyOnLoad(card.gameObject);

        return newDeck;
    }

    private List<Encounter> CreateEncounterPath()
    {
        List<Encounter> encounterList = new List<Encounter>();
        List<Card> awardCards;
        Encounter encounter;
        Enemy enemy;
        Card card;
        Action rightAction, leftAction;

        // Encounter 1
        encounter = new Encounter();
        enemy = new Enemy();
        awardCards = new List<Card>();

        card = Instantiate(cardPrefab).GetComponent<Card>();
        rightAction = new Action();
        rightAction.Initalize("Attack 10", Action.Actions.Attack, 10);
        leftAction = new Action();
        leftAction.Initalize("Shield 10", Action.Actions.Shield, 5);
        card.Initalize("Basic 3", leftAction, rightAction);
        // Initially, we'll deactivate the card objects
        card.gameObject.SetActive(false);
        DontDestroyOnLoad(card.gameObject);
        awardCards.Add(card);

        card = Instantiate(cardPrefab).GetComponent<Card>();
        rightAction = new Action();
        rightAction.Initalize("Attack 10", Action.Actions.Attack, 10);
        leftAction = new Action();
        leftAction.Initalize("Shield 10", Action.Actions.Shield, 5);
        card.Initalize("Basic 3", leftAction, rightAction);
        // Initially, we'll deactivate the card objects
        card.gameObject.SetActive(false);
        DontDestroyOnLoad(card.gameObject);
        awardCards.Add(card);

        card = Instantiate(cardPrefab).GetComponent<Card>();
        rightAction = new Action();
        rightAction.Initalize("Attack 10", Action.Actions.Attack, 10);
        leftAction = new Action();
        leftAction.Initalize("Shield 10", Action.Actions.Shield, 5);
        card.Initalize("Basic 3", leftAction, rightAction);
        // Initially, we'll deactivate the card objects
        card.gameObject.SetActive(false);
        DontDestroyOnLoad(card.gameObject);
        awardCards.Add(card);

        enemy.Initalize("Enemy 1", 50);
        encounter.Initalize("Encounter 1", "Encounter1", "Fight against Enemy 1. Enemy 1's attacks increase by 10 damage each round.", enemy, 100, awardCards, Encounter.Status.available);
        encounterList.Add(encounter);

        // Encounter 2
        encounter = new Encounter();
        enemy = new Enemy();
        awardCards = new List<Card>();

        card = Instantiate(cardPrefab).GetComponent<Card>();
        rightAction = new Action();
        rightAction.Initalize("Attack 10", Action.Actions.Attack, 10);
        leftAction = new Action();
        leftAction.Initalize("Shield 10", Action.Actions.Shield, 5);
        card.Initalize("Basic 3", leftAction, rightAction);
        // Initially, we'll deactivate the card objects
        card.gameObject.SetActive(false);
        DontDestroyOnLoad(card.gameObject);
        awardCards.Add(card);

        card = Instantiate(cardPrefab).GetComponent<Card>();
        rightAction = new Action();
        rightAction.Initalize("Attack 10", Action.Actions.Attack, 10);
        leftAction = new Action();
        leftAction.Initalize("Shield 10", Action.Actions.Shield, 5);
        card.Initalize("Basic 3", leftAction, rightAction);
        // Initially, we'll deactivate the card objects
        card.gameObject.SetActive(false);
        DontDestroyOnLoad(card.gameObject);
        awardCards.Add(card);

        card = Instantiate(cardPrefab).GetComponent<Card>();
        rightAction = new Action();
        rightAction.Initalize("Attack 10", Action.Actions.Attack, 10);
        leftAction = new Action();
        leftAction.Initalize("Shield 10", Action.Actions.Shield, 5);
        card.Initalize("Basic 3", leftAction, rightAction);
        // Initially, we'll deactivate the card objects
        card.gameObject.SetActive(false);
        DontDestroyOnLoad(card.gameObject);
        awardCards.Add(card);

        enemy.Initalize("Enemy 2", 75);
        encounter.Initalize("Encounter 2", "Encounter2", "Fight against Enemy 2. Enemy 2's attacks increase by 10 damage each round.", enemy, 200, awardCards, Encounter.Status.pending);
        encounterList.Add(encounter);

        return encounterList;
    }

    public void SetupTemplateEncounter()
    {
        player.Initalize(100, 0);

        foreach (Card card in deck)
        {
            Destroy(card.gameObject);
        }
        deck = CreateStarterDeck();

        Enemy templateEnemy = new Enemy();
        templateEnemy.Initalize("Training Dummy", 100);

        Encounter templateEncounter = new Encounter();
        templateEncounter.Initalize("Template Encounter", "", "A training encounter to test prototype rules.", templateEnemy, 0, new List<Card>(), Encounter.Status.available);
        selectedEncounter = templateEncounter;
    }

    public void AssignEncountersToButtons()
    {
        // Assign encounters to buttons
        foreach (Encounter encounter in encounterList)
        {
            string buttonName = encounter.encounterButton;
            GameObject buttonObj = GameObject.Find(buttonName);
            if (buttonObj != null)
            {
                EncounterSelectionButton encounterSelectionButton = buttonObj.GetComponent<EncounterSelectionButton>();
                Button button = buttonObj.GetComponent<Button>();

                if (encounterSelectionButton != null)
                {
                    encounterSelectionButton.encounter = encounter;
                    button.interactable = encounter.currentStatus == Encounter.Status.available;
                }
            }
        }
    }
}
