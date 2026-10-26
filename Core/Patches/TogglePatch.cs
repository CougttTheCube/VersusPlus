namespace VersusPlus.Patches;
#pragma warning disable
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.CompilerServices;

using Harmony;

using HarmonyLib;

using Il2Cpp;

using Il2CppBest.HTTP.SecureProtocol.Org.BouncyCastle.Bcpg;

using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.InteropTypes.Arrays;

using Il2CppReloaded;
using Il2CppReloaded.Data;
using Il2CppReloaded.DataModels;
using Il2CppReloaded.Gameplay;
using Il2CppReloaded.Services;
using Il2CppReloaded.TreeStateActivities;

using Il2CppSource.Binders;
using Il2CppSource.Controllers;
using Il2CppSource.DataModels;

using Il2CppSystem.Collections.Generic;
using Il2CppSystem.Numerics;

using Il2CppTMPro;

using MelonLoader;

using ReplantAPI.Managers;

using UnityEngine;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.UI;
using UnityEngine.UIElements;

using VersusPlus.Core;

using static Il2CppReloaded.Constants;
using static VersusPlus.Core.Core;

using Common = Il2CppReloaded.Gameplay.Common;
using HarmonyPatch = HarmonyLib.HarmonyPatch;
using HarmonyPostfix = HarmonyLib.HarmonyPostfix;
using HarmonyPrefix = HarmonyLib.HarmonyPrefix;
using Vector2 = UnityEngine.Vector2;
using Zombie = Il2CppReloaded.Gameplay.Zombie;

public sealed class Patch3 : MelonMod
{

    // pause music when game pauses
    [HarmonyPatch(typeof(AudioService), nameof(AudioService.GamePause))]
    public static class ForceMusicPausePatch
    {
        public static void Postfix(AudioService __instance, bool theEnteringPause)
        {
            __instance.GameMusicPause(theEnteringPause);
        }
    }

    [HarmonyPatch(typeof(GameplayActivity), nameof(GameplayActivity.StartPlaying))]
    public static class GameplayActivityTracker
    {
        public static GameplayActivity CurrentInstance;

        public static void Postfix(GameplayActivity __instance)
        {
            CurrentInstance = __instance;
        }
    }

    // music doesnt switch + music player
    [HarmonyPatch(typeof(MusicActivity), nameof(MusicActivity.TriggerAudio))]
    public static class CustomMusicPatch
    {

        public static bool EnableCustomMusic = true;
        // value placeholder
        public static MusicTune TargetTune = MusicTune.Conveyer;


        [HarmonyPrefix]
        public static bool Prefix(MusicActivity __instance)
        {
            if (key == false)
            {
                if (BloomConfig.TestEnum2.Value == Music.Grasswalk) TargetTune = MusicTune.DayGrasswalk;
                if (BloomConfig.TestEnum2.Value == Music.Moongrains) TargetTune = MusicTune.NightMoongrains;
                if (BloomConfig.TestEnum2.Value == Music.BrainiacManiac) TargetTune = MusicTune.FinalBossBrainiacManiac;
                if (BloomConfig.TestEnum2.Value == Music.Cerebrawl) TargetTune = MusicTune.PuzzleCerebrawl;
                if (BloomConfig.TestEnum2.Value == Music.Loonboon) TargetTune = MusicTune.MinigameLoonboon;
                if (BloomConfig.TestEnum2.Value == Music.Conveyer) TargetTune = MusicTune.Conveyer;
            }
            
            var current = GameplayActivityTracker.CurrentInstance;

            if (EnableCustomMusic &&
            current != null &&
            current.IsVersusMode() &&
            current.m_versusMode != null &&
            current.m_versusMode.m_gameplayInitialized)
            {
                __instance.m_musicTune = TargetTune;
            }
            return true;
        }
    }



    // Seed recharge slow
    [HarmonyPatch(typeof(SeedPacket), nameof(SeedPacket.Update))]
    public static class SeedPacketRechargeRatePatch
    {
        [HarmonyPrefix]
        public static bool Prefix(SeedPacket __instance)
        {
            if (__instance == null || __instance.mRefreshCounter <= 0)
                return true;
            if (!BloomConfig.TestBoolInput11.Value) return true;

            // Skips one update every 6 ticks 
            var board = __instance.mBoard;
            if (board != null)
            {
                if ((board.mMainCounter + __instance.mIndex) % 6 == 0)
                    return false;
            }

            return true;
        }
    }



