using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EncounterPathManager : MonoBehaviour
{
    public TextMeshProUGUI playerHealthText;
    public TextMeshProUGUI deckListText;
    public TextMeshProUGUI encounterDetailText;

    private void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        MasterGameManager.instance.AssignEncountersToButtons();
        playerHealthText.text = "Player Health: " + MasterGameManager.instance.player.currentHealth;
        encounterDetailText.text = MasterGameManager.instance.selectedEncounter.encounterName + "\n" + MasterGameManager.instance.selectedEncounter.encounterDescription;
        if (deckListText.text == "")
        {
            foreach (Card card in MasterGameManager.instance.deck.OrderBy(card => card.cardName))
            {
                deckListText.text += card.cardName + " " + " Left: " + card.leftAction.Label + ", Right: " + card.rightAction.Label + "\n";
            }
        }
    }
}
