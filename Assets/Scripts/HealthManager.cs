using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    [SerializeField] private int maximumHealth = 100; // Maksimum can
    [SerializeField] private int minimumHealth = 0; // Minimum can
    [SerializeField] private int currentHealth = 100; // Oyuncunun g�ncel can�
    [SerializeField] private HealthSystem healthSystem; // HealthSystem assetinden gelen class�n bir instance'�
    [SerializeField] private SceneLoader sceneLoader; // SceneLoader class�n�n bir instance'�

    private void Start()
    {
        healthSystem.SetMaxHealth((float)maximumHealth); // HealthSystem class�nadki maksimum can� kendi sistemimizin can�na e�itliyoruz
    }

    public void DecreaseHealth(int damage = 20)
    {
        if(currentHealth > minimumHealth + damage) // E�er hasardan sonra oyuncu hala hayatta kalmaya devam edecekse
        {
            currentHealth -= damage; // candan hasar� ��kart
            healthSystem.TakeDamage((float)damage); // HealthSystem class�nda da candan hasar� ��kart
        } else // e�er �l�yorsa
        {
            sceneLoader.LoadNextScene(); // sonraki sahne oyun sonu ekran� oldu�u i�in oyunu bitirir.
        }
        
    }
}
