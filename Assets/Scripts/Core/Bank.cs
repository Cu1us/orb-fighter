using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Bank : MonoBehaviour
{
    private static Bank _instance;
    public static Bank Instance
    {
        get
        {
            if (_instance) return _instance;
            else throw new NullReferenceException("A script attempted to access the static Bank instance, but it has not yet been initialized.");
        }
    }
    public static bool HasInstance { get { return _instance != null; } }


    public int Currency;
    public UnityEvent<int> OnBalanceChange;

    public static bool TryDeduct(int amount)
    {
        if (!HasInstance) return false;
        if (Instance.Currency >= amount)
        {
            Instance.Currency -= amount;
            Instance.OnBalanceChange?.Invoke(Instance.Currency);
            return true;
        }
        return false;
    }
    public static bool CanAfford(int cost)
    {
        if (!HasInstance) return false;
        return Instance.Currency >= cost;
    }
    public static int GetBalance()
    {
        return Instance.Currency;
    }
    public static void Add(int amount)
    {
        Instance.Currency += amount;
        Instance.OnBalanceChange?.Invoke(Instance.Currency);
    }
    public static void SetBalance(int newBalance)
    {
        Instance.Currency = newBalance;
        Instance.OnBalanceChange?.Invoke(Instance.Currency);
    }

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            Currency = 0;
        }
    }
    void Start()
    {
        SetBalance(GameManager.Settings.StartingCurrency);
    }
}
