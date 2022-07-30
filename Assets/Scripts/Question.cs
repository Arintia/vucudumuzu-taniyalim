using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Bu ScriptableObject ile h�zl� bir �ekilde yeni soru olu�turulabilir.
 */
[CreateAssetMenu(menuName = "Questions/Question", fileName = "New Question")]
public class Question : ScriptableObject
{
    [SerializeField] private string questionTxt; // Sorunun textini tutar
    [SerializeField] private string[] questionAnswers; // Sorunun cevaplar�n� tutar.
    [SerializeField] private bool questionHasImg; // Soruda resim olup olmad���n� tutar.
    [SerializeField] private Sprite questionImg; // Sorudaki resmi tutar.
    [SerializeField] private int correctAnswerIndex; // Do�ru cevab�n indexini tutar.

    // GET-SET METOTLARI
    public string QuestionTxt { get => questionTxt; }
    public string[] QuestionAnswers { get => questionAnswers;  }
    public bool QuestionHasImg { get => questionHasImg; }
    public Sprite QuestionImg { get => questionImg; }
    public int CorrectAnswerIndex { get => correctAnswerIndex; }
}
