using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SaveLoadManager : MonoBehaviour
{
    public static SaveLoadManager Instance;

    private void Awake()
    {
        //Check if Instance already exists
        if (Instance == null)
        {
            //if not, set Instance to this

            Instance = this;

            //Sets this to not be destroyed when reloading scene
            DontDestroyOnLoad(gameObject);
        }

        //If Instance already exists and it's not this:
        else if (Instance != this)
        {
            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one Instance of a SaveLoadManager.
            Destroy(gameObject);
        }
    //  File.Delete(Application.persistentDataPath + "/playerProfile.dat");
    }

    private void Start()
    {
        ////unlock default character and outfit
        //PlayerProfile player = new PlayerProfile();
        //player.UnlockedPlayerList.Add(DataManager.Instance.Players[0].PlayerID);
        //Save(player);
        ////  player.UnlockedOutfitsPerPlayer.Add(DataManager.Instance.Players[0].PlayerAvailableOutFits[0]);
    }

    public void Save(PlayerProfile playerProfile)
    {
        var bf = new BinaryFormatter();
        var file = File.Open(Application.persistentDataPath + "/playerProfile.dat", FileMode.OpenOrCreate);
        bf.Serialize(file, playerProfile);
        file.Close();
    }

    public PlayerProfile Load()
    {
        if (File.Exists(Application.persistentDataPath + "/playerProfile.dat"))
        {
            var bf = new BinaryFormatter();
            var file = File.Open(Application.persistentDataPath + "/playerProfile.dat", FileMode.Open);
            var newData = (PlayerProfile) bf.Deserialize(file);
            file.Close();
            return newData;
        }
        return new PlayerProfile();
    }
}

[Serializable]
public class PlayerProfile
{
    //tons can be added
    private int _noofCoinsAvailable;

    private int _noofDiamondsAvailable;
    private int _noofTokensAvailable;
    private int _noofEnemieskilled;
    private int _noofDeaths;
    private int _noofGamesPlayed;
    private int _nofJumps;
    private int _noofSlides;
    private List<int> _unlockedPlayerList = new List<int>();
    private Dictionary<int, List<int>> _unlockedOutfitsPerPlayer = new Dictionary<int, List<int>>();
    private Dictionary<int,Dictionary<int,List<int>>> _unlockedSkinsPerOutfit=new Dictionary<int, Dictionary<int, List<int>>>();
        
    private Dictionary<int, Dictionary<int,int>> _unlockedBlueprintsPeroutfit=new Dictionary<int, Dictionary<int,int>>();
    private Dictionary<int, Dictionary<int, int>> _noofOutfitUpgradedPerPlayer = new Dictionary<int, Dictionary<int, int>>();
    private Dictionary<int, int> _wornOutfitPerPlayer = new Dictionary<int, int>();
    private Dictionary<int,Dictionary<int,int>> _wornSkinPerOutfitPerPlayer=new Dictionary<int, Dictionary<int, int>>();
    private int _currentSelectedPlayer;
    private List<int> _unlockedWeaponsList = new List<int>();
    private Dictionary<int,int> _noOfBluePrintsUnlockedPerWeapon=new Dictionary<int, int>();
    private Dictionary<int,int> _noOfupgradesPerWeaponCompleted=new Dictionary<int, int>();
    private int _currentSelectedWeapon = 1;
    private List<int> _completedMissions = new List<int>();
    private int _scoreMultipler;
    private int _playerXp;
    private int _currentLevelXp;
    private int _currentLevel = 1;
    private int _nofOfObstaclesDodged;
    private int _noOfObstaclesDestroyed;
    private int _noOfEnemyvehiclesDestroyed;
    private int _noOfEnemyDronesDestroyed;
    private float _totalDistanceTravelled;
    private float _longestDistanceTravelledInOneRun;
    public int NoofCoinsAvailable

    {
        get { return _noofCoinsAvailable; }
        set { _noofCoinsAvailable = value; }
    }

    public int NoofDiamondsAvailable
    {
        get { return _noofDiamondsAvailable; }
        set { _noofDiamondsAvailable = value; }
    }

    public int NoofEnemieskilled
    {
        get { return _noofEnemieskilled; }
        set { _noofEnemieskilled = value; }
    }

    public int NoofDeaths
    {
        get { return _noofDeaths; }
        set { _noofDeaths = value; }
    }

    public int NoofGamesPlayed
    {
        get { return _noofGamesPlayed; }
        set { _noofGamesPlayed = value; }
    }

    public List<int> UnlockedPlayerList
    {
        get { return _unlockedPlayerList; }
        set { _unlockedPlayerList = value; }
    }


    public int CurrentSelectedPlayer
    {
        get { return _currentSelectedPlayer; }

        set { _currentSelectedPlayer = value; }
    }

    public Dictionary<int, List<int>> UnlockedOutfitsPerPlayer
    {
        get { return _unlockedOutfitsPerPlayer; }

        set { _unlockedOutfitsPerPlayer = value; }
    }

