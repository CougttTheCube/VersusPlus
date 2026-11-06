namespace VersusPlus.Patches;
#pragma warning disable
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.CompilerServices;

using HarmonyLib;

using Il2Cpp;

using Il2CppBest.HTTP.SecureProtocol.Org.BouncyCastle.Bcpg;

using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.Injection;

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

using VersusPlus.Core;

using static Il2CppReloaded.Constants;
using static VersusPlus.Core.Core;
using static VersusPlus.Helper.CoreHelper;
using static VersusPlus.Patches.Patch3;

using Common = Il2CppReloaded.Gameplay.Common;
using Vector2 = UnityEngine.Vector2;
using Zombie = Il2CppReloaded.Gameplay.Zombie;


public sealed class Patch4 : MelonMod
{

    // Adjust speed for Imp based on how it spawns
    [HarmonyPatch(typeof(Zombie), nameof(Zombie.StopEating))]
    internal static class ImpSpeedFix
    {
        [HarmonyPostfix]
        private static void Prefix0(Zombie __instance)
        {
            if (__instance.mZombieType != ZombieType.Imp) return;
            if (!__instance.mApp.IsVersusMode()) return;


            System.IntPtr ptr = IL2CPP.Il2CppObjectBaseToPtr(__instance);

            if (ImpManager.GraveImps.Contains(ptr))
            {
                // Fast Imp (from Grave)
                __instance.mVelX = ImpManager.FAST_IMP_SPEED;
            }
            else
            {
                // Normal Imp (thrown by Gargantuar)
                __instance.mVelX = ImpManager.SLOW_IMP_SPEED;
            }

            __instance.UpdateAnimSpeed();
        }
    }

    [HarmonyPatch(typeof(Zombie), nameof(Zombie.RiseFromGrave))]
    internal static class Zombie_RiseFromGrave_Patch
    {
        // Give I-Zombie Imp stats if spawned from grave
        [HarmonyPrefix]
        private static bool Prefix0(Zombie __instance)
        {

            if (__instance.mZombieType != ZombieType.Imp) return true;
            if (!__instance.mApp.IsVersusMode()) return true;

            System.IntPtr ptr = IL2CPP.Il2CppObjectBaseToPtr(__instance);
            ImpManager.GraveImps.Add(ptr);
            __instance.mBodyMaxHealth = 85;
            __instance.mBodyHealth = 85;
            __instance.mVelX = ImpManager.FAST_IMP_SPEED;
            __instance.UpdateAnimSpeed();

            return true;

        }

        // Skip rise from grave phase for balloon zombie
        [HarmonyPrefix]
        private static bool Prefix(Zombie __instance)
        {
            
            if (__instance.mZombieType != ZombieType.Balloon) return true;
            if (!__instance.mApp.IsVersusMode()) return true;

            __instance.mPosX = 800f; // Push it off-screen immediately
            __instance.mZombiePhase = ZombiePhase.BalloonFlying;


            return false;

        }

        // Skip rise from grave phase for digger zombie
        [HarmonyPrefix]
        private static bool Prefix2(Zombie __instance)
        {
            if (__instance.mZombieType != ZombieType.Digger) return true;
            if (!__instance.mApp.IsVersusMode()) return true;


            __instance.mPosX = 800f;
            __instance.mZombiePhase = ZombiePhase.DiggerTunneling;

            return false;



        }

        // Skip rise from grave phase for the zombotanies
        [HarmonyPrefix]
        private static bool Prefix3(Zombie __instance)
        {
            if (__instance.mZombieType != ZombieType.PeaHead &&
            __instance.mZombieType != ZombieType.GatlingHead &&
            __instance.mZombieType != ZombieType.JalapenoHead &&
            __instance.mZombieType != ZombieType.TallnutHead &&
            __instance.mZombieType != ZombieType.RedeyeGargantuar) return true;
            if (!__instance.mApp.IsVersusMode()) return true;

            __instance.mPosX = 800f;

            return false;

        }

        
        // Bobsled: Spawn an ice trail
        [HarmonyPrefix]
        private static bool Prefix4(Zombie __instance, int theCol, int theRow)
        {
            if (__instance.mZombieType != ZombieType.Bobsled) return true;
            if (!__instance.mApp.IsVersusMode()) return true;
            var board = BoardManager.Board;
            if (board == null) return true;


            __instance.mPosX = 870f;
            __instance.mZombiePhase = ZombiePhase.BobsledSliding;

            board.mIceMinX[theRow] = board.GridToPixelX(6, theRow);
            board.mIceTimer[theRow] = 1000;
            
            return false;
        }

