using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        MasterGameManager.instance.AssignEncountersToButtons();

        playerHealthText.text = "Player Health: " + MasterGameManager.instance.player.currentHealth;
        encounterDetailText.text = MasterGameManager.instance.selectedEncounter.encounterName + "\n"
            + MasterGameManager.instance.selectedEncounter.encounterDescription;

        var sb = new StringBuilder();
        foreach (Card card in MasterGameManager.instance.deck.OrderBy(c => c.cardName))
        {
            sb.AppendLine(card.cardName + " Left: " + card.leftAction.Label + ", Right: " + card.rightAction.Label);
        }
        deckListText.text = sb.ToString();
    }
}
