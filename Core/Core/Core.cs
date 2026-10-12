namespace VersusPlus.Core;
#pragma warning disable
// RaptorRush - Seed Recharge Tooltip

// D1GQ - SeedChooserDataModel Reload [Removed custom functions]
// https://github.com/D1GQ/ReplantedOnline/blob/3f1fe17378ec0c98ab9866aa74bb775a3fd71ff6/src/Patches/Reloaded/Gameplay/UI/SeedChooserPatch.cs#L38

using static VersusPlus.Helper.CoreHelper;
using VersusPlus.UI;
using static VersusPlus.UI.Concede0;
using static VersusPlus.UI.Timer;
using static VersusPlus.UI.Timer2;
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
using VersusPlus.Helper;

using Vector2 = UnityEngine.Vector2;


public class Core : ReplantAPI
{
    public static bool isImpSpawnedFromGrave = false;
    public bool hasPlayedSuddenDeath = false;
    public bool hasPlayedEnd = false;
    public bool hasPlayedTen = false;
    public static bool TenMinute = false;
    public static bool StartMatch = false;
    public static bool hasChaos = false;

    public static bool hasDisplayedChaosMessage = false;

    public bool InitializeMatch = false;
    public static bool activateonce = false;
    public const string Name = "VersusPlus";
    public const string Version = "1.3.2.2";
    public const string Author = "Cougtt and many more";
    public static float holdDuration = 0f;
    public static float holdDuration2 = 0f;
    SeedPacket temp;
    public enum TimerPos
    {
        TopMiddle,
        BottomMiddle,
        BottomRight
    }

    public enum Music
    {
        Loonboon,
        Conveyer,
        BrainiacManiac,
        Default
    }

    public static bool key = false;

    private bool connected = false;

    private static UnityEngine.InputSystem.Gamepad p0Pad = null;
    private static UnityEngine.InputSystem.Gamepad p1Pad = null;
    public static int random;
    internal static class BloomConfig
    {
        public static BoolConfigInput TestBoolInput { get; } = ConfigService.CreateBool(
            name: "Seed Recharge Tooltip",
            defaultValue: true,
            description: "Shows recharge of units in the tooltip box."
        );
        public static BoolConfigInput TestBoolInput2 { get; } = ConfigService.CreateBool(
            name: "Seed Recharge Timer",
            defaultValue: true,
            description: "Shows live recharge time of units."
        );
        public static BoolConfigInput TestBoolInput3 { get; } = ConfigService.CreateBool(
            name: "Instant Death",
            defaultValue: true,
            description: "Enables Instant Death Phase - A row of Peashooter is spawned at the back (if grid is empty) and all remaining lawnmowers are destroyed."
        );
        public static BoolConfigInput TestBoolInput4 { get; } = ConfigService.CreateBool(
            name: "Timer",
            defaultValue: true,
            description: "Shows the timer."
        );
        public static EnumConfigInput<TimerPos> TestEnum { get; } = ConfigService.CreateEnum<TimerPos>(
            name: "Timer Position",
            defaultValue: TimerPos.BottomMiddle,
            description: "Changes the timer's position."
        );
        public static BoolConfigInput TestBoolInput5 { get; } = ConfigService.CreateBool(
            name: "Initial Recharge",
            defaultValue: true,
            description: "Sets initial recharge for almost all units."
        );
        public static BoolConfigInput TestBoolInput10 { get; } = ConfigService.CreateBool(
            name: "Better Sudden Death",
            defaultValue: false,
            description: "Sudden Death recharge time is the same as Versus Base recharge time. Enable this if you don't like instant plants spamming."
        );
        public static BoolConfigInput TestBoolInput6 { get; } = ConfigService.CreateBool(
            name: "New Brain Rate Logic",
            defaultValue: true,
            description: "Sets Gravestone's brain rate to around 27.5 seconds, includes initial brain production. Disable this if you have custom brain rate logic (or just wants zombies to win)."
        );
        public static BoolConfigInput TestBoolInput7 { get; } = ConfigService.CreateBool(
            name: "Lighter Projectiles",
            defaultValue: true,
            description: "Targets and Gravestones take no more than 35 damage per hit. Disable this if you are not afraid of Melon-pult."
        );
        public static BoolConfigInput TestBoolInput8 { get; } = ConfigService.CreateBool(
            name: "???",
            defaultValue: false,
            description: "This option is created solely for credits only. Do not enable, or else...\n" +
            "\n" +
            "Seed Recharge Tooltip - by RaptorRush, inspired by PvZ DLC Mod\n" +
            "Instant Death, Seed Recharge Timer, Conceding feature, Better Sudden Death, Music Player - inspired by PvZ DLC Mod\n" +
            "Initial Recharge - partially helped by D1GQ\n" +
            "New Brain Rate Logic - by D1GQ\n" +
            "Most of the patches - by D1GQ and RaptorRush\n" +
            "\n" +
            "Thank you very much :) You 2 are the goat. I wouldn't have tried my best if it hadn't been for these two.\n" +
            "Also thank you HenHen for ReplantAPI, " +
            "and thank you PalmForest for BloomEngine."
        );
        public static BoolConfigInput TestBoolInput9 { get; } = ConfigService.CreateBool(
            name: "Random Events",
            defaultValue: false,
            description: "Enables random events."
        );
        public static EnumConfigInput<Music> TestEnum2 { get; } = ConfigService.CreateEnum<Music>(
            name: "Music Player",
            defaultValue: Music.Default,
            description: "Changes music."
        );
        public static BoolConfigInput TestBoolInput11 { get; } = ConfigService.CreateBool(
            name: "Slower Recharge [Exp]",
            defaultValue: false,
            description: "Sets recharge of all units to be 20% slower, therefore accurate to real life time. Experimental."
        );
        public static BoolConfigInput TestBoolInput12 { get; } = ConfigService.CreateBool(
            name: "Vs CHAOS Brain [Exp]",
            defaultValue: false,
            description: "Enable this if you play with VS CHAOS mod to fix brain rate logic. Experimental."
        );
    }
    // Custom DieRow
    
