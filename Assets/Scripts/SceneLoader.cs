using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/**
 * Menüler arasý ve sceneler arasý geçiþi saðlayan class.
 * */
public class SceneLoader : MonoBehaviour
{
    /**
     * Build indexe göre bir sonraki sahneye geçer. Eðer sonraki sahne indexi 2'den fazlaysa bunu 1'e eþitler ve sonraki sahneyi yükler.
     * Þu anki sahneler:
     * 0 - Ana Menü
     * 1 - Oyun Sahnesi
     * 2 - Oyun sonu sahnesi
     * 3 - Hazýrlayanlar
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
     * Oyundan çýkar.
     * */
    public void ExitGame()
    {
        Application.Quit();
    }

    /**
     * Hazýrlayanlar sahnesini yükler.
     */
    public void RenderCredits()
    {
        SceneManager.LoadScene(3);
    }


    /**
     * Ana menüyü yükler.
     */
    public void RenderMainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
