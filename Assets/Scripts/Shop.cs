using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    public string PurchaseName;
    public int PurchaseNumber;
    public int MoneyToSubtract;
    public void BuyItem()
    {
        if(PlayerPrefs.GetFloat("Coins") > MoneyToSubtract || PlayerPrefs.GetFloat("Coins") == MoneyToSubtract)
        {
            PlayerPrefs.SetInt(PurchaseName, PurchaseNumber);
            PlayerPrefs.SetFloat("Coins", PlayerPrefs.GetFloat("Coins") - MoneyToSubtract);
        }
    }
}