    public override void OnInitializeMelon()
    {
        this.LoggerInstance.Msg("Loading core...");
        GameplayActivityApi.ActiveStarted += GameplayStarted;

        ClassInjector.RegisterTypeInIl2Cpp<Timer>();
        ClassInjector.RegisterTypeInIl2Cpp<Timer2>();
        ClassInjector.RegisterTypeInIl2Cpp<Concede0>();

        ModMenuService.CreateEntry(this)
            .AddDisplayName(Name)
            .AddDescription($"Gives a better experience in Versus Mode!")
            .AddConfigClass(typeof(BloomConfig))
            .Register();

        // Add icon
        ModMenuService.CreateEntry(this)
            .AddIcon(AssetHelper.LoadSprite<Core>("VersusPlus.Resources.icon.png"));

    }



    public override void OnUpdate()
    {
        base.OnUpdate();
        if (!BoardManager.IsLoaded) return;

        // Detect input for both players

        var cursor0 = CursorManager.GetCursor(0);
        var cursor1 = CursorManager.GetCursor(1);

        bool p0GamepadB = false;
        bool p1GamepadB = false;

        if (cursor0 != null && cursor0.m_player != null)
        {
            if (cursor0.m_player.TryGetGamepad(out Gamepad p0Pad) && p0Pad != null)
            {
                p0GamepadB = p0Pad.bButton.isPressed;
            }
        }
        if (cursor1 != null && cursor1.m_player != null)
        {
            if (cursor1.m_player.TryGetGamepad(out Gamepad p1Pad) && p1Pad != null)
            {
                p1GamepadB = p1Pad.bButton.isPressed;
            }
        }

        // Player 0: Mouse Right-Click OR their assigned Gamepad B
        bool p0Pressed = Input.GetKey(KeyCode.Mouse1) || p0GamepadB;
        if (p0Pressed)
        {
            holdDuration += Time.deltaTime;
        }
        else
        {
            holdDuration = 0f;
        }

        // Player 1: Their assigned Gamepad B (Player 1 can never be mouse)
        bool p1Pressed = p1GamepadB;
        if (p1Pressed)
        {
            holdDuration2 += Time.deltaTime;
        }
        else
        {
            holdDuration2 = 0f;
        }



        // Conceding when hold for player 0
        if (holdDuration >= 6.5f)
        {
            if (!Core.activateonce)
            {
                AdviceManager.ShowAdvice("Player 1 CONCEDED!", MessageStyle.HugeWave, AdviceType.None);
                AdviceManager.SetDuration(500);

                hasPlayedEnd = true;
                Core.activateonce = true;
                holdDuration = 0f;

                SeedType player1Cursor = CursorManager.GetSeedTypeInCursor(1);

                if (60 <= (int)player1Cursor && (int)player1Cursor <= 80) // If player 1 is zombie, then zombie wins
                {
                    ZombieWin();
                }
                else
                {
                    PlantWin();
                }
            }
        }


        // Conceding when hold for player 1
        if (holdDuration2 >= 6.5f)
        {
            if (!Core.activateonce)
            {
                AdviceManager.ShowAdvice("Player 2 CONCEDED!", MessageStyle.HugeWave, AdviceType.None);
                AdviceManager.SetDuration(500);

                hasPlayedEnd = true;
                Core.activateonce = true;
                holdDuration2 = 0f;

                SeedType player1Cursor = CursorManager.GetSeedTypeInCursor(1);

                if (!(60 <= (int)player1Cursor && (int)player1Cursor <= 80)) // if Player 1 is plant, then zombie wins
                {

                    ZombieWin();
                }
                else
                {
                    PlantWin();
                }
            }
        }



        // Toggle: Sets initial recharge time
        if (BoardManager.Board.mApp != null && BoardManager.Board.mApp.VersusMode != null
            && BoardManager.Board.mApp.VersusMode.m_gameplayInitialized)
        {

            if (!InitializeMatch)
            {
                InitializeMatch = true;
                if (!BloomConfig.TestBoolInput5.Value) return;

                for (int index = 0; index < 2; index++)
                {
                    for (int i = 0; i < 6; i++)
                    {
                        temp = SeedBankManager.GetPacketAt(i, index);
                        SeedType seedType = temp.PacketType;
                        PlantDefinition def = BoardManager.App.GetPlantDefinition(seedType);

                        if (def.m_seedType == SeedType.Peashooter ||
                        def.m_seedType == SeedType.Sunflower ||
                        def.m_seedType == SeedType.Wallnut ||
                        def.m_seedType == SeedType.Potatomine ||
                        def.m_seedType == SeedType.Puffshroom ||
                        def.m_seedType == SeedType.Sunshroom ||
                        def.m_seedType == SeedType.Puffshroom ||
                        def.m_seedType == SeedType.Cabbagepult ||
                        def.m_seedType == SeedType.Garlic ||
                        def.m_seedType == SeedType.ZombieGravestone ||
                        def.m_seedType == SeedType.ZombieNormal ||
                        def.m_seedType == SeedType.ZombieTrashCan)
                        {
                            
                            continue;
                        }
                        else if (
                            def.m_seedType == SeedType.Cherrybomb ||
                            def.m_seedType == SeedType.Iceshroom ||
                            def.m_seedType == SeedType.Doomshroom ||
                            def.m_seedType == SeedType.Jalapeno ||
                            def.m_seedType == SeedType.Gatlingpea ||
                            def.m_seedType == SeedType.Twinsunflower ||
                            def.m_seedType == SeedType.Wintermelon)
                        {
                            temp.mActive = true;
                            temp.mActive = false;
                            temp.mRefreshTime = 3500;
                            temp.mRefreshing = true;
                        }
                        else if (def.m_seedType == SeedType.Squash)
                        {
                            temp.mActive = true;
                            temp.mActive = false;
                            temp.mRefreshTime = 2000;
                            temp.mRefreshing = true;
                        }
                        else
                        {
                            temp.mActive = true;
                            temp.mActive = false;
                            temp.mRefreshTime = def.m_versusBaseRefreshTime;
                            temp.mRefreshing = true;

                        }
                    }
                }
            }
        }



        // Plays huge wave sound when it's Sudden Death
        if (BoardManager.App != null && BoardManager.App.VersusMode != null)
        {
            if (BoardManager.App.VersusMode.Phase == VersusPhase.SuddenDeath)
            {
                if (!hasPlayedSuddenDeath)
                {
                    hasPlayedSuddenDeath = true;

                    var activity = UnityEngine.Object.FindObjectOfType<GameplayActivity>();
                    if (activity != null)
                    {
                        activity.m_audioService.PlaySample(Constants.Sound.SOUND_HUGE_WAVE, false);
                    }
                }
            }
        }

        // Plays death sounds when game ends
        if (BoardManager.App != null && BoardManager.App.VersusMode != null)
        {
            if (BoardManager.App.VersusMode.m_winInitialized)
            {
                if (!hasPlayedEnd)
                {
                    hasPlayedEnd = true;

                    var activity = UnityEngine.Object.FindObjectOfType<GameplayActivity>();
                    if (activity != null)
                    {
                        random = UnityEngine.Random.Range(1, 5);
                        activity.m_audioService.PlayMusic(MusicTune.None, 0, 0);
                        if (random == 1) { 
                            activity.m_audioService.PlaySample(Constants.Sound.SOUND_RIP_LOSE);
                            activity.m_audioService.PlaySample(Constants.Sound.SOUND_RIP_LOSE);
                        }
                        else if (random == 2)
                        {
                            activity.m_audioService.PlaySample(Constants.Sound.SOUND_RIP_PLANT_KILL_1);
                            activity.m_audioService.PlaySample(Constants.Sound.SOUND_RIP_PLANT_KILL_1);
                        }
                        else if (random == 3)
                        {
                            activity.m_audioService.PlaySample(Constants.Sound.SOUND_RIP_PLANT_KILL_2);
                            activity.m_audioService.PlaySample(Constants.Sound.SOUND_RIP_PLANT_KILL_2);
                        }
                        else if (random == 4)
                        {
                            activity.m_audioService.PlaySample(Constants.Sound.SOUND_RIP_PLANT_KILL_3);
                            activity.m_audioService.PlaySample(Constants.Sound.SOUND_RIP_PLANT_KILL_3);
                        }
                    }
                }
            }
        }


        // Toggle: Instant Death phase
        if (TenMinute && BloomConfig.TestBoolInput3.Value)
        {
            if (!hasPlayedTen)
            {
                hasPlayedTen = true;
                // Play explosion sound
                var activity = UnityEngine.Object.FindObjectOfType<GameplayActivity>();
                if (activity != null)
                {
                    activity.m_audioService.PlaySample(Constants.Sound.SOUND_FINALWAVE, false);
                }
                BoardManager.ShakeBoard(5, 5);

                // Spawn Peashooters at the back
                for (int i = 0; i < 5; i++)
                {
                    if (PlantManager.CanPlantAt(0, i, SeedType.Peashooter) == PlantingReason.Ok)
                    {
                        PlantManager.Spawn(0, i, SeedType.Peashooter);
                    }
                }

                // Destroy all lawnmowers
                DieRow(0);
                DieRow(1);
                DieRow(2);
                DieRow(3);
                DieRow(4);

                // Show INSTANT DEATH huge wave message
                AdviceManager.ShowAdvice("INSTANT DEATH!", MessageStyle.HugeWave, AdviceType.None);
                AdviceManager.SetDuration(500);
            }

        }



        // Toggle: Chaotic Death (???) phase
        if (StartMatch && BloomConfig.TestBoolInput8.Value)
        {
            if (!hasChaos)
            {
                hasChaos = true;
 
                // Destroy 2 random lawnmowers
                int random = UnityEngine.Random.Range(0, 5);
                int random2 = UnityEngine.Random.Range(0, 5);
                while (random == random2)
                {
                    random2 = UnityEngine.Random.Range(0, 5);
                }
                DieRow(random);
                DieRow(random2);

                // Set units to 9990 cost
                BoardManager.AddSun(9990, 0);
                BoardManager.AddSun(9990, 1);


                AdviceManager.ShowAdvice("CHAOTIC DEATH!", MessageStyle.HugeWave, AdviceType.None);
                AdviceManager.SetDuration(500);
                BoardManager.ShakeBoard(10, 10);

                var activity = UnityEngine.Object.FindObjectOfType<GameplayActivity>();
                if (activity != null)
                {
                    activity.m_audioService.PlaySample(Constants.Sound.SOUND_EXPLOSION, false);
                    activity.m_audioService.PlaySample(Constants.Sound.SOUND_SCREAM, false);
                }
                if (!hasDisplayedChaosMessage)
                {
                    hasDisplayedChaosMessage = true;
                    MelonLoader.MelonCoroutines.Start(WaitForChaosMessage());
                }
                MelonLoader.MelonCoroutines.Start(SpawnDoomShroom());
            }
        }


        // Shovel keybind
        if (Input.GetKeyDown(KeyCode.X)){
            if (BoardManager.Board.mApp.IsVersusMode()){
                SeedType player0Cursor = CursorManager.GetSeedTypeInCursor(1);
                // if Player 1 is zombie, then yes
                if (60 <= (int)player0Cursor && (int)player0Cursor <= 80)
                {
                    // Force drop the seed packet
                    int heldIndex = CursorManager.GetSeedBankIndex(0);

                    if (heldIndex >= 0)
                    {
                        var seedBank = BoardManager.Board.mSeedBank;
                        if (seedBank != null)
                        {
                            seedBank.mSeedPackets[heldIndex].mActive = true;
                        }
                    }
                    CursorManager.ClearCursor(false, 0);
                    CursorManager.SetCursorType(CursorType.Shovel, 0);
                    

                    var activity = UnityEngine.Object.FindObjectOfType<GameplayActivity>();
                    if (activity != null)
                    {
                        activity.m_audioService.PlaySample(Constants.Sound.SOUND_SHOVEL);

                    }
                }
            }
            
        }


    }

