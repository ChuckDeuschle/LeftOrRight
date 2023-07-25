using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Encounter
{
    public string encounterName;
    public string encounterDescription;
    public string encounterButton;
    public Enemy enemy;
    public int goldAward;
    public List<Card> awardCards;
    public enum Status { available, pending, complete};
    public Status currentStatus = Status.pending;

    public void Initalize(string _encounterName, string _encounterButton, string _encounterDescription, Enemy _enemy, int _goldAward, List<Card> _awardCards, Status _status)
    {
        encounterName = _encounterName;
        encounterButton = _encounterButton;
        encounterDescription = _encounterDescription;
        enemy = _enemy;
        goldAward = _goldAward;
        awardCards = _awardCards;
        currentStatus = _status;
    }
}
