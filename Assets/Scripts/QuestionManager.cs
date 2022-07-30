using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class QuestionManager : MonoBehaviour
{
    private Question data; // Soru ScriptableObjectini tutar.
    [SerializeField] private TMP_Text currentQuestionCountTxt; // Ka� soru ��z�ld���n� g�steren UI texti.
    [SerializeField] private TMP_Text questionTxt; // Soruyu g�steren UI texti.
    [SerializeField] private TMP_Text timeLeftTxt; // Kalan s�reyi g�steren UI texti.
    [SerializeField] private TMP_Text[] questionAnswers = new TMP_Text[4]; // Cevaplar� g�steren UI textleri.
    [SerializeField] private Button[] answerBtns = new Button[4]; // Cevaplar�n UI buton elemanlar�.
    [SerializeField] private Image questionImg; // Sorunun imaj�.
    [SerializeField] private int maxAskedQuestions = 2; // Oyuncuya sorulacak maksimum soru say�s�.
    [SerializeField] private SceneLoader sceneLoader; // SceneLoader class�n�n bir instance�.
    private string question; // soruyu tutan string.
    private string[] answers = new string[4]; // cevaplar� tutan string arrayi.
    private int correctAnsIndex; // do�ru cevab�n indexini tutar.
    private bool isBtnPressed; // bir butona bas�l�p bas�lmad���n� tutar
    private bool[] questionAlreadyAnswered; // her soru i�in cevaplan�p cevaplan�lmad���n� tutar, cevaplanan sorular tekrar sorulmaz
    private Question[] questionArr; // t�m sorular� bir arrayde tutar
    private int userAnsweredQuestions; // kullan�c�n�n cevaplad��� soru say�s�n� tutar
    private float originalTime; // orijinal s�reyi azalmadan tutar
    private float timeLeft; // kalan s�reyi tutar
    private bool isWaitingPhase; // sorudan sonra bekleme s�recinde olup olmad���n� tutar
    private bool isCountdownCRRunning; // countdown coroutine'inin o anda �al���p �al��mad���n� tutar
    private Coroutine countdownCR; // kalan s�re i�in coroutine
    [SerializeField] private ScoreManager sm; // scoremanager class instance
    [SerializeField] private AIManager ai; // aimanager class instance
    [SerializeField] private HealthManager hm; // healthmanager class instance
    [SerializeField] private AudioManager audioManager; // audioamnager class instance
    private int scoreToGive; // ne kadar skor verilece�ini tutar

    // Start is called before the first frame update
    void Start()
    {
        /**
         * A�a��daki de�i�ken atamalar� her bu class initialize oldu�unda de�i�kenleri s�f�rlamaya yar�yor.
         */
        isCountdownCRRunning = false; 
        isWaitingPhase = true; 
        userAnsweredQuestions = 0;
        timeLeft = 0;
        originalTime = 0;
        currentQuestionCountTxt.text = (userAnsweredQuestions+1).ToString();
        questionArr = Resources.LoadAll<Question>("Questions/"); // Resources'daki Questions klas�r�nden scriptable objectleri �ekip questionArr arrayine aktar�yor
        questionAlreadyAnswered = new bool[questionArr.Length]; // �stte yarat�lan arrayin b�y�kl��� kadar sorular�n cevaplan�p cevaplanmad���n� tutan bir array yarat�yor
        ResetAllQuestions(); // Sorular� s�f�rl�yor
        data = getNextQuestion(questionArr); // sonraki soruyu al�yor
        if(data == null) // e�er sonraki soru yoksa
        {
            Debug.LogError("Cant find question");
            return;
        }
        DisplayQuestion(data); // e�er sonraki soru varsa onu ekrana yans�t
    }

    /**
     *  Butona bas�ld��� zaman olacak aksiyonlar� tutan metot.
     *  
     */
    public void OnAnswerButtonPress(GameObject gameObject)
    {
        if (GameManager.isGamePaused) return; // e�er oyun durdurulduysa bir �ey yapma
        StopCoroutine(countdownCR); // Geriye do�ru sayan coroutine'i durdur
        isCountdownCRRunning = false; // geriye do�ru sayan coroutine'in durduruldu�unu belirt
        if (isBtnPressed) // butona zaten bas�ld�ysa bir �ey yapma
        {
            Debug.Log("Zaten bir butona basm��s�n");
            return;
        }
        audioManager.PlayPendingSound(); // bekleme sesini �al
        /**
         * Cevap butonlar�n�n tagleri "Answer0", "Answer1" vb. tutuluyor.
         * Bu y�zden taglerin son harfi her zaman indexin say�s� oluyor.
         * Bu indexin say�s�n� al�p integera �evirdi�imiz zaman correctAnsIndex'de tutulan say�y� elde ederiz.
         */
        var btnTagLength = gameObject.tag.Length; 
        var pressedAnswerBtnIndexStr = gameObject.tag[btnTagLength - 1];
        var pressedAnswerBtnIndex = Char.GetNumericValue(pressedAnswerBtnIndexStr);
        var isCorrectAnswer = (correctAnsIndex == pressedAnswerBtnIndex) ? true : false; // do�ru yan�t elde edildikten sonra ternary operator ile do�ruluk kontrol�
        gameObject.GetComponent<Image>().color = Color.yellow; // bekleme s�recinde butonu sar� yapar
        StartCoroutine(CheckCorrectAnswer(isCorrectAnswer, gameObject)); // do�ru cevap kontrol�
        isBtnPressed = true; // butona t�kland���n� belirtir
        isWaitingPhase = true; // bekleme s�recine sokar
    }

    /**
     * Cevab�n do�ru olup olmad���n� kontrol eder.
     * @param {bool} isCorrectAnswer - Cevab�n do�ru olup olmad���.
     * @param {GameObject} btnGameObject - T�klanan buton gameobject.
     */
    IEnumerator CheckCorrectAnswer(bool isCorrectAnswer, GameObject btnGameObject)
    {
        yield return new WaitForSeconds(2f); // 2 saniye bekle
        audioManager.StopSound(); // �alan sesleri durdur
        bool isWrongAnswer = false; // isWrongAnswer ad�nda bir de�i�ken tan�ml�yoruz
        if(isCorrectAnswer) // e�er cevap do�ruysa
        {
            audioManager.PlaySuccessSound(); // ba�ar� sesini �al 
            scoreToGive = 50; // 50 skor verilece�nii belirt
            if(timeLeft >= 5) // e�er kalan s�re 5 saniyeden fazlaysa
            {
                scoreToGive += (int)timeLeft; // s�re bonusu ver
            }
            sm.PlayerScore = sm.PlayerScore + scoreToGive; // PlayerScore setter� ile oyuncunun skorunu artt�r
            btnGameObject.GetComponent<Image>().color = Color.green; // butonu ye�il yap
        }
        else // e�er cevap yanl��sa
        {
            audioManager.PlayFailureSound(); // ba�ar�s�z sesini �al
            isWrongAnswer = true; // yanl�� cevap boolunu true yap
            btnGameObject.GetComponent<Image>().color = Color.red; // butonu k�rm�z� yap
            MarkCorrectAnswer(); // Do�ru cevab� i�aretle
            hm.DecreaseHealth(20); // can� 20 azalt
        }
        ai.AIMakeGuess(originalTime + 5f, isWrongAnswer); // Yapay zeka tahmin yapmas�, ilk parametre orijinal s�re ve yapay zekaya 5 saniye avans veriliyor, ikinci parametre ise oyuncunun do�ru cevap s�eip se�medi�i
        userAnsweredQuestions++; // oyuncunun toplam cevaplad��� soru say�s�n� artt�r
        StartCoroutine(WaitNextQuestion()); // Bir sonraki soruyu beklemek i�in coroutine ba�lat
    }

    /**
     * Kalan s�reyi her saniye 1 azaltmaya yarayan coroutine.
     */
    IEnumerator DecreaseCountdown()
    {
        if(isWaitingPhase) yield return null; // e�er bekleme s�recindeyse s�reyi azaltma
        isCountdownCRRunning = true; // countdown coroutine'in �al��t���n� bildir
        while (timeLeft > 0f && !isWaitingPhase) // e�er kalan zaman 0'dan fazlaysa ve bekleme s�recinde de�ilse
        {
            yield return new WaitForSeconds(1f); // 1 saniye bekle
            timeLeft--; // kalan s�reyi 1 azalt
            timeLeftTxt.text = parseTimeToString(timeLeft); // kalan s�reyi UI'da g�ster
        }
        if (timeLeft <= 0f) // kalan s�re 0'a e�it ya da daha azsa yani s�re bittiyse
        {
            isBtnPressed = true; // butona bas�lm�� say
            userAnsweredQuestions++; // oyuncunun cevaplad��� toplam soru say�s�n� artt�r
            MarkCorrectAnswer(); // do�ru cevab� i�aretle
            StartCoroutine(WaitNextQuestion()); // Sonraki soruyu bekleme coroutine'ini ba�lat
        }
        isCountdownCRRunning = false; // countdown coroutine'in �al��mad���n� bildir
    }

    /**
     * Belli bir s�re sonras�nda bir sonraki sorunun ��kmas�n� sa�lar. 
     */
    IEnumerator WaitNextQuestion()
    {
        yield return new WaitForSeconds(3f); // 3 saniye bekle
        audioManager.StopSound(); // �alan sesi durdur
        data = getNextQuestion(questionArr); // sonraki soruyu se�
        if(data == null) // e�er soru bulunamad�ysa hata ver
        {
            Debug.LogError("Cant find question");
            yield return null;
        }
        DisplayQuestion(data); // soruyu ekrana yazd�r
    }

    /**
     * Bir sonraki sorunun se�ilmesine yarar. Question class tipinde return eder.
     * @param {Question[]} questionArr - Question class tipinde bir array kabul eder. Burda scriptableobject olarak sorular tutulmal�d�r.
     */
    private Question getNextQuestion(Question[] questionArr)
    {
        if (userAnsweredQuestions == maxAskedQuestions) // e�er oyuncu maksimum sorulacak soru say�s�na ula�t�ysa
        {
            Debug.LogError("Soru limitine ula��ld�.");
            sceneLoader.LoadNextScene();
            return null;
        }
        Question nextQuestion = null; // sonraki soru
        int flag = -1; // flag ile daha �nce cevaplanmam�� bir soru aramas�
        foreach(bool questionAnswered in questionAlreadyAnswered)
        {
            if(!questionAnswered) // e�er cevaplanmayan soru bulunduyas
            {
                flag = 1; // flag� 1'e e�itle
                break; // loopdan ��k
            }
        }
        if (flag == -1) return null; // e�er flag -1 kald�ysa bir �ey yapma(alttaki kod i�in infinite loop safeguard)
        int chosenIndex = 0; // se�ilen soru indexi
        do
        {
            chosenIndex = UnityEngine.Random.Range(0, questionArr.Length); // 0'dan questionArr b�y�kl���ne kadar rastgele bir say�
        } while (questionAlreadyAnswered[chosenIndex] == true); // e�er soru zaten yan�tland�ysa loopa tekrar gir
        nextQuestion = questionArr[chosenIndex]; // sonraki soruyu chosenIndex'deki indexli questiona e�itle
        questionAlreadyAnswered[chosenIndex] = true; // sorunun zaten cevaplanmas�n� true yap
        return nextQuestion; // question� return et
    }

    /**
     * Soruyu ekrana yazd�rmaya yarar.
     */
    private void DisplayQuestion(Question data)
    {
        question = data.QuestionTxt;
        var index = 0;
        foreach (string answer in data.QuestionAnswers)
        {
            answers[index] = answer;
            index++;
        }
        correctAnsIndex = data.CorrectAnswerIndex;
        questionTxt.text = question;
        for (var i = 0; i < 4; i++)
        {
            questionAnswers[i].text = answers[i];
        }
        if(data.QuestionHasImg)
        {
            questionImg.sprite = data.QuestionImg;
            questionImg.enabled = true;
        } 
        else
        {
            questionImg.enabled = false;
        }
        isBtnPressed = false;
        ResetButtonStatus();
        CalculateTimeLeft();
        isWaitingPhase = false;
        currentQuestionCountTxt.text = (userAnsweredQuestions + 1).ToString();
        if (!isCountdownCRRunning)
        {
            countdownCR = StartCoroutine(DecreaseCountdown());
        }
    }

    /*
     * Verilecek s�reyi hesaplamaya yarar.
     * Kalan s�re sorudaki toplam kelime say�s�na ba�l�d�r. Toplam kelime say�s�n�n 0.2'ye �arp�m�na 10 ekleyerek bulunur.
     */
    private void CalculateTimeLeft()
    {
        float totalWordLen = 0f;
        totalWordLen += GetWordCount(question);
        foreach(string answer in answers)
        {
            totalWordLen += GetWordCount(answer);
        }
        timeLeft = (float)Math.Floor((totalWordLen * 0.2f) + 10f);
        originalTime = timeLeft;
        timeLeftTxt.text = parseTimeToString(timeLeft);
    }

    /*
     * S�reyi float halinden UI'da yazd�r�labilecek string haline getirir.
     */
    private string parseTimeToString(float timeLeft)
    {
        int seconds = (int)timeLeft % 60; // saniyeleri kalan s�renin 60'a b�l�m�nden kalana e�itle
        int minutes = (int)timeLeft / 60; // dakikalar� kalan s�renin 60'a b�l�m�ne e�itle
        string minuteString = (minutes < 10) ? "0" + minutes.ToString() : minutes.ToString(); // e�er 10'dan k���kse yan�na 0 ekle
        string secondString = (seconds < 10) ? "0" + seconds.ToString() : seconds.ToString(); // e�er 10'dan k���kse yan�na 0 ekle
        return minuteString + ":" + secondString;
    }

    /**
     * Toplam kelime say�s�n� bulur.
     */
    private int GetWordCount(string sentence)
    {
        int count = 0;
        for(int i = 0; i < sentence.Length; i++)
        {
            if(sentence[i] == ' ' || sentence.EndsWith(sentence[i]))
            {
                count++;
            }
        }
        return count;
    }

    /*
     * T�m butonlar�n rengini tekrar beyaz yapar.
     */

    private void ResetButtonStatus()
    {
        foreach(Button btn in answerBtns)
        {
            btn.GetComponent<Image>().color = Color.white;
        }
    }

    /*
     * T�m sorular� tekrar cevaplanmam�� i�aretler.
     */
    private void ResetAllQuestions()
    {
        for(int i = 0; i < questionAlreadyAnswered.Length; i++)
        {
            questionAlreadyAnswered[i] = false;
        }
    }
    
    /*
     * Do�ru cevab�n butonunu ye�il yapar ve do�ru cevab� g�sterir.
     */
    private void MarkCorrectAnswer()
    {
        string correctAnswerBtnTag = "Answer" + correctAnsIndex;
        GameObject correctAnswerBtn = GameObject.FindWithTag(correctAnswerBtnTag);
        correctAnswerBtn.GetComponent<Image>().color = Color.green;
    }
}

