using System;
using System.Net;
using System.Net.Sockets;
using Assets.SimpleAndroidNotifications;
using EndlessRunner;
using UnityEngine;
using Random = System.Random;

public class ChestTimer : MonoBehaviour
{
    public static ChestTimer Instance;
    public float MsToWait;
    public const string LastChestOpenedTimeKey = "LastChestOpenedTime";
    private static ulong LastChestOpenedTime;
    private readonly float TimerToActivateChestAnimation = 5f;
    private float CurrentTimeRemainingToactivateChestAnimation;
    private bool _chestAnimationActive;

    private void Awake()
    {
        Instance = this;
        CurrentTimeRemainingToactivateChestAnimation = TimerToActivateChestAnimation;
    }

    // Use this for initialization
    private void Start() //ned to change later
    {
        if (PlayerPrefs.HasKey(LastChestOpenedTimeKey))
            LastChestOpenedTime = ulong.Parse(PlayerPrefs.GetString(LastChestOpenedTimeKey));
        if (NetworkConnection.isNetworkAvailable())
        {
            if (Util.IsSystemTimeSyncingWithNetworkServer)
            {
                //Debug.Log("current time" + GetNetworkTime().Ticks);
                if (!IsChestReady())
                {
                    UiManager.Instance.DisableChestButton();
                   // StartCoroutine(UiManager.Instance.StopChestBoxAnimation());
                }
                else
                {
                    UiManager.Instance.EnableChestButton();
                    UiManager.Instance.UpdateRemainingTimeForChest("Ready!");
                    //StartCoroutine(UiManager.Instance.PlayChestBoxAnimation());
                }
            }
            else
            {
                Debug.Log("not syncing with serber");
                UiManager.Instance.HideChestButton();
            }
        }
            
        else
            UiManager.Instance.HideChestButton();


        //if (IsSystemTimeSyncWithNetworkTime())
        //{
        //   // Debug.Log("sytem time syncing with server");
        //}
        //else
        //{
        //   // Debug.Log("sytem time not syncing with server");
        //    return;
        //}
    }

    //public void InitTimerChest()
    //{
    //    if (IsChestReady())
    //    {
    //        UiManager.Instance.EnableChestButton();
    //        UiManager.Instance.UpdateRemainingTimeForChest("Ready!"); //can change later
    //        UiManager.Instance.HideFreeText();
    //    //    StartCoroutine(UiManager.Instance.PlayChestBoxAnimation());
    //    }
    //    else
    //    {
    //   //     StartCoroutine(UiManager.Instance.StopChestBoxAnimation());
    //    }
    //}

    // Update is called once per frame
    private void Update()
    {
      //  if (!GameManager.Instance.IsGameActive())
            if (!UiManager.Instance.IsChestButtonInteractable() && Util.IsSystemTimeSyncingWithNetworkServer)
            {
                if (IsChestReady())
                {
                    Debug.Log("chest is ready");
                    UiManager.Instance.EnableChestButton();
                    UiManager.Instance.HideFreeText();
                    UiManager.Instance.UpdateRemainingTimeForChest("Ready!"); //can change later
               //     StartCoroutine(UiManager.Instance.PlayChestBoxAnimation());
                    return;
                }

                var diff = (ulong) DateTime.Now.Ticks - LastChestOpenedTime;
                var millisec = diff / TimeSpan.TicksPerMillisecond;
                var secondsLeft = (MsToWait - millisec) / 1000;

                var remainingTime = "";
                //hours
                remainingTime += (int) secondsLeft / 3600 + "h";
                //minutes
                secondsLeft -= (int) secondsLeft / 3600 * 3600;
                remainingTime += ((int) secondsLeft / 60).ToString("00") + "m";
                //seconds
                remainingTime += (secondsLeft % 60).ToString("00") + "s";
                UiManager.Instance.ShowFreetext();
                UiManager.Instance.UpdateRemainingTimeForChest(remainingTime);
            }
            else
            {
                CurrentTimeRemainingToactivateChestAnimation -= Time.deltaTime;
                if (CurrentTimeRemainingToactivateChestAnimation < 0 && !_chestAnimationActive)
                {
                    if (!IsChestReady())
                    {
                        UiManager.Instance.DisableChestButton();
                //        StartCoroutine(UiManager.Instance.StopChestBoxAnimation());
                    }
                    else
                    {
                        UiManager.Instance.EnableChestButton();
                        UiManager.Instance.HideFreeText();
                        UiManager.Instance.UpdateRemainingTimeForChest("Ready!");
             //           StartCoroutine(UiManager.Instance.PlayChestBoxAnimation());
                    }
                    _chestAnimationActive = true;
                }
            }
    }

    //public static DateTime GetNetworkTime()
    //{
    //    const string ntpServer = "pool.ntp.org";
    //    var ntpData = new byte[48];
    //    ntpData[0] = 0x1B; //LeapIndicator = 0 (no warning), VersionNum = 3 (IPv4 only), Mode = 3 (Client Mode)

    //    var addresses = Dns.GetHostEntry(ntpServer).AddressList;
    //    var ipEndPoint = new IPEndPoint(addresses[0], 123);
    //    var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

    //    socket.Connect(ipEndPoint);
    //    socket.Send(ntpData);
    //    socket.Receive(ntpData);
    //    socket.Close();

