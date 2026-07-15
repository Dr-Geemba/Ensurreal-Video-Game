using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerUi : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI manaText;
    private const int mainMenu = 0;
    public void ReturnToMenu()
    {
        SceneManager.LoadScene(mainMenu);
    }
    void Awake()
    {
        moneyText.text = "Money: " + CurrentData.Instance.playerMoney;
        healthText.text = "Health: " + CurrentData.Instance.playerHealth;
        manaText.text = "Mana: " + CurrentData.Instance.playerMana;
    }
    void OnEnable()
    {
        CurrentData.OnPlayerHealthChange += UpdateHealthCount;
        CurrentData.OnPlayerMoneyChange += UpdateMoneyCount;
        CurrentData.OnPlayerManaChange += UpdateManaCount;
    }
    void OnDisable()
    {
        CurrentData.OnPlayerHealthChange -= UpdateHealthCount;
        CurrentData.OnPlayerMoneyChange -= UpdateMoneyCount;
        CurrentData.OnPlayerManaChange -= UpdateManaCount;
    }
    void UpdateMoneyCount(int newAmount)
    {
        moneyText.text = "Money: " + newAmount;
    }
    void UpdateHealthCount(decimal newAmount)
    {
        healthText.text = "Health: " + newAmount;
    }
    void UpdateManaCount(int newAmount)
    {
        manaText.text = "Mana: " + newAmount;
    }
}
