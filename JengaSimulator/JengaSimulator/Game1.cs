using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using PatrickModafferiA3.Systems;

namespace PatrickModafferiA3
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        const float DEFAULT_CAMERA_DISTANCE = 75f;
        const float GROUND_LEVEL = -50f;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont font;
        Matrix view = Matrix.CreateLookAt(new Vector3(0, 0, DEFAULT_CAMERA_DISTANCE), Vector3.Zero, Vector3.UnitY);
        Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(70), 800 / 600, 1, 300);

        private Model ball;
        private Model stick;
        private Texture2D smoke;
        float angleX = 0;
        float angleY = 0;
        float zoom = 0;

        bool pressed;
        public enum states { box, pause, play, main1, victory };
        states state = states.box;
        states question = states.main1;
        Keys previousKey;

        ParticleSystem Stars = new Stars();
        ParticleSystem Fireworks = new Fireworks(new Vector3(0, GROUND_LEVEL, GROUND_LEVEL - 10));

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            Stars.content = Content;
            Fireworks.content = Content;
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 600;
            pressed = false;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            base.Initialize();

            Stars.initialize();
            Fireworks.initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            ball = Content.Load<Model>("star");
            stick = Content.Load<Model>("ball");
            font = Content.Load<SpriteFont>("Score");
            smoke = Content.Load<Texture2D>("smoke");
            // TODO: use this.Content to load your game content here
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
            KeyboardState keyboard = Keyboard.GetState();
            cameraMotion(keyboard);
            handleGameState(keyboard);

            Stars.update(gameTime.ElapsedGameTime.Milliseconds);
            
            if (question == states.victory)
            {
                Fireworks.update(gameTime.ElapsedGameTime.Milliseconds);
            }
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            
            Stars.draw(view, projection);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone);              
            spriteBatch.DrawString(font, "STAR SYSTEM", new Vector2(5, 5), Color.White);
            spriteBatch.DrawString(font, "Press Space to go to Q2", new Vector2(5, 550), Color.White);
            
            if (question == states.victory)
            {

                Stars.draw(view, projection);
                Fireworks.draw(view, projection);

                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone);              
                spriteBatch.DrawString(font, "FIREWORKS", new Vector2(5, 5), Color.White);
                spriteBatch.DrawString(font, "Press Space to go to Q1", new Vector2(5, 550), Color.White);
            }

            spriteBatch.DrawString(font, "Use W/A/S/D to rotate camera", new Vector2(5, 25), Color.White);
            spriteBatch.DrawString(font, "Use Up/Down/Left/Right to move camera", new Vector2(5, 45), Color.White);
            spriteBatch.DrawString(font, "Use Q/E to move forward and backward", new Vector2(5, 65), Color.White);
            spriteBatch.DrawString(font, "X to reset camera", new Vector2(5, 85), Color.White);
            spriteBatch.DrawString(font, "Particles: " + Particle.particleCount.ToString(), new Vector2(5, 105), Color.White);

            spriteBatch.End();
            base.Draw(gameTime);
        }

        public void cameraMotion(KeyboardState keyboardState)
        {
            if (keyboardState.IsKeyDown(Keys.X))
            {
                view = Matrix.CreateLookAt(new Vector3(0, 0, DEFAULT_CAMERA_DISTANCE), Vector3.Zero, Vector3.UnitY);
            }
            if (keyboardState.IsKeyDown(Keys.W))
            {
                // view = Matrix.CreateLookAt(new Vector3(0, viewDist++, 10), new Vector3(0, 0, 0), Vector3.UnitY);
                angleX = -0.005f;
                view = view * Matrix.CreateRotationX(angleX);                
            }
            if (keyboardState.IsKeyDown(Keys.S))
            {
                // view = Matrix.CreateLookAt(new Vector3(0, viewDist++, 10), new Vector3(0, 0, 0), Vector3.UnitY);
                angleX = 0.005f;
                view = view * Matrix.CreateRotationX(angleX);                
            }
            if (keyboardState.IsKeyDown(Keys.D))
            {
                // view = Matrix.CreateLookAt(new Vector3(0, viewDist++, 10), new Vector3(0, 0, 0), Vector3.UnitY);
                angleX = 0.005f;
                view = view * Matrix.CreateRotationY(angleX);                
            }
            if (keyboardState.IsKeyDown(Keys.A))
            {
                // view = Matrix.CreateLookAt(new Vector3(0, viewDist++, 10), new Vector3(0, 0, 0), Vector3.UnitY);
                angleX = -0.005f;
                view = view * Matrix.CreateRotationY(angleX);
                
            }
            if (keyboardState.IsKeyDown(Keys.E))
            {
                // view = Matrix.CreateLookAt(new Vector3(0, viewDist++, 10), new Vector3(0, 0, 0), Vector3.UnitY);
                zoom = 0.5f;
                view = view * Matrix.CreateTranslation(new Vector3(0, 0, zoom));
            }
            if (keyboardState.IsKeyDown(Keys.Q))
            {
                // view = Matrix.CreateLookAt(new Vector3(0, viewDist--, 10), new Vector3(0, 0, 0), Vector3.UnitY);
                zoom = -0.5f;
                view = view * Matrix.CreateTranslation(new Vector3(0, 0, zoom));
            }
            if (keyboardState.IsKeyDown(Keys.Up))
            {
                // view = Matrix.CreateLookAt(new Vector3(0, viewDist++, 10), new Vector3(0, 0, 0), Vector3.UnitY);
                zoom = -0.5f;
                view = view * Matrix.CreateTranslation(new Vector3(0, zoom, 0));
            }
            if (keyboardState.IsKeyDown(Keys.Down))
            {
                // view = Matrix.CreateLookAt(new Vector3(0, viewDist--, 10), new Vector3(0, 0, 0), Vector3.UnitY);
                zoom = 0.5f;
                view = view * Matrix.CreateTranslation(new Vector3(0, zoom, 0));
            }
            if (keyboardState.IsKeyDown(Keys.Left))
            {
                // view = Matrix.CreateLookAt(new Vector3(0, viewDist++, 10), new Vector3(0, 0, 0), Vector3.UnitY);
                zoom = 0.5f;
                view = view * Matrix.CreateTranslation(new Vector3(zoom, 0, 0));
            }
            if (keyboardState.IsKeyDown(Keys.Right))
            {
                // view = Matrix.CreateLookAt(new Vector3(0, viewDist--, 10), new Vector3(0, 0, 0), Vector3.UnitY);
                zoom = -0.5f;
                view = view * Matrix.CreateTranslation(new Vector3(zoom, 0, 0));
            }
        }

        public void handleGameState(KeyboardState keyboardState)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
               keyboardState.IsKeyDown(Keys.Escape))
                this.Exit();

            // Switch Full Screen
            if (keyboardState.IsKeyDown(Keys.F11))
            {
                graphics.ToggleFullScreen();
                projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(80), 1980 / 720, 1, 300);
                graphics.PreferredBackBufferWidth = 1980;
                graphics.PreferredBackBufferHeight = 720;
            }

            if (question == states.main1)
            {
                if (keyboardState.IsKeyDown(Keys.D2))
                {
                    state = states.box;
                }
                if (keyboardState.IsKeyDown(Keys.D3))
                {
                    state = states.pause;
                }
                if (keyboardState.IsKeyDown(Keys.D4))
                {
                    state = states.play;
                }
                if (keyboardState.IsKeyDown(Keys.D5) && previousKey != Keys.D5)
                {
                    previousKey = Keys.D5;
                }
                else if (keyboardState.IsKeyDown(Keys.D7) && previousKey != Keys.D7)
                {                   
                    previousKey = Keys.D6;
                }
                else if (keyboardState.IsKeyDown(Keys.D6) && previousKey != Keys.D6)
                {
                    previousKey = Keys.D7;
                }
                else if (keyboardState.IsKeyDown(Keys.D8) && previousKey != Keys.D8)
                {
                    previousKey = Keys.D8;
                }
                else
                {
                    previousKey = Keys.D0;
                }
            }
            if (question == states.victory)
            {
                if (keyboardState.IsKeyDown(Keys.D1))
                {
                }
                if (keyboardState.IsKeyDown(Keys.D2))
                { 
                }
                if (keyboardState.IsKeyDown(Keys.D3))
                {
                }
                if (keyboardState.IsKeyDown(Keys.D4))
                {                    
                }
                if (keyboardState.IsKeyDown(Keys.D5))
                {
                }
                if (keyboardState.IsKeyDown(Keys.D6))
                {
                }
            }
            if (keyboardState.IsKeyDown(Keys.Space) && !pressed)
            {
                pressed = true;
                if (question == states.main1)
                {
                    //view = Matrix.CreateLookAt(new Vector3(0, 0, 45f), Vector3.Zero, Vector3.UnitY);
                    question = states.victory;

                }
                else
                {
                    //view = Matrix.CreateLookAt(new Vector3(0, 0, 25f), cube.Center, Vector3.UnitY);
                    question = states.main1;
                }
            }
            if (keyboardState.IsKeyUp(Keys.Space))
            {
                pressed = false;
            }
        }
    }
}