    //    ulong intPart = (ulong)ntpData[40] << 24 | (ulong)ntpData[41] << 16 | (ulong)ntpData[42] << 8 | (ulong)ntpData[43];
    //    ulong fractPart = (ulong)ntpData[44] << 24 | (ulong)ntpData[45] << 16 | (ulong)ntpData[46] << 8 | (ulong)ntpData[47];

    //    var milliseconds = (intPart * 1000) + ((fractPart * 1000) / 0x100000000L);
    //    var networkDateTime = (new DateTime(1900, 1, 1)).AddMilliseconds((long)milliseconds);

    //    return networkDateTime;
    //}
    public static DateTime GetNetworkTime()
    {
        try
        {
            //default Windows time server
            const string ntpServer = "in.pool.ntp.org";

            // NTP message size - 16 bytes of the digest (RFC 2030)
            var ntpData = new byte[48];

            //Setting the Leap Indicator, Version Number and Mode values
            ntpData[0] = 0x1B; //LI = 0 (no warning), VN = 3 (IPv4 only), Mode = 3 (Client Mode)

            var addresses = Dns.GetHostEntry(ntpServer).AddressList;

            //The UDP port number assigned to NTP is 123
            var ipEndPoint = new IPEndPoint(addresses[0], 123);
            //NTP uses UDP
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            socket.Connect(ipEndPoint);

            ////Stops code hang if NTP is blocked
            //socket.Blocking = true;
            socket.ReceiveTimeout = 3000;
            socket.Send(ntpData);
            socket.Receive(ntpData);
            socket.Close();

            //Offset to get to the "Transmit Timestamp" field (time at which the reply 
            //departed the server for the client, in 64-bit timestamp format."
            const byte serverReplyTime = 40;

            //Get the seconds part
            ulong intPart = BitConverter.ToUInt32(ntpData, serverReplyTime);

            //Get the seconds fraction
            ulong fractPart = BitConverter.ToUInt32(ntpData, serverReplyTime + 4);

            //Convert From big-endian to little-endian
            intPart = SwapEndianness(intPart);
            fractPart = SwapEndianness(fractPart);

            var milliseconds = intPart * 1000 + fractPart * 1000 / 0x100000000L;

            //**UTC** time
            var networkDateTime =
                new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds((long) milliseconds);

            return networkDateTime.ToLocalTime();
        }
        catch (SocketException e)
        {
            Console.WriteLine(e);
            return DateTime.MinValue;
        }
    }

    // stackoverflow.com/a/3294698/162671
    private static uint SwapEndianness(ulong x)
    {
        return (uint) (((x & 0x000000ff) << 24) +
                       ((x & 0x0000ff00) << 8) +
                       ((x & 0x00ff0000) >> 8) +
                       ((x & 0xff000000) >> 24));
    }

    public void TimedChestClicked()
    {
        if (IsChestReady())
        {
            //        PlayerProfile playerProfile = SaveLoadManager.Instance.Load();
            //  PlayerData.PlayerProfile
            var random = new Random();
            var randomNumber = random.Next(0,2);


            if (randomNumber == 0)
            {
                PlayerData.PlayerProfile.NoofCoinsAvailable += 115;
                UiManager.Instance.UpdateBonusReceivedText("Received " + 115 + " Coins", Rewardtype.Coins);
            }
            else if (randomNumber == 1)
            {
                PlayerData.PlayerProfile.NoofDiamondsAvailable += 15;
                UiManager.Instance.UpdateBonusReceivedText("Received " + 15 + " Diamonds", Rewardtype.Diamonds);
            }
  
            //update ui
            UiManager.Instance.UpdateUi();

            //show dialog
            UiManager.Instance.ShowBonusReceivedDialog();

            //save profile
            // SaveLoadManager.Instance.Save(playerProfile);
            PlayerData.SavePlayerData();
        }
        if (Util.IsSystemTimeSyncingWithNetworkServer)
        {
            LastChestOpenedTime = (ulong) DateTime.Now.Ticks;
            PlayerPrefs.SetString(LastChestOpenedTimeKey, LastChestOpenedTime.ToString());

            //local notification
            var notificationParams = new NotificationParams
            {
                Id = UnityEngine.Random.Range(0, int.MaxValue),
                Delay = TimeSpan.FromSeconds(MsToWait / 1000f),
                Title = Application.productName,
                Message = "Come back for bonus now!",
                Ticker = "Ticker",
                Sound = true,
                Vibrate = true,
                Light = true,
                SmallIcon = NotificationIcon.Message,
                SmallIconColor = new Color(0, 0.5f, 0),
                LargeIcon = "app_icon"
            };

            NotificationManager.SendCustom(notificationParams);


#if UNITY_IOS
//handle ios notification system
#endif
        }
        //
    }

    //private IEnumerator IsSystemTimeSyncWithNetworkTime(System.Action<bool> callback)
    //{
    //    yield return null;
    //    callback(isSystemTimeSyncingWithNetworkServer=GetNetworkTime().Hour == DateTime.Now.ToLocalTime().Hour);        
    //}

    private bool IsChestReady()
    {
        var diff = (ulong) DateTime.Now.Ticks - LastChestOpenedTime;
        var millisec = diff / TimeSpan.TicksPerMillisecond;
        var secondsLeft = (MsToWait - millisec) / 1000;
        if (secondsLeft < 0)
            return true;
        return false;
    }
}

