namespace VersusPlus;
#pragma warning disable
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using BloomEngine.Config;
using BloomEngine.Config.Inputs;
using BloomEngine.Helpers;
using BloomEngine.ModMenu;

using HarmonyLib;

using Il2CppGoogle.Protobuf.WellKnownTypes;

using Il2CppInterop.Runtime.Injection;

using Il2CppMono.Security.Authenticode;

using Il2CppReloaded;
using Il2CppReloaded.Data;
using Il2CppReloaded.DataModels;
using Il2CppReloaded.Gameplay;
using Il2CppReloaded.Services;
using Il2CppReloaded.TreeStateActivities;

using Il2CppSentry;
using Il2CppSentry.Protocol;

using Il2CppSource.Controllers;
using Il2CppSource.DataModels;

using Il2CppSystem;
using Il2CppSystem.Numerics;
using Il2CppSystem.Reflection;

using Il2CppTekly.Localizations;

using Il2CppTMPro;

using MelonLoader;

using ReplantAPI;
using ReplantAPI.Managers;

using UnityEngine;

public class Events : ReplantAPI
{
    static int random1, random2, random3;
    static int random4;
    static SeedType theSeedType;
    static ZombieType theZombieType;
    static int pixelRow;
    static int pixelRow2;
    public static System.Collections.IEnumerator Event1()
    {
        // Wait exactly 1.5 seconds
        yield return new WaitForSecondsRealtime(0f);
        AdviceManager.ShowAdvice("AMBUSH!", MessageStyle.HugeWave, AdviceType.None);
        AdviceManager.SetDuration(500);
        var activity = UnityEngine.Object.FindObjectOfType<GameplayActivity>();
        if (activity != null)
        {
            activity.m_audioService.PlaySample(Constants.Sound.SOUND_HUGE_WAVE);

        }

        random1 = UnityEngine.Random.Range(0, 6);
        random2 = UnityEngine.Random.Range(0, 5);
        theSeedType = (SeedType)UnityEngine.Random.Range(0, 48);

        // Exclude aquatic plants and grave buster
        while (theSeedType == SeedType.Gravebuster ||
       theSeedType == SeedType.Lilypad ||
       theSeedType == SeedType.Tanglekelp ||
       theSeedType == SeedType.Seashroom )
        {
            theSeedType = (SeedType)UnityEngine.Random.Range(0, 48);
        }
        while (GridItemManager.GetAt(GridItemType.Crater, random1, random2) != null){
            random1 = UnityEngine.Random.Range(0, 6);
            random2 = UnityEngine.Random.Range(0, 5);
           
        }
        PlantManager.Spawn(random1, random2, theSeedType);


        random3 = UnityEngine.Random.Range(0, 5);
        theZombieType = (ZombieType)UnityEngine.Random.Range(0, 37);
        while (theZombieType == ZombieType.Snorkel ||
       theZombieType == ZombieType.DolphinRider ||
       theZombieType == ZombieType.ZombieCachedPolevaulterWithPole ||
       theZombieType == ZombieType.Zombatar ||
       theZombieType == ZombieType.Target ||
       theZombieType == ZombieType.NumZombieTypes ||
       theZombieType == ZombieType.NumCachedZombieTypes ||
       theZombieType == ZombieType.Invalid)
        {
            theZombieType = (ZombieType)UnityEngine.Random.Range(0, 37);
        }

        // Bungee
        if ((int)theZombieType == 20)
        {
            random1 = UnityEngine.Random.Range(0, 6);
            random2 = UnityEngine.Random.Range(0, 5);
            while (GridItemManager.GetAt(GridItemType.Crater, random1, random2) != null)
            {
                random1 = UnityEngine.Random.Range(0, 6);
                random2 = UnityEngine.Random.Range(0, 5);

            }
            ZombieManager.SpawnAtCell(theZombieType, random1, random2);
        }
        else if (theZombieType == ZombieType.Boss)
        {
            ZombieManager.SpawnInRow(ZombieType.Boss, 2);
            if (activity != null)
            {
                activity.m_audioService.PlayMusic(MusicTune.FinalBossBrainiacManiac,0, 0);
                activity.m_audioService.MakeSureMusicIsPlaying(MusicTune.FinalBossBrainiacManiac);

            }
        }
        else
        {
            ZombieManager.SpawnInRow(theZombieType, random3);
        }


    }



    public static System.Collections.IEnumerator Event2()
    {
        yield return new WaitForSecondsRealtime(0f);
        AdviceManager.ShowAdvice("FLOURISHING ECONOMY!", MessageStyle.HugeWave, AdviceType.None);
        AdviceManager.SetDuration(500);
        var activity = UnityEngine.Object.FindObjectOfType<GameplayActivity>();
        if (activity != null)
        {
            activity.m_audioService.PlaySample(Constants.Sound.SOUND_PRIZE);

        }
        BoardManager.AddSun(750, 0);
        BoardManager.AddSun(750, 1);
    }




