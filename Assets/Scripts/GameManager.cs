using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    public static bool isGamePaused = false; // oyunun duraklat�l�p duraklat�lmad���n� kontrol eder
    [SerializeField] private GameObject pauseContainer; // duraklatma men�s�n�n gameobjecti

    // Update is called once per frame
    void Update()
    {
        if(SceneManager.GetActiveScene().name == "GameScene") // e�er aktif sahne gamescene ise, duraklatma sadece oyun oynarken �al��mal�
        {
            if (Input.GetKeyDown(KeyCode.Escape)) // ESC tu�una bas�ld�ysa
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
        isGamePaused = false; // oyun durdurulmad� 
        Time.timeScale = 1; // timeScale 0 ise t�m coroutineler durur, oyunumuz coroutineler �st�nde �al���yor
        pauseContainer.SetActive(false); // pauseContainer objesini deaktif eder
    }

    private void PauseGame()
    {
        isGamePaused = true; // oyun durduruldu
        Time.timeScale = 0; // timeScale 0 ise t�m coroutineler durur, oyunumuz coroutineler �st�nde �al���yor
        pauseContainer.SetActive(true); // pauseContainer objesini aktifle�tirir.
    }
}
