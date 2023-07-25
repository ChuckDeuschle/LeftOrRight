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
        // Complete the encounter
        MasterGameManager.instance.selectedEncounter.currentStatus = Encounter.Status.complete;

        // Set the next encounter in the list to available
        List<Encounter> encounters = MasterGameManager.instance.encounterList;
        int selectedEncounterPosition = encounters.IndexOf(MasterGameManager.instance.selectedEncounter);
        if (selectedEncounterPosition + 1 < encounters.Count)
        {
            encounters[selectedEncounterPosition + 1].currentStatus = Encounter.Status.available;
            MasterGameManager.instance.selectedEncounter = encounters[selectedEncounterPosition + 1];
        }

        // Grant player rewards, will want to push this to a reward method later for multiple encounters
        MasterGameManager.instance.deck.Add(MasterGameManager.instance.selectedEncounter.awardCards[gameManager.awardCardSelection]);
        MasterGameManager.instance.player.goldValue += MasterGameManager.instance.selectedEncounter.goldAward;
        SceneManager.LoadScene("EncountersScene");
    }
}
