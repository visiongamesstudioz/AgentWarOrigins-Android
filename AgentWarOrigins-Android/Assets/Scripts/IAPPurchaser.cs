using System;
using System.Collections.Generic;
using EndlessRunner;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;

// Placing the Purchaser class in the CompleteProject namespace allows it to interact with ScoreManager, 
// one of the existing Survival Shooter scripts.
// Deriving the Purchaser class from IStoreListener enables it to receive messages from Unity Purchasing.
public class IAPPurchaser : MonoBehaviour, IStoreListener
{
    public int NoOfIapItems;
    private static IStoreController m_StoreController; // The Unity Purchasing system.
    private static IExtensionProvider m_StoreExtensionProvider; // The store-specific Purchasing subsystems.
    // Product identifiers for all products capable of being purchased: 
    // "convenience" general identifiers for use with Purchasing, and their store-specific identifier 
    // counterparts for use with and outside of Unity Purchasing. Define store-specific identifiers 
    // also on each platform's publisher dashboard (iTunes Connect, Google Play Developer Console, etc.)

    // General product identifiers for the consumable, non-consumable, and subscription products.
    // Use these handles in the code to reference which product to purchase. Also use these values 
    // when defining the Product Identifiers on the store. Except, for illustration purposes, the 
    // kProductIDSubscription - it has custom Apple and Google identifiers. We declare their store-
    // specific mapping to Unity Purchasing's AddProduct, below.
    public static string kPocketOfCoinsProductIDConsumable = "PocketOfCoinsConsumable";
    public static string kHandfulOfCoinsIDConsumable = "HandfulOfCoinsConsumable";
    public static string kBagOfCoinsIDConsumable = "BagOfCoinsConsumable";
    public static string kBountifulOfCoinsIDConsumable = "BountifulOfCoinsConsumable";
    public static string kSafeOfCoinsIDConsumable = "SafeOfCoinsConsumable";
    public static string kLockerOfCoinsIDConsumable = "LockerOfCoinsConsumable";
    public static string kPocketOfDiamondsProductIDConsumable = "PocketOfDiamondsConsumable";
    public static string kHandfulOfDiamondsIDConsumable = "HandfulOfDiamondsConsumable";
    public static string kBagOfDiamondsIDConsumable = "BagOfDiamondsConsumable";
    public static string kBountifulOfDiamondsIDConsumable = "BountifulOfDiamondsConsumable";
    public static string kSafeOfDiamondsIDConsumable = "SafeOfDiamondsConsumable";
    public static string kLockerOfDiamondsIDConsumable = "LockerOfDiamondsConsumable";

    //combo packs non consumable items
    public static string kStarterPackProductIDNonConsumable = "StarterPackNonConsumable";
    public static string kCoinDoublerProductIDNonConsumable = "CoinDoublerNonConsumable";
    public static string kCharacterPackProductIDNonConsumable = "CharacterPackNonConsumable";
    public static string kWeaponPackProductIDNonConsumable = "WeaponPackNonConsumable";

    // Apple App Store-specific product identifier for the subscription product.
    private static readonly string kProductNameAppleSubscription = "com.unity3d.subscription.new";

    // Google Play Store-specific product identifier subscription product.
    private static readonly string kProductNameGooglePlaySubscription = "com.unity3d.subscription.original";
    private string[] prices;

    void Awake()
    {
        prices=new string[NoOfIapItems];
    }
    private void Start()
    {
        var i = 0;
        // If we haven't set up the Unity Purchasing reference
        if (m_StoreController == null)
        {
            InitializePurchasing();
        }

        else if (m_StoreController != null)
        {
            foreach (var product in m_StoreController.products.all)
            {
                prices[i] = product.metadata.localizedPriceString;
                Debug.Log(product.metadata.localizedTitle);
                i++;
            }
            SetPrices();
        } 

    }

