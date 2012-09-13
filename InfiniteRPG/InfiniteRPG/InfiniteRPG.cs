using System;
using System.Collections.Generic;
using System.Linq;
using InfiniteRPG.Components;
using InfiniteRPG.Data;
using InfiniteRPG.Data.Map;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace InfiniteRPG
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class InfiniteRPG : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Camera2D camera;
        CameraMovementState movementState;
        Texture2D myTexture;
        SpriteFont diagFont;
        private Tileset tileset;
        private MapSection[] mapSections;

        public InfiniteRPG()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1024;
            graphics.PreferredBackBufferHeight = 768;
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            movementState = new CameraMovementState();
            camera = new Camera2D(this, movementState);
            camera.Viewport = graphics.GraphicsDevice.Viewport;

            graphics.DeviceReset += (sender, args) => camera.Viewport = graphics.GraphicsDevice.Viewport;

            Components.Add(camera);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            myTexture = Content.Load<Texture2D>("Tilesets/Sample1");
            diagFont = Content.Load<SpriteFont>("Fonts/DiagFont");

            tileset = new Tileset("Sample1", myTexture, 32, 32);
            mapSections = new MapSection[2];
            mapSections[0] = Content.Load<MapSection>("Maps/layers-example");
            mapSections[1] = Content.Load<MapSection>("Maps/layers-example");
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            var keyState = Keyboard.GetState();

            if (keyState.IsKeyDown(Keys.Escape))
                this.Exit();

            movementState.Reset();

            // Directional movement
            if (keyState.IsKeyDown(Keys.NumPad4) || keyState.IsKeyDown(Keys.Left))
                movementState.SetMoving(CameraMovement.Left);
            if (keyState.IsKeyDown(Keys.NumPad6) || keyState.IsKeyDown(Keys.Right))
                movementState.SetMoving(CameraMovement.Right);
            if (keyState.IsKeyDown(Keys.NumPad8) || keyState.IsKeyDown(Keys.Up))
                movementState.SetMoving(CameraMovement.Up);
            if (keyState.IsKeyDown(Keys.NumPad2) || keyState.IsKeyDown(Keys.Down))
                movementState.SetMoving(CameraMovement.Down);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            var cameraTransform = camera.GetTransformMatrix();

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, DepthStencilState.Default, null, null, cameraTransform);

            var loc = Vector2.Zero;
            foreach (var section in mapSections)
            {
                section.Draw(spriteBatch, loc, tileset);
                loc = Vector2.Add(loc, new Vector2(0, 1));
            }

            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, DepthStencilState.None, null, null, Matrix.Identity);
            spriteBatch.DrawString(diagFont, string.Format("x: {0}\ny: {1}", camera.X, camera.Y),
                Vector2.Zero, Color.DarkGreen);
            spriteBatch.End();


            base.Draw(gameTime);
        }
    }
}
