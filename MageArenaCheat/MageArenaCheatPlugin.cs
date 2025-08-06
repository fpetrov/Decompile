using System;
using System.Reflection;
using BepInEx;
using HarmonyLib;
using UnityEngine;

namespace MageArenaCheat
{
    [BepInPlugin("com.example.magearena.cheat", "MageArena Cheat", "1.0.0")]
    [BepInProcess("MageArena.exe")]
    public class MageArenaCheatPlugin : BaseUnityPlugin
    {
        private Harmony _harmony;
        private bool _showGui;

        public static bool NoCooldown = true;
        public static bool NumpadCasting = true;
        public static bool GodMode = true;

        private static readonly string[] CooldownFields = { "fbcd", "frostcd", "wormcd", "holecd", "wardcd" };

        private void Awake()
        {
            _harmony = new Harmony("com.example.magearena.cheat");
            var pmType = AccessTools.TypeByName("PlayerMovement");
            if (pmType != null)
            {
                var method = AccessTools.Method(pmType, "NonRpcDamagePlayer");
                var prefix = AccessTools.Method(typeof(MageArenaCheatPlugin), nameof(PreventDamagePrefix));
                _harmony.Patch(method, prefix: new HarmonyMethod(prefix));
            }
        }

        private void OnDestroy()
        {
            _harmony?.UnpatchSelf();
        }

        private static bool PreventDamagePrefix()
        {
            return !GodMode;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F6))
                _showGui = !_showGui;

            if (GodMode)
            {
                var player = FindLocalPlayer();
                if (player != null)
                {
                    var healthField = player.GetType().GetField("playerHealth", BindingFlags.NonPublic | BindingFlags.Instance);
                    if (healthField != null)
                        healthField.SetValue(player, 999999f);
                }
            }

            if (NoCooldown)
            {
                var inv = FindLocalInventory();
                if (inv != null)
                {
                    var invType = inv.GetType();
                    foreach (var fieldName in CooldownFields)
                    {
                        var fld = invType.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
                        if (fld != null)
                            fld.SetValue(inv, -9999f);
                    }
                    var eqField = invType.GetField("equippedItems", BindingFlags.NonPublic | BindingFlags.Instance);
                    if (eqField?.GetValue(inv) is GameObject[] items)
                    {
                        var pcType = AccessTools.TypeByName("PageController");
                        var timerField = pcType?.GetField("PageCoolDownTimer", BindingFlags.Public | BindingFlags.Instance);
                        if (pcType != null && timerField != null)
                        {
                            foreach (var item in items)
                            {
                                if (item == null) continue;
                                var pc = item.GetComponent(pcType);
                                if (pc != null)
                                    timerField.SetValue(pc, -9999f);
                            }
                        }
                    }
                }
            }

            if (NumpadCasting)
            {
                var inv = FindLocalInventory();
                if (inv != null)
                {
                    var t = inv.GetType();
                    if (Input.GetKeyDown(KeyCode.Keypad1))
                        t.GetMethod("cFireball", BindingFlags.Public | BindingFlags.Instance)?.Invoke(inv, null);
                    if (Input.GetKeyDown(KeyCode.Keypad2))
                        t.GetMethod("cFrostbolt", BindingFlags.Public | BindingFlags.Instance)?.Invoke(inv, null);
                    if (Input.GetKeyDown(KeyCode.Keypad3))
                        t.GetMethod("cCastworm", BindingFlags.Public | BindingFlags.Instance)?.Invoke(inv, null);
                    if (Input.GetKeyDown(KeyCode.Keypad4))
                        t.GetMethod("cCasthole", BindingFlags.Public | BindingFlags.Instance)?.Invoke(inv, null);
                    if (Input.GetKeyDown(KeyCode.Keypad5))
                        t.GetMethod("cCastWard", BindingFlags.Public | BindingFlags.Instance)?.Invoke(inv, null);
                }
            }
        }

        private static object? FindLocalInventory()
        {
            var type = AccessTools.TypeByName("PlayerInventory");
            return type != null ? UnityEngine.Object.FindObjectOfType(type) : null;
        }

        private static object? FindLocalPlayer()
        {
            var type = AccessTools.TypeByName("PlayerMovement");
            return type != null ? UnityEngine.Object.FindObjectOfType(type) : null;
        }

        private void OnGUI()
        {
            if (!_showGui) return;

            GUILayout.BeginArea(new Rect(20f, 20f, 200f, 150f), "MageArena Cheat", GUI.skin.window);
            NoCooldown = GUILayout.Toggle(NoCooldown, "No Cooldown");
            NumpadCasting = GUILayout.Toggle(NumpadCasting, "Numpad Casting");
            GodMode = GUILayout.Toggle(GodMode, "God Mode");
            GUILayout.EndArea();
        }
    }
}
