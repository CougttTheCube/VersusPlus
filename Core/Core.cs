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
using UnityEngine.AddressableAssets;
using UnityEngine.Rendering.RenderGraphModule.NativeRenderPassCompiler;

using static Il2CppSystem.Net.Http.Headers.Parser;

using static UnityEngine.ParticleSystem.PlaybackState;

// Todo:
// - Add Balloon, Imp, Blover (done)
// - Fix Balloon logic (done)
// - Port over the patches (done)

// - Detect the controllers image

// - Add Seed Tooltip (done) and toggle for it (done)
// - Add Recharge Timer (done) and toggle for it (done)
// - Add Instant Death (done) and toggle for it (done)
// - Add Timer (done) and toggle for it (done)

// - Fix Digger spawns at the back (done)
// - Enhance Random Mode
// - Fix initial recharge (done)
// - Fix timer layer and add font (done)
// - Fix Imp logic (done)

// - (maybe) Add custom zombie seed packet for the Zombotanies (Peashooter, Wall-nut, Tall-nut, Jalapeno, Squash, Gatling Pea)

// - Add Chaos Mode: all units are instantly recharged, 0 cost, but instas will have their recharge time increased for each time using

// - Known issues:
// + System.NullReferenceException for Timer at the end
// + 
// - Hidden issues:
// + idk, it's hidden lol

// 1.0 : initial release

// 1.0.1
// Fix Huge wave theme of Flag Zombie still plays outside of Versus Mode
// Fix Plantern, Marigold being banned
// Fix ??? mode enable on default

// 1.1
// Add Timer Position (done)
// Add Random Events
// Shockwave (done)
// Ambush (done)
// Crashing Economy (set sun/brain to 0)
// Flourishing Economy (add 750 sun/brain to both)
// Fix Instance bug in Timer.Update() (done)
public class Core : ReplantAPI
{
    public bool hasPlayedSuddenDeath = false;
    public bool hasPlayedEnd = false;
    public bool hasPlayedTen = false;
    public static bool TenMinute = false;
    public static bool StartMatch = false;
    public static bool hasChaos = false;

    public static bool hasDisplayedChaosMessage = false;

    public bool InitializeMatch = false;

    public const string Name = "VersusPlus";
    public const string Version = "1.1";
    public const string Author = "Cougtt and many more";

