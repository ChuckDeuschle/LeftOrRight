using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action : MonoBehaviour
{
    public enum Actions { Attack, Shield }
    public Actions ActionType;
    public string Label;
    public int Magnitude;

    // Start is called before the first frame update
    public void Initalize(string _label, Actions _action, int _magnitude)
    {
        Label = _label;
        ActionType = _action;
        Magnitude = _magnitude;
    }

    public void Play(GameManager _gameManager)
    {
        // Complete Action
        switch (ActionType)
        {
            case Actions.Attack:
                _gameManager.currentEnemy.currentHealth -= Magnitude;
                break;
            case Actions.Shield:
                _gameManager.player.shield += Magnitude;
                break;
            default: break;
        }

        Debug.Log("Action played: " + Label);
    }
}
