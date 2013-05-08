using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System.Xml;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GameUI.Components
{
    /// <summary>
    /// The base "window" class that will display a UI window.
    /// This class inherits from Component but also contains a list of components
    /// that belong to it.
    /// </summary>
    public class BaseWindow : FrameComponent
    {
        private List<Component> _components;
        private bool _followMouse = false;
        private Vector2 _mousePosition;

        /// <summary>
        /// A window also contains a window which represent the "title" bar
        /// that displays at the top of the window
        /// </summary>
        public TitleBar TitleBar { get; set; }

        public BaseWindow() : base(new Rectangle(0, 0, 64, 64))
        {
            Construct();
        }

        public BaseWindow(Rectangle position) : base(position)
        {
            Construct();
        }

        private void Construct()
        {
            _components = new List<Component>();
        }

        public override bool HandleInput(Input.InputState input)
        {
            // Is the mouse over this window?
            bool inputHandled = (TitleBar == null) ? false : TitleBar.HandleInput(input);

            if (!inputHandled)
            {
                foreach (Component child in _components)
                {
                    inputHandled = child.HandleInput(input);
                    if (inputHandled)
                        break;
                }
            }

            if (!inputHandled)
                inputHandled = base.HandleInput(input);

            return inputHandled;
        }

        public override void Draw(SpriteBatch spriteBatch, Rectangle parent)
        {
            if (!Visible)
                return;

            base.Draw(spriteBatch, parent);

            if (TitleBar != null)
                TitleBar.Draw(spriteBatch, parent);

            // Sort any child components
            _components.Sort(delegate(Component a, Component b) { return a.Layer.CompareTo(b.Layer); });

            // Draw any child components
            foreach (Component child in _components)
                child.Draw(spriteBatch, ContentPane);

        }

        

        public static BaseWindow LoadWindowFromXML(ContentManager content, string file)
        {
            BaseWindow window = new BaseWindow();
            XmlDocument document = new XmlDocument();
            document.Load(file);

            XmlNode windowNode = document.SelectSingleNode("Window");
            XmlNode titleNode = document.SelectSingleNode("Window/Title");
            XmlNode contentPane = document.SelectSingleNode("Window/Content");
            XmlNode components = document.SelectSingleNode("Window/Components");
            XmlNode frame;
            string[] frames;

            string[] positions = windowNode.Attributes["Position"].Value.Split(',');
            Rectangle position = new Rectangle(
                                        Convert.ToInt32(positions[0]),
                                        Convert.ToInt32(positions[1]),
                                        Convert.ToInt32(positions[2]),
                                        Convert.ToInt32(positions[3])
                                    );

            // Setup the title bar
            if (titleNode != null)
            {
                XmlNode titleHeight = titleNode.SelectSingleNode("Height");
                int height = Convert.ToInt32(titleHeight.InnerText);

                window.TitleBar = new TitleBar(new Rectangle(position.X, position.Y, position.Width, height));
                window.TitleBar.Texture = content.Load<Texture2D>(titleNode.Attributes["Src"].Value);
                window.TitleBar.Font = content.Load<SpriteFont>(titleNode.Attributes["Font"].Value);
                window.TitleBar.Text = titleNode.SelectSingleNode("Text").InnerText;

                frame = titleNode.SelectSingleNode("Frame");
                frames = frame.InnerText.Split(',');

                window.TitleBar.Frame = new WindowFrame(
                                                Convert.ToInt32(frames[0]),
                                                Convert.ToInt32(frames[1]),
                                                Convert.ToInt32(frames[2]),
                                                Convert.ToInt32(frames[3])
                                            );

                window.Position = new Rectangle(position.X, position.Y + height, position.Width, position.Height - height);
                window.TitleBar.onMouseClick += new OnMouseClickEventHandler(window.TitleBar_onMouseClick);
                window.TitleBar.onDrag += new OnDragEventHandler(window.TitleBar_onDrag);
                window.TitleBar.onDragEnd += new OnDragEndEventHandler(window.TitleBar_onDragEnd);
            }
            else
            {
                window.Position = position;
            }

            window.Texture = content.Load<Texture2D>(contentPane.Attributes["Src"].Value);
            window.Font = content.Load<SpriteFont>(contentPane.Attributes["Font"].Value);

            frame = contentPane.SelectSingleNode("Frame");
            frames = frame.InnerText.Split(',');

            window.Frame = new WindowFrame(
                                        Convert.ToInt32(frames[0]),
                                        Convert.ToInt32(frames[1]),
                                        Convert.ToInt32(frames[2]),
                                        Convert.ToInt32(frames[3])
                                    );

            if (contentPane.Attributes["Padding"] == null)
                window.SetAllPadding(4);
            else
            {
                string[] paddings = contentPane.Attributes["Padding"].Value.Split(',');

                if (paddings.Length == 1)
                {
                    window.SetAllPadding(Convert.ToInt32(paddings[0]));
                }
                else if (paddings.Length == 2)
                {
                    window.PaddingTop = Convert.ToInt32(paddings[0]);
                    window.PaddingBottom = Convert.ToInt32(paddings[0]);
                    window.PaddingLeft = Convert.ToInt32(paddings[1]);
                    window.PaddingRight = Convert.ToInt32(paddings[1]);
                }
                else if (paddings.Length == 4)
                {
                    window.PaddingTop = Convert.ToInt32(paddings[0]);
                    window.PaddingBottom = Convert.ToInt32(paddings[1]);
                    window.PaddingLeft = Convert.ToInt32(paddings[2]);
                    window.PaddingRight = Convert.ToInt32(paddings[3]);
                }
            }

            return window;
        }

        private void TitleBar_onMouseClick(object sender, EventArgs e)
        {
            _followMouse = true;

            MouseState mouse = (MouseState)sender;
            _mousePosition = new Vector2(mouse.X - TitleBar.Position.X, mouse.Y - TitleBar.Position.Y);
        }

        private void TitleBar_onDragEnd(object sender, EventArgs e)
        {
            _followMouse = false;
        }

        private void TitleBar_onDrag(object sender, EventArgs e)
        {
            MouseState mouse = (MouseState)sender;

            int x = (int)(mouse.X - _mousePosition.X);
            int y = (int)(mouse.Y - _mousePosition.Y);

            TitleBar.Position = new Rectangle(x, y, TitleBar.Position.Width, TitleBar.Position.Height);
            Position = new Rectangle(x, y + TitleBar.Position.Height, Position.Width, Position.Height);
        }

    }
}
