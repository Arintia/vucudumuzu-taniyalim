using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

/**
 * Kazananý belirleyen ve UI üzerinde modifikasyonlar yapan sýnýf.
 * 
 * */
public class WinManager : MonoBehaviour
{
    /**
     * Instance variablelarý
     */
    private ScoreManager scoreManager; // ScoreManager classýný tutar
    private int playerScore; // oyuncunun mevcut skoru
    private int winnerScore; // en yüksek skor
    private int winner; // kazanan(0-oyuncu, 1-2-3 sýrasýyla her AI)
    private int[] aiScores = new int[3]; // AI skorlarýný tutar
    private TMP_Text playerScoreTxt; // oyun sonu ekranýnda kullanýcýnýn skorunu yazan text
    private TMP_Text[] aiScoresTxt = new TMP_Text[3]; // oyun sonu ekranýnda yapay zekalarýn skorunu yazan text
    private Image playerMedal; // oyuncu madalya imajý
    private Image[] aiMedals = new Image[3]; // yapay zeka madalya imajý

    // Start is called before the first frame update
    void Start()
    {
        scoreManager = gameObject.GetComponent<ScoreManager>(); // baþlangýçta scoremanager classýný assign ediyoruz
        
    }

    private void Update()
    {
        if(SceneManager.GetActiveScene().name == "EndScene")
        {
            GetGameObjects();
            GetScores();
            DisplayScores();
            DetermineWinner();
            Destroy(gameObject);
        }
    }

    /**
     * Gerekli tüm oyun objelerini alýp assign eder.
     * Bu objeler oyuncu skor texti, yapay zeka skor texti, oyuncu madalya imajý ve yapay zeka madalya imajlarýdýr.
     * */
    private void GetGameObjects()
    {
        string gameObjectName;
        playerScoreTxt = GameObject.Find("PlayerScore").GetComponent<TMP_Text>();
        playerMedal = GameObject.Find("PlayerMedal").GetComponent<Image>();
        for (int i = 1; i <= aiScores.Length; i++)
        {
            gameObjectName = "AI" + i + "Score";
            aiScoresTxt[i-1] = GameObject.Find(gameObjectName).GetComponent<TMP_Text>();
            gameObjectName = "AI" + i + "Medal";
            aiMedals[i - 1] = GameObject.Find(gameObjectName).GetComponent<Image>();
        }
    }

    /**
     * ScoreManager classýndan oyuncunun ve yapay zekalarýn skorunu çeker.
     * */
    private void GetScores()
    {
        playerScore = scoreManager.PlayerScore;
        for (int i = 0; i < aiScores.Length; i++)
        {
            aiScores[i] = scoreManager.getAiScore(i);
        }
    }

    /**
     * Gelen int skor deðerlerini stringe çevirerek UI üzerinde text olarak yazdýrýr.
     * */
    private void DisplayScores()
    {
        playerScoreTxt.text = playerScore.ToString();
        for (int i = 0; i < aiScores.Length; i++)
        {
            aiScoresTxt[i].text = aiScores[i].ToString();
        }
    }

    /**
     * Kazanan oyuncuyu belirler. Bunu yaparken önce en yüksek skoru oyuncunun skoru olarak belirler ve sonrasýnda
     * yapay zekalar arasýnda döngüye girerek yapay zekalarýn skorunu kontrol eder. Daha yüksek skorlar en yüksek skor
     * olarak ve kazanan olarak assign edilirler.
     * Kazanan indexleri:
     * 0 - Oyuncu
     * 1 - Yapay Zeka 1
     * 2 - Yapay Zeka 2
     * 3 - Yapay Zeka 3
     * Sonrasýnda kazanan oyuncunun madalya imajýný aktifleþtirir.
     * */
    private void DetermineWinner()
    {
        winnerScore = playerScore;
        winner = 0;
        for (int i = 0; i < aiScores.Length; i++)
        {
            if(aiScores[i] > winnerScore)
            {
                winnerScore = aiScores[i];
                winner = i + 1;
            }
        }
        if(winner == 0)
        {
            playerMedal.enabled = true;
        } else
        {
            aiMedals[winner - 1].enabled = true;
        }
    }


}
