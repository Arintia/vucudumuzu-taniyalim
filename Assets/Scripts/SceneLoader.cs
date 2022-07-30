using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/**
 * Men�ler aras� ve sceneler aras� ge�i�i sa�layan class.
 * */
public class SceneLoader : MonoBehaviour
{
    /**
     * Build indexe g�re bir sonraki sahneye ge�er. E�er sonraki sahne indexi 2'den fazlaysa bunu 1'e e�itler ve sonraki sahneyi y�kler.
     * �u anki sahneler:
     * 0 - Ana Men�
     * 1 - Oyun Sahnesi
     * 2 - Oyun sonu sahnesi
     * 3 - Haz�rlayanlar
     */
    public void LoadNextScene()
    {
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextSceneIndex > 2)
        {
            nextSceneIndex = 1;
        }
        SceneManager.LoadScene(nextSceneIndex);
    }

    /**
     * Oyundan ��kar.
     * */
    public void ExitGame()
    {
        Application.Quit();
    }

    /**
     * Haz�rlayanlar sahnesini y�kler.
     */
    public void RenderCredits()
    {
        SceneManager.LoadScene(3);
    }


    /**
     * Ana men�y� y�kler.
     */
    public void RenderMainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
