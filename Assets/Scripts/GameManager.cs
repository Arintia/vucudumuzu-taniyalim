using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    public static bool isGamePaused = false; // oyunun duraklatýlýp duraklatýlmadýðýný kontrol eder
    [SerializeField] private GameObject pauseContainer; // duraklatma menüsünün gameobjecti

    // Update is called once per frame
    void Update()
    {
        if(SceneManager.GetActiveScene().name == "GameScene") // eðer aktif sahne gamescene ise, duraklatma sadece oyun oynarken çalýþmalý
        {
            if (Input.GetKeyDown(KeyCode.Escape)) // ESC tuþuna basýldýysa
            {
                if (isGamePaused) // oyun zaten durdurulduysa
                {
                    ResumeGame(); // devam ettir
                }
                else // yoksa
                {
                    PauseGame(); // oyunu durdur
                }
            }
        }
    }

    /**
     * Oyunu devam ettirmeye yarar.
     */
    private void ResumeGame() 
    {
        isGamePaused = false; // oyun durdurulmadý 
        Time.timeScale = 1; // timeScale 0 ise tüm coroutineler durur, oyunumuz coroutineler üstünde çalýþýyor
        pauseContainer.SetActive(false); // pauseContainer objesini deaktif eder
    }

    private void PauseGame()
    {
        isGamePaused = true; // oyun durduruldu
        Time.timeScale = 0; // timeScale 0 ise tüm coroutineler durur, oyunumuz coroutineler üstünde çalýþýyor
        pauseContainer.SetActive(true); // pauseContainer objesini aktifleþtirir.
    }
}
