using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace BetterPlaying
{
    [BepInPlugin("com.xjpjeass.convstore.dialogue_hotkey", "Dialogue Hotkey", "1.0")]
    public class DialogueHotkey: BaseUnityPlugin
    {
        public static ConfigEntry<KeyboardShortcut> hotkeyConfig;

        void Start()
        {
            hotkeyConfig = Config.Bind<KeyboardShortcut>("Keyboard Shortcuts", "Dialogue Hotkey", new KeyboardShortcut(KeyCode.Return), "對話下一句 熱鍵");
            Harmony.CreateAndPatchAll(typeof(DialogueHotkey));
        }

        void Update()
        {
        }

        [HarmonyPostfix, HarmonyPatch(typeof(DramaEvent.WaitForInputEvent), "Play")]
        static IEnumerator Postfix(IEnumerator values, DramaEvent.WaitForInputEvent __instance, float ___autoTime)
        {
            float waitTime = Time.time + ___autoTime * (float)GameLoopManager.instance.om.optionData.GetAutoPlaySpeed() / 100f;
            __instance.IsRunning = true;
            while (!hotkeyConfig.Value.IsDown() && !Input.GetMouseButtonDown(0) && !StorySceneManager.fast && (!StorySceneManager.auto || Time.time <= waitTime) && !StorySceneManager.skip)
            {
                yield return null;
            }
            yield return new WaitForSeconds(0.01f);
            __instance.IsRunning = false;
            yield break;
        }
    }

    [BepInPlugin("com.xjpjeass.convstore.show_base_sale_value", "Show Base Sale Value", "1.0")]
    public class ShowBaseSaleValue : BaseUnityPlugin
    {
        public static ConfigEntry<bool> baseSaleValueConfig;

        void Start()
        {
            baseSaleValueConfig = Config.Bind<bool>("Switches", "Show Base Sale Value?", true, "是否秀出 基礎銷量？");
            Harmony.CreateAndPatchAll(typeof(ShowBaseSaleValue));
        }

        void Update()
        {
        }

        [HarmonyPostfix, HarmonyPatch(typeof(Prefab.RestockItemPrefab), "UpdateData")]
        static void Postfix(int id, PlayerDataManager pm, GameDataManager gm, Prefab.RestockItemPrefab __instance)
        {
            if (baseSaleValueConfig.Value)
            {
                __instance.NameText.fontSize = 45;
                __instance.NameText.text += " \n基礎銷量:" + Formula.CommoditySaleQuantity.BaseSaleValue(gm, id).ToString();
            }
            else
            {
                __instance.NameText.fontSize = 60;
            }
        }
    }
}
