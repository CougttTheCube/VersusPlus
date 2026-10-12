namespace VersusPlus.Patches;
#pragma warning disable
using VersusPlus.Core;
using static VersusPlus.Core.Core;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using HarmonyLib;

using Il2Cpp;

using Il2CppBest.HTTP.SecureProtocol.Org.BouncyCastle.Bcpg;

using Il2CppReloaded;
using Il2CppReloaded.Data;
using Il2CppReloaded.DataModels;
using Il2CppReloaded.Gameplay;
using Il2CppReloaded.Services;
using Il2CppReloaded.TreeStateActivities;

using Il2CppSource.Binders;
using Il2CppSource.Controllers;
using Il2CppSource.DataModels;

using Il2CppSystem.Numerics;

using Il2CppTMPro;

using MelonLoader;

using ReplantAPI.Managers;

using UnityEngine;
using UnityEngine.UI;

using static Il2CppReloaded.Constants;


using Common = Il2CppReloaded.Gameplay.Common;
using Vector2 = UnityEngine.Vector2;
using Zombie = Il2CppReloaded.Gameplay.Zombie;

using System.Runtime.CompilerServices;


public sealed class Patch3 : MelonMod
{

    [HarmonyPatch(typeof(AudioService), nameof(AudioService.GamePause))]
    public static class ForceMusicPausePatch
    {
        public static void Postfix(AudioService __instance, bool theEnteringPause)
        {
            // Force the music to pause/resume alongside the game pause state
            __instance.GameMusicPause(theEnteringPause);
        }
    }


    // pause music when game pauses
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
        // Define the tune you want to force (e.g., Loonboon / Minigame)

        public static bool EnableCustomMusic = true;
        public static MusicTune TargetTune = MusicTune.Conveyer;


        [HarmonyPrefix]
        public static bool Prefix(MusicActivity __instance)
        {
            if (key == false)
            {
                if (BloomConfig.TestEnum2.Value == Music.Default) TargetTune = MusicTune.DayGrasswalk;
                if (BloomConfig.TestEnum2.Value == Music.BrainiacManiac) TargetTune = MusicTune.FinalBossBrainiacManiac;
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
            // Uses the packet's own internal counter as the clock
            // Skips one update every 6 ticks without storing any variables
            var board = __instance.mBoard; // or BoardManager.Board
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

}
