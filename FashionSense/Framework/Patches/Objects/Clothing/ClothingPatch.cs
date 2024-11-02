using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;

namespace FashionSense.Framework.Patches.Objects
{
    internal class ClothingPatch : PatchTemplate
    {
        internal ClothingPatch(IMonitor modMonitor, IModHelper modHelper) : base(modMonitor, modHelper)
        {

        }

        internal void Apply(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method(typeof(Clothing), nameof(Clothing.drawInMenu), new[] { typeof(SpriteBatch), typeof(Vector2), typeof(float), typeof(float), typeof(float), typeof(StackDrawType), typeof(Color), typeof(bool) }), prefix: new HarmonyMethod(GetType(), nameof(DrawInMenuPrefix)));
            harmony.Patch(AccessTools.Method(typeof(Boots), nameof(Boots.drawInMenu), new[] { typeof(SpriteBatch), typeof(Vector2), typeof(float), typeof(float), typeof(float), typeof(StackDrawType), typeof(Color), typeof(bool) }), prefix: new HarmonyMethod(GetType(), nameof(DrawInMenuPrefix)));
            harmony.Patch(AccessTools.Method(typeof(Hat), nameof(Hat.drawInMenu), new[] { typeof(SpriteBatch), typeof(Vector2), typeof(float), typeof(float), typeof(float), typeof(StackDrawType), typeof(Color), typeof(bool) }), prefix: new HarmonyMethod(GetType(), nameof(DrawInMenuPrefix)));
        }

        private static bool DrawInMenuPrefix(Object __instance, SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, StackDrawType drawStackNumber, Color color, bool drawShadow)
        {
            if (__instance is null)
            {
                return true;
            }

            var appearanceContentPack = FashionSense.textureManager.GetAppearanceContentPackByItemId(__instance.ItemId);
            if (appearanceContentPack is null)
            {
                return true;
            }

            var item = appearanceContentPack.Item;
            scaleSize *= item.InventoryScale;

            spriteBatch.Draw(appearanceContentPack.Texture, location + new Vector2(32f, 32f), item.GetSpriteRectangle(), color * transparency, 0f, new Vector2(item.SpriteSize.Width / 2, item.SpriteSize.Length / 2), 4f * scaleSize, SpriteEffects.None, layerDepth);
            return false;
        }
    }
}
