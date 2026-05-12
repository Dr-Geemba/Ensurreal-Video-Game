using System;
using UnityEditor;
using UnityEngine;

public class CurrentData : MonoBehaviour
{
    public static CurrentData Instance;
    private int m_playerHealth = 5;
    public static event Action<int> OnPlayerHealthChange;
    public static event Action<int> OnPlayerMoneyChange;
    public static event Action<int> OnPlayerManaChange;
    public int playerHealth
    {
        get { return m_playerHealth; }
        set
        {
            if (value >= 0)
            {
                m_playerHealth = value;
                OnPlayerHealthChange?.Invoke(m_playerHealth);
            }
            else
            {
                Debug.LogError("Attempted to set player health to a value below zero");
            }
        }
    }
    private int m_playerMoney = 0;
    public int playerMoney
    {
        get { return m_playerMoney; }
        set
        {
            if (value >= 0)
            {
                m_playerMoney = value;
                OnPlayerMoneyChange?.Invoke(m_playerMoney);
            }
            else
            {
                Debug.LogError("Attempted to set player money to a value below zero");
            }
        }
    }
    private int m_playerMana = 99;
    public int playerMana
    {
        get { return m_playerMana; }
        set
        {
            if (value >= 0 && value <= 99)
            {
                m_playerMana = value;
                OnPlayerManaChange?.Invoke(m_playerMana);
            }
        }
    }
    void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