    public void InitializePurchasing()
    {
        // If we have already connected to Purchasing ...
        if (IsInitialized())
            return;

        // Create a builder, first passing in a suite of Unity provided stores.
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        // Add a product to sell / restore by way of its identifier, associating the general identifier
        // with its store-specific identifiers.

        builder.AddProduct(kPocketOfCoinsProductIDConsumable, ProductType.Consumable, new IDs
        {
            {
                AvailableItemsForSale.Instance.IapItems[0].IAPName,
                GooglePlay.Name
            },
            {
                AvailableItemsForSale.Instance.IapItems[0].IAPName,
                AppleAppStore.Name
            }
        });
        builder.AddProduct(kHandfulOfCoinsIDConsumable, ProductType.Consumable, new IDs
        {
            {
                AvailableItemsForSale.Instance.IapItems[1].IAPName,
                GooglePlay.Name
            },
            {
                AvailableItemsForSale.Instance.IapItems[1].IAPName,
                AppleAppStore.Name
            }
        });
        builder.AddProduct(kBagOfCoinsIDConsumable, ProductType.Consumable, new IDs
        {
            {
                AvailableItemsForSale.Instance.IapItems[2].IAPName,
                GooglePlay.Name
            },
            {
                AvailableItemsForSale.Instance.IapItems[2].IAPName,
                AppleAppStore.Name
            }
        });
        builder.AddProduct(kBountifulOfCoinsIDConsumable, ProductType.Consumable, new IDs
        {
            {
                AvailableItemsForSale.Instance.IapItems[3].IAPName,
                GooglePlay.Name
            },
            {
                AvailableItemsForSale.Instance.IapItems[3].IAPName,
                AppleAppStore.Name
            }
        });

        builder.AddProduct(kSafeOfCoinsIDConsumable, ProductType.Consumable, new IDs
        {
            {
                AvailableItemsForSale.Instance.IapItems[4].IAPName,
                GooglePlay.Name
            },
            {
                AvailableItemsForSale.Instance.IapItems[4].IAPName,
                AppleAppStore.Name
            }
        });
        builder.AddProduct(kLockerOfCoinsIDConsumable, ProductType.Consumable, new IDs
        {
            {
                AvailableItemsForSale.Instance.IapItems[5].IAPName,
                GooglePlay.Name
            },
            {
                AvailableItemsForSale.Instance.IapItems[5].IAPName,
                AppleAppStore.Name
            }
        });
        builder.AddProduct(kPocketOfDiamondsProductIDConsumable, ProductType.Consumable, new IDs
        {
            {
                AvailableItemsForSale.Instance.IapItems[6].IAPName,
                GooglePlay.Name
            },
            {
                AvailableItemsForSale.Instance.IapItems[6].IAPName,
                AppleAppStore.Name
            }
        });
        builder.AddProduct(kHandfulOfDiamondsIDConsumable, ProductType.Consumable, new IDs
        {
            {
                AvailableItemsForSale.Instance.IapItems[7].IAPName,
                GooglePlay.Name
            },
            {
                AvailableItemsForSale.Instance.IapItems[7].IAPName,
                AppleAppStore.Name
            }
        });
        builder.AddProduct(kBagOfDiamondsIDConsumable, ProductType.Consumable, new IDs
        {
            {
                AvailableItemsForSale.Instance.IapItems[8].IAPName,
                GooglePlay.Name
            },
            {
                AvailableItemsForSale.Instance.IapItems[8].IAPName,
                AppleAppStore.Name
            }
        });
        builder.AddProduct(kBountifulOfDiamondsIDConsumable, ProductType.Consumable, new IDs
        {
            {
                AvailableItemsForSale.Instance.IapItems[9].IAPName,
                GooglePlay.Name
            },
            {
                AvailableItemsForSale.Instance.IapItems[9].IAPName,
                AppleAppStore.Name
            }
        });
        builder.AddProduct(kSafeOfDiamondsIDConsumable, ProductType.Consumable, new IDs
        {
            {
                AvailableItemsForSale.Instance.IapItems[10].IAPName,
                GooglePlay.Name
            },
            {
                AvailableItemsForSale.Instance.IapItems[10].IAPName,
                AppleAppStore.Name
            }
        });

        builder.AddProduct(kLockerOfDiamondsIDConsumable, ProductType.Consumable, new IDs
        {
            {
                AvailableItemsForSale.Instance.IapItems[11].IAPName,
                GooglePlay.Name
            },
            {
                AvailableItemsForSale.Instance.IapItems[11].IAPName,
                AppleAppStore.Name
            }
        });
        builder.AddProduct(kStarterPackProductIDNonConsumable, ProductType.Consumable, new IDs
        {
            {
                AvailableItemsForSale.Instance.IapItems[12].IAPName,
                GooglePlay.Name
            },
            {
                AvailableItemsForSale.Instance.IapItems[12].IAPName,
                AppleAppStore.Name
            }
        });
        builder.AddProduct(kCoinDoublerProductIDNonConsumable, ProductType.NonConsumable, new IDs
        {
            {
                AvailableItemsForSale.Instance.IapItems[13].IAPName,
                GooglePlay.Name
            },
            {
                AvailableItemsForSale.Instance.IapItems[13].IAPName,
                AppleAppStore.Name
            }
        });
        builder.AddProduct(kCharacterPackProductIDNonConsumable, ProductType.NonConsumable, new IDs
        {
            {
                AvailableItemsForSale.Instance.IapItems[14].IAPName,
                GooglePlay.Name
            },
            {
                AvailableItemsForSale.Instance.IapItems[14].IAPName,
                AppleAppStore.Name
            }
        });
        builder.AddProduct(kWeaponPackProductIDNonConsumable, ProductType.NonConsumable, new IDs
        {
            {
                AvailableItemsForSale.Instance.IapItems[15].IAPName,
                GooglePlay.Name
            },
            {
                AvailableItemsForSale.Instance.IapItems[15].IAPName,
                AppleAppStore.Name
            }
        });

        //// Continue adding the non-consumable product.
        //builder.AddProduct(kProductIDNonConsumable, ProductType.NonConsumable);
        //// And finish adding the subscription product. Notice this uses store-specific IDs, illustrating
        //// if the Product ID was configured differently between Apple and Google stores. Also note that
        //// one uses the general kProductIDSubscription handle inside the game - the store-specific IDs 
        //// must only be referenced here. 
        //builder.AddProduct(kProductIDSubscription, ProductType.Subscription, new IDs
        //{
        //    {kProductNameAppleSubscription, AppleAppStore.Name},
        //    {kProductNameGooglePlaySubscription, GooglePlay.Name}
        //});

        // Kick off the remainder of the set-up with an asynchrounous call, passing the configuration 
        // and this class' instance. Expect a response either in OnInitialized or OnInitializeFailed.
        UnityPurchasing.Initialize(this, builder);
    }