    public override void OnSceneWasInitialized(int buildIndex, string sceneName)
    {
        base.OnSceneWasInitialized(buildIndex, sceneName);
        hasPlayedSuddenDeath = false;
        hasPlayedEnd = false;
        hasPlayedTen = false;
        TenMinute = false;
        StartMatch = false;
        hasChaos = false;

        activateonce = false;

        InitializeMatch = false;

        key = false;

    }

    // Toggle: Seed recharge tooltip
    // RaptorRush
    private static void GameplayStarted(GameplayActivity gameplayActivity)
    {
        foreach (var (@override, definition) in SeedPacketToolTipApi.ToolTipsOverrides
            .Where(t => t.Definition.SeedType != SeedType.Imitater))
        {
            int refreshTime = gameplayActivity.IsVersusMode()
                ? definition.m_versusBaseRefreshTime
                : definition.m_refreshTime;
            if (!BloomConfig.TestBoolInput.Value) break;
            if (gameplayActivity.IsVersusMode() && BloomConfig.TestBoolInput.Value)
            {
                if (definition.m_seedType == SeedType.ZombieImp)
                {
                    @override.Set(
                    $"<color=#00ff00>Recharge: {refreshTime / 100.0} seconds<color=#000000>\nImps are tiny zombies that can sprint to your house.");
                }
                else
                {
                    @override.Set(
                        $"<color=#00ff00>Recharge: {refreshTime / 100.0} seconds<color=#000000>\n" +
                        $"{@override.OriginalFormat}");
                }
            }
            else
            {
                @override.Set(
                            $"<color=#00ff00>Recharge: {refreshTime / 100.0} seconds<color=#000000>\n" +
                            $"{@override.OriginalFormat}");
            }

        }
    }
    public override void OnSceneWasLoaded(int buildIndex, string sceneName)
    {
        base.OnSceneWasLoaded(buildIndex, sceneName);

        // Toggle: Better Sudden Death
        var allDefs = UnityEngine.Object.FindObjectsOfTypeAll(Il2CppInterop.Runtime.Il2CppType.Of<PlantDefinition>());
        foreach (var obj in allDefs)
        {
            if (!BloomConfig.TestBoolInput10.Value) break;
            var def = obj.Cast<PlantDefinition>();

            def.m_versusSuddenDeathRefreshTime = def.m_versusBaseRefreshTime;

        }

        /*
         * Not related to Vs Mode
        foreach (var obj in allDefs){
            var def = obj.Cast<PlantDefinition>();
            if (def.m_seedType == SeedType.Wallnut){
                //def.m_previewSpriteOffset = new Vector2(35.1f, -20f); //-20
            }
            if (def.m_seedType == SeedType.Repeater){
                //def.m_previewSpriteOffset = new Vector2(20f, -57f);
            }
            if (def.m_seedType == SeedType.Gatlingpea)
            {
                //def.m_previewSpriteOffset = new Vector2(18.5f, -27.2f);
            }
            if (def.m_seedType == SeedType.ZombieFlag){
                //def.m_previewSpriteScale = 1.8f; 
            
            }
        }
        */
        
        // Add health to Wall-nut and Tall-nut Zombie in Versus Mode
        var allDefsZ = UnityEngine.Object.FindObjectsOfTypeAll(Il2CppInterop.Runtime.Il2CppType.Of<ZombieDefinition>());
        foreach (var obj in allDefsZ)
        {
            var def = obj.Cast<ZombieDefinition>();
            if (def.m_zombieType == ZombieType.WallnutHead)
            {
                def.m_versusBodyHealth = 270;
                def.m_versusArmorHealth = 1100;
            }
            if (def.m_zombieType == ZombieType.TallnutHead)
            {
                def.m_versusBodyHealth = 270;
                def.m_versusArmorHealth = 2400;
            }


        }

    }


