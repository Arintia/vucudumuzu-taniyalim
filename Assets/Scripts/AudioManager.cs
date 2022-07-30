using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioClip successSound; // oyuncu do�ru se�ene�i se�ti�inde �alacak ses
    [SerializeField] private AudioClip failureSound; // oyuncu yanl�� se�ene�i se�ti�inde �alacak ses
    [SerializeField] private AudioClip pendingSound; // oyuncu herhangi bir se�ene�e bast���nda �alacak ses
    private AudioSource audioSource; // sesin �alaca�� kaynak

    private void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>(); // ba�lang��ta componenti assign ediyoruz
    }

    /**
     * Ba�ar� sesini bir kere �alar.
     */
    public void PlaySuccessSound()
    {
        audioSource.PlayOneShot(successSound); 
    }

    /**
     * Ba�ar�s�zl�k sesini bir kere �alar.
     */
    public void PlayFailureSound()
    {
        audioSource.PlayOneShot(failureSound);
    }

    /*
     * Bekleme sesini bir kere �alar. 
     */
    public void PlayPendingSound()
    {
        audioSource.PlayOneShot(pendingSound);
    }

    /**
     * Kaynaktan �alan t�m sesleri durdurur.
     */
    public void StopSound()
    {
        audioSource.Stop();
    }
}
