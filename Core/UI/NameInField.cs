namespace VersusPlus.UI;
#pragma warning disable
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using HarmonyLib;

using Il2CppReloaded;
using Il2CppReloaded.Data;
using Il2CppReloaded.DataModels;
using Il2CppReloaded.Gameplay;
using Il2CppReloaded.Input;
using Il2CppReloaded.Services;
using Il2CppReloaded.TreeStateActivities;

using Il2CppSource.Controllers;
using Il2CppSource.DataModels;

using Il2CppSteamworks;

using Il2CppSystem;
using Il2CppSystem.Numerics;

using Il2CppTekly.Localizations;
using Il2CppTMPro;

using Il2CppTMPro;

using MelonLoader;

using ReplantAPI;
using ReplantAPI.Managers;

using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

using static MelonLoader.MelonLogger;
using VersusPlus.Core;
using static VersusPlus.Core.Core;

using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;
using UnityEngine.UIElements;

internal class VersusNameInField1 : MonoBehaviour
{
    Transform cursorSprite;
    RectTransform rect;
    Camera cam;
    void Start(){
        Canvas customCanvas = gameObject.AddComponent<Canvas>();
        customCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        customCanvas.sortingOrder = 0;

        CanvasScaler scaler = gameObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080); // Your baseline design resolution
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f; // 0.5 scales evenly based on both width and height


        //GameObject versusCursorTwo = GameObject.Find("Cursor/VersusVisibilityLayer");

        GameObject versusCursorTwo = GameObject.Find("Panels/P_Gameplay_MainHUD/Canvas/Layout/Center");
        Transform slotTransform = versusCursorTwo.transform;

        GameObject textObj = new GameObject("VersusName1");
        //textObj.layer = versusCursorTwo.layer;
        textObj.transform.SetParent(slotTransform, false);
        textObj.transform.SetAsLastSibling();

        TextMeshProUGUI VersusText2 = textObj.AddComponent<TextMeshProUGUI>();


        VersusText2.text = BloomConfig.Player1Name.Value;
        VersusText2.color = new UnityEngine.Color(0.9804f, 0.9294f ,0.0392f , 1f);
        VersusText2.fontSize = 80;
        VersusText2.alignment = TextAlignmentOptions.Center;
        VersusText2.characterSpacing = 1f;

        rect = VersusText2.GetComponent<RectTransform>();
        //Set center which is the middle of the screen
        rect.anchorMin = new Vector2(1f, 1f);
        rect.anchorMax = new Vector2(1f, 1f);
        // Set center in the text
        rect.pivot = new Vector2(0.5f, 0.5f);
        // Move
        rect.localPosition = new Vector2(0, 0);
        rect.sizeDelta = new Vector2(1000f, 50f);
        
        VersusText2.raycastTarget = false;


        var loadedFonts = Resources.FindObjectsOfTypeAll(Il2CppInterop.Runtime.Il2CppType.Of<TMP_FontAsset>());

        foreach (var obj in loadedFonts)
        {
            var font = obj.Cast<TMP_FontAsset>();
            //MelonLogger.Msg($"Found font: {font.name}");
            if (font.name == "FNT_MonsterFonts-HouseofTerror SDF") // check actual names via logging
            {
                VersusText2.font = font;
            }
        }
        //VersusText2.outlineColor = Color.black;
        //VersusText2.outlineWidth = 0.25f;
        VersusText2.fontMaterial = new Material(VersusText2.fontSharedMaterial);
        VersusText2.fontMaterial.EnableKeyword("UNDERLAY_ON");
        VersusText2.fontMaterial.SetFloat(ShaderUtilities.ID_UnderlayOffsetX, 0f);
        VersusText2.fontMaterial.SetFloat(ShaderUtilities.ID_UnderlayOffsetY, 0f);
        //  0.3f to 0.8f
        VersusText2.fontMaterial.SetFloat(ShaderUtilities.ID_UnderlayDilate, 0.55f);
        VersusText2.fontMaterial.SetFloat(ShaderUtilities.ID_UnderlaySoftness, 0f);
        VersusText2.fontMaterial.SetColor(ShaderUtilities.ID_UnderlayColor, Color.black);