    void SetPrices()
    {
        PriceButtons.Instance.PocketOfCoinsPriceText.text = prices[0];
        PriceButtons.Instance.HandfulOfCoinsPriceText.text = prices[1];
        PriceButtons.Instance.BagOfCoinsPricetext.text = prices[2];
        PriceButtons.Instance.BountifulOfCoinsPriceText.text = prices[3];
        PriceButtons.Instance.SafeOfCoinsPriceText.text = prices[4];
        PriceButtons.Instance.LockerOfCoinsPriceText.text = prices[5];
        PriceButtons.Instance.PocketOfDiamondsPriceText.text = prices[6];
        PriceButtons.Instance.HandfulOfDiamondsPriceText.text = prices[7];
        PriceButtons.Instance.BagOfDiamondsPriceText.text = prices[8];
        PriceButtons.Instance.BountifulOfDiamondsPriceText.text = prices[9];
        PriceButtons.Instance.SafeOfDiamondsPriceText.text = prices[10];
        PriceButtons.Instance.LockerOfDiamondsPriceText.text = prices[11];
        PriceButtons.Instance.StarterPackPriceText.text = prices[12];
        PriceButtons.Instance.XpDoublerPriceText.text = prices[13];
        PriceButtons.Instance.CharacterPackPricetext.text = prices[14];
        PriceButtons.Instance.WeaponPackPriceText.text = prices[15];
    }


    private bool IsInitialized()
    {
        // Only say we are initialized if both the Purchasing references are set.
        return m_StoreController != null && m_StoreExtensionProvider != null;
    }


    public void BuyPocketOfCoins()
    {
        // Buy the consumable product using its general identifier. Expect a response either 
        // through ProcessPurchase or OnPurchaseFailed asynchronously.
        BuyProductID(kPocketOfCoinsProductIDConsumable);
    }

    public void BuyHandfulOfCoins()
    {
        // Buy the consumable product using its general identifier. Expect a response either 
        // through ProcessPurchase or OnPurchaseFailed asynchronously.
        BuyProductID(kHandfulOfCoinsIDConsumable);
    }

    public void BuyBagOfCoins()
    {
        // Buy the consumable product using its general identifier. Expect a response either 
        // through ProcessPurchase or OnPurchaseFailed asynchronously.
        BuyProductID(kBagOfCoinsIDConsumable);
    }

    public void BuyBountifulOfCoins()
    {
        // Buy the consumable product using its general identifier. Expect a response either 
        // through ProcessPurchase or OnPurchaseFailed asynchronously.
        BuyProductID(kBountifulOfCoinsIDConsumable);
    }

