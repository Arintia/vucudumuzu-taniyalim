using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIManager : MonoBehaviour
{
    private int guessChance = 20; // yapay zekanýn doðru bilme þansý
    private int[] aiGuesses = new int[3]; // yapay zekalarýn tahminleri
    [SerializeField] private ScoreManager sm; // ScoreManager classýnýn bir instanceý
    private int scoreToGive; // verilecek skor
    public int GuessChance { get; set; } // yapay zekanýn doðru bilme þansýnýn public getter setter metodu

    /**
     *  Yapay zekanýn bir soru için cevap tahmini yapmasýný saðlar.
        @param {float} timeForQuestion - Soruyu cevaplamak için maksimum süreyi ifade eder.
        @param {bool} isPlayerWrongAnswer - Oyuncunun soruyu doðru cevaplayýp cevaplamadýðýný ifade eder. Eðer oyuncu soruyu yanlýþ cevapladýysa yapay zeka %100 ihtimalle doðru cevabý bulur.
     */
    public void AIMakeGuess(float timeForQuestion, bool isPlayerWrongAnswer = false)
    {
        int guess, timeTaken;
        for (int i = 0; i < aiGuesses.Length; i++) // tüm yapay zekalarýn tahmin etmesi için
        {
            guess = Random.Range(0, 100); // 0 ile 100 arasý rastgele bir sayý alýnýr
            timeTaken = Random.Range(1, (int)Mathf.Floor(timeForQuestion)); // 1 ile maksimum süre arasýnda baþka bir sayý daha alýnýr
            if (guess <= guessChance || isPlayerWrongAnswer) // eðer oyuncu yanlýþ cevap verdiyse ya da yapay zekanýn rastgele ürettiði sayý bilme þansýna eþit ya da daha düþükse
            {
                scoreToGive = 50; // 50 skor ver
                if(timeTaken >= 5) // eðer rastgele süre 5'den büyükse
                {
                    scoreToGive += timeTaken; // süre bonusu ver
                }
                sm.setAiScore(i, sm.getAiScore(i) + scoreToGive); // yapay zekanýn skorunu deðiþtir
            }
        }
    }
}
