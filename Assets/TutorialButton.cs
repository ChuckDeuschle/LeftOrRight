using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialButton : MonoBehaviour
{
    public void LoadTutorial()
    {
        MasterGameManager.isTutorialLaunch = true;
        SceneManager.LoadScene("GameScene");
    }
}
