using UnityEngine;
using UnityEngine.SceneManagement;

// Attach to each prototype button on PrototypeSelectScene.
// Set prototypeMode in the Inspector to select which ruleset this button launches.
public class PrototypeButton : MonoBehaviour
{
    public MasterGameManager.PrototypeMode prototypeMode;

    public void SelectPrototype()
    {
        MasterGameManager.pendingPrototype = prototypeMode;

        if (MasterGameManager.instance != null)
        {
            MasterGameManager.instance.selectedPrototype = prototypeMode;
            MasterGameManager.instance.SetupTemplateEncounter();
        }

        SceneManager.LoadScene("GameScene");
    }
}
