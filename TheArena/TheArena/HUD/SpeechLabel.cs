using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameUI.Components;
using TheArena.Conversations;

namespace TheArena.HUD
{
    public class SpeechLabel : Label
    {
        public Speech Speech { get; set; }

        public SpeechLabel()
        {
        }
    }
}
