using FashionSense.Framework.Models;
using FashionSense.Framework.Utilities;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;

namespace FashionSense.Framework.Patches.Objects
{
    internal class MannequinPatch : PatchTemplate
    {
        private readonly System.Type _entity = typeof(Mannequin);

        internal MannequinPatch(IMonitor modMonitor, IModHelper modHelper) : base(modMonitor, modHelper)
        {

        }

        internal void Apply(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method(_entity, nameof(Mannequin.draw), new[] { typeof(SpriteBatch), typeof(int), typeof(int), typeof(float) }), prefix: new HarmonyMethod(GetType(), nameof(DrawPrefix)));
            harmony.Patch(AccessTools.Method(_entity, nameof(Mannequin.updateWhenCurrentLocation), new[] { typeof(GameTime) }), postfix: new HarmonyMethod(GetType(), nameof(UpdateWhenCurrentLocationPostfix)));
            harmony.Patch(AccessTools.Method(_entity, nameof(Mannequin.performObjectDropInAction), new[] { typeof(Item), typeof(bool), typeof(Farmer), typeof(bool) }), prefix: new HarmonyMethod(GetType(), nameof(PerformObjectDropInActionPrefix)));
            harmony.Patch(AccessTools.Method(_entity, nameof(Mannequin.checkForAction), new[] { typeof(Farmer), typeof(bool) }), prefix: new HarmonyMethod(GetType(), nameof(CheckForActionPrefix)));
            harmony.Patch(AccessTools.Method(_entity, nameof(Mannequin.checkForAction), new[] { typeof(Farmer), typeof(bool) }), postfix: new HarmonyMethod(GetType(), nameof(CheckForActionPostfix)));
            harmony.Patch(AccessTools.Method(_entity, "GetFarmerForRendering", null), postfix: new HarmonyMethod(GetType(), nameof(GetFarmerForRenderingPostfix)));
        }

        private static bool DrawPrefix(Mannequin __instance, SpriteBatch spriteBatch, int x, int y, float alpha = 1f)
        {
            if (__instance is null)
            {
                return true;
            }

            return true;
        }


        private static void UpdateWhenCurrentLocationPostfix(Mannequin __instance, Farmer ___renderCache, GameTime time)
        {
            if (FashionSense.modConfig.AllowMannequinAnimations is false)
            {
                return;
            }

            Farmer farmer = ___renderCache;
            if (farmer is null)
            {
                farmer = _helper.Reflection.GetMethod(__instance, "GetFarmerForRendering").Invoke<Farmer>();
            }

            if (farmer is null)
            {
                return;
            }

            FashionSense.UpdateElapsedDuration(farmer);
        }

        private static bool PerformObjectDropInActionPrefix(Mannequin __instance, ref bool __result, Item dropInItem, bool probe, Farmer who, bool returnFalseIfItemConsumed = false)
        {
            if (who is not null && who.CurrentTool is not null && who.CurrentTool.modData.ContainsKey(ModDataKeys.HAND_MIRROR_FLAG))
            {
                return false;
            }

            Farmer farmer = _helper.Reflection.GetMethod(__instance, "GetFarmerForRendering").Invoke<Farmer>();
            if (farmer is not null && AppearanceHelpers.GetCurrentlyEquippedModels(farmer, __instance.facing.Value).Count > 0)
            {
                Game1.addHUDMessage(new HUDMessage(_helper.Translation.Get("messages.warning.mannequin_using_fashion_sense"), 3));

                __result = false;
                return false;
            }

            return true;
        }

        private static bool CheckForActionPrefix(Mannequin __instance, ref bool __result, Farmer who, bool justCheckingForActivity = false)
        {
            if (justCheckingForActivity || who is null || who.CurrentTool is null || who.CurrentTool.modData.ContainsKey(ModDataKeys.HAND_MIRROR_FLAG) is false)
            {
                return true;
            }
            __result = false;

            // Copy appearance, if available
            Farmer farmer = _helper.Reflection.GetMethod(__instance, "GetFarmerForRendering").Invoke<Farmer>();
            if (farmer is not null && AppearanceHelpers.GetCurrentlyEquippedModels(farmer, __instance.facing.Value).Count > 0)
            {
                CopyModDataFromMannequinToFarmer(__instance, who);
                Game1.addHUDMessage(new HUDMessage(_helper.Translation.Get("messages.warning.mannequin_copied_appearance"), 2));
            }

            return false;
        }

        private static void CheckForActionPostfix(Mannequin __instance, Farmer who, bool justCheckingForActivity = false)
        {
            if (justCheckingForActivity)
            {
                return;
            }

            Farmer farmer = _helper.Reflection.GetMethod(__instance, "GetFarmerForRendering").Invoke<Farmer>();
            if (farmer is null)
            {
                return;
            }
        }

        private static void GetFarmerForRenderingPostfix(Mannequin __instance, Farmer __result)
        {
            if (__result is null)
            {
                return;
            }

            CopyModDataFromMannequinToFarmer(__instance, __result);
        }

        internal static void CopyModDataFromMannequinToFarmer(Mannequin mannequin, Farmer farmer)
        {
            if (mannequin.modData.ContainsKey(ModDataKeys.MANNEQUIN_OUTFIT_DATA) is false)
            {
                return;
            }

            Outfit outfit = JsonConvert.DeserializeObject<Outfit>(mannequin.modData[ModDataKeys.MANNEQUIN_OUTFIT_DATA]);
            if (farmer.modData.ContainsKey(ModDataKeys.CUSTOM_BODY_ID))
            {
                outfit.BodyId = farmer.modData[ModDataKeys.CUSTOM_BODY_ID];
            }

            FashionSense.outfitManager.SetOutfit(farmer, outfit);
        }

        internal static bool CanEquipFashionSenseAppearance(Mannequin mannequin)
        {
            if (mannequin.hat.Value == null && mannequin.shirt.Value == null && mannequin.pants.Value == null && mannequin.boots.Value == null)
            {
                return true;
            }

            return false;
        }
    }
}