        // Boss
        [HarmonyPrefix]
        private static bool Prefix5(Zombie __instance, int theCol, int theRow)
        {
            if (__instance.mZombieType != ZombieType.Boss) return true;
            if (!__instance.mApp.IsVersusMode()) return true;
            var board = BoardManager.Board;
            if (board == null) return true;


            __instance.mPosX = 800f;
            __instance.mZombiePhase = ZombiePhase.BossEnter;


            return false;
        }
    }



    // Remove from list if Imp die
    [HarmonyPatch(typeof(Zombie), nameof(Zombie.DieNoLoot))]
    internal static class ImpDeathCleanupPatch
    {
        [HarmonyPostfix]
        private static void Postfix(Zombie __instance)
        {

            if (__instance.mZombieType != ZombieType.Imp) return;

            System.IntPtr ptr = IL2CPP.Il2CppObjectBaseToPtr(__instance);
            ImpManager.GraveImps.Remove(ptr);
        }
    }

    // Give normal Imp stats if spawned from gargantuar throw
    [HarmonyPatch(typeof(Zombie), "set_mZombiePhase")]
    internal static class GargantuarThrownImpPhasePatch
    {
        [HarmonyPostfix]
        private static void Postfix(Zombie __instance, ZombiePhase theZombiePhase)
        {
            if (__instance.mZombieType != ZombieType.Imp) return;

            // Trigger only when the Imp is actively thrown by a Gargantuar
            if (theZombiePhase == ZombiePhase.ImpGettingThrown)
            {
                __instance.mBodyMaxHealth = 270;
                __instance.mBodyHealth = 270;
            }
        }
    }




    [HarmonyPatch(typeof(Board), nameof(Board.AddZombieInRow))]
    internal static class SpawnPlant
    {
        [HarmonyPrefix]
        private static bool Prefix0(Board __instance, ref ZombieType theZombieType, ref int theRow)
        {
            if (!__instance.mApp.IsVersusMode()) return true;

            SeedType zombieCursor = CursorManager.GetSeedTypeInCursor(1);
            SeedType plantCursor = CursorManager.GetSeedTypeInCursor(0);

            // Swap ZombiquariumSnorkel with Backup Dancer
            if (zombieCursor == SeedType.ZombiquariumSnorkel
            || plantCursor == SeedType.ZombiquariumSnorkel)
            {
                // Force the translation
                theZombieType = ZombieType.BackupDancer;
                return true;
            }

            // Swap ZombiquariumTrophy with Peashooter Zombie
            if (zombieCursor == SeedType.ZombiquariumTrophy
            || plantCursor == SeedType.ZombiquariumTrophy)
            {
                // Force the translation
                theZombieType = ZombieType.PeaHead;
                return true;
            }

            if (zombieCursor == SeedType.BeghouledButtonCrater
            || plantCursor == SeedType.BeghouledButtonCrater)
            {
                // Force the translation
                theZombieType = ZombieType.GatlingHead;
                return true;
            }

            if (zombieCursor == SeedType.BeghouledButtonShuffle
            || plantCursor == SeedType.BeghouledButtonShuffle)
            {
                // Force the translation
                theZombieType = ZombieType.TallnutHead;
                return true;
            }

            if (zombieCursor == SeedType.SlotMachineSun
            || plantCursor == SeedType.SlotMachineSun)
            {
                // Force the translation
                theZombieType = ZombieType.JalapenoHead;
                return true;
            }

            if (zombieCursor == SeedType.SlotMachineDiamond
            || plantCursor == SeedType.SlotMachineDiamond)
            {
                // Force the translation
                theZombieType = ZombieType.RedeyeGargantuar;
                return true;
            }


            if (zombieCursor == SeedType.Sprout
            || plantCursor == SeedType.Sprout)
            {
                // Force the translation
                theZombieType = ZombieType.Bobsled;
                return true;
            }

            if (zombieCursor == SeedType.NumSeedTypes
            || plantCursor == SeedType.NumSeedTypes)
            {
                // Force the translation
                theZombieType = ZombieType.Boss;
                theRow = 2;
                return true;
            }

            return true;
        }

