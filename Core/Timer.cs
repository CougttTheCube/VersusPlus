namespace VersusPlus;
#pragma warning disable
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
using static VersusPlus.Core;

using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

internal class Timer : MonoBehaviour
{
    private TextMeshProUGUI timerText;
    float elapsedTime;
    bool checkEnd = false;

    private float nextTriggerTime = 19f;
    private const float Interval = 19f; // Repeats every 1 minute (1:30, 2:30, etc.)
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
        
        if (BloomConfig.TestEnum.Value == TimerPos.BottomMiddle){
            rect.localPosition = new Vector2(0, -1030);
        } else if (BloomConfig.TestEnum.Value == TimerPos.BottomRight)
        {
            rect.localPosition = new Vector2(1690, -985);
        } else if (BloomConfig.TestEnum.Value == TimerPos.TopMiddle)
        {
            rect.localPosition = new Vector2(0, 650);
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
        /*
        [20:03:08.500] Found font: LiberationSans SDF - Fallback
[20:03:08.501] Found font: LiberationSans SDF
[20:03:08.501] Found font: FNT_BrianneTod SDF
[20:03:08.501] Found font: FNT_MonsterFonts-HouseofTerror SDF
[20:03:08.501] Found font: NotoSansJP-Regular SDF
[20:03:08.501] Found font: NotoSansJP-Regular SDF_fallback
[20:03:08.501] Found font: NotoSansSC-Medium SDF
[20:03:08.502] Found font: NotoSansSC-Medium SDF_fallback
[20:03:08.502] Found font: NotoSansTC-Medium SDF
[20:03:08.502] Found font: NotoSansTC-Medium SDF_fallback
[20:03:08.502] Found font: LiberationSans SDF
[20:03:08.503] Found font: FNT_MonsterFonts-HouseofTerror SDF
[20:03:08.503] Found font: FNT_DwarvenTodcraft SDF
[20:03:08.503] Found font: FNT_BrianneTod SDF
[20:03:08.503] Found font: FNT_contb SDF
[20:03:08.504] Found font: NotoSansTC-Medium SDF_fallback
[20:03:08.504] Found font: NotoSansTC-Medium SDF
[20:03:08.505] Found font: NotoSansSC-Medium SDF
[20:03:08.507] Found font: NotoSansSC-Medium SDF_fallback
[20:03:08.508] Found font: NotoSansJP-Regular SDF
[20:03:08.508] Found font: NotoSansJP-Regular SDF_fallback
[20:03:08.508] Found font: pico12 SDF
[20:03:08.508] Found font: FNT_Anton-Regular SDF
[20:03:08.508] Found font: FNT_Knewave-Regular SDF
         */

         // Add border

         // Create the unique material instance
         timerText.fontMaterial = new Material(timerText.fontSharedMaterial);
         // Enable the Underlay feature instead of the standard Outline
         timerText.fontMaterial.EnableKeyword("UNDERLAY_ON");
         // Center the underlay perfectly behind the text (no offset)
         timerText.fontMaterial.SetFloat(ShaderUtilities.ID_UnderlayOffsetX, 0f);
         timerText.fontMaterial.SetFloat(ShaderUtilities.ID_UnderlayOffsetY, 0f);
         // Expand the underlay outward to create the outline thickness (try 0.3f to 0.8f)
         timerText.fontMaterial.SetFloat(ShaderUtilities.ID_UnderlayDilate, 0.8f);
         // Make the edges sharp instead of blurry
         timerText.fontMaterial.SetFloat(ShaderUtilities.ID_UnderlaySoftness, 0f);
         // Set the color to black
         timerText.fontMaterial.SetColor(ShaderUtilities.ID_UnderlayColor, Color.black);

        

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

                    // Advance to the next mark
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
                //if (minutes == 0 && seconds == 20) Core.TenMinute = true;
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
                //elapsedTime -= Time.deltaTime;
            }
        }
    }

    private void OnTimerMilestoneReached(float time)
    {
        // Your event code here (e.g., spawn special wave, grant sun/brains)
        MelonLogger.Msg($"Triggered milestone at {timerText.text}!");
        random = UnityEngine.Random.Range(1, 5);
        if (random == 1) MelonLoader.MelonCoroutines.Start(Events.Event1());
        else if (random == 2) MelonLoader.MelonCoroutines.Start(Events.Event2());
        else if (random == 3) MelonLoader.MelonCoroutines.Start(Events.Event3());
        else if (random == 4){ 
            MelonLoader.MelonCoroutines.Start(Events.Event4());
            MelonLoader.MelonCoroutines.Start(Events.Event42());
        }
    }
}
