namespace VersusPlus.Helper;
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

using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.Injection;

using Il2CppMono.Security.Authenticode;

using Il2CppReloaded;
using Il2CppReloaded.Data;
using Il2CppReloaded.DataModels;
using Il2CppReloaded.DataModels;
using Il2CppReloaded.Gameplay;
using Il2CppReloaded.Input;
using Il2CppReloaded.Services;
using Il2CppReloaded.TreeStateActivities;

using Il2CppSentry;
using Il2CppSentry.Protocol;

using Il2CppSource.Controllers;
using Il2CppSource.DataModels;

using Il2CppSystem;
using Il2CppSystem.Numerics;
using Il2CppSystem.Reflection;

using Il2CppTekly.Injectors;
using Il2CppTekly.Localizations;

using Il2CppTMPro;

using MelonLoader;

using ReplantAPI;
using ReplantAPI.Managers;

using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.Switch;
using UnityEngine.InputSystem.XInput;
using UnityEngine.Rendering.RenderGraphModule.NativeRenderPassCompiler;

using static Il2CppSystem.Net.Http.Headers.Parser;
using static UnityEngine.ParticleSystem.PlaybackState;
using VersusPlus.Patches;

using Vector2 = UnityEngine.Vector2;

using static VersusPlus.Core.Core;
public class CoreHelper : ReplantAPI
{
    internal static class ImpManager
    {
        // Store native pointers of Imps spawned from graves (I-Zombie Imps)
        public static readonly HashSet<System.IntPtr> GraveImps = new HashSet<System.IntPtr>();

        public const float FAST_IMP_SPEED = 0.87338448f; // I-Zombie Imp speed
        public const float SLOW_IMP_SPEED = 0.29112816f; // Standard Garg-thrown Imp speed
    }

    public static void DieRow(int row)
    {
        LawnMower inRow = LawnMowerManager.GetInRow(row);
        if (inRow == null)
        {
            //MelonLogger.Warning($"[ReplantAPI] LawnMowerManager.TriggerRow: no mower found in row {row}.");
        }
        else
        {
            if (row == 0) EffectsManager.Spawn(10, 160, ParticleEffect.JackExplode);
            if (row == 1) EffectsManager.Spawn(10, 260, ParticleEffect.JackExplode);
            if (row == 2) EffectsManager.Spawn(10, 360, ParticleEffect.JackExplode);
            if (row == 3) EffectsManager.Spawn(10, 460, ParticleEffect.JackExplode);
            if (row == 4) EffectsManager.Spawn(10, 560, ParticleEffect.JackExplode);
            inRow.Die();
            var activity = UnityEngine.Object.FindObjectOfType<GameplayActivity>();
            if (activity != null)
            {
                activity.m_audioService.PlaySample(Constants.Sound.SOUND_EXPLOSION, false);

            }
        }
    }

    // Spawn a zombie directly in the house -> Zombies win
    public static void ZombieWin(){
        var board = BoardManager.Board;
        if (board != null)
        {
            ZombieManager.SpawnAtCell(ZombieType.Normal, -2, 2);

            var activity = UnityEngine.Object.FindObjectOfType<GameplayActivity>();
            if (activity != null)
            {
                activity.m_audioService.PlayMusic(MusicTune.None, 0, 0);
                activity.m_audioService.PlaySample(Constants.Sound.SOUND_EVILLAUGH, false);
                activity.m_audioService.PlaySample(Constants.Sound.SOUND_SCREAM, false);

            }
            EffectsManager.Spawn(410, 355, ParticleEffect.ScreenFlash);
            BoardManager.Board.mX = 0;


        }
    }


    // Kill the Target zombies -> Plants win
    public static void PlantWin()
    {
        var board = BoardManager.Board;
        if (board != null)
        {
            var focusCircle = GameObject.Find("Area1-Day(Clone)/P_FocusCircle(Clone)/Canvas");
            if (focusCircle != null)
            {
                //UnityEngine.Object.Destroy(focusCircle);

                focusCircle.SetActive(false);
            }
            foreach (var zombie in board.GetZombies())
            {
                if (zombie == null || zombie.mDead) continue;

                if (zombie.mZombieType == ZombieType.Target)
                {
                    zombie.TakeDamage(99999, DamageFlags.DoesntLeaveBody);
                    zombie.TakeDamage(99999, DamageFlags.DoesntLeaveBody);
                    zombie.TakeDamage(99999, DamageFlags.DoesntLeaveBody);
                    zombie.TakeDamage(99999, DamageFlags.DoesntLeaveBody);
                    zombie.TakeDamage(99999, DamageFlags.DoesntLeaveBody);
                    zombie.TakeDamage(99999, DamageFlags.DoesntLeaveBody);
                    zombie.TakeDamage(99999, DamageFlags.DoesntLeaveBody);

                }
            }

            var activity = UnityEngine.Object.FindObjectOfType<GameplayActivity>();
            if (activity != null)
            {
                activity.m_audioService.PlayMusic(MusicTune.None, 0, 0);
                activity.m_audioService.PlaySample(Constants.Sound.SOUND_CHIME, false);
                activity.m_audioService.PlaySample(Constants.Sound.SOUND_GARGANTUDEATH, false);

            }
            EffectsManager.Spawn(410, 355, ParticleEffect.ScreenFlash);
        }

    }











