using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MasterGameManager : MonoBehaviour
{
    public static MasterGameManager instance = null;  // Singleton instance

    // Game Data
    public Player player;
 
    public List<Card> deck;
    public List<Encounter> encounterList;
    public GameObject cardPrefab; // This should be your Card prefab
    public Encounter selectedEncounter;
    // You can add other game data that you need to pass across scenes here...

    void Awake()
    {
        // Singleton pattern
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    // Use this for initialization
    void Start()
    {
        player = new Player();
        player.Initalize(100, 0);

        deck = CreateStarterDeck();

        encounterList = CreateEncounterPath();
        selectedEncounter = encounterList[0];
        AssignEncountersToButtons();
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
