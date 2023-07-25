using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncounterSelectionButton : MonoBehaviour
{
    public Encounter encounter;

    public void UpdateEncounterSelection()
    {
        MasterGameManager.instance.selectedEncounter = encounter;
    }
}
