using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Bu ScriptableObject ile hýzlý bir þekilde yeni soru oluþturulabilir.
 */
[CreateAssetMenu(menuName = "Questions/Question", fileName = "New Question")]
public class Question : ScriptableObject
{
    [SerializeField] private string questionTxt; // Sorunun textini tutar
    [SerializeField] private string[] questionAnswers; // Sorunun cevaplarýný tutar.
    [SerializeField] private bool questionHasImg; // Soruda resim olup olmadýðýný tutar.
    [SerializeField] private Sprite questionImg; // Sorudaki resmi tutar.
    [SerializeField] private int correctAnswerIndex; // Doðru cevabýn indexini tutar.

    // GET-SET METOTLARI
    public string QuestionTxt { get => questionTxt; }
    public string[] QuestionAnswers { get => questionAnswers;  }
    public bool QuestionHasImg { get => questionHasImg; }
    public Sprite QuestionImg { get => questionImg; }
    public int CorrectAnswerIndex { get => correctAnswerIndex; }
}
