using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

/**
 * Kazanan� belirleyen ve UI �zerinde modifikasyonlar yapan s�n�f.
 * 
 * */
public class WinManager : MonoBehaviour
{
    /**
     * Instance variablelar�
     */
    private ScoreManager scoreManager; // ScoreManager class�n� tutar
    private int playerScore; // oyuncunun mevcut skoru
    private int winnerScore; // en y�ksek skor
    private int winner; // kazanan(0-oyuncu, 1-2-3 s�ras�yla her AI)
    private int[] aiScores = new int[3]; // AI skorlar�n� tutar
    private TMP_Text playerScoreTxt; // oyun sonu ekran�nda kullan�c�n�n skorunu yazan text
    private TMP_Text[] aiScoresTxt = new TMP_Text[3]; // oyun sonu ekran�nda yapay zekalar�n skorunu yazan text
    private Image playerMedal; // oyuncu madalya imaj�
    private Image[] aiMedals = new Image[3]; // yapay zeka madalya imaj�

    // Start is called before the first frame update
    void Start()
    {
        scoreManager = gameObject.GetComponent<ScoreManager>(); // ba�lang��ta scoremanager class�n� assign ediyoruz
        
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
     * Gerekli t�m oyun objelerini al�p assign eder.
     * Bu objeler oyuncu skor texti, yapay zeka skor texti, oyuncu madalya imaj� ve yapay zeka madalya imajlar�d�r.
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
     * ScoreManager class�ndan oyuncunun ve yapay zekalar�n skorunu �eker.
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
     * Gelen int skor de�erlerini stringe �evirerek UI �zerinde text olarak yazd�r�r.
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
     * Kazanan oyuncuyu belirler. Bunu yaparken �nce en y�ksek skoru oyuncunun skoru olarak belirler ve sonras�nda
     * yapay zekalar aras�nda d�ng�ye girerek yapay zekalar�n skorunu kontrol eder. Daha y�ksek skorlar en y�ksek skor
     * olarak ve kazanan olarak assign edilirler.
     * Kazanan indexleri:
     * 0 - Oyuncu
     * 1 - Yapay Zeka 1
     * 2 - Yapay Zeka 2
     * 3 - Yapay Zeka 3
     * Sonras�nda kazanan oyuncunun madalya imaj�n� aktifle�tirir.
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
