namespace VersusPlus.UI;
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
using VersusPlus.Core;
using static VersusPlus.Core.Core;

using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

internal class Concede0 : MonoBehaviour
{
    private TextMeshProUGUI concedeText;
    private TextMeshProUGUI concedeText2;
    float elapsedTime;

    float elapsedTime2;
    void Start()
    {
        Canvas customCanvas = gameObject.AddComponent<Canvas>();
        customCanvas.renderMode = RenderMode.ScreenSpaceOverlay;



        customCanvas.sortingOrder = 0;

        CanvasScaler scaler = gameObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080); // Your baseline design resolution
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;

        // Find the exact container that holds the seed bank
        GameObject seedContainerPlants = GameObject.Find("Panels/P_Gameplay_MainHUD/Canvas/Layout/Center/TopLeftLayout/SeedBankContainer/SeedBank/SeedPacks_Layout");  
        GameObject seedContainerZombies = GameObject.Find("Panels/P_Gameplay_MainHUD/Canvas/Layout/Center/VersusBank/VersusBankContainer/P_VsZombiePacks_Layout");

        SeedType player1Cursor = CursorManager.GetSeedTypeInCursor(1);
        
        if (60 <= (int)player1Cursor && (int)player1Cursor <= 80) //player1 is zombie
        {
            if (seedContainerPlants != null)
                // player1 is zombie -> player0 is plant -> take seedcontainerplant
            {
                Transform slotTransform = seedContainerPlants.transform.GetChild(1);

                GameObject textObj = new GameObject("ConcedeText");
                textObj.transform.SetParent(slotTransform, false);
                textObj.transform.SetAsLastSibling();

                concedeText = textObj.AddComponent<TextMeshProUGUI>();
                concedeText.color = Color.white;
                concedeText.fontSize = 85;
                concedeText.alignment = TextAlignmentOptions.Center;
                concedeText.characterSpacing = 4f;

                RectTransform rect = concedeText.GetComponent<RectTransform>();
                //Set center which is the middle of the screen
                rect.anchorMin = new Vector2(1f, 1f);
                rect.anchorMax = new Vector2(1f, 1f);
                // Set center in the text
                rect.pivot = new Vector2(0.5f, 0.5f);
                // Move
                rect.sizeDelta = new Vector2(500, 50);
                rect.localPosition = new Vector2(0, -270);

                concedeText.raycastTarget = false;



            }

            if (seedContainerZombies != null)
            {
                Transform slotTransform = seedContainerZombies.transform.GetChild(1);

                GameObject textObj = new GameObject("ConcedeText2");
                textObj.transform.SetParent(slotTransform, false);
                textObj.transform.SetAsLastSibling();

                concedeText2 = textObj.AddComponent<TextMeshProUGUI>();
                concedeText2.color = Color.white;
                concedeText2.fontSize = 85;
                concedeText2.alignment = TextAlignmentOptions.Center;
                concedeText2.characterSpacing = 4f;

                RectTransform rect = concedeText2.GetComponent<RectTransform>();
                //Set center which is the middle of the screen
                rect.anchorMin = new Vector2(1f, 1f);
                rect.anchorMax = new Vector2(1f, 1f);
                // Set center in the text
                rect.pivot = new Vector2(0.5f, 0.5f);
                // Move
                rect.sizeDelta = new Vector2(500, 50);
                rect.localPosition = new Vector2(-120, -270);

                concedeText2.raycastTarget = false;



            }

        } else {
            
            if (seedContainerZombies != null){
                Transform slotTransform = seedContainerZombies.transform.GetChild(1);

                GameObject textObj = new GameObject("ConcedeText");
                textObj.transform.SetParent(slotTransform, false);
                textObj.transform.SetAsLastSibling();

                concedeText = textObj.AddComponent<TextMeshProUGUI>();
                concedeText.color = Color.white;
                concedeText.fontSize = 85;
                concedeText.alignment = TextAlignmentOptions.Center;
                concedeText.characterSpacing = 4f;

                RectTransform rect = concedeText.GetComponent<RectTransform>();
                //Set center which is the middle of the screen
                rect.anchorMin = new Vector2(1f, 1f);
                rect.anchorMax = new Vector2(1f, 1f);
                // Set center in the text
                rect.pivot = new Vector2(0.5f, 0.5f);
                // Move
                rect.sizeDelta = new Vector2(500, 50);
                rect.localPosition = new Vector2(-120, -270);

                concedeText.raycastTarget = false;


                
            }

            if (seedContainerPlants != null)
            // player1 is zombie -> player0 is plant -> take seedcontainerplant
            {
                Transform slotTransform = seedContainerPlants.transform.GetChild(1);

                GameObject textObj = new GameObject("ConcedeText2");
                textObj.transform.SetParent(slotTransform, false);
                textObj.transform.SetAsLastSibling();

                concedeText2 = textObj.AddComponent<TextMeshProUGUI>();
                concedeText2.color = Color.white;
                concedeText2.fontSize = 85;
                concedeText2.alignment = TextAlignmentOptions.Center;
                concedeText2.characterSpacing = 4f;

                RectTransform rect = concedeText2.GetComponent<RectTransform>();
                //Set center which is the middle of the screen
                rect.anchorMin = new Vector2(1f, 1f);
                rect.anchorMax = new Vector2(1f, 1f);
                // Set center in the text
                rect.pivot = new Vector2(0.5f, 0.5f);
                // Move
                rect.sizeDelta = new Vector2(500, 50);
                rect.localPosition = new Vector2(0, -270);

                concedeText2.raycastTarget = false;



            }
        }



        




