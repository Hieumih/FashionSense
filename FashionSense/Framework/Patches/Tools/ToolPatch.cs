﻿using FashionSense.Framework.Models;
using FashionSense.Framework.Patches.Objects;
using FashionSense.Framework.UI;
using FashionSense.Framework.Utilities;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using System;

namespace FashionSense.Framework.Patches.Tools
{
    internal class ToolPatch : PatchTemplate
    {
        private readonly Type _object = typeof(Tool);

        internal ToolPatch(IMonitor modMonitor, IModHelper modHelper) : base(modMonitor, modHelper)
        {

        }

        internal void Apply(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method(_object, "get_DisplayName", null), postfix: new HarmonyMethod(GetType(), nameof(GetNamePostfix)));
            harmony.Patch(AccessTools.Method(_object, "get_description", null), postfix: new HarmonyMethod(GetType(), nameof(GetDescriptionPostfix)));
            harmony.Patch(AccessTools.Method(typeof(Item), nameof(Item.canBeTrashed), null), postfix: new HarmonyMethod(GetType(), nameof(CanBeTrashedPostfix)));

            harmony.Patch(AccessTools.Method(_object, nameof(Tool.drawInMenu), new[] { typeof(SpriteBatch), typeof(Vector2), typeof(float), typeof(float), typeof(float), typeof(StackDrawType), typeof(Color), typeof(bool) }), prefix: new HarmonyMethod(GetType(), nameof(DrawInMenuPrefix)));
            harmony.Patch(AccessTools.Method(_object, nameof(Tool.beginUsing), new[] { typeof(GameLocation), typeof(int), typeof(int), typeof(Farmer) }), prefix: new HarmonyMethod(GetType(), nameof(BeginUsingPrefix)));
        }

        private static void GetNamePostfix(Tool __instance, ref string __result)
        {
            if (__instance.modData.ContainsKey(ModDataKeys.HAND_MIRROR_FLAG))
            {
                __result = _helper.Translation.Get("tools.name.hand_mirror");
                return;
            }
        }

        private static void GetDescriptionPostfix(Tool __instance, ref string __result)
        {
            if (__instance.modData.ContainsKey(ModDataKeys.HAND_MIRROR_FLAG))
            {
                __result = _helper.Translation.Get("tools.description.hand_mirror");
                return;
            }
        }

        private static void CanBeTrashedPostfix(Tool __instance, ref bool __result)
        {
            if (__instance.modData.ContainsKey(ModDataKeys.HAND_MIRROR_FLAG))
            {
                __result = true;
                return;
            }
        }

        private static bool DrawInMenuPrefix(Tool __instance, SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, StackDrawType drawStackNumber, Color color, bool drawShadow)
        {
            if (__instance.modData.ContainsKey(ModDataKeys.HAND_MIRROR_FLAG))
            {
                spriteBatch.Draw(FashionSense.assetManager.GetHandMirrorTexture(), location + new Vector2(32f, 32f), new Rectangle(0, 0, 16, 16), color * transparency, 0f, new Vector2(8f, 8f), 4f * scaleSize, SpriteEffects.None, layerDepth);

                return false;
            }

            return true;
        }

        private static bool BeginUsingPrefix(Tool __instance, ref bool __result, GameLocation location, int x, int y, Farmer who)
        {
            if (__instance.modData.ContainsKey(ModDataKeys.HAND_MIRROR_FLAG) && who == Game1.player)
            {
                __result = true;
                __instance.lastUser = who;

                if (GetMannequin(location, x, y) is Mannequin mannequin)
                {
                    if (MannequinPatch.CanEquipFashionSenseAppearance(mannequin) is false)
                    {
                        Game1.addHUDMessage(new HUDMessage(_helper.Translation.Get("messages.warning.mannequin_occupied"), 3));
                        return CancelUsing(who);
                    }

                    Farmer mannequinFarmer = _helper.Reflection.GetMethod(mannequin, "GetFarmerForRendering").Invoke<Farmer>();
                    if (mannequinFarmer is null)
                    {
                        return CancelUsing(who);
                    }

                    if (Game1.GetKeyboardState().IsKeyDown(Keys.LeftShift) || Game1.GetKeyboardState().IsKeyDown(Keys.RightShift))
                    {
                        mannequin.modData.Remove(ModDataKeys.MANNEQUIN_OUTFIT_DATA);
                        FashionSense.outfitManager.ClearOutfit(mannequinFarmer);
                        _helper.Reflection.GetField<Farmer>(mannequin, "renderCache").SetValue(null);
                    }
                    else
                    {
                        AttemptCopyAppearanceToMannequin(mannequin, who);
                        MannequinPatch.CopyModDataFromMannequinToFarmer(mannequin, mannequinFarmer);
                    }

                    return CancelUsing(who);
                }

                return UseHandMirror(location, x, y, who);
            }

            return true;
        }

        private static Mannequin GetMannequin(GameLocation location, int x, int y)
        {
            Utility.clampToTile(new Vector2(x, y));
            int tileX = x / 64;
            int tileY = y / 64;
            Vector2 tileLocation = new Vector2(tileX, tileY);
            Vector2 altTileLocation = new Vector2(tileX, tileY + 1);

            if (location.objects.TryGetValue(tileLocation, out var obj) && obj is Mannequin mannequin)
            {
                return mannequin;
            }
            else if (location.objects.TryGetValue(altTileLocation, out var altObj) && altObj is Mannequin altMannequin)
            {
                return altMannequin;
            }

            return null;
        }

        private static void AttemptCopyAppearanceToMannequin(Mannequin mannequin, Farmer who)
        {
            var outfit = new Outfit(who, "Mannequin Outfit");

            // Exclude body appearances to prevent display issues
            outfit.BodyId = "None";

            mannequin.modData[ModDataKeys.MANNEQUIN_OUTFIT_DATA] = JsonConvert.SerializeObject(outfit);
        }

        private static bool UseHandMirror(GameLocation location, int x, int y, Farmer who)
        {
            if (FashionSense.textureManager.GetAllAppearanceModels().Count == 0)
            {
                Game1.addHUDMessage(new HUDMessage(_helper.Translation.Get("messages.warning.no_content_packs"), 3));
                return CancelUsing(who);
            }

            if (Game1.activeClickableMenu is null)
            {
                Game1.activeClickableMenu = new HandMirrorMenu();
            }

            return CancelUsing(who);
        }

        private static bool CancelUsing(Farmer who)
        {
            who.CanMove = true;
            who.UsingTool = false;
            return false;
        }
    }
}