    // Text enumerator
    public static System.Collections.IEnumerator WaitForChaosMessage()
    {
        // Wait exactly 1.5 seconds
        yield return new WaitForSecondsRealtime(5f);
        AdviceManager.ShowAdvice("So, you have chosen death...", MessageStyle.TutorialLevel1, AdviceType.None);
        AdviceManager.SetDuration(500);
    }


    // Doom-shroom spawn enumerator
    public static System.Collections.IEnumerator SpawnDoomShroom()
    {
        int random, random2;
        for (int i = 1; i < 8; i++)
        {

            random = UnityEngine.Random.Range(0, 5);
            random2 = UnityEngine.Random.Range(0, 5);
            while (random == random2)
            {
                random2 = UnityEngine.Random.Range(0, 5);
            }

            if (i <= 5)
            {
                if (i % 2 == 0)
                {
                    PlantManager.Spawn(i, random, SeedType.Doomshroom);
                    PlantManager.Spawn(i, random2, SeedType.Doomshroom);

                    PlantManager.Spawn(i, random, SeedType.InstantCoffee);
                    PlantManager.Spawn(i, random2, SeedType.InstantCoffee);
                }
                else
                {
                    PlantManager.Spawn(i, random, SeedType.Doomshroom);

                    PlantManager.Spawn(i, random, SeedType.InstantCoffee);

                    //BoardManager.Board.mEnableGraveStones = true;
                    //BoardManager.Board.AddAGraveStone(i, random);

                }

                //BoardManager.Board.AddAGraveStone(i, random2);

            }
            else
            {
                PlantManager.Spawn(i, random, SeedType.Doomshroom);
                PlantManager.Spawn(i, random2, SeedType.Doomshroom);

                PlantManager.Spawn(i, random, SeedType.InstantCoffee);
                PlantManager.Spawn(i, random2, SeedType.InstantCoffee);
            }

            //BoardManager.Board.mEnableGraveStones = true;
            //BoardManager.Board.AddAGraveStone(i, random);
            //BoardManager.Board.AddAGraveStone(i, random2);

            yield return new WaitForSecondsRealtime(0.15f);
        }



    }


    public static void MovePlantTitle(float xOffset, float yOffset)
    {
        GameObject titleObj = Resources.FindObjectsOfTypeAll<GameObject>()
        .FirstOrDefault(g => g.name == "Title" &&
                             g.transform.parent != null &&
                             g.transform.parent.name == "SidePlants");

        if (titleObj != null)
        {
            RectTransform rect = titleObj.GetComponent<RectTransform>();
            if (rect != null)
            {
                // Set custom anchored position (X, Y)
                rect.localPosition = new UnityEngine.Vector3(xOffset, yOffset, 0);
                MelonLogger.Msg("Moved plant title!!");
            }
        }

        GameObject titleObj2 = Resources.FindObjectsOfTypeAll<GameObject>()
        .FirstOrDefault(g => g.name == "Title" &&
                             g.transform.parent != null &&
                             g.transform.parent.name == "SideZombies");

        if (titleObj2 != null)
        {
            RectTransform rect2 = titleObj2.GetComponent<RectTransform>();
            if (rect2 != null)
            {
                // Set custom anchored position (X, Y)
                rect2.localPosition = new UnityEngine.Vector3(58f, -461.5f, 0f);
                MelonLogger.Msg("Moved zombie title!!");
            }
        }


    }

    public static void CameraZooming(int value){
        if (Camera.main != null)
        {
            Camera.main.orthographicSize = value;
        }
    }

    public static void FlipZombieAvatar()
    {
        GameObject titleObj = Resources.FindObjectsOfTypeAll<GameObject>()
        .FirstOrDefault(g => g.name == "ZombieAvatar" &&
                             g.transform.parent != null &&
                             g.transform.parent.name == "PlayerAvatar");

        if (titleObj != null)
        {
            RectTransform rect = titleObj.GetComponent<RectTransform>();
            if (rect != null)
            {
                // Set custom anchored position (X, Y)
                rect.localScale = new UnityEngine.Vector3(-1, 1, 1);
                rect.localPosition = new UnityEngine.Vector3(4f, 7.2f, 0);
                MelonLogger.Msg("Flipped zombie avatar!");
            }
        }
    }

    public static void MoveChooseYourPlantHeader()
    {
        GameObject titleObj = Resources.FindObjectsOfTypeAll<GameObject>()
        .FirstOrDefault(g => g.name == "Header" &&
                             g.transform.parent != null &&
                             g.transform.parent.name == "SeedChooser");

        if (titleObj != null)
        {
            RectTransform rect = titleObj.GetComponent<RectTransform>();
            if (rect != null)
            {
                // Set custom anchored position (X, Y)
                rect.localPosition = new UnityEngine.Vector3(0f, 769f, 0);
                MelonLogger.Msg("Moved ChooseYourPlant header!");
            }
        }
    }


    public static void MoveZombiePanel()
    {
        GameObject titleObj = Resources.FindObjectsOfTypeAll<GameObject>()
        .FirstOrDefault(g => g.name == "Background" &&
                             g.transform.parent != null &&
                             g.transform.parent.name == "SideZombies");

        if (titleObj != null)
        {
            RectTransform rect = titleObj.GetComponent<RectTransform>();
            if (rect != null)
            {
                // Set custom anchored position (X, Y)
                rect.localPosition = new UnityEngine.Vector3(40f, -26f, 0);
                MelonLogger.Msg("Moved Zombie panel!");
            }
        }
    }



}