        var loadedFonts = Resources.FindObjectsOfTypeAll(Il2CppInterop.Runtime.Il2CppType.Of<TMP_FontAsset>());

        foreach (var obj in loadedFonts)
        {
            var font = obj.Cast<TMP_FontAsset>();
            //MelonLogger.Msg($"Found font: {font.name}");
            if (font.name == "FNT_MonsterFonts-HouseofTerror SDF") // check actual names via logging
            {      
                concedeText.font = font;
                concedeText2.font = font;
                break;
            }
        }
        concedeText.fontMaterial = new Material(concedeText.fontSharedMaterial);
        concedeText.fontMaterial.EnableKeyword("UNDERLAY_ON");
        concedeText.fontMaterial.SetFloat(ShaderUtilities.ID_UnderlayOffsetX, 0f);
        concedeText.fontMaterial.SetFloat(ShaderUtilities.ID_UnderlayOffsetY, 0f);

        concedeText.fontMaterial.SetFloat(ShaderUtilities.ID_UnderlayDilate, 7f);
        concedeText.fontMaterial.SetFloat(ShaderUtilities.ID_UnderlaySoftness, 0f);
        concedeText.fontMaterial.SetColor(ShaderUtilities.ID_UnderlayColor, Color.black);


        concedeText2.fontMaterial = new Material(concedeText2.fontSharedMaterial);
        concedeText2.fontMaterial.EnableKeyword("UNDERLAY_ON");
        concedeText2.fontMaterial.SetFloat(ShaderUtilities.ID_UnderlayOffsetX, 0f);
        concedeText2.fontMaterial.SetFloat(ShaderUtilities.ID_UnderlayOffsetY, 0f);

        concedeText2.fontMaterial.SetFloat(ShaderUtilities.ID_UnderlayDilate, 7f);
        concedeText2.fontMaterial.SetFloat(ShaderUtilities.ID_UnderlaySoftness, 0f);
        concedeText2.fontMaterial.SetColor(ShaderUtilities.ID_UnderlayColor, Color.black);
    }


    void Update(){
        var board = BoardManager.Board;
        if (board == null) return;

        elapsedTime = 6.5f - Core.holdDuration;
        elapsedTime2 = 6.5f - Core.holdDuration2;
        float clampedTime = Mathf.Max(0f, elapsedTime);
        float clampedTime2 = Mathf.Max(0f, elapsedTime2);

        int seconds = Mathf.FloorToInt(clampedTime);
        int centiseconds = Mathf.FloorToInt((clampedTime * 100f) % 100f);

        int seconds2 = Mathf.FloorToInt(clampedTime2);
        int centiseconds2 = Mathf.FloorToInt((clampedTime2 * 100f) % 100f);

        concedeText.color = Color.white;
        concedeText.text = $"Conceding in: {seconds:D2}:{centiseconds:D2} seconds";
        concedeText2.color = Color.white;
        concedeText2.text = $"Conceding in: {seconds2:D2}:{centiseconds2:D2} seconds";



        if (elapsedTime >= 4f){
            // dont show the timer yet, but is countdown
            concedeText.color = new Color(1f, 1f, 1f, 0.0f);
        }
        else if (elapsedTime <= 4f && elapsedTime >= 0f)
        {
            // show the timer
            concedeText.color = Color.red;
        }

        // activate concede
        else { 
            // check who player0 is
            concedeText.color = new Color(1f, 1f, 1f, 0.0f);
        }

        


        if (elapsedTime2 >= 4f)
        {
            // dont show the timer yet, but is countdown
            concedeText2.color = new Color(1f, 1f, 1f, 0.0f);
        }
        else if (elapsedTime2 <= 4f && elapsedTime2 >= 0f)
        {
            // show the timer
            concedeText2.color = Color.red;
        }

        // activate concede
        else
        {
            // check who player0 is
            concedeText2.color = new Color(1f, 1f, 1f, 0.0f);
        }

    }
}