    SeedPacket temp;
    public enum TimerPos{
        TopMiddle,
        BottomMiddle,
        BottomRight
    }


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
        public static EnumConfigInput<TimerPos> TestEnum{ get; } = ConfigService.CreateEnum<TimerPos>(
            name: "Timer Position",
            defaultValue: TimerPos.BottomMiddle,
            description: "Changes the timer's position."
        );
        public static BoolConfigInput TestBoolInput5 { get; } = ConfigService.CreateBool(
            name: "Initial Recharge",
            defaultValue: true,
            description: "Sets initial recharge for almost all units."
        );
        public static BoolConfigInput TestBoolInput6 { get; } = ConfigService.CreateBool(
            name: "New Brain Rate Logic",
            defaultValue: true,
            description: "Sets Gravestone's brain rate to around 27 - 30 seconds and initial brain production. Disable this if you have custom brain rate logic (or just wants zombies to win)."
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
            "Instant Death, Seed Recharge Timer - inspired by PvZ DLC Mod\n" +
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
    }
    // Custom DieRow
    public static void DieRow(int row)
    {
        LawnMower inRow = LawnMowerManager.GetInRow(row);
        if (inRow == null)
        {
            MelonLogger.Warning($"[ReplantAPI] LawnMowerManager.TriggerRow: no mower found in row {row}.");
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
    public override void OnInitializeMelon()
    {
        this.LoggerInstance.Msg("Loading core...");
        GameplayActivityApi.ActiveStarted += GameplayStarted;

        ClassInjector.RegisterTypeInIl2Cpp<Timer>();
        ClassInjector.RegisterTypeInIl2Cpp<Timer2>();

        ModMenuService.CreateEntry(this)
            .AddDisplayName(Name)
            .AddDescription($"Gives a better experience in Versus Mode!")
            .AddConfigClass(typeof(BloomConfig))
            .Register();
    }
   
    

    public override void OnUpdate()
    {
        base.OnUpdate();
        if (!BoardManager.IsLoaded) return;

        if (BoardManager.Board.mApp != null && BoardManager.Board.mApp.VersusMode != null
            && BoardManager.Board.mApp.VersusMode.m_gameplayInitialized)
        {

            if (!InitializeMatch){
                InitializeMatch = true;
                if (!BloomConfig.TestBoolInput5.Value) return;

                for (int index = 0; index < 2; index++){
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
                        } else if(
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
                        activity.m_audioService.PlayMusic(MusicTune.None, 0, 0);
                        activity.m_audioService.PlaySample(Constants.Sound.SOUND_RIP_LOSE, false);

                    }
                }
            }
        }
        


        // Instant Death phase
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
                LoggerInstance.Msg("Lawnmowers left: " + LawnMowerManager.CountUntriggered());

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



        // Chaos Phase
        
        if (StartMatch && BloomConfig.TestBoolInput8.Value)
        {
            if (!hasChaos){
                hasChaos = true;

                //all units are instantly recharged, 0 cost, but instas will have their recharge time increased for each time using

                // Destroy 2 random lawnmowers
                int random = UnityEngine.Random.Range(0, 5);
                int random2 = UnityEngine.Random.Range(0, 5);
                while (random == random2){
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
                    activity.m_audioService.PlaySample(Constants.Sound.SOUND_SCREAM, false); //crezy dev scream
                }
                if (!hasDisplayedChaosMessage){
                    hasDisplayedChaosMessage = true;
                    MelonLoader.MelonCoroutines.Start(WaitForChaosMessage());
                }
                MelonLoader.MelonCoroutines.Start(SpawnDoomShroom());
                // Spawms 2 random Doom-shroom on each column for plant side (not counting col 0), 1 for zombie side(not counting col 8)



                // Every 15 seconds
                // Spawn a random plant

                // Spawn a random zombie


            }
        }



        // test
        if (Input.GetKeyDown(KeyCode.T)){
            MelonLoader.MelonCoroutines.Start(Events.Event1());
        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            MelonLoader.MelonCoroutines.Start(Events.Event4());
            MelonLoader.MelonCoroutines.Start(Events.Event42());
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

        InitializeMatch = false;

    }
   
    private static void GameplayStarted(GameplayActivity gameplayActivity)
    {
        foreach (var (@override, definition) in SeedPacketToolTipApi.ToolTipsOverrides
            .Where(t => t.Definition.SeedType != SeedType.Imitater))
        {
            int refreshTime = gameplayActivity.IsVersusMode()
                ? definition.m_versusBaseRefreshTime
                : definition.m_refreshTime;
            if (!BloomConfig.TestBoolInput.Value) break;
            if (gameplayActivity.IsVersusMode())
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
            } else {
                @override.Set(
                            $"<color=#00ff00>Recharge: {refreshTime / 100.0} seconds<color=#000000>\n" +
                            $"{@override.OriginalFormat}");
            }

        }
    }
    public override void OnSceneWasLoaded(int buildIndex, string sceneName)
	{
		base.OnSceneWasLoaded(buildIndex, sceneName);
        //LoggerInstance.Msg("Scene Loaded: " + sceneName);
        /*
        var allDefs = UnityEngine.Object.FindObjectsOfTypeAll(Il2CppInterop.Runtime.Il2CppType.Of<PlantDefinition>());
        PlantDefinition coneTemplate = null;
        PlantDefinition snorkelDef = null;

        // Pass 1: Find both definitions
        foreach (var obj in allDefs)
        {
            var def = obj.Cast<PlantDefinition>();
            if (def.m_seedType == (SeedType)61) // Conehead template
            {
                coneTemplate = def;
            }
            else if (def.m_seedType == SeedType.ZombiquariumSnorkel)
            {
                snorkelDef = def;
            }

            if (coneTemplate != null && snorkelDef != null) break;
        }

        // Pass 2: Apply the override
        if (snorkelDef != null)
        {
            snorkelDef.m_plantName = "Peashooter Zombie";
            snorkelDef.m_plantToolTip = "Shoots peas at the enemy.";
            snorkelDef.m_defaultSkin = "Normal";

            snorkelDef.m_seedCost = 100;
            snorkelDef.m_versusCost = 50;
            snorkelDef.m_refreshTime = 1500;
            snorkelDef.m_versusBaseRefreshTime = 1500;
            snorkelDef.m_versusSuddenDeathRefreshTime = 500;

            snorkelDef.m_packetIndex = 22;
            snorkelDef.m_subClass = PlantSubClass.Shooter; // or 0

            snorkelDef.m_entryStatus = ReloadedEntryStatus.Ready;


            // Borrow working Addressable references from Conehead so UI and preview don't break
            if (coneTemplate != null)
            {
                snorkelDef.m_plantImage = coneTemplate.m_plantImage;
                snorkelDef.m_versusImage = coneTemplate.m_versusImage;
                snorkelDef.m_previewSprite = coneTemplate.m_previewSprite;
                snorkelDef.m_previewSpriteOffset = coneTemplate.m_previewSpriteOffset;
                snorkelDef.m_previewSpriteScale = coneTemplate.m_previewSpriteScale;
                snorkelDef.m_animationType = coneTemplate.m_animationType;
            }
        }

        
        foreach (var obj in allDefs)
        {
			var def = obj.Cast<PlantDefinition>();
            
            //                                                                                                                 Upgraded plants
                                                                                                                         

            if (def.m_seedType == SeedType.ZombiquariumSnorkel)
            {
                def.m_plantToolTip = "Shoots peas at the enemy.";
                //def.m_animationType = ReanimationType.ZombiePeashooterHead;
                def.m_plantName = "Peashooter Zombie";
                def.m_defaultSkin = "Normal";
                //def.m_plantImage = AssetReferenceSprite.

                AssetReferenceSprite emptyImageRef = new("");
                AssetReferenceGameObject emptyGoRef = new("");
                def.m_prefab = emptyGoRef;
                def.m_preview = emptyGoRef;
                def.m_easterEggGameObject = emptyGoRef;
                def.m_preorderGameObject = emptyGoRef;
                def.m_chinaGameObject = emptyGoRef;
                def.m_chinaPlantImage = emptyImageRef;
                def.m_chinaPreviewSprite = emptyImageRef;
                def.m_decemberGameObject = emptyGoRef;
                def.m_seedCost = 50;    
                def.m_versusBaseRefreshTime = 1500;
                def.m_versusSuddenDeathRefreshTime = 500;


            }
            
            

        }
        

        var allDefsZ = UnityEngine.Object.FindObjectsOfTypeAll(Il2CppInterop.Runtime.Il2CppType.Of<ZombieDefinition>());
        foreach (var obj in allDefsZ){
            var def = obj.Cast<ZombieDefinition>();
            if (def.m_zombieType == ZombieType.Imp){
                
            }
        }
        */
        var allDefs = UnityEngine.Object.FindObjectsOfTypeAll(Il2CppInterop.Runtime.Il2CppType.Of<PlantDefinition>());
        foreach (var obj in allDefs)
        {
            var def = obj.Cast<PlantDefinition>();
            if (def.m_versusCost == 500 || def.m_seedCost == 500)
            {
                MelonLogger.Msg($"[MATCH] Name: {def.name} | SeedType: {def.m_seedType} ({(int)def.m_seedType}) | PlantName: {def.PlantName}");
            }
        }


    }



    // Patch: Allow everything in Versus Mode
    [HarmonyPatch(typeof(SeedChooserScreen), nameof(SeedChooserScreen.SeedNotAllowedToPick))]
    internal static class SeedChooserDataModelPatch
    {
        
        [HarmonyPrefix]
        private static bool Prefix4(SeedChooserScreen __instance, SeedType theSeedType, ref bool __result)
        {
            if (__instance.mApp.IsVersusMode() && 
            (theSeedType == SeedType.ZombieBalloon ||
            theSeedType == SeedType.ZombieImp ||
            theSeedType == SeedType.Gatlingpea ||
            theSeedType == SeedType.Twinsunflower ||
            theSeedType == SeedType.Wintermelon ||
            theSeedType == SeedType.Flowerpot ||
            theSeedType == SeedType.Blover ||
            theSeedType == SeedType.Plantern ||
            theSeedType == SeedType.Marigold
            ))
            {
                __result = false;
                return false;
            }

            return true;
        }
        
    }








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
                
                // Make Gravestone on top
                for (int i = 0; i < __instance.m_seedChooserScreen.mChosenZombies.Count; i++)
                {
                    var zombieChosenSeed = __instance.m_seedChooserScreen.mChosenZombies[i];
                    if (zombieChosenSeed.mSeedType == SeedType.None) continue;
                    if (zombieChosenSeed.mSeedType == SeedType.ZombieGravestone){
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
    [HarmonyPatch(typeof(Zombie), nameof(Zombie.StopEating))]
    internal static class ImpSpeedFix
    {
        [HarmonyPostfix]
        private static void Prefix0(Zombie __instance)
        {

            if (__instance.mZombieType != ZombieType.Imp) return;

            if (!__instance.mApp.IsVersusMode()) return;


            //__instance.mZombiePhase = ZombiePhase.ZombieNormal;
            __instance.mVelX = BalloonSkipRiseFromGrave.speed;
            __instance.UpdateAnimSpeed();
        }
    }

    [HarmonyPatch(typeof(Zombie), nameof(Zombie.RiseFromGrave))]
    internal static class BalloonSkipRiseFromGrave
    {
        public static float speed;
        [HarmonyPrefix]
        private static bool Prefix0(Zombie __instance)
        {
            // Skip rise from grave entirely for balloon
            if (__instance.mZombieType != ZombieType.Imp) return true;
            if (!__instance.mApp.IsVersusMode()) return true;


            //__instance.mZombiePhase = ZombiePhase.ZombieNormal;
            speed = __instance.mVelX * 2f;
            __instance.mVelX  = speed;
            __instance.UpdateAnimSpeed();

            return true; // skip original method

        }

        [HarmonyPrefix]
        private static bool Prefix(Zombie __instance)
        {
            // Skip rise from grave entirely for balloon
            if (__instance.mZombieType != ZombieType.Balloon) return true;
            if (!__instance.mApp.IsVersusMode()) return true;

            __instance.mPosX = 800f; // Push it off-screen immediately
            __instance.mZombiePhase = ZombiePhase.BalloonFlying;


            return false; // skip original method
            
        }
        [HarmonyPrefix]
        private static bool Prefix2(Zombie __instance)
        {
            // Skip rise from grave entirely for balloon
            if (__instance.mZombieType != ZombieType.Digger) return true;
            if (!__instance.mApp.IsVersusMode()) return true;


            __instance.mPosX = 800f; // Push it off-screen immediately
            __instance.mZombiePhase = ZombiePhase.DiggerTunneling;


            return false; // skip original method



        }
        [HarmonyPrefix]
        private static bool Prefix3(Zombie __instance)
        {
            // Skip rise from grave entirely for the Zombotanies
            if (__instance.mZombieType != ZombieType.PeaHead) return true;
            if (__instance.mZombieType != ZombieType.WallnutHead) return true;
            if (__instance.mZombieType != ZombieType.SquashHead) return true;
            if (__instance.mZombieType != ZombieType.TallnutHead) return true;
            if (__instance.mZombieType != ZombieType.GatlingHead) return true;
            if (__instance.mZombieType != ZombieType.JalapenoHead) return true;
            if (!__instance.mApp.IsVersusMode()) return true;


            __instance.mPosX = 800f; // Push it off-screen immediately
            __instance.mZombiePhase = ZombiePhase.ZombieNormal;

            return false; // skip original method


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

                
        
            [HarmonyPrefix]
            private static bool Prefix(Board __instance, ref ZombieType theZombieType, int theRow)
            {
                if (!__instance.mApp.IsVersusMode()) return true;

                SeedType zombieCursor = CursorManager.GetSeedTypeInCursor(1);
                SeedType plantCursor = CursorManager.GetSeedTypeInCursor(0);
                // Check if holding seed packet
                bool holdingZombieSeed = ((int)zombieCursor >= 60 && (int)zombieCursor <= 80) ||
                         ((int)plantCursor >= 60 && (int)plantCursor <= 80);

            if ((int)zombieCursor == 65 ||
            (int)zombieCursor == 68 ||
            (int)zombieCursor == 70 ||
            (int)zombieCursor == 72 ||
            (int)zombieCursor == 73 ||
            (int)zombieCursor == 75 ||
            (int)zombieCursor == 78 ||
            (int)zombieCursor == 79) holdingZombieSeed = false;

            if ((int)plantCursor == 65 ||
            (int)plantCursor == 68 ||
            (int)plantCursor == 70 ||
            (int)plantCursor == 72 ||
            (int)plantCursor == 73 ||
            (int)plantCursor == 75 ||
            (int)plantCursor == 78 ||
            (int)plantCursor == 79) holdingZombieSeed = false;


            if (holdingZombieSeed)
                {

                int targetCol = __instance.PixelToGridX(
                    __instance.mApp.m_widgetManager.LastMouseX[0],
                    __instance.mApp.m_widgetManager.LastMouseY[0]
                );
                int targetCol2 = __instance.PixelToGridX(
                    __instance.mApp.m_widgetManager.LastMouseX[1],
                    __instance.mApp.m_widgetManager.LastMouseY[1]   
                );

                if (GridItemManager.GetAt(GridItemType.Crater, targetCol, theRow) != null ||
                GridItemManager.GetAt(GridItemType.Crater, targetCol2, theRow) != null)
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
        




        // Plays huge wave sound when Flag Zombie is spawned
        [HarmonyPrefix]
        private static bool Prefix2(Board __instance, ZombieType theZombieType)
        {
            if (!__instance.mApp.IsVersusMode()) return true;
            if (theZombieType == ZombieType.Flag){
                var activity = UnityEngine.Object.FindObjectOfType<GameplayActivity>();
                if (activity != null)
                {
                    activity.m_audioService.PlaySample(Constants.Sound.SOUND_HUGE_WAVE);

                }
            }

            return true;
        }


      
    }



    [HarmonyPatch(typeof(Board), nameof(Board.StartLevel))]
    internal static class SpawnTimerPatch
    {
        [HarmonyPostfix]
        private static void Postfix(Board __instance)
        {
            // Only spawn the timer if it's a Versus match
            if (__instance.mApp.IsVersusMode())
            {
                // Create an empty object and attach your script. 
                // The Start() method will automatically handle the text and layers.
                GameObject timerObj = new GameObject("Timer");
                timerObj.AddComponent<Timer>();

                if (BloomConfig.TestBoolInput2.Value){
                    GameObject timerObj2 = new GameObject("LiveRecharge");
                    timerObj2.AddComponent<Timer2>();
                }
                

            }
            
        }
    }



    /*
    [HarmonyPatch(typeof(Board), nameof(Board.AddPlant))]
    internal static class AddPlantPatch
    {
        [HarmonyPostfix]
        private static void Postfix(Board __instance, SeedType theSeedType)
        {
            // Only spawn the timer if it's a Versus match
            if (!__instance.mApp.IsVersusMode()) return;
            if (!BloomConfig.TestBoolInput8.Value) return;
            if (theSeedType != SeedType.Sunflower) return;
            for (int i = 0; i < 6; i++){
                SeedPacket temp = SeedBankManager.GetPacketAt(i, 0);
                if (temp != null && temp.PacketType == SeedType.Sunflower){
                    temp.mRefreshTime += 1000;
                    break;
                }
            }


        }
    }
    */
    private static System.Collections.IEnumerator WaitForChaosMessage()
    {
        // Wait exactly 1.5 seconds
        yield return new WaitForSecondsRealtime(5f);
        AdviceManager.ShowAdvice("So, you have chosen death...", MessageStyle.TutorialLevel1, AdviceType.None);
        AdviceManager.SetDuration(500);
    }





    

    

    private static System.Collections.IEnumerator SpawnDoomShroom()
    {
        int random, random2;
        for (int i = 1; i < 8; i++){

            random = UnityEngine.Random.Range(0, 5);
            random2 = UnityEngine.Random.Range(0, 5);
            while (random == random2)
            {
                random2 = UnityEngine.Random.Range(0, 5);
            }

            if (i <= 5){
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

            } else {
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


    [HarmonyPatch(typeof(Board), nameof(Board.CanPlantAt))]
    internal static class ZombieNotPlantingonGraves
    {
        [HarmonyPrefix]
        private static bool Prefix(int theGridX, int theGridY, SeedType theType, ref PlantingReason __result)
        {
            if (( (int)theType >= 60 && (int)theType <= 80 ) && 
            GridItemManager.GetAt(GridItemType.Crater, theGridX, theGridY) != null)
            {
                if ((int)theType == 65 ||
                (int)theType == 68 ||
                (int)theType == 70 ||
                (int)theType == 72 ||
                (int)theType == 73 ||
                (int)theType == 75 ||
                (int)theType == 78 ||
                (int)theType == 79) return true;
                __result = PlantingReason.NotOnCrater;
                return false;
            }

            return true;
        }
    }
    
    
    
}
