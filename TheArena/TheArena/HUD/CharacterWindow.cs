using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameUI.Components;
using Microsoft.Xna.Framework;
using GameEngine.Extensions;
using Microsoft.Xna.Framework.Input;

namespace TheArena.HUD
{
    public class CharacterWindow : FrameComponent
    {
        public CharacterWindow() : base(new Rectangle(0, 0, 64, 64))
        {
            Construct();
        }

        public CharacterWindow(Rectangle position)
            : base(position)
        {
            Construct();
        }

        private void Construct()
        {
            SetAllPadding(4);
        }

        public override void Update(GameUI.ArenaUI hud, GameTime dt, GameUI.Input.InputState input)
        {
            base.Update(hud, dt, input);

            if (KeyboardExtensions.GetKeyDownState(input.CurrentKeyboardStates[0], Keys.C, this))
                this.Visible = !this.Visible;
        }
    }
}
