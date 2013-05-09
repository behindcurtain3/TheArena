﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Xml;
using Microsoft.Xna.Framework.Content;
using System.IO;
using GameUI.Extensions;
using System.Reflection;

namespace GameUI.Components
{
    public class Component : MouseListener
    {
        #region Properties

        public List<Component> Children { get; set; }

        public Component Parent { get; set; }

        public bool Visible { get; set; }

        public string Name { get; set; }

        public Rectangle ContentPane { get; private set; }

        public Color Color { get; set; }

        public SpriteFont Font { get; set; }

        public Texture2D Texture { get; set; }

        public int Layer { get; set; }

        public int PaddingTop { get; set; }
        public int PaddingLeft { get; set; }
        public int PaddingRight { get; set; }
        public int PaddingBottom { get; set; }

        public bool UseParentContentPane { get; set; }

        #endregion

        public Component()
        {
            Construct(Rectangle.Empty);
        }

        public Component(Rectangle position)
        {
            Construct(position);
        }

        private void Construct(Rectangle position)
        {
            Visible = true;
            Position = position;
            Color = Color.White;
            Children = new List<Component>();
            Parent = null;
            UseParentContentPane = true;

            PaddingTop = PaddingBottom = PaddingLeft = PaddingRight = 4;

            ResetContentPane();

            base.onPositionChanged += new OnPositionChangedEventHandler(Component_onPositionChanged);
        }

        public override bool HandleInput(Input.InputState input, Rectangle parent)
        {
            bool inputHandled = false;
            
            foreach (Component child in Children)
            {
                inputHandled = child.HandleInput(input, ContentPane);
                if (inputHandled)
                    break;
            }

            // If none of the children handled the input then this component should try
            if (!inputHandled)
                inputHandled = base.HandleInput(input, parent);

            return inputHandled;
        }

        public virtual void Draw(SpriteBatch spriteBatch, Rectangle parent)
        {
            if (!Visible)
                return;

            // Sort any child components
            Children.Sort(delegate(Component a, Component b) { return a.Layer.CompareTo(b.Layer); });

            // Draw any child components
            foreach (Component child in Children)
            {
                if (child.UseParentContentPane)
                    child.Draw(spriteBatch, ContentPane);
                else
                    child.Draw(spriteBatch, Position);
            }
        }

        /// <summary>
        /// Set the padding for all four sides of the component
        /// </summary>
        /// <param name="padding">Number of pixels to pad each side of the content pane from the component border.</param>
        public void SetAllPadding(int padding)
        {
            PaddingTop = PaddingBottom = PaddingLeft = PaddingRight = padding;

            ResetContentPane();
        }

        private void ResetContentPane()
        {
            ContentPane = new Rectangle(
                                Position.X + PaddingLeft,
                                Position.Top + PaddingTop,
                                Position.Width - (PaddingRight + PaddingLeft),
                                Position.Height - (PaddingBottom + PaddingTop)
                            );
        }

        private void Component_onPositionChanged(Rectangle position)
        {
            ResetContentPane();
        }

        public static List<Component> LoadComponentsFromXml(string path, ContentManager content)
        {
            if (File.Exists(path))
            {
                List<Component> componentsLoaded = new List<Component>();

                XmlDocument document = new XmlDocument();
                document.Load(path);
                XmlNode components = document.SelectSingleNode("components");
                string assembly = XmlExtensions.GetAttributeValue(components, "assembly", "TheArena");

                foreach (XmlNode node in components.SelectNodes("component"))
                {
                    Component c = LoadComponentFromXmlNode(node, content, assembly);

                    if (c != null)
                        componentsLoaded.Add(c);
                }

                return componentsLoaded;
            }
            else
                return null;
        }

        /// <summary>
        /// Takes a single XmlNode and loads it into a new component object, will also load any sub-components
        /// and add them to the new component as children
        /// </summary>
        /// <param name="parentNode">The XmlNode component node to load</param>
        /// <param name="content">ContentManage to load content from Pipeline</param>
        /// <param name="assembly">String that represents the assembly objects are in (outside the GameUI assembly)</param>
        /// <returns></returns>
        public static Component LoadComponentFromXmlNode(XmlNode parentNode, ContentManager content, string assembly)
        {
            Component comp = null;

            Assembly userAssembly = Assembly.Load(assembly);
            Assembly engineAssembly = Assembly.GetExecutingAssembly();

            object createdObject = null;
            string type = XmlExtensions.GetAttributeValue(parentNode, "type", "GameUI.Components.Component");

            if (userAssembly != null)
                createdObject = userAssembly.CreateInstance(type);

            if (createdObject == null)
                createdObject = engineAssembly.CreateInstance(type);

            if (createdObject == null)
                throw new ArgumentException(string.Format("'{0}' does not exist in any of the loaded Assemblies", type));

            if (createdObject is Component)
                comp = (Component)createdObject;
            else
                throw new ArgumentException(string.Format("'{0}' is not a Component object", type));

            comp.Name = XmlExtensions.GetAttributeValue(parentNode, "name");

            // Set position
            comp.Position = new Rectangle(
                                    XmlExtensions.GetAttributeValue<int>(parentNode, "x", 0),
                                    XmlExtensions.GetAttributeValue<int>(parentNode, "y", 0),
                                    XmlExtensions.GetAttributeValue<int>(parentNode, "width", 32),
                                    XmlExtensions.GetAttributeValue<int>(parentNode, "height", 32)
                                );

            // Load the properties for the component
            foreach (XmlNode property in parentNode.SelectNodes("properties/property"))
            {
                string name = XmlExtensions.GetAttributeValue(property, "name");
                string value = XmlExtensions.GetAttributeValue(property, "value");

                if (name.StartsWith("$"))
                {
                    string methodName = value;
                    string eventName = name.Substring(1);

                    MethodInfo methodInfo = comp.GetType().GetMethod(methodName);
                    EventInfo eventInfo = comp.GetType().GetEvent(eventName);
                    Delegate delegateMethod = Delegate.CreateDelegate(eventInfo.EventHandlerType, comp, methodInfo);

                    eventInfo.AddEventHandler(comp, delegateMethod);
                }
                else if (name.Equals("Texture"))
                    comp.Texture = content.Load<Texture2D>(value);
                else if (name.Equals("Font"))
                    comp.Font = content.Load<SpriteFont>(value);
                else
                    ReflectionExtensions.SmartSetProperty(comp, name, value);
            }

            // Reset the content pane on the component
            comp.ResetContentPane();

            // Load any sub-components
            foreach (XmlNode child in parentNode.SelectNodes("components/component"))
            {
                Component childComponent = Component.LoadComponentFromXmlNode(child, content, assembly);

                if (childComponent != null)
                {
                    childComponent.Parent = comp;
                    comp.Children.Add(childComponent);
                }
            }
            
            return comp;
        }

    }
}
