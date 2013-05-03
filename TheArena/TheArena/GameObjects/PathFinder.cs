using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameEngine.GameObjects;
using GameEngine.Pathfinding;
using GameEngine.Drawing;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace TheArena.GameObjects
{
    public class PathFinder : Entity
    {

        public Path CurrentPath { get; set; }

        private float _moveSpeed = 1.8f;

        public PathFinder()
        {
            Construct(0, 0);
        }

        public PathFinder(float x, float y) : base(x, y)
        {
            Construct(x, y);
        }

        private void Construct(float x, float y)
        {

        }

        public override void LoadContent(ContentManager content)
        {
            Animation.LoadAnimationXML(Drawables, "Animations/Characters/female_npc.anim", content);
            CurrentDrawableState = "Idle_Down";

            base.LoadContent(content);
        }

        public override void Update(GameTime gameTime, GameEngine.TeeEngine engine)
        {
            MouseState mouseState = Mouse.GetState();
            KeyboardState keyboardState = Keyboard.GetState();
            
            // Check to see if the path should be recreated
            if (mouseState.RightButton == ButtonState.Pressed)
            {
                int tileX = (int)mouseState.X / engine.Map.TileWidth;
                int tileY = (int)mouseState.Y / engine.Map.TileHeight;
                ANode target = engine.Map.Nodes[tileX, tileY];

                tileX = (int)Pos.X / engine.Map.TileWidth;
                tileY = (int)Pos.Y / engine.Map.TileHeight;
                ANode start = engine.Map.Nodes[tileX, tileY];

                CurrentPath = AStar.GeneratePath(start, target);
            } 

            // Path to the next nobe on current path
            if (CurrentPath != null && CurrentPath.Count > 0)
            {
                ANode nextNode = CurrentPath.Peek();

                double moveAngle = Math.Atan2(Pos.Y - nextNode.Center.Y, Pos.X - nextNode.Center.X);

                Pos.X += (float)(Math.Cos(moveAngle) * _moveSpeed);
                Pos.Y += (float)(Math.Sin(moveAngle) * _moveSpeed);

                // If we are close enough to the node pop it off the path
                if (Vector2.Distance(Pos, nextNode.Center) < 10)
                    CurrentPath.Pop();
            }

            base.Update(gameTime, engine);
        }
    }
}