        // Plays huge wave sound when Flag Zombie is spawned
        [HarmonyPrefix]
        private static bool Prefix2(Board __instance, ZombieType theZombieType)
        {
            if (!__instance.mApp.IsVersusMode()) return true;
            if (theZombieType == ZombieType.Flag)
            {
                var activity = UnityEngine.Object.FindObjectOfType<GameplayActivity>();
                if (activity != null)
                {
                    activity.m_audioService.PlaySample(Constants.Sound.SOUND_HUGE_WAVE);

                }
                // Buff Flag Zombie: Add 2 more zombies
                int random = UnityEngine.Random.Range(0, 5);
                ZombieManager.SpawnInRow(ZombieType.Pail, random);
                random = UnityEngine.Random.Range(0, 5);
                ZombieManager.SpawnInRow(ZombieType.Normal, random);
            }

            return true;
        }

    }



    // Preview image doesnt show on craters for zombies
    [HarmonyPatch(typeof(Board), nameof(Board.CanPlantAt))]
    internal static class ZombieNotPlantingonGraves
    {
        private static void Postfix(Board __instance, int theGridX, int theGridY, SeedType theType, ref PlantingReason __result)
        {

            if (__instance == null) return;
            if (!__instance.mApp.IsVersusMode()) return;

            if (((int)theType >= 60 && (int)theType <= 80) &&
            (GridItemManager.GetAt(GridItemType.Crater, theGridX, theGridY) != null))
            {
                if ((int)theType == 65 ||
                (int)theType == 68 ||
                (int)theType == 70 ||
                (int)theType == 72 ||
                (int)theType == 73 ||
                (int)theType == 75 ||
                (int)theType == 78 ||
                (int)theType == 79) return;

                __result = PlantingReason.NotOnCrater;

            }

            // Too lazy to find out why it's on plant side so brute force custom zombie to be placed on zombie side
            if (theType == SeedType.ZombiquariumSnorkel || 
            theType == SeedType.ZombiquariumTrophy || 
            theType == SeedType.BeghouledButtonCrater ||
            theType == SeedType.BeghouledButtonShuffle ||
            theType == SeedType.SlotMachineDiamond ||
            theType == SeedType.SlotMachineSun ||
            theType == SeedType.Sprout ||
            theType == SeedType.NumSeedTypes) 
            {
                if (theGridX >= 6 && theGridX <= 8){
                    __result = PlantingReason.Ok;
                    return;
                } else {
                    __result = PlantingReason.VSNotHere;
                    return;
                }
            }

            return;
        }
    }

    // Not allowing zombie to be place on crater rework
    [HarmonyPatch(typeof(Challenge), nameof(Challenge.IZombieMouseDownWithZombie))]
    internal static class BlockZombiePlacementOnCraterPatch
    {
        [HarmonyPrefix]
        private static bool Prefix(Challenge __instance, int x, int y, int theClickCount, int playerIndex)
        {
            if (__instance == null || __instance.mBoard == null) return true;

            SeedType cursorSeed = CursorManager.GetSeedTypeInCursor(playerIndex);

            // Check if holding seed packet
            bool holdingZombieSeed = ((int)cursorSeed >= 60 && (int)cursorSeed <= 80);

            if ((int)cursorSeed == 65 ||
                (int)cursorSeed == 68 ||
                (int)cursorSeed == 70 ||
                (int)cursorSeed == 72 ||
                (int)cursorSeed == 73 ||
                (int)cursorSeed == 75 ||
                (int)cursorSeed == 78 ||
                (int)cursorSeed == 79)
            {
                holdingZombieSeed = false;
            }

            if (holdingZombieSeed)
            {
                int targetCol = __instance.mBoard.PixelToGridX(x, y);
                int targetRow = __instance.mBoard.PixelToGridY(x, y);

                Plant plan = PlantManager.GetTopAt(targetCol, targetRow);

                if (GridItemManager.GetAt(GridItemType.Crater, targetCol, targetRow) != null)
                {
                    var activity = UnityEngine.Object.FindObjectOfType<GameplayActivity>();
                    if (activity != null)
                    {
                        activity.m_audioService.PlaySample(Constants.Sound.SOUND_BUZZER);
                    }

                    return false; 
                }
            }

            return true;
        }



        [HarmonyPrefix]
        private static bool Prefix2(Challenge __instance, int x, int y, int theClickCount, int playerIndex)
        {
            if (__instance == null || __instance.mBoard == null) return true;

            SeedType cursorSeed = CursorManager.GetSeedTypeInCursor(playerIndex);

            // Check if holding gravestone seed packet
            bool holdingZombieSeed = (cursorSeed == SeedType.ZombieGravestone);
            if (!holdingZombieSeed) return true;

            int targetCol = __instance.mBoard.PixelToGridX(x, y);
            int targetRow = __instance.mBoard.PixelToGridY(x, y);

            Plant plan = PlantManager.GetTopAt(targetCol, targetRow);

            if (plan != null && plan.mSeedType == SeedType.Lilypad)
            {
                var activity = UnityEngine.Object.FindObjectOfType<GameplayActivity>();
                if (activity != null)
                {
                    activity.m_audioService.PlaySample(Constants.Sound.SOUND_BUZZER);
                }

                return false;
            }
            

            return true;
        }

    }


