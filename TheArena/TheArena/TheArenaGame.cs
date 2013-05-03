using System;
using System.Collections.Generic;
using System.Text;
using GameEngine;
using GameEngine.Drawing;
using GameEngine.Extensions;
using GameEngine.GameObjects;
using GameEngine.Info;
using GameEngine.Tiled;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TheArena.Shaders;
using TheArena.GameObjects;
using TheArena.GameObjects.Heroes;
//using GameEngine.Pathfinding;

namespace TheArena
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class TheArenaGame : Microsoft.Xna.Framework.Game
    {
        public static ViewPortInfo ViewPortInfo { get; set; }
        
        // Constant (Editable) Valuables.
        const bool DEBUG = true;

        const int CIRCLE_POINT_ACCURACY = 36;

        const int WINDOW_HEIGHT = 700;
        const int WINDOW_WIDTH = 1200;

        bool helmetVisible = true;
        bool showDebugInfo = true;
        bool showDiagnostics = false;

        float Zoom = 1.2f;

        int TextCounter = 0;
        int SamplerIndex = 0;
        SamplerState CurrentSampler;
        SamplerState[] SamplerStates = new SamplerState[] { 
            SamplerState.PointWrap,
            SamplerState.PointClamp,
            SamplerState.LinearWrap,
            SamplerState.LinearClamp,
            SamplerState.AnisotropicWrap,
            SamplerState.AnisotropicClamp 
        };

        LightShader LightShader;

        // Graphic Related Variables.
        GraphicsDeviceManager Graphics;
        SpriteBatch SpriteBatch;
        SpriteFont DefaultSpriteFont;

        // Game Specific Variablies.
        Random Random = new Random();

        TeeEngine Engine;

        public TheArenaGame()
        {
            Graphics = new GraphicsDeviceManager(this);

            Content.RootDirectory = "Content";
            Graphics.PreferredBackBufferHeight = WINDOW_HEIGHT;
            Graphics.PreferredBackBufferWidth = WINDOW_WIDTH;
            IsMouseVisible = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            Engine = new TeeEngine(this, WINDOW_WIDTH, WINDOW_HEIGHT);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            LightShader = new LightShader(this.GraphicsDevice, CIRCLE_POINT_ACCURACY);
            LightShader.AmbientLight = new Color(30, 15, 15);
            LightShader.Enabled = false;
            Engine.RegisterGameShader("LightShader", LightShader);

            Engine.LoadMap("Content/Maps/arena.tmx");

            CurrentSampler = SamplerStates[SamplerIndex];

            Engine.DrawingOptions.ShowEntityDebugInfo = false;
            Engine.DrawingOptions.ShowBoundingBoxes = false;
            Engine.DrawingOptions.ShowTileGrid = false;
            Engine.DrawingOptions.ShowColliderDebugInfo = false;

            Engine.LoadContent();

            SpriteBatch = new SpriteBatch(GraphicsDevice);

            DefaultSpriteFont = Content.Load<SpriteFont>(@"Fonts\Default");

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // F1 = Show/Hide Bounding Boxes
            // F2 = Show/Hide Debug Info
            // F3 = Enable/Disable LightShader
            // F4 = Change Current SamplerState
            // F5 = Show/Hide Tile Grid
            // F6 = Show/Hide Quad Tree
            // F7 = Show Performance Diagnostics
            // F8 = Show Entity Debug Info
            // F10 = Toggle Fullscreen Mode
            // F11 = Show/Hide Player Helmet
            // F12 = Disable Player Collision Detection

            KeyboardState keyboardState = Keyboard.GetState();

            if (KeyboardExtensions.GetKeyDownState(keyboardState, Keys.F1, this, true))
                Engine.DrawingOptions.ShowBoundingBoxes = !Engine.DrawingOptions.ShowBoundingBoxes;

            if (KeyboardExtensions.GetKeyDownState(keyboardState, Keys.F2, this, true))
                showDebugInfo = !showDebugInfo;

            if (KeyboardExtensions.GetKeyDownState(keyboardState, Keys.F3, this, true))
                LightShader.Enabled = !LightShader.Enabled;

            if (KeyboardExtensions.GetKeyDownState(keyboardState, Keys.F4, this, true))
                CurrentSampler = SamplerStates[++SamplerIndex % SamplerStates.Length];

            if (KeyboardExtensions.GetKeyDownState(keyboardState, Keys.F5, this, true))
                Engine.DrawingOptions.ShowTileGrid = !Engine.DrawingOptions.ShowTileGrid;

            if (KeyboardExtensions.GetKeyDownState(keyboardState, Keys.F6, this, true))
                Engine.DrawingOptions.ShowColliderDebugInfo = !Engine.DrawingOptions.ShowColliderDebugInfo;

            if (KeyboardExtensions.GetKeyDownState(keyboardState, Keys.F7, this, true))
                showDiagnostics = !showDiagnostics;

            if (KeyboardExtensions.GetKeyDownState(keyboardState, Keys.F8, this, true))
                Engine.DrawingOptions.ShowEntityDebugInfo = !Engine.DrawingOptions.ShowEntityDebugInfo;

            if (KeyboardExtensions.GetKeyDownState(keyboardState, Keys.F10, this, true))
                Engine.DrawingOptions.ShowDrawableComponents = !Engine.DrawingOptions.ShowDrawableComponents;

            if (KeyboardExtensions.GetKeyDownState(keyboardState, Keys.F12, this, true))
            {
                Hero player = (Hero)Engine.GetEntity("Player");
                player.CollisionDetection = !player.CollisionDetection;
            }

            // INCREASE ZOOM LEVEL
            if (KeyboardExtensions.GetKeyDownState(keyboardState, Keys.OemPlus, this, true))
                Zoom += 0.1f;

            // DECREASE ZOOM LEVEL
            if (KeyboardExtensions.GetKeyDownState(keyboardState, Keys.OemMinus, this, true))
                Zoom -= 0.1f;
            
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Viewport = new Viewport
            {
                X = 0,
                Y = 0,
                Width = WINDOW_WIDTH,
                Height = WINDOW_HEIGHT,
                MinDepth = 0,
                MaxDepth = 1
            };
            GraphicsDevice.Clear(Color.Black);

            TextCounter = 0;
            float textHeight = DefaultSpriteFont.MeasureString("d").Y;

            Rectangle pxDestRectangle = new Rectangle(0, 0, WINDOW_WIDTH, WINDOW_HEIGHT);

            // Draw the World View Port, Centered on the Engine.GetEntity("Player") Actor.
            ViewPortInfo viewPort = Engine.DrawWorldViewPort(
                                            SpriteBatch,
                                            Engine.GetEntity("Player").Pos,
                                            Zoom,
                                            pxDestRectangle,
                                            Color.White,
                                            CurrentSampler,
                                            DefaultSpriteFont);

            // Update static variable
            ViewPortInfo = viewPort;


            // DRAW DEBUGGING INFORMATION
            SpriteBatch.Begin();
            {
                if (showDebugInfo)
                {
                    // DRAW THE LIGHT MAP OUTPUT TO THE SCREEN FOR DEBUGGING
                    if (LightShader.Enabled)
                    {
                        int lightMapHeight = 100;
                        int lightMapWidth = (int)Math.Ceiling(100 * ((float)LightShader.LightMap.Width / LightShader.LightMap.Height));

                        SpriteBatch.Draw(
                            LightShader.LightMap,
                            new Rectangle(
                                WINDOW_WIDTH - lightMapWidth, 0,
                                lightMapWidth, lightMapHeight
                            ),
                            Color.White
                        );
                    }

                    double fps = 1000 / gameTime.ElapsedGameTime.TotalMilliseconds;
                    Color fpsColor = (Math.Ceiling(fps) < 60) ? Color.Red : Color.White;

                    float TX = Engine.GetEntity("Player").Pos.X / Engine.Map.TileWidth;
                    float TY = Engine.GetEntity("Player").Pos.Y / Engine.Map.TileHeight;
                    Hero player = (Hero)Engine.GetEntity("Player");

                    SpriteBatch.DrawString(DefaultSpriteFont, Engine.GetEntity("Player").Pos.ToString(), GeneratePos(textHeight), Color.White);
                    SpriteBatch.DrawString(DefaultSpriteFont, TX.ToString("0.0") + "," + TY.ToString("0.0"), GeneratePos(textHeight), Color.White);
                    SpriteBatch.DrawString(DefaultSpriteFont, "Coins=" + player.Gold, GeneratePos(textHeight), Color.White);
                    SpriteBatch.DrawString(DefaultSpriteFont, String.Format("Lvl={0}", player.Lvl), GeneratePos(textHeight), Color.White);
                    SpriteBatch.DrawString(DefaultSpriteFont, String.Format("HP={0}/{1}", player.HP, player.MaxHP), GeneratePos(textHeight), Color.White);
                    SpriteBatch.DrawString(DefaultSpriteFont, String.Format("Mana={0}/{1}", player.Mana, player.MaxMana), GeneratePos(textHeight), Color.White);
                    SpriteBatch.DrawString(DefaultSpriteFont, String.Format("XP={0}/{1}", player.XP, player.XPToNextLevel), GeneratePos(textHeight), Color.White);
                    SpriteBatch.DrawString(DefaultSpriteFont, String.Format("Stats={0},{1},{2}", player.Strength, player.Dexterity, player.Wisdom), GeneratePos(textHeight), Color.White);
                    SpriteBatch.DrawString(DefaultSpriteFont, String.Format("Intensity = {0}", player.Intensity), GeneratePos(textHeight), Color.White);
                    SpriteBatch.DrawString(DefaultSpriteFont, fps.ToString("0.0 FPS"), GeneratePos(textHeight), fpsColor);
                    SpriteBatch.DrawString(DefaultSpriteFont, "Resolution=" + Engine.PixelWidth + "x" + Engine.PixelHeight, GeneratePos(textHeight), Color.White);
                    SpriteBatch.DrawString(DefaultSpriteFont, "MapSize=" + Engine.Map.txWidth + "x" + Engine.Map.txHeight, GeneratePos(textHeight), Color.White);
                    SpriteBatch.DrawString(DefaultSpriteFont, "Sampler=" + CurrentSampler.ToString(), GeneratePos(textHeight), Color.White);
                    SpriteBatch.DrawString(DefaultSpriteFont, "Entities On Screen = " + Engine.EntitiesOnScreen.Count, GeneratePos(textHeight), Color.White);
                    SpriteBatch.DrawString(DefaultSpriteFont, "Total Entities = " + Engine.Entities.Count, GeneratePos(textHeight), Color.White);
                    SpriteBatch.DrawString(DefaultSpriteFont, "Actual Zoom = " + viewPort.ActualZoom, GeneratePos(textHeight), Color.White);
                }

                if (showDiagnostics)
                {
                    StringBuilder builder = new StringBuilder();
                    builder.AppendLine(Engine.DebugInfo.ToString());

                    builder.AppendLine("Entity Update Times");
                    Dictionary<string, TimeSpan> topUpdateTimes = Engine.DebugInfo.GetTop(Engine.DebugInfo.EntityUpdateTimes, 3);
                    foreach (string entityId in topUpdateTimes.Keys)
                        builder.AppendLine(string.Format("'{0}' = {1}", entityId, topUpdateTimes[entityId]));

                    builder.AppendLine("Entity Rendering Times");
                    Dictionary<string, TimeSpan> topRenderTimes = Engine.DebugInfo.GetTop(Engine.DebugInfo.EntityRenderingTimes, 3);
                    foreach (string entityId in topRenderTimes.Keys)
                        builder.AppendLine(string.Format("'{0}' = {1}", entityId, topRenderTimes[entityId]));

                    string textOutput = builder.ToString();
                    SpriteBatch.DrawString(DefaultSpriteFont, textOutput, new Vector2(0, WINDOW_HEIGHT - DefaultSpriteFont.MeasureString(textOutput).Y), Color.White);
                }
            }
            SpriteBatch.End();
            base.Draw(gameTime);
        }

        private Vector2 GeneratePos(float textHeight)
        {
            return new Vector2(0, TextCounter++ * textHeight);
        }
    }
}
