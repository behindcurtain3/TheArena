using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace GameUI.Extensions
{
    public class StringExtensions
    {
        public static List<string> WrapText(string text, int width, int height, SpriteFont font)
        {
            List<string> processedText = new List<string>();
            StringBuilder builder = new StringBuilder();

            string[] words = text.Split(' ');

            float currTextWidth = 0;

            float lineSpaceHeight = font.MeasureString("\r\n").Y;
            float whiteSpaceWidth = font.MeasureString(" ").X;

            for (int i = 0; i < words.Length; i++)
            {
                Vector2 wordMetrics = font.MeasureString(words[i]);

                if (currTextWidth + wordMetrics.X + whiteSpaceWidth < width)
                {
                    string word = words[i];
                    bool reset = false;
                    if (word.Contains("\r\n"))
                    {
                        reset = true;
                        word = word.Remove(word.IndexOf("\r\n"));
                    }
   
                    builder.Append(word.Trim());
                    builder.Append(' ');

                    if (reset)
                    {
                        processedText.Add(builder.ToString());

                        currTextWidth = 0;
                        builder.Clear();
                    }
                    else
                        currTextWidth += wordMetrics.X + whiteSpaceWidth;
                }
                else
                {
                    processedText.Add(builder.ToString());

                    currTextWidth = 0;
                    builder.Clear();
                    i = i - 1;
                }
            }

            if (builder.Length > 0) processedText.Add(builder.ToString());

            return processedText;
        }
    }
}
