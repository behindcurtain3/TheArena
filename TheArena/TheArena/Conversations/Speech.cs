using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using System.Xml;
using GameUI.Extensions;
using System.IO;
using GameEngine.Drawing.Text;

namespace TheArena.Conversations
{
    public class Speech
    {
        /// <summary>
        /// Unique name for this Speech, do not use "Child"
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The text to display when this Speech is selected
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// The text shown to select this option from another Speech
        /// </summary>
        public string Prompt { get; set; }

        /// <summary>
        /// The "root" Speech of the tree containing this speech
        /// Used to be able to link to anywhere within the structure
        /// of the tree from any node
        /// </summary>
        public Speech Root { get; set; }

        /// <summary>
        /// What does the prompt link to?
        /// Default to "Child" which will just load the Speech associated with the Prompt
        /// Can also set to:
        ///     File name - Name of a new .speech file to load (must have .speech)
        ///     Node name - Name of a node that can be found from traversing down from Root
        ///     "Exit" - Will close the Conversation when selected
        /// </summary>
        public string PromptLink { get; set; }

        private List<Speech> _children;

        public Speech()
        {
            Construct();
        }

        private void Construct()
        {
            _children = new List<Speech>();
            PromptLink = "Child";
        }

        public List<Speech> GetChildren()
        {
            return _children;
        }

        public void AddChild(Speech child)
        {
            _children.Add(child);
        }

        public Speech FindNodeFromRoot(string name)
        {
            if (Root.Name.Equals(name))
                return Root;

            return FindNode(Root, name);
        }

        private Speech FindNode(Speech node, string name)
        {
            if (node.Name.Equals(name))
                return node;

            foreach (Speech child in node.GetChildren())
                if (child.Name.Equals(name))
                    return child;
                else
                    FindNode(child, name);

            return null;
        }

        public static Speech LoadSpeechFromXml(string path)
        {
            if (File.Exists(path))
            {
                XmlDocument document = new XmlDocument();
                document.Load(path);
                XmlNode speechNode = document.SelectSingleNode("Speech");

                return LoadSpeechFromXmlNode(speechNode, null);
            }
            else
                return null;
        }

        public static Speech LoadSpeechFromXmlNode(XmlNode node, Speech root)
        {
            Speech speech = new Speech();

            if (root == null)
                root = speech;

            speech.Name = XmlExtensions.GetAttributeValue(node, "name", "SpeechNode");

            if (node["Text"] != null)
                speech.Text = node.SelectSingleNode("Text").InnerText;

            if (node["Prompt"] != null)
                speech.Prompt = node.SelectSingleNode("Prompt").InnerText;

            if (node["PromptLink"] != null)
                speech.PromptLink = node.SelectSingleNode("PromptLink").InnerText;

            // Load children
            foreach (XmlNode childNode in node.SelectNodes("Children/Speech"))
            {

                Speech child = LoadSpeechFromXmlNode(childNode, root);
                child.Root = root;

                speech.AddChild(child);
            }

            return speech;
        }


    }
}
