using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioClip successSound; // oyuncu doðru seçeneði seçtiðinde çalacak ses
    [SerializeField] private AudioClip failureSound; // oyuncu yanlýþ seçeneði seçtiðinde çalacak ses
    [SerializeField] private AudioClip pendingSound; // oyuncu herhangi bir seçeneðe bastýðýnda çalacak ses
    private AudioSource audioSource; // sesin çalacaðý kaynak

    private void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>(); // baþlangýçta componenti assign ediyoruz
    }

    /**
     * Baþarý sesini bir kere çalar.
     */
    public void PlaySuccessSound()
    {
        audioSource.PlayOneShot(successSound); 
    }

    /**
     * Baþarýsýzlýk sesini bir kere çalar.
     */
    public void PlayFailureSound()
    {
        audioSource.PlayOneShot(failureSound);
    }

    /*
     * Bekleme sesini bir kere çalar. 
     */
    public void PlayPendingSound()
    {
        audioSource.PlayOneShot(pendingSound);
    }

    /**
     * Kaynaktan çalan tüm sesleri durdurur.
     */
    public void StopSound()
    {
        audioSource.Stop();
    }
}
