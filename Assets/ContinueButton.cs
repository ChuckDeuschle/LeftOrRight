using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ContinueButton : MonoBehaviour
{
    public GameManager gameManager;

    public void ContinueGame()
    {
        Encounter completedEncounter = MasterGameManager.instance.selectedEncounter;
        completedEncounter.currentStatus = Encounter.Status.complete;

        // Grant rewards from the completed encounter before advancing
        List<Card> awardCards = completedEncounter.awardCards;
        if (gameManager.awardCardSelection >= 0 && gameManager.awardCardSelection < awardCards.Count)
        {
            MasterGameManager.instance.deck.Add(awardCards[gameManager.awardCardSelection]);
        }
        MasterGameManager.instance.player.goldValue += completedEncounter.goldAward;

        // Advance selectedEncounter to the next one (if any)
        List<Encounter> encounters = MasterGameManager.instance.encounterList;
        int selectedEncounterPosition = encounters.IndexOf(completedEncounter);
        if (selectedEncounterPosition + 1 < encounters.Count)
        {
            encounters[selectedEncounterPosition + 1].currentStatus = Encounter.Status.available;
            MasterGameManager.instance.selectedEncounter = encounters[selectedEncounterPosition + 1];
        }

        SceneManager.LoadScene("EncountersScene");
    }
}
