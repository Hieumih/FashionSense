using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FashionSense.Framework.Utilities;
using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace FashionSense.Framework.Patches.Menus
{
    internal class AdvancedGameOptionsPatch : PatchTemplate
    {
        private readonly Type _menu = typeof(AdvancedGameOptions);
        internal AdvancedGameOptionsPatch(IMonitor modMonitor, IModHelper modHelper) : base(modMonitor, modHelper)
        {

        }

        internal void Apply(Harmony harmony) 
        {
            harmony.Patch(AccessTools.Method(_menu, nameof(AdvancedGameOptions.PopulateOptions), null), postfix: new HarmonyMethod(GetType(), nameof(PopulateOptions)));
        }
        
        private static void PopulateOptions(AdvancedGameOptions __instance)
        {
            __instance.AddCheckbox(
                label: _helper.Translation.Get("ui.fashion_sense.start_with_hand_mirror"),
                tooltip: FashionSense.modHelper.Translation.Get("ui.fashion_sense.start_with_hand_mirror.description"),
                get: delegate
                {
                    if (!Game1.player.modData.ContainsKey(ModDataKeys.STARTS_WITH_HAND_MIRROR))
                    {
                        return false;
                    }
                    return bool.Parse(Game1.player.modData[ModDataKeys.STARTS_WITH_HAND_MIRROR]);
                }, 
                set: delegate (bool value)
                {
                    if (!value)
                    {
                        Game1.player.modData.Remove(ModDataKeys.STARTS_WITH_HAND_MIRROR);
                        return;
                    }
                    Game1.player.modData.Add(ModDataKeys.STARTS_WITH_HAND_MIRROR, true.ToString());
                });
        }

    }
}