    // Reload the Seed chooser screen
    // Units with m_versusBaseRefreshTime = 0 will not be displayed
    // D1GQ
    
    [HarmonyPatch(typeof(SeedChooserDataModel), nameof(SeedChooserDataModel.UpdateEntries))]
    internal static class SeedChooserDataModel_UpdateEntries_Patch
    {
        [HarmonyPrefix]
        private static bool Prefix(SeedChooserDataModel __instance)
        {
            if (__instance.m_seedChooserScreen.mApp.IsVersusMode())
            {
                //Clear data models
                __instance.m_entriesModel.Clear();


                for (int i = 0; i < __instance.m_seedChooserScreen.mChosenSeeds.Count; i++)
                {
                    var plantChosenSeed = __instance.m_seedChooserScreen.mChosenSeeds[i];
                    if (plantChosenSeed.mSeedType == SeedType.None) continue;

                    PlantDefinition plantDefinition = __instance.m_seedChooserScreen.mApp.GetPlantDefinition(plantChosenSeed.mSeedType);

                    if (plantDefinition == null || plantDefinition.m_versusBaseRefreshTime == 0) continue;

                    SeedChooserEntryModel plantEntry = new SeedChooserEntryModel(plantDefinition, plantChosenSeed, __instance.m_seedChooserScreen, __instance, false, i);
                    __instance.m_entriesModel.Add(i.ToString(), plantEntry);
                }


                return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(SeedChooserDataModel), nameof(SeedChooserDataModel.UpdateZombieEntries))]
    internal static class SeedChooserDataModel_UpdateEntries_PatchZombie
    {
        [HarmonyPrefix]
        private static bool Prefix(SeedChooserDataModel __instance)
        {
            if (__instance.m_seedChooserScreen.mApp.IsVersusMode())
            {
                //Clear data models

                __instance.m_zombieEntriesModel.Clear();

                // Make Gravestone on top of seed list
                for (int i = 0; i < __instance.m_seedChooserScreen.mChosenZombies.Count; i++)
                {
                    var zombieChosenSeed = __instance.m_seedChooserScreen.mChosenZombies[i];
                    if (zombieChosenSeed.mSeedType == SeedType.None) continue;
                    if (zombieChosenSeed.mSeedType == SeedType.ZombieGravestone)
                    {
                        PlantDefinition zombieDefinition = __instance.m_seedChooserScreen.mApp.GetPlantDefinition(zombieChosenSeed.mSeedType);
                        if (zombieDefinition == null || zombieDefinition.m_versusBaseRefreshTime == 0) continue;

                        SeedChooserEntryModel zombieEntry = new SeedChooserEntryModel(zombieDefinition, zombieChosenSeed, __instance.m_seedChooserScreen, __instance, false, i);
                        __instance.m_zombieEntriesModel.Add(i.ToString(), zombieEntry);
                        break;
                    }


                }
                for (int i = 0; i < __instance.m_seedChooserScreen.mChosenZombies.Count; i++)
                {
                    var zombieChosenSeed = __instance.m_seedChooserScreen.mChosenZombies[i];
                    if (zombieChosenSeed.mSeedType == SeedType.None) continue;
                    if (zombieChosenSeed.mSeedType == SeedType.ZombieGravestone) continue;

                    PlantDefinition zombieDefinition = __instance.m_seedChooserScreen.mApp.GetPlantDefinition(zombieChosenSeed.mSeedType);
                    if (zombieDefinition == null || zombieDefinition.m_versusBaseRefreshTime == 0) continue;

                    SeedChooserEntryModel zombieEntry = new SeedChooserEntryModel(zombieDefinition, zombieChosenSeed, __instance.m_seedChooserScreen, __instance, false, i);
                    __instance.m_zombieEntriesModel.Add(i.ToString(), zombieEntry);
                }
                return false;
            }

            return true;
        }

    }
    
