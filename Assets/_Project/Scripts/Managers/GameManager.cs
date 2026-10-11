using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int Coins { get; private set; }
    public bool IsLevelComplete { get; private set; }

    public event Action<int> CoinsChanged;
    public event Action LevelCompleted;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    public void AddCoin(int amount = 1)
    {
        Coins += amount;
        CoinsChanged?.Invoke(Coins);
    }

    public void CompleteLevel()
    {
        if (IsLevelComplete) return; // Evita completar el nivel dos veces
        IsLevelComplete = true;
        LevelCompleted?.Invoke();
    }
}