    public Dictionary<int, int> WornOutfitPerPlayer
    {
        get { return _wornOutfitPerPlayer; }

        set { _wornOutfitPerPlayer = value; }
    }

    public int CurrentSelectedWeapon
    {
        get { return _currentSelectedWeapon; }

        set { _currentSelectedWeapon = value; }
    }

    public List<int> UnlockedWeaponsList
    {
        get { return _unlockedWeaponsList; }
        set { _unlockedWeaponsList = value; }
    }

    public List<int> CompletedMissions
    {
        get { return _completedMissions; }
        set { _completedMissions = value; }
    }

    public int ScoreMultipler
    {
        get { return _scoreMultipler; }
        set { _scoreMultipler = value; }
    }

    public int PlayerXp
    {
        get { return _playerXp; }

        set { _playerXp = value; }
    }

    public int NofOfObstaclesDodged
    {
        get { return _nofOfObstaclesDodged; }

        set { _nofOfObstaclesDodged = value; }
    }

    public int CurrentLevel
    {
        get { return _currentLevel; }
        set { _currentLevel = value; }
    }

    public int CurrentLevelXp
    {
        get { return _currentLevelXp; }
        set { _currentLevelXp = value; }
    }

    public float TotalDistanceTravelled
    {
        get { return _totalDistanceTravelled; }
        set { _totalDistanceTravelled = value; }
    }

    public Dictionary<int, Dictionary<int, int>> NoofOutfitUpgradedPerPlayer
    {
        get { return _noofOutfitUpgradedPerPlayer; }
        set { _noofOutfitUpgradedPerPlayer = value; }
    }

    public Dictionary<int, Dictionary<int, int>> UnlockedBlueprintsPeroutfit
    {
        get { return _unlockedBlueprintsPeroutfit; }
        set { _unlockedBlueprintsPeroutfit = value; }
    }

    public Dictionary<int, int> NoOfBluePrintsUnlockedPerWeapon
    {
        get { return _noOfBluePrintsUnlockedPerWeapon; }
        set { _noOfBluePrintsUnlockedPerWeapon = value; }
    }

    public Dictionary<int, int> NoOfupgradesPerWeaponCompleted
    {
        get
        {
            return _noOfupgradesPerWeaponCompleted;
        }

        set
        {
            _noOfupgradesPerWeaponCompleted = value;
        }
    }

    public int NofJumps
    {
        get { return _nofJumps; }
        set { _nofJumps = value; }
    }

    public int NoofSlides
    {
        get { return _noofSlides; }
        set { _noofSlides = value; }
    }

    public Dictionary<int, Dictionary<int, List<int>>> UnlockedSkinsPerOutfit
    {
        get { return _unlockedSkinsPerOutfit; }
        set { _unlockedSkinsPerOutfit = value; }
    }

    public Dictionary<int, Dictionary<int, int>> WornSkinPerOutfitPerPlayer
    {
        get { return _wornSkinPerOutfitPerPlayer; }
        set { _wornSkinPerOutfitPerPlayer = value; }
    }

    public int NoOfObstaclesDestroyed
    {
        get
        {
            return _noOfObstaclesDestroyed;
        }

        set
        {
            _noOfObstaclesDestroyed = value;
        }
    }

    public int NoOfEnemyvehiclesDestroyed
    {
        get
        {
            return _noOfEnemyvehiclesDestroyed;
        }

        set
        {
            _noOfEnemyvehiclesDestroyed = value;
        }
    }

    public int NoOfEnemyDronesDestroyed
    {
        get
        {
            return _noOfEnemyDronesDestroyed;
        }

        set
        {
            _noOfEnemyDronesDestroyed = value;
        }
    }

    public float LongestDistanceTravelledInOneRun
    {
        get
        {
            return _longestDistanceTravelledInOneRun;
        }

        set
        {
            _longestDistanceTravelledInOneRun = value;
        }
    }

    public int NoofTokensAvailable
    {
        get { return _noofTokensAvailable; }
        set { _noofTokensAvailable = value; }
    }
}

[Serializable]
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
{
    [SerializeField] private readonly List<TKey> _keys = new List<TKey>();

    [SerializeField] private readonly List<TValue> _values = new List<TValue>();

    // save the dictionary to lists
    public void OnBeforeSerialize()
    {
        _keys.Clear();
        _values.Clear();
        foreach (var pair in this)
        {
            _keys.Add(pair.Key);
            _values.Add(pair.Value);
        }
    }

    // load dictionary from lists
    public void OnAfterDeserialize()
    {
        Clear();

        if (_keys.Count != _values.Count)
            throw new Exception(
                string.Format(
                    "there are {0} keys and {1} values after deserialization. Make sure that both key and value types are serializable."));

        for (var i = 0; i < _keys.Count; i++)
            Add(_keys[i], _values[i]);
    }
}

