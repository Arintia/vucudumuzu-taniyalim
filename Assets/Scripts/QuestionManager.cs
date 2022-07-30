using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class QuestionManager : MonoBehaviour
{
    private Question data; // Soru ScriptableObjectini tutar.
    [SerializeField] private TMP_Text currentQuestionCountTxt; // Kaç soru çözüldüðünü gösteren UI texti.
    [SerializeField] private TMP_Text questionTxt; // Soruyu gösteren UI texti.
    [SerializeField] private TMP_Text timeLeftTxt; // Kalan süreyi gösteren UI texti.
    [SerializeField] private TMP_Text[] questionAnswers = new TMP_Text[4]; // Cevaplarý gösteren UI textleri.
    [SerializeField] private Button[] answerBtns = new Button[4]; // Cevaplarýn UI buton elemanlarý.
    [SerializeField] private Image questionImg; // Sorunun imajý.
    [SerializeField] private int maxAskedQuestions = 2; // Oyuncuya sorulacak maksimum soru sayýsý.
    [SerializeField] private SceneLoader sceneLoader; // SceneLoader classýnýn bir instanceý.
    private string question; // soruyu tutan string.
    private string[] answers = new string[4]; // cevaplarý tutan string arrayi.
    private int correctAnsIndex; // doðru cevabýn indexini tutar.
    private bool isBtnPressed; // bir butona basýlýp basýlmadýðýný tutar
    private bool[] questionAlreadyAnswered; // her soru için cevaplanýp cevaplanýlmadýðýný tutar, cevaplanan sorular tekrar sorulmaz
    private Question[] questionArr; // tüm sorularý bir arrayde tutar
    private int userAnsweredQuestions; // kullanýcýnýn cevapladýðý soru sayýsýný tutar
    private float originalTime; // orijinal süreyi azalmadan tutar
    private float timeLeft; // kalan süreyi tutar
    private bool isWaitingPhase; // sorudan sonra bekleme sürecinde olup olmadýðýný tutar
    private bool isCountdownCRRunning; // countdown coroutine'inin o anda çalýþýp çalýþmadýðýný tutar
    private Coroutine countdownCR; // kalan süre için coroutine
    [SerializeField] private ScoreManager sm; // scoremanager class instance
    [SerializeField] private AIManager ai; // aimanager class instance
    [SerializeField] private HealthManager hm; // healthmanager class instance
    [SerializeField] private AudioManager audioManager; // audioamnager class instance
    private int scoreToGive; // ne kadar skor verileceðini tutar

    // Start is called before the first frame update
    void Start()
    {
        /**
         * Aþaðýdaki deðiþken atamalarý her bu class initialize olduðunda deðiþkenleri sýfýrlamaya yarýyor.
         */
        isCountdownCRRunning = false; 
        isWaitingPhase = true; 
        userAnsweredQuestions = 0;
        timeLeft = 0;
        originalTime = 0;
        currentQuestionCountTxt.text = (userAnsweredQuestions+1).ToString();
        questionArr = Resources.LoadAll<Question>("Questions/"); // Resources'daki Questions klasöründen scriptable objectleri çekip questionArr arrayine aktarýyor
        questionAlreadyAnswered = new bool[questionArr.Length]; // Üstte yaratýlan arrayin büyüklüðü kadar sorularýn cevaplanýp cevaplanmadýðýný tutan bir array yaratýyor
        ResetAllQuestions(); // Sorularý sýfýrlýyor
        data = getNextQuestion(questionArr); // sonraki soruyu alýyor
        if(data == null) // eðer sonraki soru yoksa
        {
            Debug.LogError("Cant find question");
            return;
        }
        DisplayQuestion(data); // eðer sonraki soru varsa onu ekrana yansýt
    }

    /**
     *  Butona basýldýðý zaman olacak aksiyonlarý tutan metot.
     *  
     */
    public void OnAnswerButtonPress(GameObject gameObject)
    {
        if (GameManager.isGamePaused) return; // eðer oyun durdurulduysa bir þey yapma
        StopCoroutine(countdownCR); // Geriye doðru sayan coroutine'i durdur
        isCountdownCRRunning = false; // geriye doðru sayan coroutine'in durdurulduðunu belirt
        if (isBtnPressed) // butona zaten basýldýysa bir þey yapma
        {
            Debug.Log("Zaten bir butona basmýþsýn");
            return;
        }
        audioManager.PlayPendingSound(); // bekleme sesini çal
        /**
         * Cevap butonlarýnýn tagleri "Answer0", "Answer1" vb. tutuluyor.
         * Bu yüzden taglerin son harfi her zaman indexin sayýsý oluyor.
         * Bu indexin sayýsýný alýp integera çevirdiðimiz zaman correctAnsIndex'de tutulan sayýyý elde ederiz.
         */
        var btnTagLength = gameObject.tag.Length; 
        var pressedAnswerBtnIndexStr = gameObject.tag[btnTagLength - 1];
        var pressedAnswerBtnIndex = Char.GetNumericValue(pressedAnswerBtnIndexStr);
        var isCorrectAnswer = (correctAnsIndex == pressedAnswerBtnIndex) ? true : false; // doðru yanýt elde edildikten sonra ternary operator ile doðruluk kontrolü
        gameObject.GetComponent<Image>().color = Color.yellow; // bekleme sürecinde butonu sarý yapar
        StartCoroutine(CheckCorrectAnswer(isCorrectAnswer, gameObject)); // doðru cevap kontrolü
        isBtnPressed = true; // butona týklandýðýný belirtir
        isWaitingPhase = true; // bekleme sürecine sokar
    }

    /**
     * Cevabýn doðru olup olmadýðýný kontrol eder.
     * @param {bool} isCorrectAnswer - Cevabýn doðru olup olmadýðý.
     * @param {GameObject} btnGameObject - Týklanan buton gameobject.
     */
    IEnumerator CheckCorrectAnswer(bool isCorrectAnswer, GameObject btnGameObject)
    {
        yield return new WaitForSeconds(2f); // 2 saniye bekle
        audioManager.StopSound(); // Çalan sesleri durdur
        bool isWrongAnswer = false; // isWrongAnswer adýnda bir deðiþken tanýmlýyoruz
        if(isCorrectAnswer) // eðer cevap doðruysa
        {
            audioManager.PlaySuccessSound(); // baþarý sesini çal 
            scoreToGive = 50; // 50 skor verileceðnii belirt
            if(timeLeft >= 5) // eðer kalan süre 5 saniyeden fazlaysa
            {
                scoreToGive += (int)timeLeft; // süre bonusu ver
            }
            sm.PlayerScore = sm.PlayerScore + scoreToGive; // PlayerScore setterý ile oyuncunun skorunu arttýr
            btnGameObject.GetComponent<Image>().color = Color.green; // butonu yeþil yap
        }
        else // eðer cevap yanlýþsa
        {
            audioManager.PlayFailureSound(); // baþarýsýz sesini çal
            isWrongAnswer = true; // yanlýþ cevap boolunu true yap
            btnGameObject.GetComponent<Image>().color = Color.red; // butonu kýrmýzý yap
            MarkCorrectAnswer(); // Doðru cevabý iþaretle
            hm.DecreaseHealth(20); // caný 20 azalt
        }
        ai.AIMakeGuess(originalTime + 5f, isWrongAnswer); // Yapay zeka tahmin yapmasý, ilk parametre orijinal süre ve yapay zekaya 5 saniye avans veriliyor, ikinci parametre ise oyuncunun doðru cevap sçeip seçmediði
        userAnsweredQuestions++; // oyuncunun toplam cevapladýðý soru sayýsýný arttýr
        StartCoroutine(WaitNextQuestion()); // Bir sonraki soruyu beklemek için coroutine baþlat
    }

    /**
     * Kalan süreyi her saniye 1 azaltmaya yarayan coroutine.
     */
    IEnumerator DecreaseCountdown()
    {
        if(isWaitingPhase) yield return null; // eðer bekleme sürecindeyse süreyi azaltma
        isCountdownCRRunning = true; // countdown coroutine'in çalýþtýðýný bildir
        while (timeLeft > 0f && !isWaitingPhase) // eðer kalan zaman 0'dan fazlaysa ve bekleme sürecinde deðilse
        {
            yield return new WaitForSeconds(1f); // 1 saniye bekle
            timeLeft--; // kalan süreyi 1 azalt
            timeLeftTxt.text = parseTimeToString(timeLeft); // kalan süreyi UI'da göster
        }
        if (timeLeft <= 0f) // kalan süre 0'a eþit ya da daha azsa yani süre bittiyse
        {
            isBtnPressed = true; // butona basýlmýþ say
            userAnsweredQuestions++; // oyuncunun cevapladýðý toplam soru sayýsýný arttýr
            MarkCorrectAnswer(); // doðru cevabý iþaretle
            StartCoroutine(WaitNextQuestion()); // Sonraki soruyu bekleme coroutine'ini baþlat
        }
        isCountdownCRRunning = false; // countdown coroutine'in çalýþmadýðýný bildir
    }

    /**
     * Belli bir süre sonrasýnda bir sonraki sorunun çýkmasýný saðlar. 
     */
    IEnumerator WaitNextQuestion()
    {
        yield return new WaitForSeconds(3f); // 3 saniye bekle
        audioManager.StopSound(); // Çalan sesi durdur
        data = getNextQuestion(questionArr); // sonraki soruyu seç
        if(data == null) // eðer soru bulunamadýysa hata ver
        {
            Debug.LogError("Cant find question");
            yield return null;
        }
        DisplayQuestion(data); // soruyu ekrana yazdýr
    }

    /**
     * Bir sonraki sorunun seçilmesine yarar. Question class tipinde return eder.
     * @param {Question[]} questionArr - Question class tipinde bir array kabul eder. Burda scriptableobject olarak sorular tutulmalýdýr.
     */
    private Question getNextQuestion(Question[] questionArr)
    {
        if (userAnsweredQuestions == maxAskedQuestions) // eðer oyuncu maksimum sorulacak soru sayýsýna ulaþtýysa
        {
            Debug.LogError("Soru limitine ulaþýldý.");
            sceneLoader.LoadNextScene();
            return null;
        }
        Question nextQuestion = null; // sonraki soru
        int flag = -1; // flag ile daha önce cevaplanmamýþ bir soru aramasý
        foreach(bool questionAnswered in questionAlreadyAnswered)
        {
            if(!questionAnswered) // eðer cevaplanmayan soru bulunduyas
            {
                flag = 1; // flagý 1'e eþitle
                break; // loopdan çýk
            }
        }
        if (flag == -1) return null; // eðer flag -1 kaldýysa bir þey yapma(alttaki kod için infinite loop safeguard)
        int chosenIndex = 0; // seçilen soru indexi
        do
        {
            chosenIndex = UnityEngine.Random.Range(0, questionArr.Length); // 0'dan questionArr büyüklüðüne kadar rastgele bir sayý
        } while (questionAlreadyAnswered[chosenIndex] == true); // eðer soru zaten yanýtlandýysa loopa tekrar gir
        nextQuestion = questionArr[chosenIndex]; // sonraki soruyu chosenIndex'deki indexli questiona eþitle
        questionAlreadyAnswered[chosenIndex] = true; // sorunun zaten cevaplanmasýný true yap
        return nextQuestion; // questioný return et
    }

    /**
     * Soruyu ekrana yazdýrmaya yarar.
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
     * Verilecek süreyi hesaplamaya yarar.
     * Kalan süre sorudaki toplam kelime sayýsýna baðlýdýr. Toplam kelime sayýsýnýn 0.2'ye çarpýmýna 10 ekleyerek bulunur.
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
     * Süreyi float halinden UI'da yazdýrýlabilecek string haline getirir.
     */
    private string parseTimeToString(float timeLeft)
    {
        int seconds = (int)timeLeft % 60; // saniyeleri kalan sürenin 60'a bölümünden kalana eþitle
        int minutes = (int)timeLeft / 60; // dakikalarý kalan sürenin 60'a bölümüne eþitle
        string minuteString = (minutes < 10) ? "0" + minutes.ToString() : minutes.ToString(); // eðer 10'dan küçükse yanýna 0 ekle
        string secondString = (seconds < 10) ? "0" + seconds.ToString() : seconds.ToString(); // eðer 10'dan küçükse yanýna 0 ekle
        return minuteString + ":" + secondString;
    }

    /**
     * Toplam kelime sayýsýný bulur.
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
     * Tüm butonlarýn rengini tekrar beyaz yapar.
     */

    private void ResetButtonStatus()
    {
        foreach(Button btn in answerBtns)
        {
            btn.GetComponent<Image>().color = Color.white;
        }
    }

    /*
     * Tüm sorularý tekrar cevaplanmamýþ iþaretler.
     */
    private void ResetAllQuestions()
    {
        for(int i = 0; i < questionAlreadyAnswered.Length; i++)
        {
            questionAlreadyAnswered[i] = false;
        }
    }
    
    /*
     * Doðru cevabýn butonunu yeþil yapar ve doðru cevabý gösterir.
     */
    private void MarkCorrectAnswer()
    {
        string correctAnswerBtnTag = "Answer" + correctAnsIndex;
        GameObject correctAnswerBtn = GameObject.FindWithTag(correctAnswerBtnTag);
        correctAnswerBtn.GetComponent<Image>().color = Color.green;
    }
}

