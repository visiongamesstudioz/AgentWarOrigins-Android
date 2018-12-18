using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Player : MonoBehaviour
{
    public int PlayerID;
    public string PlayerName;
    public string PlayerDescription;
    public int PlayerCost;
    public int LevelRequiredToUnlock;
    public AudioClip[] JumpAudioClips;
    public AudioClip[] SlideAudioClips;
    public AudioClip[] HitAudioClips;
    public AudioClip[] DieAudioClips;
    public LockType PlayerLockType;
    public List<OutfitSkins> PlayerAvailableOutFitsWithSkins;
    private PlayerStats m_PlayerStats;
    private List<Outfit> m_playerBoughtOutfits;

}
[Serializable]
public class OutfitSkins
{
    public int OutfitID;
    public List<Outfit> AvailableSkinsPerOutfit;
}

public enum LockType
{
    Coins,
    Diamonds,
    Blueprints
}

public class PlayerStats
{
    private int m_KillCount;
    private int m_DeathCount;
    private int m_NofGamesPlayed;
}