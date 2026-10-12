namespace VersusPlus.UI;
#pragma warning disable
using VersusPlus.Core;
using static VersusPlus.Core.Core;
using System;
using System.Collections.Generic;

using HarmonyLib;

using Il2CppReloaded;
using Il2CppReloaded.Data;
using Il2CppReloaded.DataModels;
using Il2CppReloaded.Gameplay;
using Il2CppReloaded.Services;
using Il2CppReloaded.TreeStateActivities;

using Il2CppSource.Controllers;
using Il2CppSource.DataModels;

using Il2CppSteamworks;

using Il2CppSystem.Numerics;

using Il2CppTekly.Localizations;

using Il2CppTMPro;
using Il2CppTMPro;

using MelonLoader;

using ReplantAPI;
using ReplantAPI.Managers;

using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

using static MelonLoader.MelonLogger;


using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

using VersusPlus.Gameplay;

internal class Timer : MonoBehaviour
{
    private TextMeshProUGUI timerText;
    public static float elapsedTime = 0;
    bool checkEnd = false;

    private float nextTriggerTime = 19f;
    private const float Interval = 19f; // Repeats every 19s
    int random;
    void Start()
    {

        Canvas customCanvas = gameObject.AddComponent<Canvas>();
        customCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        customCanvas.sortingOrder = 0;

        CanvasScaler scaler = gameObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080); // Your baseline design resolution
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f; // 0.5 scales evenly based on both width and height

        // Enum: Timer position
        if (BloomConfig.TestEnum.Value == TimerPos.BottomRight){
            GameObject targetContainer = GameObject.Find("Panels/P_Gameplay_MainHUD/Canvas/Layout/Center/BottomRight");
            GameObject textObj = new GameObject("Timer");
            textObj.transform.SetParent(targetContainer.transform, false);
            textObj.transform.SetAsLastSibling();

            timerText = textObj.AddComponent<TextMeshProUGUI>();
            timerText.color = Color.white;
            timerText.fontSize = 120;
            timerText.alignment = TextAlignmentOptions.Bottom;
            timerText.characterSpacing = 5f;

            var rect = timerText.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(300, 60);
            rect.localPosition = new Vector2(-200, 100);
        }
        else {
            GameObject seedContainerPlants = GameObject.Find("Panels/P_Gameplay_MainHUD/Canvas/Layout/Center");
            GameObject textObj = new GameObject("Timer");
            textObj.transform.SetParent(seedContainerPlants.transform, false);
            textObj.transform.SetAsLastSibling();

            timerText = textObj.AddComponent<TextMeshProUGUI>();
            timerText.color = Color.white;
            timerText.fontSize = 120;
            timerText.alignment = TextAlignmentOptions.Bottom;
            timerText.characterSpacing = 5f;

            var rect = timerText.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(300, 60);

            if (BloomConfig.TestEnum.Value == TimerPos.BottomMiddle)
            {
                rect.localPosition = new Vector2(0, -1030);
            }
            else if (BloomConfig.TestEnum.Value == TimerPos.TopMiddle)
            {
                rect.localPosition = new Vector2(0, 650);
            }
        }

        
        
        
        var loadedFonts = Resources.FindObjectsOfTypeAll(Il2CppInterop.Runtime.Il2CppType.Of<TMP_FontAsset>());

        foreach (var obj in loadedFonts)
        {
            var font = obj.Cast<TMP_FontAsset>();
            //MelonLogger.Msg($"Found font: {font.name}");
            if (font.name == "FNT_MonsterFonts-HouseofTerror SDF")
            {
                timerText.font = font;
                break;
            }
        }


         // Add border

         // Create the unique material instance
         timerText.fontMaterial = new Material(timerText.fontSharedMaterial);
         timerText.fontMaterial.EnableKeyword("UNDERLAY_ON");
         timerText.fontMaterial.SetFloat(ShaderUtilities.ID_UnderlayOffsetX, 0f);
         timerText.fontMaterial.SetFloat(ShaderUtilities.ID_UnderlayOffsetY, 0f);
         //  0.3f to 0.8f
         timerText.fontMaterial.SetFloat(ShaderUtilities.ID_UnderlayDilate, 0.8f);
         timerText.fontMaterial.SetFloat(ShaderUtilities.ID_UnderlaySoftness, 0f);
         timerText.fontMaterial.SetColor(ShaderUtilities.ID_UnderlayColor, Color.black);

         elapsedTime = 0;

    }

    void Update()
    {

        var board = BoardManager.Board;
        if (board == null) return;
        if (BoardManager.Board.m_startDrawTime == 0)
        {
            if (!checkEnd) elapsedTime += Time.deltaTime;
            Core.StartMatch = true;
            if (BloomConfig.TestBoolInput9.Value){
                if (elapsedTime >= nextTriggerTime)
                {
                    OnTimerMilestoneReached(nextTriggerTime);
                    nextTriggerTime += Interval;
                }
            }
            

            int minutes = Mathf.FloorToInt(elapsedTime / 60);
            int seconds = Mathf.FloorToInt(elapsedTime % 60);
            timerText.color = Color.white;
            if (!BloomConfig.TestBoolInput4.Value) { 
            // If disabled then just make the text invis (still need timer)
                timerText.color = new Color(1f, 1f, 1f, 0.0f);
                if (minutes >= 10)
                {
                    Core.TenMinute = true;
                }
            }
            else
            {   
                if ((minutes >= 5 && minutes < 10))
                {
                    timerText.color = Color.yellow;
                }
                if (minutes >= 10)
                {
                    Core.TenMinute = true;
                    timerText.color = Color.red;
                }
            }
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
            
            if (BoardManager.Board.mApp != null 
            && BoardManager.Board.mApp.VersusMode.m_winInitialized)
            {
                checkEnd = true;
            }
        }
    }

    // Randomly selecting events
    private void OnTimerMilestoneReached(float time)
    {
        random = UnityEngine.Random.Range(1, 101);
        if (BoardManager.Board.mApp.VersusMode.Phase != VersusPhase.SuddenDeath)
        {
            // 
            if (1 <= random && random <= 30) MelonLoader.MelonCoroutines.Start(Events.Event1()); //      30
            else if (30 < random && random <= 40) MelonLoader.MelonCoroutines.Start(Events.Event2()); // 10
            else if (40 < random && random <= 50) MelonLoader.MelonCoroutines.Start(Events.Event3()); // 10
            else if (50 < random && random <= 75)
            {
                MelonLoader.MelonCoroutines.Start(Events.Event4()); // 25
                MelonLoader.MelonCoroutines.Start(Events.Event42());
            }
            else if (75 < random && random <= 100)
            {
                MelonLoader.MelonCoroutines.Start(Events.Event5()); // 25
            }
        } else {
            if (1 <= random && random <= 40) MelonLoader.MelonCoroutines.Start(Events.Event1()); //40
            else if (40 < random && random <= 70)
            {
                MelonLoader.MelonCoroutines.Start(Events.Event4()); // 30
                MelonLoader.MelonCoroutines.Start(Events.Event42());
            }
            else if (70 < random && random <= 100)
            {
                MelonLoader.MelonCoroutines.Start(Events.Event5()); // 30
            }
        }
        
        
    }
}
