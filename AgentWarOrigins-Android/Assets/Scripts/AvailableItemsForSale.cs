using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvailableItemsForSale : MonoBehaviour
{
    public static AvailableItemsForSale Instance;
    public IAPPurchaser.IAPItemForPurhase[] IapItems;

    void Awake()
    {
        Instance = this;
    }
}
