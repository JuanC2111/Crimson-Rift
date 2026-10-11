using TMPro;
using UnityEngine;

// HUD de monedas: solo MUESTRA datos. Escucha el evento del GameManager
// y se actualiza cuando cambia, sin preguntar cada frame.
public class CoinHUD : MonoBehaviour
{
    [SerializeField] private TMP_Text label;
    [SerializeField] private int totalCoins = 10;

    private void Start()
    {
        GameManager.Instance.CoinsChanged += Refresh;
        Refresh(GameManager.Instance.Coins);
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.CoinsChanged -= Refresh;
    }

    private void Refresh(int coins) => label.text = "Monedas: " + coins + " / " + totalCoins;
}