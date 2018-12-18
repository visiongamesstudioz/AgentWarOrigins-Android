using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EndlessRunner
{

    [Serializable]
    public class Level
    {
        public int LevelNumber;
        public int XpRequiredToReachNextLevel;
        public List<Reward> RewardsList;


    }
    [Serializable]
    public class Reward
    {
        public Rewardtype RewardType;
        public int RewardAmountOrId;
        public Sprite RewardSprite;
    }
}