    /*
    [HarmonyPatch(typeof(SeedChooserDataModel), nameof(SeedChooserDataModel.UpdateEntries))]
    internal static class SeedChooserDataModel_UpdateEntries_Patch
    {
        [HarmonyPostfix]
        private static void Postfix(SeedChooserDataModel __instance)
        {
            if (!__instance.m_seedChooserScreen.mApp.IsVersusMode()) return;
            // Remove aquatic plants
            //__instance.m_entriesModel.RemoveModel("16");
            //__instance.m_entriesModel.RemoveModel("19");
            //__instance.m_entriesModel.RemoveModel("24");

            // Add Gatling Pea
            //ChosenSeed gatlingPea = __instance.m_seedChooserScreen.mChosenSeeds[40];

            // 2. Build entries manually up to Gatling Pea (40) and Twin Sunflower (41)

            __instance.m_entriesModel.Clear();
            for (int i = 0; i <= 48; i++)
            {
                if (i >= 42 && i <= 48) continue;

                var plantChosenSeed = __instance.m_seedChooserScreen.mChosenSeeds[i];

                SeedType seedType = (SeedType)i;
                PlantDefinition plantDef = __instance.m_seedChooserScreen.mApp.GetPlantDefinition(seedType);

                if (plantDef != null)
                {
                    SeedChooserEntryModel entryModel = new SeedChooserEntryModel(
                        plantDef,
                        plantChosenSeed,
                        __instance.m_seedChooserScreen,
                        __instance,
                        false, 
                        i
                    );

                    __instance.m_entriesModel.Add(i.ToString(), entryModel);

                }
            }

            return;

        }
    }
    */











    // Start the UI timers (Timer, Live Recharge timer, Concede timer)
    // Toggle: Timer
    [HarmonyPatch(typeof(Board), nameof(Board.StartLevel))]
    internal static class SpawnTimerPatch
    {
        [HarmonyPostfix]
        private static void Postfix(Board __instance)
        {
            if (__instance.mApp.IsVersusMode())
            {
                GameObject timerObj = new GameObject("Timer");
                timerObj.AddComponent<Timer>();

                if (BloomConfig.TestBoolInput2.Value)
                {
                    GameObject timerObj2 = new GameObject("LiveRecharge");
                    timerObj2.AddComponent<Timer2>();
                }

                GameObject timerObj3 = new GameObject("ConcedeTimer");
                timerObj3.AddComponent<Concede0>();
            }

        }
    }
}
