namespace VersusPlus.Gameplay;
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
using UnityEngine.Bindings;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

using VersusPlus.Patches;
using static VersusPlus.Patches.Patch;
using static VersusPlus.Patches.Patch2;
using static VersusPlus.Patches.Patch3;
using static VersusPlus.Patches.Patch4;

using VersusPlus.Core;
using static VersusPlus.Core.Core;

using VersusPlus.UI;


public class Events : ReplantAPI
{
    static int random1, random2, random3;
    static int random4;
    static SeedType theSeedType;
    static ZombieType theZombieType;
    static int pixelRow;
    static int pixelRow2;

    static int temp;

    public static System.Collections.IEnumerator Event1()
    {
        yield return new WaitForSecondsRealtime(0f);
        AdviceManager.ShowAdvice("AMBUSH!", MessageStyle.HugeWave, AdviceType.None);
        AdviceManager.SetDuration(500);
        var activity = UnityEngine.Object.FindObjectOfType<GameplayActivity>();
        if (activity != null)
        {
            activity.m_audioService.PlaySample(Constants.Sound.SOUND_HUGE_WAVE);

        }

        temp = UnityEngine.Random.Range(1, 4);

        for (int i = 0; i < temp; i++){
            random1 = UnityEngine.Random.Range(0, 6);
            random2 = UnityEngine.Random.Range(0, 5);
            theSeedType = (SeedType)UnityEngine.Random.Range(0, 48);

            // Exclude aquatic plants and grave buster
            while (theSeedType == SeedType.Gravebuster ||
           theSeedType == SeedType.Lilypad ||
           theSeedType == SeedType.Tanglekelp ||
           theSeedType == SeedType.Seashroom)
            {
                theSeedType = (SeedType)UnityEngine.Random.Range(0, 48);
            }
            while (GridItemManager.GetAt(GridItemType.Crater, random1, random2) != null)
            {
                random1 = UnityEngine.Random.Range(0, 6);
                random2 = UnityEngine.Random.Range(0, 5);
            }
            PlantManager.Spawn(random1, random2, theSeedType);

            // spawns coffee bean along with nocturnal plants
            if (theSeedType == SeedType.Puffshroom ||
            theSeedType == SeedType.Sunshroom ||
            theSeedType == SeedType.Fumeshroom ||
            theSeedType == SeedType.Hypnoshroom ||
            theSeedType == SeedType.Scaredyshroom ||
            theSeedType == SeedType.Iceshroom ||
            theSeedType == SeedType.Doomshroom ||
            theSeedType == SeedType.Magnetshroom ||
            theSeedType == SeedType.Gloomshroom)
            {
                PlantManager.Spawn(random1, random2, SeedType.InstantCoffee);
            }




            // Reselecting zombies if invalid
            random3 = UnityEngine.Random.Range(0, 5);
            theZombieType = (ZombieType)UnityEngine.Random.Range(0, 37);
            while (theZombieType == ZombieType.Snorkel ||
                   theZombieType == ZombieType.DolphinRider ||
                   theZombieType == ZombieType.ZombieCachedPolevaulterWithPole ||
                   theZombieType == ZombieType.Zombatar ||
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
            // Zomboss
            else if (theZombieType == ZombieType.Boss)
            {
                if (Timer.elapsedTime < 300f)
                {
                    // Re-roll to pick any regular zombie except the Boss
                    ZombieType fallbackType;
                    do
                    {
                        fallbackType = (ZombieType)UnityEngine.Random.Range(0, 37);
                    }
                    while (fallbackType == ZombieType.Boss || theZombieType == ZombieType.Snorkel ||
                            theZombieType == ZombieType.DolphinRider ||
                            theZombieType == ZombieType.ZombieCachedPolevaulterWithPole ||
                            theZombieType == ZombieType.Zombatar ||
                            theZombieType == ZombieType.NumZombieTypes ||
                            theZombieType == ZombieType.NumCachedZombieTypes ||
                            theZombieType == ZombieType.Invalid);

                    ZombieManager.SpawnInRow(fallbackType, random3);
                }
                else {
                    ZombieManager.SpawnInRow(ZombieType.Boss, 2);
                    if (activity != null)
                    {
                        // Switch soundtrack to Brainiac Maniac
                        // Activate key
                        activity.m_audioService.PlayMusic(MusicTune.FinalBossBrainiacManiac, 0, 0);
                        CustomMusicPatch.TargetTune = MusicTune.FinalBossBrainiacManiac;
                        Core.key = true;
                    }
                }
                
            }
            // Giga gargantuar
            else if (theZombieType == ZombieType.RedeyeGargantuar)
            {
                if (Timer.elapsedTime < 240f)
                {
                    // Re-roll to pick any regular zombie except the Giga gargantuar
                    ZombieType fallbackType;
                    do
                    {
                        fallbackType = (ZombieType)UnityEngine.Random.Range(0, 37);
                    }
                    while (fallbackType == ZombieType.RedeyeGargantuar || theZombieType == ZombieType.Snorkel ||
                            theZombieType == ZombieType.DolphinRider ||
                            theZombieType == ZombieType.ZombieCachedPolevaulterWithPole ||
                            theZombieType == ZombieType.Zombatar ||
                            theZombieType == ZombieType.NumZombieTypes ||
                            theZombieType == ZombieType.NumCachedZombieTypes ||
                            theZombieType == ZombieType.Invalid);

                    ZombieManager.SpawnInRow(fallbackType, random3);
                }
                else ZombieManager.SpawnInRow(ZombieType.RedeyeGargantuar, random3);

            // Target
            } else if (theZombieType == ZombieType.Target){
                if (BoardManager.Board.mApp.VersusMode.ZombieLife == 3)
                {
                    // Re-roll to pick any regular zombie except the Target
                    ZombieType fallbackType;
                    do
                    {
                        fallbackType = (ZombieType)UnityEngine.Random.Range(0, 37);
                    }
                    while (fallbackType == ZombieType.Target || theZombieType == ZombieType.Snorkel ||
                                            theZombieType == ZombieType.DolphinRider ||
                                            theZombieType == ZombieType.ZombieCachedPolevaulterWithPole ||
                                            theZombieType == ZombieType.Zombatar ||
                                            theZombieType == ZombieType.NumZombieTypes ||
                                            theZombieType == ZombieType.NumCachedZombieTypes ||
                                            theZombieType == ZombieType.Invalid);

                    ZombieManager.SpawnInRow(fallbackType, random3);
                } else {
                    ZombieManager.SpawnAtCell(ZombieType.Target, 8, random3);
                    BoardManager.Board.mApp.VersusMode.ZombieLife++;
                    if (activity != null)
                    {
                        activity.m_audioService.PlaySample(Constants.Sound.SOUND_GROAN);

                    }
                }

                
            }
            else
            {
                ZombieManager.SpawnInRow(theZombieType, random3);
            }
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
        BoardManager.AddSun(250, 0);
        BoardManager.AddSun(250, 1);
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
        int Sun1 = BoardManager.Board.mSunMoney[0];
        int Sun2 = BoardManager.Board.mSunMoney[1];
        int moneytake = 150;
        int moneytake2 = 150;

        if (Sun1 < 150) moneytake = Sun1;
        if (Sun2 < 150) moneytake2 = Sun2;

        BoardManager.AddSun(-moneytake, 0);
        BoardManager.AddSun(-moneytake2, 1);
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

    public static System.Collections.IEnumerator Event5()
    {
        yield return new WaitForSecondsRealtime(0f);
        AdviceManager.ShowAdvice("THUNDERSTORM!", MessageStyle.HugeWave, AdviceType.None);
        AdviceManager.SetDuration(500);
        var activity = UnityEngine.Object.FindObjectOfType<GameplayActivity>();
        if (activity != null)
        {
            activity.m_audioService.PlaySample(Constants.Sound.SOUND_HUGE_WAVE);
            activity.m_audioService.PlaySample(Constants.Sound.SOUND_CLOUDY_THUNDER_1);

        }

        yield return new WaitForSecondsRealtime(3f);
        var board = activity?.m_board;

        random3 = UnityEngine.Random.Range(2, 6);

        for (int i  = 0; i < random3; i++){
            random1 = UnityEngine.Random.Range(0, 9);
            random2 = UnityEngine.Random.Range(0, 5);

            Plant getPlant = PlantManager.GetTopAt(random1, random2);
            if (getPlant != null)
            {
                PlantManager.Kill(getPlant);
            }

            if (board == null) break;
   
            foreach (var zombie in board.GetZombies())
            {
                if (zombie.mDead || zombie == null) continue;
                if (zombie.mZombieType == ZombieType.Target || zombie.mZombieType == ZombieType.Boss) continue;
                int zombieCol = BoardManager.PixelToGridX( (int)(zombie.mPosX + zombie.mWidth / 2), (int)zombie.mPosY );
                if (zombie.mRow == random2 && zombieCol == random1)
                {
                    ZombieManager.KillNoLoot(zombie);
                }
            }

            int gridX = BoardManager.GridToPixelX(random1, random2);
            int gridY = BoardManager.GridToPixelY(random1, random2);

            EffectsManager.Spawn(410, 355, ParticleEffect.ScreenFlash);
            EffectsManager.Spawn(gridX + 35, gridY + 35, ParticleEffect.PotatoMine);
            

            if (activity != null)
            {
                activity.m_audioService.PlaySample(Constants.Sound.SOUND_THUNDER);
                activity.m_audioService.PlaySample(Constants.Sound.SOUND_POTATO_MINE);
                activity.m_audioService.PlaySample(Constants.Sound.SOUND_THUNDER);
            }


            yield return new WaitForSecondsRealtime(0.2f);

        }

    }

}
