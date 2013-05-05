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
using GameEngine.Info;

namespace TheArena.GameObjects
{
    public class PathFinder : CollidableEntity
    {

        public Path CurrentPath { get; set; }
        public Direction Direction { get; set; }

        private float _moveSpeed = 1.8f;

        public PathFinder()
        {
            Construct(0, 0);
        }

        public PathFinder(float x, float y)
        {
            Construct(x, y);
        }

        private void Construct(float x, float y)
        {
            Pos = new Vector2(x, y);
            Direction = GameObjects.Direction.Down;
        }

        public override void LoadContent(ContentManager content)
        {
            Animation.LoadAnimationXML(Drawables, "Animations/Characters/female_npc.anim", content);
            CurrentDrawableState = "Idle_" + Direction;

            base.LoadContent(content);
        }

        public override void Update(GameTime gameTime, GameEngine.TeeEngine engine)
        {
            MouseState mouseState = Mouse.GetState();
            KeyboardState keyboardState = Keyboard.GetState();
            
            // Check to see if the path should be recreated
            if (mouseState.RightButton == ButtonState.Pressed)
            {
                ViewPortInfo viewPort = TheArenaGame.ViewPortInfo;

                Vector2 worldCoord = viewPort.GetWorldCoordinates(new Point(mouseState.X, mouseState.Y));
                Vector2 tileCoord = worldCoord / (new Vector2(engine.Map.TileWidth, engine.Map.TileHeight)); ;
                tileCoord.X = (int)Math.Floor(tileCoord.X);
                tileCoord.Y = (int)Math.Floor(tileCoord.Y);

                if (tileCoord.X > 0 && tileCoord.X < engine.Map.txWidth)
                {
                    ANode target = engine.Pathfinding.Nodes[(int)tileCoord.X, (int)tileCoord.Y];

                    tileCoord = Pos / (new Vector2(engine.Map.TileWidth, engine.Map.TileHeight)); ;
                    ANode start = engine.Pathfinding.Nodes[(int)tileCoord.X, (int)tileCoord.Y];

                    CurrentPath = engine.Pathfinding.GeneratePath(start, target);
                }
            } 

            // Path to the next nobe on current path
            if (CurrentPath != null && CurrentPath.Count > 0)
            {
                float prevX = Pos.X;
                float prevY = Pos.Y;

                ANode nextNode = CurrentPath.Peek();
                double moveAngle = Math.Atan2(nextNode.Center.Y - Pos.Y, nextNode.Center.X - Pos.X);

                Pos.X += (float)(Math.Cos(moveAngle) * _moveSpeed);
                Pos.Y += (float)(Math.Sin(moveAngle) * _moveSpeed);

                // If we are close enough to the node pop it off the path
                if (Vector2.Distance(Pos, nextNode.Center) < 5)
                    CurrentPath.Pop();

                float horzDelta = Pos.X - prevX;
                float vertDelta = Pos.Y - prevY;

                if (Math.Abs(horzDelta) > 1)
                {
                    if (prevX < Pos.X)
                        Direction = Direction.Right;
                    else if (prevX > Pos.X)
                        Direction = Direction.Left;
                }
                else
                {
                    if (prevY < Pos.Y)
                        Direction = Direction.Down;
                    else if (prevY > Pos.Y)
                        Direction = Direction.Up;
                }
   
                CurrentDrawableState = "Walk_" + Direction;
            }
            else
            {
                CurrentDrawableState = "Idle_" + Direction;
            }

            base.Update(gameTime, engine);
        }
    }
}
