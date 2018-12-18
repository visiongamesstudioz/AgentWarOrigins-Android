using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PriceButtons : MonoBehaviour
{
    public static PriceButtons Instance;

    void Awake()
    {
        Instance = this;
    }
    public Text PocketOfCoinsPriceText;
    public Text HandfulOfCoinsPriceText;
    public Text BagOfCoinsPricetext;
    public Text BountifulOfCoinsPriceText;
    public Text SafeOfCoinsPriceText;
    public Text LockerOfCoinsPriceText;
    public Text PocketOfDiamondsPriceText;
    public Text HandfulOfDiamondsPriceText;
    public Text BagOfDiamondsPriceText;
    public Text BountifulOfDiamondsPriceText;
    public Text SafeOfDiamondsPriceText;
    public Text LockerOfDiamondsPriceText;
    public Text StarterPackPriceText;
    public Text XpDoublerPriceText;
    public Text CharacterPackPricetext;
    public Text WeaponPackPriceText;
    public Text TransactionFailedMessage;
    public Image TransactionFailedDialog;
}
