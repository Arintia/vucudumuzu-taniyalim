using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIManager : MonoBehaviour
{
    private int guessChance = 20; // yapay zekan�n do�ru bilme �ans�
    private int[] aiGuesses = new int[3]; // yapay zekalar�n tahminleri
    [SerializeField] private ScoreManager sm; // ScoreManager class�n�n bir instance�
    private int scoreToGive; // verilecek skor
    public int GuessChance { get; set; } // yapay zekan�n do�ru bilme �ans�n�n public getter setter metodu

    /**
     *  Yapay zekan�n bir soru i�in cevap tahmini yapmas�n� sa�lar.
        @param {float} timeForQuestion - Soruyu cevaplamak i�in maksimum s�reyi ifade eder.
        @param {bool} isPlayerWrongAnswer - Oyuncunun soruyu do�ru cevaplay�p cevaplamad���n� ifade eder. E�er oyuncu soruyu yanl�� cevaplad�ysa yapay zeka %100 ihtimalle do�ru cevab� bulur.
     */
    public void AIMakeGuess(float timeForQuestion, bool isPlayerWrongAnswer = false)
    {
        int guess, timeTaken;
        for (int i = 0; i < aiGuesses.Length; i++) // t�m yapay zekalar�n tahmin etmesi i�in
        {
            guess = Random.Range(0, 100); // 0 ile 100 aras� rastgele bir say� al�n�r
            timeTaken = Random.Range(1, (int)Mathf.Floor(timeForQuestion)); // 1 ile maksimum s�re aras�nda ba�ka bir say� daha al�n�r
            if (guess <= guessChance || isPlayerWrongAnswer) // e�er oyuncu yanl�� cevap verdiyse ya da yapay zekan�n rastgele �retti�i say� bilme �ans�na e�it ya da daha d���kse
            {
                scoreToGive = 50; // 50 skor ver
                if(timeTaken >= 5) // e�er rastgele s�re 5'den b�y�kse
                {
                    scoreToGive += timeTaken; // s�re bonusu ver
                }
                sm.setAiScore(i, sm.getAiScore(i) + scoreToGive); // yapay zekan�n skorunu de�i�tir
            }
        }
    }
}