        cursorSprite = GameObject.Find("Cursor/TreeOfWisdomVisibilityLayer/VersusVisibilityLayer/Sprite").transform;
        cam = Camera.main;
    }



    void LateUpdate()
    {
        if (cursorSprite == null || rect == null) { 
        // follow mouse
            rect.localPosition = new Vector2(11110f, 111110f);

            return; 
        }
        // follow cursor controller
        Vector3 local = rect.parent.InverseTransformPoint(cursorSprite.position);
        local.z = 0f;
        rect.localPosition = local + new Vector3(0f, 65f, 0f);  
    }
}


internal class VersusNameInField2 : MonoBehaviour
{
    Transform cursorSprite;
    RectTransform rect;
    Camera cam;
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


        //GameObject versusCursorTwo = GameObject.Find("Cursor/VersusVisibilityLayer");

        GameObject versusCursorTwo = GameObject.Find("Panels/P_Gameplay_MainHUD/Canvas/Layout/Center");
        Transform slotTransform = versusCursorTwo.transform;

        GameObject textObj = new GameObject("VersusName2");
        //textObj.layer = versusCursorTwo.layer;
        textObj.transform.SetParent(slotTransform, false);
        textObj.transform.SetAsLastSibling();

        TextMeshProUGUI VersusText2 = textObj.AddComponent<TextMeshProUGUI>();


        VersusText2.text = BloomConfig.Player2Name.Value;
        VersusText2.color = new UnityEngine.Color(0.2784f, 0.7765f, 1f, 1f);
        VersusText2.fontSize = 80;
        VersusText2.alignment = TextAlignmentOptions.Center;
        VersusText2.characterSpacing = 1f;

        rect = VersusText2.GetComponent<RectTransform>();
        //Set center which is the middle of the screen
        rect.anchorMin = new Vector2(1f, 1f);
        rect.anchorMax = new Vector2(1f, 1f);
        // Set center in the text
        rect.pivot = new Vector2(0.5f, 0.5f);
        // Move
        rect.localPosition = new Vector2(0, 0);
        rect.sizeDelta = new Vector2(1000f, 50f);

        VersusText2.raycastTarget = false;


        var loadedFonts = Resources.FindObjectsOfTypeAll(Il2CppInterop.Runtime.Il2CppType.Of<TMP_FontAsset>());

        foreach (var obj in loadedFonts)
        {
            var font = obj.Cast<TMP_FontAsset>();
            //MelonLogger.Msg($"Found font: {font.name}");
            if (font.name == "FNT_MonsterFonts-HouseofTerror SDF") // check actual names via logging
            {
                VersusText2.font = font;
            }
        }
        //VersusText2.outlineColor = Color.black;
        //VersusText2.outlineWidth = 0.25f;
        VersusText2.fontMaterial = new Material(VersusText2.fontSharedMaterial);
        VersusText2.fontMaterial.EnableKeyword("UNDERLAY_ON");
        VersusText2.fontMaterial.SetFloat(ShaderUtilities.ID_UnderlayOffsetX, 0f);
        VersusText2.fontMaterial.SetFloat(ShaderUtilities.ID_UnderlayOffsetY, 0f);
        //  0.3f to 0.8f
        VersusText2.fontMaterial.SetFloat(ShaderUtilities.ID_UnderlayDilate, 0.55f);
        VersusText2.fontMaterial.SetFloat(ShaderUtilities.ID_UnderlaySoftness, 0f);
        VersusText2.fontMaterial.SetColor(ShaderUtilities.ID_UnderlayColor, Color.black);

        cursorSprite = GameObject.Find("Cursor/VersusVisibilityLayer/Sprite").transform;
        cam = Camera.main;
    }




    void LateUpdate()
    {
        if (cursorSprite == null || rect == null) return;

        Vector3 local = rect.parent.InverseTransformPoint(cursorSprite.position);
        local.z = 0f;
        rect.localPosition = local + new Vector3(0f, 65f, 0f);
    }
}
