using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FashionSense.Framework
{
    public class ModConfig
    {
        public bool RequireHandMirrorInInventory { get; set; } = true;
        public SButton QuickMenuKey { get; set; }
        public bool AllowMannequinAnimations { get; set; } = true;
    }
}
