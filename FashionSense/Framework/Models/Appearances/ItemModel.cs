using FashionSense.Framework.Models.Appearances.Generic;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FashionSense.Framework.Models.Appearances
{
    public class ItemModel
    {
        public string Id { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }
        public float InventoryScale { get; set; } = 1f;
        public Position SpritePosition { get; set; }
        public Size SpriteSize { get; set; }

        internal bool IsValid()
        {
            return SpritePosition is not null && SpriteSize is not null;
        }

        internal Rectangle GetSpriteRectangle()
        {
            return new Rectangle(SpritePosition.X, SpritePosition.Y, SpriteSize.Width, SpriteSize.Length);
        }
    }
}
