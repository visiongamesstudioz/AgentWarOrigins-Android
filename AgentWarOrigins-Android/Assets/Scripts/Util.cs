using EndlessRunner;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public static class Util
{

    public const string VungleResumeVideoPlacementId = "RESUMEG40651";
    public const string VungleEarnDiamondsPlacementId = "EARNDIA54487";
    public const string VungleNonSkippablePlacementId = "NONSKIP64586";
    public const string VungleSkippablePlacementId = "SKIPPAB91447";
    public const string GOOGLE_APP_URL = "market://details?id=com.vgsz.android.agentwarorigins";
    public const string APPLE_APP_URL = "";
    public const string WINDOWS_APP_URL = "";

    public static bool IsSystemTimeSyncingWithNetworkServer;


    public static float Aspectratio;
    public static int ScreenWidth;
    public static int ScreenHeight;

    public static void SetResolution(int percentage)
    {
        ScreenWidth = PlayerPrefs.GetInt("ScreenWidth");
        ScreenHeight = PlayerPrefs.GetInt("ScreenHeight");
        Screen.SetResolution(ScreenWidth * percentage / 100, ScreenHeight * percentage / 100, true);
    }

    public static GameObject FindGameObjectWithTag(GameObject parent, string tag)
    {
        var trs = parent.GetComponentsInChildren<Transform>(true);
        foreach (var t in trs)
        {
            if (t.CompareTag(tag))
            {
                return t.gameObject;
            }

        }
        return null;
    }

    public static GameObject FindGameObjectWithName(GameObject parent, string name)
    {
        var trs = parent.GetComponentsInChildren<Transform>(true);
        foreach (var t in trs)
        {
            if (t.name == name)
            {
                return t.gameObject;
            }
        }
        return null;
    }

    public static T FindGameObjectWithComponent<T>(GameObject parent, string name) where T: Component 
    {
        var trs = parent.GetComponentsInChildren<T>(true);
        foreach (var t in trs)
        {
            if (t.name == name)
            {
                return t;
            }
        }
        return null;
    }

    public static GameObject FindParentWithTag(GameObject childObject, string tag)
    {
        Transform t = childObject.transform;

        while (t.parent != null)
        {
            if (t.parent.CompareTag(tag))
            {
                return t.gameObject;
            }
            t = t.parent.transform;
        }
        return null; // Could not find a parent with given tag.
    }
    public static List<Collider> GetColliderObjectsInLayer(GameObject root, int layer)
    {
        var ret = new List<Collider>();
        foreach (var t in root.GetComponentsInChildren<Collider>(true))
        {
            if (t.gameObject.layer == layer)
            {
                ret.Add(t);
            }
        }
        return ret;
    }
    public static List<GameObject> GetGameObjectsInLayer(GameObject root, int layer)

    {
        var list = new List<GameObject>();
        foreach (Transform t in root.transform.GetComponentsInChildren<Transform>(true))
        {
            if (t.gameObject.layer == layer)
            {
                list.Add(t.gameObject);
            }
        }
        return list;
    }

    public static List<T> GetComponentsInLayer<T>(GameObject root, int layer) where T:Component

    {
        var list = new List<T>();
        foreach (T t in root.transform.GetComponentsInChildren<T>(true))
        {
            if (t.gameObject.layer == layer)
            {
                list.Add(t);
            }
        }
        return list;
    }
    public static Texture2D TextureFromSprite(Sprite sprite)
    {
        if (Math.Abs(sprite.rect.width - sprite.texture.width) > 0.001f)
        {
            Texture2D newTexture = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
            Color[] newColors = sprite.texture.GetPixels((int)sprite.textureRect.x,
                                                         (int)sprite.textureRect.y,
                                                         (int)sprite.textureRect.width,
                                                         (int)sprite.textureRect.height);
            newTexture.SetPixels(newColors);
            newTexture.Apply();
            return newTexture;
        }
        else
            return sprite.texture;
    }
    public static void ResetCurrentGameStats(CurrentGameStats currentGameStats)
    {
        currentGameStats.TokensCollected = 0;
        currentGameStats.CurrentDeaths = 0;
        currentGameStats.CurrentDistanceTravelled = 0f;
        currentGameStats.CurrentEnergyAmount = 0f;
        currentGameStats.CurrentKills = 0;
        currentGameStats.CurrentMoveMultiplier = 0;
        currentGameStats.CurrentMovementSpeed = 0;
        currentGameStats.CurrentObstaclesDodged = 0;
        currentGameStats.CurrentStoreTypeClicked = 0;
        currentGameStats.CurrentXpEarned = 0;
        currentGameStats.DiamondsCollected = 0;
        currentGameStats.NoOfJumps = 0;
        currentGameStats.CurrentDronesDestroyed = 0;
        currentGameStats.CurrentEnemyVehiclesDestroyed = 0;
        currentGameStats.CurrentObstaclesDestroyed = 0;

        //set if
        PlayerData.CurrentGameStats = currentGameStats;
        PlayerData.SavePlayerData();
    }
    public static bool IsGamePlayedFirstTimeOnDevice()
    {
        if (!PlayerPrefs.HasKey("IsFirstTime"))
            PlayerPrefs.SetInt("IsFirstTime", 1);
        if (PlayerPrefs.HasKey("IsFirstTime"))
            if (PlayerPrefs.GetInt("IsFirstTime") == 1)
                return true;
        return false;
    }
    public static bool IsTutorialComplete()
    {
        if (!PlayerPrefs.HasKey("IsTutorialComplete"))
            PlayerPrefs.SetInt("IsTutorialComplete", 0);
        if (PlayerPrefs.HasKey("IsTutorialComplete"))
            if (PlayerPrefs.GetInt("IsTutorialComplete") == 1)
                return true;
        return false;
    }

    public static bool IsCutScenePlayed()
    {
        if (!PlayerPrefs.HasKey("IsCutScenePlayed"))
            PlayerPrefs.SetInt("IsCutScenePlayed", 0);
        if (PlayerPrefs.HasKey("IsCutScenePlayed"))
            if (PlayerPrefs.GetInt("IsCutScenePlayed") == 1)
                return true;
        return false;
    }

    public static void OnCutSceneComplete()
    {
        PlayerPrefs.SetInt("IsCutScenePlayed",1);
    }

    public static void ChangeCanvasRenderMode(Canvas canvas,RenderMode renderMode)
    {
        canvas.renderMode= renderMode;
     
    }
    public static void SetLayerRecursively(GameObject go, int layer)
    {
        go.layer = layer;
        foreach (Transform child in go.transform)
            SetLayerRecursively(child.gameObject, layer);
    }
 
    public static DateTime GetNetworkTime()
    {
        try
        {
            //default Windows time server
            const string ntpServer = "asia.pool.ntp.org";

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
            var networkDateTime = new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds((long)milliseconds);

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
        return (uint)(((x & 0x000000ff) << 24) +
                       ((x & 0x0000ff00) << 8) +
                       ((x & 0x00ff0000) >> 8) +
                       ((x & 0xff000000) >> 24));
    }

    public static bool IsObjectInFront(GameObject source, GameObject target)
    {
        Vector3 distanceToTarget = source.transform.InverseTransformPoint(target.transform.position);
        if (distanceToTarget.z > 0)
            return true;
        return false;
    }

    public static int FindClosestGameObjectIndexInFront(GameObject sourceGameObject, List<Collider> targetPoints)
    {
        if (targetPoints.Count == 0)
        {
            return -1;
        }
        int closest = 0;
        float lastDistance = Vector3.Distance(sourceGameObject.transform.position,
            targetPoints[0].transform.position);
        
        for (int i = 1; i < targetPoints.Count; i++)
        {
            float thisDistance = Vector3.Distance(sourceGameObject.transform.position,
           targetPoints[i].transform.position);

            if (lastDistance > thisDistance && i != closest)
            {
                if (IsObjectInFront(sourceGameObject,targetPoints[i].gameObject))
                {
                    closest = i;
                }

            }
        }
        if (closest == 0)
        {
            //check if first point is in front of player
            if(IsObjectInFront(sourceGameObject, targetPoints[closest].gameObject))
            {
                return closest;
            }
            return -1;
        }
        return closest;

    }
    public static bool HasParameter(int paramHashId, Animator animator)
    {
        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            if (param.nameHash == paramHashId)
                return true;
        }
        return false;
    }

    public static bool HasParameter(string name, Animator animator)
    {
        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            if (param.name == name)
                return true;
        }
        return false;
    }

    public static void RateGame()
    {
#if UNITY_ANDROID

        PlayerPrefs.SetInt("AlreadyRated", 1);
        Application.OpenURL(GOOGLE_APP_URL);
#endif
    }

    public static void ReviewLater()
    {
        ulong LastRateGameShowTime = (ulong)DateTime.Now.Ticks;
        PlayerPrefs.SetString("LastRateShowTime", LastRateGameShowTime.ToString());

        PlayerPrefs.Save();
    }
}