    public static System.Collections.IEnumerator Event3()
    {
        yield return new WaitForSecondsRealtime(0f);
        AdviceManager.ShowAdvice("CRASHING ECONOMY!", MessageStyle.HugeWave, AdviceType.None);
        AdviceManager.SetDuration(500);
        var activity = UnityEngine.Object.FindObjectOfType<GameplayActivity>();
        if (activity != null)
        {
            activity.m_audioService.PlaySample(Constants.Sound.SOUND_CRAZYDAVESCREAM);

        }
        BoardManager.AddSun(-250, 0);
        BoardManager.AddSun(-250, 1);
    }


    // Shockwave
    public static System.Collections.IEnumerator Event4()
    {

        yield return new WaitForSecondsRealtime(0f);
        int temp = UnityEngine.Random.Range(0, 5);
        if (temp == 0) pixelRow = 155;
        else if (temp == 1) pixelRow = 255;
        else if (temp == 2) pixelRow = 355;
        else if (temp == 3) pixelRow = 455;
        else if (temp == 4) pixelRow = 555;

        AdviceManager.ShowAdvice("SHOCKWAVE!", MessageStyle.HugeWave, AdviceType.None);
        AdviceManager.SetDuration(300);
        var activity = UnityEngine.Object.FindObjectOfType<GameplayActivity>();
        if (activity != null)
        {
            activity.m_audioService.PlaySample(Constants.Sound.SOUND_EXPLOSION);

        }
        // Destroy a plant
        // todo: check stacked plants (flower pot, pumpkin)
        for (int i = 510; i >= 0; i -= 50){
            EffectsManager.Spawn(i, pixelRow, ParticleEffect.Pow); //pow
            yield return new WaitForSecondsRealtime(0.2f);

            int gridX = BoardManager.PixelToGridX(i, pixelRow);
            int gridY = BoardManager.PixelToGridY(i, pixelRow);

            Plant getPlant = PlantManager.GetTopAt(gridX, gridY);
            if (getPlant != null){
                EffectsManager.Spawn(i, pixelRow - 25, ParticleEffect.PotatoMine);
                if (activity != null)
                {
                    activity.m_audioService.PlaySample(Constants.Sound.SOUND_POTATO_MINE);
                }
                PlantManager.Kill(getPlant);
                //MelonLogger.Msg("kill plant");
                break;
            }
        }
    }
    public static System.Collections.IEnumerator Event42()
    {

        yield return new WaitForSecondsRealtime(0f);
        //random4 = UnityEngine.Random.Range(0, 5);
        var activity = UnityEngine.Object.FindObjectOfType<GameplayActivity>();
        int temp = UnityEngine.Random.Range(0, 5);

        if (temp == 0) pixelRow2 = 155;
        else if (temp == 1) pixelRow2 = 255;
        else if (temp == 2) pixelRow2 = 355;
        else if (temp == 3) pixelRow2 = 455;
        else if (temp == 4) pixelRow2 = 555;
        var board = activity?.m_board;

        bool checkBreak = false;


        // Destroy a gravestone
        for (int i = 510; i <= 1010; i += 50)
        {
            EffectsManager.Spawn(i, pixelRow2, ParticleEffect.Pow); //pow
            yield return new WaitForSecondsRealtime(0.2f);
            int gridX = BoardManager.PixelToGridX(i, pixelRow2);
            int gridY = BoardManager.PixelToGridY(i, pixelRow2);
            foreach (var theZombie in board.GetZombies())
            {
                //if (!(theZombie.mZombieType == ZombieType.Gravestone)) continue;
                if (theZombie.mDead || theZombie == null) continue;
                if (!(theZombie.mZombieType == ZombieType.Gravestone)) continue;
                int zombieCol = BoardManager.PixelToGridX((int)theZombie.mPosX, (int)theZombie.mPosY);
                if (theZombie.mRow == temp && zombieCol == gridX)
                {
                    if (gridX == 5) EffectsManager.Spawn(i+50, pixelRow2 - 25, ParticleEffect.PotatoMine);
                    else EffectsManager.Spawn(i + 100, pixelRow2 - 25, ParticleEffect.PotatoMine);



                    if (activity != null)
                    {
                        activity.m_audioService.PlaySample(Constants.Sound.SOUND_POTATO_MINE);
                    }
                    ZombieManager.KillNoLoot(theZombie);
                    checkBreak = true;
                    break;
                }
                if (checkBreak) break;
            }
            if (checkBreak) break;

        }
    }




}