    public void BuySafeOfCoins()
    {
        // Buy the consumable product using its general identifier. Expect a response either 
        // through ProcessPurchase or OnPurchaseFailed asynchronously.
        BuyProductID(kSafeOfCoinsIDConsumable);
    }

    public void BuyLockerOfCoins()
    {
        // Buy the consumable product using its general identifier. Expect a response either 
        // through ProcessPurchase or OnPurchaseFailed asynchronously.
        BuyProductID(kLockerOfCoinsIDConsumable);
    }

    public void BuyPocketOfDiamonds()
    {
        // Buy the consumable product using its general identifier. Expect a response either 
        // through ProcessPurchase or OnPurchaseFailed asynchronously.
        BuyProductID(kPocketOfDiamondsProductIDConsumable);
    }

    public void BuyHandfulOfDiamonds()
    {
        // Buy the consumable product using its general identifier. Expect a response either 
        // through ProcessPurchase or OnPurchaseFailed asynchronously.
        BuyProductID(kHandfulOfDiamondsIDConsumable);
    }

    public void BuyBagOfdiamonds()
    {
        // Buy the consumable product using its general identifier. Expect a response either 
        // through ProcessPurchase or OnPurchaseFailed asynchronously.
        BuyProductID(kBagOfDiamondsIDConsumable);
    }

    public void BuyBountifulOfDiamonds()
    {
        // Buy the consumable product using its general identifier. Expect a response either 
        // through ProcessPurchase or OnPurchaseFailed asynchronously.
        BuyProductID(kBountifulOfDiamondsIDConsumable);
    }

    public void BuySafeOfDiamonds()
    {
        // Buy the consumable product using its general identifier. Expect a response either 
        // through ProcessPurchase or OnPurchaseFailed asynchronously.
        BuyProductID(kSafeOfDiamondsIDConsumable);
    }

    public void BuyLockerOfDiamonds()
    {
        // Buy the consumable product using its general identifier. Expect a response either 
        // through ProcessPurchase or OnPurchaseFailed asynchronously.
        BuyProductID(kLockerOfDiamondsIDConsumable);
    }

    public void BuyStarterPack()
    {
        // Buy the consumable product using its general identifier. Expect a response either 
        // through ProcessPurchase or OnPurchaseFailed asynchronously.
        BuyProductID(kStarterPackProductIDNonConsumable);
    }

    public void BuyCoinsDoubler()
    {
        // Buy the consumable product using its general identifier. Expect a response either 
        // through ProcessPurchase or OnPurchaseFailed asynchronously.
        BuyProductID(kCoinDoublerProductIDNonConsumable);
    }

    public void BuyCharacterPack()
    {
        // Buy the consumable product using its general identifier. Expect a response either 
        // through ProcessPurchase or OnPurchaseFailed asynchronously.
        BuyProductID(kCharacterPackProductIDNonConsumable);
    }

    public void BuyWeaponpack()
    {
        // Buy the consumable product using its general identifier. Expect a response either 
        // through ProcessPurchase or OnPurchaseFailed asynchronously.
        BuyProductID(kWeaponPackProductIDNonConsumable);
    }

