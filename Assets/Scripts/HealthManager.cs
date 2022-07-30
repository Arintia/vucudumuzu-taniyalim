using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    [SerializeField] private int maximumHealth = 100; // Maksimum can
    [SerializeField] private int minimumHealth = 0; // Minimum can
    [SerializeField] private int currentHealth = 100; // Oyuncunun güncel caný
    [SerializeField] private HealthSystem healthSystem; // HealthSystem assetinden gelen classýn bir instance'ý
    [SerializeField] private SceneLoader sceneLoader; // SceneLoader classýnýn bir instance'ý

    private void Start()
    {
        healthSystem.SetMaxHealth((float)maximumHealth); // HealthSystem classýnadki maksimum caný kendi sistemimizin canýna eþitliyoruz
    }

    public void DecreaseHealth(int damage = 20)
    {
        if(currentHealth > minimumHealth + damage) // Eðer hasardan sonra oyuncu hala hayatta kalmaya devam edecekse
        {
            currentHealth -= damage; // candan hasarý çýkart
            healthSystem.TakeDamage((float)damage); // HealthSystem classýnda da candan hasarý çýkart
        } else // eðer ölüyorsa
        {
            sceneLoader.LoadNextScene(); // sonraki sahne oyun sonu ekraný olduðu için oyunu bitirir.
        }
        
    }
}
