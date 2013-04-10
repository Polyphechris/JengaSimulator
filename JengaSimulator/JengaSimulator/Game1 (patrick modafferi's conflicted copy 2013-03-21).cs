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
using PatrickModafferiA2.Systems;

namespace PatrickModafferiA2
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        const float DEFAULT_CAMERA_DISTANCE = 100f;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont font;
        Matrix view = Matrix.CreateLookAt(new Vector3(0, 0, DEFAULT_CAMERA_DISTANCE), Vector3.Zero, Vector3.UnitY);
        Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(70), 800 / 600, 1, 200);

        private Model ball;
        private Model stick;
        private Texture2D smoke;
        float angleX = 0;
        float angleY = 0;
        float zoom = 0;

        bool pressed;
        public enum states { box, pause, play, main1, main2 };
        states state = states.box;
        states question = states.main1;
        Keys previousKey;

        ParticleSystem Stars = new Stars();
        ParticleSystem Fireworks = new Fireworks(new Vector3(0, -20, 0));

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
            view = Matrix.CreateLookAt(new Vector3(0, 0, 25f), Vector3.Zero, Vector3.UnitY);
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
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            KeyboardState keyboard = Keyboard.GetState();
            cameraMotion(keyboard);
            handleGameState(keyboard);
            if (question == states.main1)
            {
                Stars.update(gameTime.ElapsedGameTime.Milliseconds);
            }
            else if (question == states.main2)
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


            if (question == states.main1)
            {
                Stars.draw(view, projection);

                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone);
              
                spriteBatch.DrawString(font, "QUESTION 1", new Vector2(5, 5), Color.White);
                spriteBatch.DrawString(font, "Use W/A/S/D UP/DOWN/LEFT/RIGHT to move camera", new Vector2(5, 25), Color.White);
                spriteBatch.DrawString(font, "1 - +particles", new Vector2(5, 45), Color.White);
                spriteBatch.DrawString(font, "2 - -particles", new Vector2(5, 65), Color.White);
                spriteBatch.DrawString(font, "Press Space to go to Q2", new Vector2(5, 550), Color.White);
            }
            if (question == states.main2)
            {

                Stars.draw(view, projection);
                Fireworks.draw(view, projection);

                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone);
              
                spriteBatch.DrawString(font, "QUESTION 2", new Vector2(5, 5), Color.White);
                spriteBatch.DrawString(font, "Use W/A/S/D to move the ball", new Vector2(5, 25), Color.White);
                spriteBatch.DrawString(font, "1 - Increase Red", new Vector2(5, 55), Color.White);
                spriteBatch.DrawString(font, "2 - Increase Green", new Vector2(5, 85), Color.White);
                spriteBatch.DrawString(font, "3 - Increase Blue", new Vector2(5, 115), Color.White);
                spriteBatch.DrawString(font, "4 - Decrease Red", new Vector2(5, 145), Color.White);
                spriteBatch.DrawString(font, "5 - Decrease Green", new Vector2(5, 175), Color.White);
                spriteBatch.DrawString(font, "6 - Decrease Blue", new Vector2(5, 205), Color.White);
                spriteBatch.DrawString(font, "X to reset ball", new Vector2(5, 235), Color.White);
                spriteBatch.DrawString(font, "Press Space to go to Q1", new Vector2(5, 550), Color.White);
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }

        public void cameraMotion(KeyboardState keyboardState)
        {
            if (keyboardState.IsKeyDown(Keys.X))
            {
                if (question == states.main2)
                {
                    view = Matrix.CreateLookAt(new Vector3(0, 0, DEFAULT_CAMERA_DISTANCE), Vector3.Zero, Vector3.UnitY);
                }
                else
                {
                    // view = Matrix.CreateLookAt(new Vector3(0, viewDist++, 10), new Vector3(0, 0, 0), Vector3.UnitY);
                  //  view = Matrix.CreateLookAt(new Vector3(0, 0, 25f), cube.Center, Vector3.UnitY);
                }

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
                zoom = 0.5f;
                view = view * Matrix.CreateTranslation(new Vector3(0, zoom, 0));
            }
            if (keyboardState.IsKeyDown(Keys.Down))
            {
                // view = Matrix.CreateLookAt(new Vector3(0, viewDist--, 10), new Vector3(0, 0, 0), Vector3.UnitY);
                zoom = -0.5f;
                view = view * Matrix.CreateTranslation(new Vector3(0, zoom, 0));
            }
            if (keyboardState.IsKeyDown(Keys.Left))
            {
                // view = Matrix.CreateLookAt(new Vector3(0, viewDist++, 10), new Vector3(0, 0, 0), Vector3.UnitY);
                zoom = -0.5f;
                view = view * Matrix.CreateTranslation(new Vector3(zoom, 0, 0));
            }
            if (keyboardState.IsKeyDown(Keys.Right))
            {
                // view = Matrix.CreateLookAt(new Vector3(0, viewDist--, 10), new Vector3(0, 0, 0), Vector3.UnitY);
                zoom = 0.5f;
                view = view * Matrix.CreateTranslation(new Vector3(zoom, 0, 0));
            }
        }

        public void handleGameState(KeyboardState keyboardState)
        {
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
            if (question == states.main2)
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
                {;
                }
            }
            if (keyboardState.IsKeyDown(Keys.Space) && !pressed)
            {
                pressed = true;
                if (question == states.main1)
                {
                    //view = Matrix.CreateLookAt(new Vector3(0, 0, 45f), Vector3.Zero, Vector3.UnitY);
                    question = states.main2;

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