    // Fix Imp in I-zombie having tuff health
    [HarmonyPatch(typeof(Zombie), nameof(Zombie.ZombieInitialize))]
    internal static class ImpDefaultHealthPatch
    {
        [HarmonyPostfix]
        private static void Postfix(Zombie __instance)
        {
            if (__instance == null || __instance.mApp == null || __instance.mZombieType != ZombieType.Imp) return;
            if (!__instance.mApp.IsIZombieLevel()) return;

            __instance.mBodyMaxHealth = 85;
            __instance.mBodyHealth = 85;
            
        }
    }


    // Boss doesn't kill every zombie on the lawn once dead
    [HarmonyPatch(typeof(Zombie), nameof(Zombie.BossDie))]
    public static class ZombossDeathPatch
    {
        [HarmonyPrefix]
        public static bool Prefix(Zombie __instance)
        {
            if (!__instance.mApp.IsVersusMode()) return true;
            var activity = UnityEngine.Object.FindObjectOfType<GameplayActivity>();
            if (activity != null) { 
                //activity.m_audioService.MusicFadeOut(0.1f);
                activity.m_audioService.PlayMusic(MusicTune.None, 0, 0);
                CustomMusicPatch.TargetTune = MusicTune.None;
            }

            
            return false;
        }
    }

    // Skip the RV throwing phase because it's evil
    [HarmonyPatch(typeof(Zombie), nameof(Zombie.BossRVAttack))]
    public static class ZombossRVhPatch
    {
        [HarmonyPostfix]
        public static bool Prefix(Zombie __instance)
        {
            if (!__instance.mApp.IsVersusMode()) return true;
            return false;
        }
    }

    [HarmonyPatch(typeof(Challenge), nameof(Challenge.IsZombieSeedType))]
    public static class dih
    {
        [HarmonyPostfix]
        public static void Postfix(SeedType theSeedType, ref bool __result)
        {
            if (theSeedType == SeedType.ZombiquariumSnorkel || 
            theSeedType == SeedType.ZombiquariumTrophy ||
            theSeedType == SeedType.BeghouledButtonCrater ||
            theSeedType == SeedType.BeghouledButtonShuffle ||
            theSeedType == SeedType.SlotMachineSun ||
            theSeedType == SeedType.SlotMachineDiamond ||
            theSeedType == SeedType.Sprout ||
            theSeedType == SeedType.NumSeedTypes)
            {
                __result = true;
            }
        }
    }

    /*
    [HarmonyPatch(typeof(Challenge), nameof(Challenge.IZombieSeedTypeToZombieType))]
    public static class dih2
    {
        [HarmonyPostfix]
        public static void Postfix(SeedType theSeedType, ref ZombieType __result)
        {
            if (theSeedType == SeedType.ZombiquariumSnorkel)
            {
                __result = ZombieType.BackupDancer;
            }
        }
    }
    */


    // Too lazy to find out why it's on plant side so brute force custom zombie to be placed on zombie side
    [HarmonyPatch(typeof(Challenge), nameof(Challenge.CanPlantAt))]
    internal static class CustomZombieCanPlantAt_Patch
    {
        private static void Postfix(Challenge __instance, int theGridX, int theGridY, SeedType theType, ref PlantingReason __result)
        {

            if (BoardManager.Board == null || BoardManager.App == null) return;
            if (!__instance.mApp.IsVersusMode()) return;

            if (theType == SeedType.ZombiquariumSnorkel || 
            theType == SeedType.ZombiquariumTrophy ||
            theType == SeedType.BeghouledButtonCrater ||
            theType == SeedType.BeghouledButtonShuffle ||
            theType == SeedType.SlotMachineSun ||
            theType == SeedType.SlotMachineDiamond ||
            theType == SeedType.Sprout ||
            theType == SeedType.NumSeedTypes)
            {
                if (theGridX >= 6 && theGridX <= 8){
                    __result = PlantingReason.Ok;
                    return;
                } else {
                    __result = PlantingReason.VSNotHere;
                    return;
                }
            }

            return;
        }
    }

}
