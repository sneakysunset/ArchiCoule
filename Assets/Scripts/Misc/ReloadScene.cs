using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReloadScene : MonoBehaviour
{
    public void SceneReloader()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void TestScene()
    {
        SceneManager.LoadScene("SceneTest");
    }

    public void PlayGround()
    {
        SceneManager.LoadScene("PlayGround");
    }
}
