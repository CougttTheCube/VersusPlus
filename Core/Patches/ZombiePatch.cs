namespace VersusPlus.Patches;
#pragma warning disable
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.Injection;
using Il2CppTMPro;

using MelonLoader;

using ReplantAPI.Managers;

using UnityEngine;
using UnityEngine.UI;

using VersusPlus.Core;

using static Il2CppReloaded.Constants;
using static VersusPlus.Core.Core;
using static VersusPlus.Helper.CoreHelper;

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

        // Skip rise from grave phase for zombotanies
        /*
        [HarmonyPrefix]
        private static bool Prefix3(Zombie __instance)
        {
            if (__instance.mZombieType != ZombieType.PeaHead) return true;
            if (__instance.mZombieType != ZombieType.WallnutHead) return true;
            if (__instance.mZombieType != ZombieType.SquashHead) return true;
            if (__instance.mZombieType != ZombieType.TallnutHead) return true;
            if (__instance.mZombieType != ZombieType.GatlingHead) return true;
            if (__instance.mZombieType != ZombieType.JalapenoHead) return true;
            if (!__instance.mApp.IsVersusMode()) return true;


            __instance.mPosX = 800f;
            __instance.mZombiePhase = ZombiePhase.ZombieNormal;

            return false;

        }
        */


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
    [HarmonyPatch(typeof(Zombie), nameof(Zombie.mZombiePhase))]
    internal static class GargantuarThrownImpPhasePatch
    {
        [HarmonyPostfix]
        private static void Postfix(Zombie __instance, ZombiePhase theZombiePhase)
        {
            if (__instance.mZombieType != ZombieType.Imp) return;
            if (!__instance.mApp.IsVersusMode()) return;

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
        private static bool Prefix0(Board __instance, ref ZombieType theZombieType)
        {
            if (!__instance.mApp.IsVersusMode()) return true;

            SeedType zombieCursor = CursorManager.GetSeedTypeInCursor(1);
            SeedType plantCursor = CursorManager.GetSeedTypeInCursor(0);

            if (zombieCursor == SeedType.ZombiquariumSnorkel
            || plantCursor == SeedType.ZombiquariumSnorkel)
            {
                // Force the translation
                theZombieType = ZombieType.PeaHead;
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
        private static void Postfix(int theGridX, int theGridY, SeedType theType, ref PlantingReason __result)
        {
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

}