    private void BuyProductID(string productId)
    {
        // If Purchasing has been initialized ...
        if (IsInitialized())
        {
            // ... look up the Product reference with the general product identifier and the Purchasing 
            // system's products collection.
            var product = m_StoreController.products.WithID(productId);

            // If the look up found a product for this device's store and that product is ready to be sold ... 
            if (product != null && product.availableToPurchase)
            {
                Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
                // ... buy the product. Expect a response either through ProcessPurchase or OnPurchaseFailed 
                // asynchronously.
                m_StoreController.InitiatePurchase(product);
            }
            // Otherwise ...
            else
            {
                // ... report the product look-up failure situation  
                Debug.Log(
                    "BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
            }
        }
        // Otherwise ...
        else
        {
            // ... report the fact Purchasing has not succeeded initializing yet. Consider waiting longer or 
            // retrying initiailization.
            Debug.Log("BuyProductID FAIL. Not initialized.");
        }
    }


    // Restore purchases previously made by this customer. Some platforms automatically restore purchases, like Google. 
    // Apple currently requires explicit purchase restoration for IAP, conditionally displaying a password prompt.
    public void RestorePurchases()
    {
        // If Purchasing has not yet been set up ...
        if (!IsInitialized())
        {
            // ... report the situation and stop restoring. Consider either waiting longer, or retrying initialization.
            Debug.Log("RestorePurchases FAIL. Not initialized.");
            return;
        }

        // If we are running on an Apple device ... 
        if (Application.platform == RuntimePlatform.IPhonePlayer ||
            Application.platform == RuntimePlatform.OSXPlayer)
        {
            // ... begin restoring purchases
            Debug.Log("RestorePurchases started ...");

            // Fetch the Apple store-specific subsystem.
            var apple = m_StoreExtensionProvider.GetExtension<IAppleExtensions>();
            // Begin the asynchronous process of restoring purchases. Expect a confirmation response in 
            // the Action<bool> below, and ProcessPurchase if there are previously purchased products to restore.
            apple.RestoreTransactions(result =>
            {
                // The first phase of restoration. If no more responses are received on ProcessPurchase then 
                // no purchases are available to be restored.
                Debug.Log("RestorePurchases continuing: " + result +
                          ". If no further messages, no purchases available to restore.");
            });
        }
        // Otherwise ...
        else
        {
            // We are not running on an Apple device. No work is necessary to restore purchases.
            Debug.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
        }
    }


    //  
    // --- IStoreListener
    //

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        // Purchasing has succeeded initializing. Collect our Purchasing references.

        // Overall Purchasing system, configured with products for this application.
        m_StoreController = controller;
        // Store specific subsystem, for accessing device-specific store features.
        m_StoreExtensionProvider = extensions;
        int i = 0;
        foreach (var product in m_StoreController.products.all)
        {
            prices[i] = product.metadata.localizedPriceString;
            i++;
        }
        SetPrices();
    }


