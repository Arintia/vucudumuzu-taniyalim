using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/**
 * Oyuncunun ve yapay zekan�n skor de�erlerini tutan class.
 * */
public class ScoreManager : MonoBehaviour
{
    private int playerScore; // oyuncunun skoru
    [SerializeField] private TMP_Text playerScoreTxt; // oyuncunun oyun ekran�nda skorunun yaz�ld��� UI text eleman�
    private int[] aiScores = new int[3]; // yapay zekalar�n skoru
    [SerializeField] private AIManager ai; // AIManager class�n�n bir instance'�

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject); // scene de�i�se bile bu objenin silinmemesi ve de�erlerini korumas� gerekiyor(oyun sonu ekran� i�in)
    }

    void Start()
    {
        playerScore = 0; // ba�lang��ta oyuncu skoru s�f�rlan�yor
        playerScoreTxt.text = playerScore.ToString(); // oyuncu skoru int de�erinden stringe �evrilerek ekrana yazd�r�l�yor
        foreach (int aiScore in aiScores)
        {
            aiScores[aiScore] = 0; // ba�lang��ta yapay zeka skoru s�f�rlan�yor
        }
    }
    
    /**
     * playerScore de�i�keninin getter ve setter metodu. Getter metodu g�ncel oyuncu skorunu d�nd�r�yor.
     * Setter metodunda �nce playerScore set ediliyor sonras�nda ise skor int de�erinden stringe �evrilerek ekrana yazd�r�l�yor.
     * E�er oyuncu skoru 0 ile 375 aras�ndaysa yapay zeka %55 ihtimalle do�ru cevap veriyor.
     * E�er oyuncu skoru 375 ile 750 aras�ndaysa yapay zeka %70 ihtimalle do�ru cevap veriyor.
     * E�er oyuncu skoru 750'den fazlaysa yapay zeka %90 ihtimalle do�ru cevap veriyor.
     * */
    public int PlayerScore { 
        get
        {
            return playerScore;
        }
        set
        {
            playerScore = value;
            playerScoreTxt.text = playerScore.ToString();
            if (playerScore >= 0 && playerScore < 375) ai.GuessChance = 55;
            else if (playerScore >= 375 && playerScore < 750) ai.GuessChance = 70;
            else ai.GuessChance = 90;
        }
    }
    public int getAiScore(int index) { return aiScores[index]; } // belli bir yapay zekan�n skorunu getleme metodu
    public void setAiScore(int index, int value) { aiScores[index] = value; } // belli bir yapay zekan�n metodunu setleme metodu
    public int[] getAiScoreArr() { return aiScores; } // t�m yapay zekalar�n skorunu getleme metodu
}
