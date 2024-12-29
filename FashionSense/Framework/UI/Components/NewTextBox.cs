using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley.Menus;

namespace FashionSense.Framework.UI.Components
{
    internal class NewTextBox : TextBox
    {
        public event TextBoxEvent onShowAndroidKeyBoard;

        public NewTextBox(Texture2D textBoxTexture, Texture2D caretTexture, SpriteFont font, Color textColor, bool drawBackground = true, bool centerText = false) : base(textBoxTexture, caretTexture, font, textColor, drawBackground, centerText)
        {
        }

        protected override void ShowAndroidKeyboard()
        {
            Task<string> task = KeyboardInput.Show("", "", Text);
            task.ContinueWith((Task<string> s) =>
            {
                Text = s.Result;
                if (onShowAndroidKeyBoard != null)
                {
                    onShowAndroidKeyBoard(this);
                }
            });
            Selected = false;

        }
    }
}
