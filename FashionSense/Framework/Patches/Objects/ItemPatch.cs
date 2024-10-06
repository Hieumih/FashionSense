using FashionSense.Framework.Patches.Renderer;
using FashionSense.Framework.Utilities;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using xTile.Dimensions;

namespace FashionSense.Framework.Patches.Objects
{
    internal class ItemPatch : PatchTemplate
    {
        private readonly System.Type _entity = typeof(Item);

        internal ItemPatch(IMonitor modMonitor, IModHelper modHelper) : base(modMonitor, modHelper)
        {

        }

        internal void Apply(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method(_entity, nameof(Object.onEquip), new[] { typeof(Farmer) }), postfix: new HarmonyMethod(GetType(), nameof(OnEquipPostfix)));
            harmony.Patch(AccessTools.Method(_entity, nameof(Object.onUnequip), new[] { typeof(Farmer) }), postfix: new HarmonyMethod(GetType(), nameof(OnUnequipPostfix)));
        }

        private static void OnEquipPostfix(Object __instance, Farmer who)
        {
            if (__instance is null || who is null)
            {
                return;
            }

            var appearanceContentPack = FashionSense.textureManager.GetAppearanceContentPackByItemId(__instance.ItemId);
            if (appearanceContentPack is not null)
            {
                switch (appearanceContentPack.PackType)
                {
                    case Interfaces.API.IApi.Type.Shirt:
                        who.modData[ModDataKeys.CUSTOM_SHIRT_ID] = appearanceContentPack.Id;
                        break;
                    case Interfaces.API.IApi.Type.Pants:
                        who.modData[ModDataKeys.CUSTOM_PANTS_ID] = appearanceContentPack.Id;
                        break;
                    case Interfaces.API.IApi.Type.Hat:
                        who.modData[ModDataKeys.CUSTOM_HAT_ID] = appearanceContentPack.Id;
                        break;
                    case Interfaces.API.IApi.Type.Shoes:
                        who.modData[ModDataKeys.CUSTOM_SHOES_ID] = appearanceContentPack.Id;
                        break;
                }
            }
        }

        private static void OnUnequipPostfix(Object __instance, Farmer who)
        {
            if (__instance is null || who is null)
            {
                return;
            }

            var appearanceContentPack = FashionSense.textureManager.GetAppearanceContentPackByItemId(__instance.ItemId);
            if (appearanceContentPack is not null)
            {
                switch (appearanceContentPack.PackType)
                {
                    case Interfaces.API.IApi.Type.Shirt:
                        who.modData[ModDataKeys.CUSTOM_SHIRT_ID] = "None";
                        break;
                    case Interfaces.API.IApi.Type.Pants:
                        who.modData[ModDataKeys.CUSTOM_PANTS_ID] = "None";
                        break;
                    case Interfaces.API.IApi.Type.Hat:
                        who.modData[ModDataKeys.CUSTOM_HAT_ID] = "None";
                        break;
                    case Interfaces.API.IApi.Type.Shoes:
                        who.modData[ModDataKeys.CUSTOM_SHOES_ID] = "None";
                        break;
                }
            }
        }
    }
}
