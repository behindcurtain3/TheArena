using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameUI.Components;
using TheArena.Conversations;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace TheArena.HUD
{
    public class Conversation : FrameComponent
    {
        public Speech Speech { get; set; }

        private ContentManager _content;

        public Conversation()
        {
            Construct();
        }

        private void Construct()
        {
            FrameTop = 4;
            SetAllPadding(4);
        }

        public void LoadSpeech(string speech, ContentManager content)
        {
            LoadSpeech(Speech.LoadSpeechFromXml(speech), content);
        }

        public void LoadSpeech(Speech speech, ContentManager content)
        {
            if(speech == null)
                return;

            this.Speech = speech;
            _content = content;

            // Clear any children (get rid of the labels)
            Children.Clear();

            if (speech.GetChildren().Count > 0)
            {
                foreach (Speech child in Speech.GetChildren())
                {
                    SpeechLabel lbl = new SpeechLabel();
                    lbl.Speech = child;
                    lbl.Name = "ConversationPrompt";
                    lbl.SetAllPadding(0);
                    lbl.Font = content.Load<SpriteFont>("Fonts/DefaultBold");
                    lbl.Text = child.Prompt;
                    lbl.Position = GetPromptPosition(lbl.Font, child.Prompt);
                    lbl.onMouseOver += new OnMouseOverEventHandler(delegate(object sender)
                    {
                        lbl.Color = Color.Yellow;
                    });

                    lbl.onMouseOut += new OnMouseOutEventHandler(delegate(object sender)
                    {
                        lbl.Color = Color.White;
                    });

                    lbl.onMouseClick += new OnMouseClickEventHandler(delegate(object sender)
                    {
                        if (lbl.Speech.PromptLink.Equals("Child"))
                            this.LoadSpeech(lbl.Speech, _content);
                        else if (lbl.Speech.PromptLink.Equals("Exit"))
                            this.Visible = false;
                        else if (lbl.Speech.PromptLink.Contains(".speech"))
                            this.LoadSpeech(lbl.Speech.PromptLink, _content);
                        else
                            this.LoadSpeech(lbl.Speech.FindNodeFromRoot(child.PromptLink), _content);
                    });

                    Children.Add(lbl);
                }
            }
            else
            {
                this.onMouseClick += new OnMouseClickEventHandler(delegate(object sender){ this.Visible = false; });
            }
        }

        public override void Draw(SpriteBatch spriteBatch, Rectangle parent, GameTime gameTime)
        {
            if (!Visible || Speech == null)
                return;

            // Draw the base frame
            base.Draw(spriteBatch, parent, gameTime);

            float x = parent.X + ContentPane.X;
            float y = parent.Y + ContentPane.Y;

            // Draw the Speech         
            // TODO: Make sure the text wraps
            spriteBatch.DrawString(Font, Speech.Text, new Vector2(x, y), Color);
        }

        private Rectangle GetPromptPosition(SpriteFont font, string text)
        {
            Vector2 vText = font.MeasureString(text);

            return new Rectangle(35, 35 + (Children.Count * (int)vText.Y) + (Children.Count * (int)(vText.Y / 2)), (int)vText.X, (int)vText.Y);
        }
    }
}
