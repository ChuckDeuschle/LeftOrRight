using UnityEngine;
using UnityEngine.SceneManagement;

// Attach to Retry and Back to Menu buttons on templateWinPanel / templateLosePanel.
public class RetryButton : MonoBehaviour
{
    public void RetryPrototype()
    {
        if (MasterGameManager.instance != null)
        {
            MasterGameManager.instance.SetupTemplateEncounter();
        }
        SceneManager.LoadScene("GameScene");
    }

    public void BackToMenu()
    {
        if (MasterGameManager.instance != null)
        {
            Destroy(MasterGameManager.instance.gameObject);
        }
        SceneManager.LoadScene("PrototypeSelectScene");
    }
}
