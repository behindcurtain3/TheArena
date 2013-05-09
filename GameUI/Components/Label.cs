using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Reflection;
using System.ComponentModel;
using GameUI.Extensions;

namespace GameUI.Components
{
    public class Label : Component
    {
        public string Text { get; set; }
        public string Data { get; set; }

        private string _boundProperty = String.Empty;

        public Label()
        {
            SetAllPadding(0);
        }

        public override void Draw(SpriteBatch spriteBatch, Rectangle parent)
        {
            base.Draw(spriteBatch, parent);

            spriteBatch.DrawString(Font, Text, new Vector2(ContentPane.X + parent.X + 1, ContentPane.Y + parent.Y + 1), Color.Black);
            spriteBatch.DrawString(Font, Text, new Vector2(ContentPane.X + parent.X, ContentPane.Y + parent.Y), Color);            

            // Draw data, aligned to the right
            if (Data != null)
            {
                Vector2 dataPositon = new Vector2(ContentPane.X + parent.X + ContentPane.Width - Font.MeasureString(Data).X, ContentPane.Y + parent.Y);
                spriteBatch.DrawString(Font, Data, new Vector2(dataPositon.X + 1, dataPositon.Y + 1), Color.Black);
                spriteBatch.DrawString(Font, Data, dataPositon, Color);
            }
        }

        public void SetDataBinding(string property, INotifyPropertyChanged source)
        {
            source.PropertyChanged += new PropertyChangedEventHandler(binding_PropertyChanged);
            _boundProperty = property;

            // Update the data immediately
            binding_PropertyChanged(source, new PropertyChangedEventArgs(property));
        }

        private void binding_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(_boundProperty))
            {
                Data = Convert.ToString(sender.GetType().GetProperty(_boundProperty).GetValue(sender, null));
            }
        }

    }
}
