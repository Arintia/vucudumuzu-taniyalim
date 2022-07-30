using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/**
 * Oyuncunun ve yapay zekanýn skor deðerlerini tutan class.
 * */
public class ScoreManager : MonoBehaviour
{
    private int playerScore; // oyuncunun skoru
    [SerializeField] private TMP_Text playerScoreTxt; // oyuncunun oyun ekranýnda skorunun yazýldýðý UI text elemaný
    private int[] aiScores = new int[3]; // yapay zekalarýn skoru
    [SerializeField] private AIManager ai; // AIManager classýnýn bir instance'ý

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject); // scene deðiþse bile bu objenin silinmemesi ve deðerlerini korumasý gerekiyor(oyun sonu ekraný için)
    }

    void Start()
    {
        playerScore = 0; // baþlangýçta oyuncu skoru sýfýrlanýyor
        playerScoreTxt.text = playerScore.ToString(); // oyuncu skoru int deðerinden stringe çevrilerek ekrana yazdýrýlýyor
        foreach (int aiScore in aiScores)
        {
            aiScores[aiScore] = 0; // baþlangýçta yapay zeka skoru sýfýrlanýyor
        }
    }
    
    /**
     * playerScore deðiþkeninin getter ve setter metodu. Getter metodu güncel oyuncu skorunu döndürüyor.
     * Setter metodunda önce playerScore set ediliyor sonrasýnda ise skor int deðerinden stringe çevrilerek ekrana yazdýrýlýyor.
     * Eðer oyuncu skoru 0 ile 375 arasýndaysa yapay zeka %55 ihtimalle doðru cevap veriyor.
     * Eðer oyuncu skoru 375 ile 750 arasýndaysa yapay zeka %70 ihtimalle doðru cevap veriyor.
     * Eðer oyuncu skoru 750'den fazlaysa yapay zeka %90 ihtimalle doðru cevap veriyor.
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
    public int getAiScore(int index) { return aiScores[index]; } // belli bir yapay zekanýn skorunu getleme metodu
    public void setAiScore(int index, int value) { aiScores[index] = value; } // belli bir yapay zekanýn metodunu setleme metodu
    public int[] getAiScoreArr() { return aiScores; } // tüm yapay zekalarýn skorunu getleme metodu
}
