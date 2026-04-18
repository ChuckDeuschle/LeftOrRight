using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Restart : MonoBehaviour
{
    public void RestartGame()
    {
        Destroy(MasterGameManager.instance.gameObject);
        SceneManager.LoadScene("PrototypeSelectScene");
    }
}
