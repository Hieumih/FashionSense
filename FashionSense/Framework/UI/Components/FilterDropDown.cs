using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.Menus;

namespace FashionSense.Framework.UI.Components
{
    internal class FilterDropDown : OptionsDropDown
    {
        //public bool IsClicked { get; set; }
        private Rectangle DropDownBoundsOpen;
        private Rectangle DropDownBoundsClose;

        public FilterDropDown(string label, int whichOption, int x = -1, int y = -1) : base(label, whichOption, x, y)
        {

        }

        public override void RecalculateBounds()
        {
            DropDownBoundsOpen = new Rectangle(bounds.X, bounds.Y, bounds.Width - 48, bounds.Height * dropDownOptions.Count);
            DropDownBoundsClose = new Rectangle(bounds.X, bounds.Y, bounds.Width - 48, bounds.Height);
            base.RecalculateBounds();
        }

        //public override void receiveKeyPress(Keys key)
        //{
        //    base.receiveKeyPress(key);
        //}

        //public override void receiveLeftClick(int x, int y)
        //{
        //    base.receiveLeftClick(x, y);
        //    //IsClicked = true;
        //}

        public bool IsBoundContain(int x, int y)
        {
            return dropDownOpen ? DropDownBoundsOpen.Contains(x, y) : DropDownBoundsClose.Contains(x, y);
        }

        //public override void leftClickHeld(int x, int y)
        //{
        //    base.leftClickHeld(x, y);
        //}

        //public override void leftClickReleased(int x, int y)
        //{
        //    if (!base.greyedOut && base.dropDownOptions.Count > 0)
        //    {
        //        base.leftClickReleased(x, y);

        //        if (Game1.options.gamepadControls && !Game1.lastCursorMotionWasMouse)
        //        {
        //            _ = base.selectedOption;
        //        }
        //        else
        //        {
        //            base.selectedOption = base.startingSelected;
        //        }
        //        OptionsDropDown.selected = null;
        //    }

        //    IsClicked = false;
        //}

        //public override void draw(SpriteBatch b, int slotX, int slotY)
        //{
        //    base.draw(b, slotX, slotY);
        //}
    }
}