    // Custom brain rate
    // Toggle for Vs Chaos
    [HarmonyPatch(typeof(Zombie), nameof(Zombie.ZombieInitialize))]
    internal static class GravestoneBrainRate_Patch
    {
        [HarmonyPostfix]
        private static void Prefix9(Zombie __instance)
        {
            if (!BloomConfig.TestBoolInput6.Value) return;
            if (__instance == null) return;
            if (__instance.mZombieType != ZombieType.Gravestone) return;
            if (!__instance.mApp.IsVersusMode()) return;

            if (!BloomConfig.TestBoolInput12.Value) __instance.mBrainRate = 6865;
            else __instance.mBrainRate = (int)(__instance.mBrainRate * 2.746 / 1.5);
            // set to 6865 normally
            // but vs chaos multiplies ts value to 10297
            // so add a toggle that divides back to 6865
            // workaround to make the 100% brain production perk work
        }
    }


    // Toggle: Stage
    public static void ApplyStageToLevelDataModel()
    {

        var allDefs = Resources.FindObjectsOfTypeAll(Il2CppType.Of<LevelEntryData>());

        foreach (var obj in allDefs)
        {
            if (obj == null) continue;
            var entry = obj.Cast<LevelEntryData>();

            if (entry.m_gameMode == GameMode.Versus || entry.name.Contains("Versus"))
            {
                if (BloomConfig.TestEnum3.Value == Stage.Night)
                {
                    entry.m_gameArea = GameArea.Night;
                    if (entry.m_backgroundPrefab != null)
                    {
                        entry.m_backgroundPrefab.m_AssetGUID = "3e831435e93959f40b51dfac058dec5f";
                    }

                }
                else if (BloomConfig.TestEnum3.Value == Stage.Day)
                {
                    entry.m_gameArea = GameArea.Day;
                    if (entry.m_backgroundPrefab != null)
                    {
                        entry.m_backgroundPrefab.m_AssetGUID = "45d8eb3daa42e2e4ba4e8c9c6bd295ac";
                    }

                }
            }
        }
    }





    // Stop gravestones from being added in Night Vs
    [HarmonyPatch(typeof(Board), nameof(Board.AddGraveStones))]
    public static class StopAddingGravestoneInVSNight
    {
        public static bool Prefix(Board __instance)
        {
            if (__instance != null && __instance.mLevelEntryData.m_gameMode == GameMode.Versus &&
            BloomConfig.TestEnum3.Value == Stage.Night)
            {
                return false;
            }
            return true;
        }
    }

    // Spawn a column of graves in Night Vs
    [HarmonyPatch(typeof(Board), nameof(Board.StartLevel))]
    public static class StartLevelVSNight
    {
        public static void Postfix(ref Board __instance)
        {
            if (__instance != null && __instance.mLevelEntryData.m_gameMode == GameMode.Versus &&
            BloomConfig.TestEnum3.Value == Stage.Night)
            {

                //PlantManager.Spawn(0, 0, SeedType.Sunflower);
                //PlantManager.Spawn(0, 2, SeedType.Sunflower);
                //PlantManager.Spawn(0, 4, SeedType.Sunflower);
                
                //Zombie newGravestone = ZombieManager.SpawnAtCell(ZombieType.Gravestone, 8, 0);
                
                //newGravestone.mZombiePhase = ZombiePhase.RisingFromGrave;
                //newGravestone.mPosX = 8 * 75 + 55;
                //newGravestone.mPosY += 300f;
                //newGravestone.mGraveX = 8; newGravestone.mGraveY = 0;
                //newGravestone.mX = newGravestone.mPosX;// newGravestone.mY = newGravestone.mPosY;
                //newGravestone.mPosY = __instance.GridToPixelY(8, 0);

                //.mPosX = 8; newGravestone.mY = 0;


                BoardManager.Board.AddAGraveStone(5, 0);
                BoardManager.Board.AddAGraveStone(5, 1);
                BoardManager.Board.AddAGraveStone(5, 2);
                BoardManager.Board.AddAGraveStone(5, 3);
                BoardManager.Board.AddAGraveStone(5, 4);
                
                __instance.mEnableGraveStones = true;
                
                //MelonLogger.Msg("GRAVESTONE COUNT: " + BoardManager.Board.GetGraveStoneCount());
            }

        }
    }


    



}