    public void OnInitializeFailed(InitializationFailureReason error)
    {
        // Purchasing set-up has not succeeded. Check error for reason. Consider sharing this reason with the user.
        Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);

    }


    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        //get stats of player

        // A consumable product has been purchased by this user.

        if (string.Equals(args.purchasedProduct.definition.id, kPocketOfCoinsProductIDConsumable,
            StringComparison.Ordinal))
            RewardPlayer(0);
        if (string.Equals(args.purchasedProduct.definition.id, kHandfulOfCoinsIDConsumable, StringComparison.Ordinal))
            RewardPlayer(1);
        if (string.Equals(args.purchasedProduct.definition.id, kBagOfCoinsIDConsumable, StringComparison.Ordinal))
            RewardPlayer(2);
        if (string.Equals(args.purchasedProduct.definition.id, kBountifulOfCoinsIDConsumable, StringComparison.Ordinal))
            RewardPlayer(3);
        if (string.Equals(args.purchasedProduct.definition.id, kSafeOfCoinsIDConsumable, StringComparison.Ordinal))
            RewardPlayer(4);
        if (string.Equals(args.purchasedProduct.definition.id, kLockerOfCoinsIDConsumable, StringComparison.Ordinal))
            RewardPlayer(5);
        if (string.Equals(args.purchasedProduct.definition.id, kPocketOfDiamondsProductIDConsumable,
            StringComparison.Ordinal))
            RewardPlayer(6);
        if (string.Equals(args.purchasedProduct.definition.id, kHandfulOfDiamondsIDConsumable, StringComparison.Ordinal))
            RewardPlayer(7);
        if (string.Equals(args.purchasedProduct.definition.id, kBagOfDiamondsIDConsumable, StringComparison.Ordinal))
            RewardPlayer(8);
        if (string.Equals(args.purchasedProduct.definition.id, kBountifulOfDiamondsIDConsumable,
            StringComparison.Ordinal))
            RewardPlayer(9);
        if (string.Equals(args.purchasedProduct.definition.id, kSafeOfDiamondsIDConsumable, StringComparison.Ordinal))
            RewardPlayer(10);
        if (string.Equals(args.purchasedProduct.definition.id, kLockerOfDiamondsIDConsumable, StringComparison.Ordinal))
            RewardPlayer(11);
        if (string.Equals(args.purchasedProduct.definition.id, kStarterPackProductIDNonConsumable,
            StringComparison.Ordinal))
            RewardPlayer(12);
        // Or ... a non-consumable product has been purchased by this user.

        if (string.Equals(args.purchasedProduct.definition.id, kCoinDoublerProductIDNonConsumable,
            StringComparison.Ordinal))
            RewardPlayer(13);
        if (string.Equals(args.purchasedProduct.definition.id, kCharacterPackProductIDNonConsumable,
            StringComparison.Ordinal))
        {
              RewardPlayer(14);
        }
        if (string.Equals(args.purchasedProduct.definition.id, kWeaponPackProductIDNonConsumable,
            StringComparison.Ordinal))
        {
            RewardPlayer(15);
        }
        else
        {
            Debug.Log(string.Format("ProcessPurchase: FAIL. Unrecognized product: '{0}'",
               args.purchasedProduct.definition.id));
        }
           

        // Return a flag indicating whether this product has completely been received, or if the application needs 
        // to be reminded of this purchase at next app launch. Use PurchaseProcessingResult.Pending when still 
        // saving purchased products to the cloud, and when that save is delayed. 
        return PurchaseProcessingResult.Complete;
    }


    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        // A product purchase attempt did not succeed. Check failureReason for more detail. Consider sharing 
        // this reason with the user to guide their troubleshooting actions.
        Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}",
            product.definition.storeSpecificId, failureReason));
        PriceButtons.Instance.TransactionFailedDialog.gameObject.SetActive(true);
        PriceButtons.Instance.TransactionFailedMessage.text =
            "  Transaction Failed\n\n<color=red>ERROR CODE</color> : " + failureReason;

    }

    public void CloseTransactionFailedDialog()
    {
        PriceButtons.Instance.TransactionFailedDialog.gameObject.SetActive(false);
    }
    public void RewardPlayer(int iapItemId)
    {
        var rewardTypes = AvailableItemsForSale.Instance.IapItems[iapItemId].Rewardtypes;
        var rewardAmountOrIds = AvailableItemsForSale.Instance.IapItems[iapItemId].Rewards;
        for (var i = 0; i < rewardTypes.Length; i++)
        {
            var rewardtype = rewardTypes[i];
            switch (rewardtype)
            {
                case Rewardtype.Coins:
                    PlayerData.PlayerProfile.NoofCoinsAvailable += rewardAmountOrIds[i];
                    UiManager.Instance.UpdateCoins(PlayerData.PlayerProfile.NoofCoinsAvailable);
                    break;
                case Rewardtype.Diamonds:
                    PlayerData.PlayerProfile.NoofDiamondsAvailable += rewardAmountOrIds[i];
                    UiManager.Instance.UpdateDiamonds(PlayerData.PlayerProfile.NoofDiamondsAvailable);
                    break;
                case Rewardtype.XpDoubler:
                    //double coins collected in every run

                    break;
                case Rewardtype.Xp:

                    PlayerData.PlayerProfile.CurrentLevelXp += rewardAmountOrIds[i];
                    PlayerData.PlayerProfile.PlayerXp += rewardAmountOrIds[i];
                    while (PlayerData.PlayerProfile.CurrentLevelXp >=
       DataManager.Instance.GetLevel(PlayerData.PlayerProfile.CurrentLevel).XpRequiredToReachNextLevel)
                    {
                        PlayerData.PlayerProfile.CurrentLevelXp -=
                            DataManager.Instance.GetLevel(PlayerData.PlayerProfile.CurrentLevel).XpRequiredToReachNextLevel;
                        PlayerData.PlayerProfile.CurrentLevel++;
                    }
                   
                    UiManager.Instance.UpdateUi();
                    break;
                case Rewardtype.UnlockCharacter:
                    PlayerData.PlayerProfile.UnlockedPlayerList.Add(rewardAmountOrIds[i]);
                    //show dialog for unlocked character
                    break;
                case Rewardtype.UnlockWeapon:
                    PlayerData.PlayerProfile.UnlockedWeaponsList.Add(rewardAmountOrIds[i]);
                    //show dialog for unlocked weapon
                    break;
                case Rewardtype.AllCharacters:
                    foreach (var player in DataManager.Instance.Players)
                        if (!PlayerData.PlayerProfile.UnlockedPlayerList.Contains(player.PlayerID))
                            PlayerData.PlayerProfile.UnlockedPlayerList.Add(player.PlayerID);
                    break;
                case Rewardtype.AllWeapons:
                    foreach (var weapon in DataManager.Instance.Weapons)
                        if (!PlayerData.PlayerProfile.UnlockedWeaponsList.Contains(weapon.WeaponId))
                            PlayerData.PlayerProfile.UnlockedWeaponsList.Add(weapon.WeaponId);
                    break;
            }
        }
        //save data to file
        PlayerData.SavePlayerData();
    }

    [Serializable]
    public class IAPItemForPurhase
    {
        public string IAPName;
        public Rewardtype[] Rewardtypes;
        public int[] Rewards;
    }
}