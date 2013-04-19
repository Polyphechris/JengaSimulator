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
using JengaSimulator.Systems;
using JengaSimulator;
using JengaSimulator.Human;

namespace JengaSimulator
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        public static SystemState systemState;

        const float DEFAULT_CAMERA_DISTANCE = 75f;
        const float GROUND_LEVEL = -50f;
        GraphicsDeviceManager graphics;

        SpriteBatch spriteBatch;
        SpriteFont font;
        //Optimal Spot for viewing env.
        //public static Matrix view = Matrix.CreateLookAt(new Vector3(0, 0, DEFAULT_CAMERA_DISTANCE), Vector3.Zero, Vector3.UnitY);
        //Optimal SPot for viewing tower
        public static Matrix view = Matrix.CreateLookAt(new Vector3(0, 0, 35), new Vector3(0, -35, -20), Vector3.UnitY);
        public static Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(60), 16 / 9, 1, 300);
        public static Matrix rotation = Matrix.Identity;

        private Model ball;
        private Model stick;
        private Texture2D smoke;
        float angleX = 0;
        float angleY = 0;
        float zoom = 0;
                
        bool pressed;
        public enum states { box = 0, pause, play, main1, victory , night, day, instructions, last };
        public enum weathers { none = 0, snow, rain, last };
        states state = states.day;
        weathers currentWeather = (weathers)0;
        int currentWeatherIndex = 0;
        states gameState = states.main1;
        Keys previousKey;

        ParticleSystem Stars = new Stars();
        ParticleSystem Snow = new Snow();
        ParticleSystem Rain = new Rain();
        ParticleSystem Fireworks = new Fireworks(new Vector3(0, GROUND_LEVEL, GROUND_LEVEL - 10));
        CollisionManager controller;
        
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            Stars.content = Content;
            Fireworks.content = Content;
            Rain.content = Content;
            Snow.content = Content;
          //  graphics.PreferredBackBufferWidth = 800;
          //  graphics.PreferredBackBufferHeight = 600;
            pressed = false;

            systemState = SystemState.Idle;
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
            Snow.initialize();
            Rain.initialize();
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
            controller = new CollisionManager(Content);
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
            float timer = gameTime.ElapsedGameTime.Milliseconds;

            KeyboardState keyboard = Keyboard.GetState();
            cameraMotion(keyboard);
            handleGameState(keyboard);

            //Game state Updates
            if (gameState == states.play)
            {
                controller.Update(timer, keyboard);
            }
            else if (gameState == states.victory)
            {
                currentWeather = weathers.none;
                Fireworks.update(timer);
            }

            //Time of day Updates
            if (state == states.night)
            {
                Stars.update(timer);
            }
            else
            {
                if (currentWeather == weathers.snow)
                {
                    Snow.update(timer);
                }
                if (currentWeather == weathers.rain)
                {
                    Rain.update(timer);
                }
                //update whatever happens only in day time
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            if (state == states.night)
            {
                GraphicsDevice.Clear(Color.Black);
                Stars.draw(view, projection);
            }
            else if (state == states.day)
            {
                //Add some rain-snow logic with sky and particles
               // GraphicsDevice.Clear(Color.FromNonPremultiplied(150,150,175,255));
                if (currentWeather == weathers.none)
                {
                    GraphicsDevice.Clear(Color.Blue);
                }
                else
                {
                    GraphicsDevice.Clear(Color.DarkBlue);
                }
                if (currentWeather == weathers.rain)
                {
                    Rain.draw(view, projection);
                }
                if (currentWeather == weathers.snow)
                {
                    Snow.draw(view, projection);
                }
            }
            
            if (gameState == states.victory)
            {
                Fireworks.draw(view, projection);
            } 
            if (gameState == states.play || gameState == states.pause || gameState == states.instructions || gameState == states.victory)
            {
                controller.Draw();

                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone);
                    spriteBatch.DrawString(font, "JENGA TIME!", new Vector2(5, 5), Color.White);
                    spriteBatch.DrawString(font, "Particles: " + Particle.particleCount.ToString(), new Vector2(5, 25), Color.White);
                spriteBatch.End();
            }
            if (gameState == states.pause)
            {
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone);
                    spriteBatch.DrawString(font, "PAUSED", new Vector2((graphics.PreferredBackBufferWidth / 2) - 25, graphics.PreferredBackBufferHeight / 2), Color.White);
                    spriteBatch.DrawString(font, "(i)Instructions", new Vector2((graphics.PreferredBackBufferWidth / 2) - 65, graphics.PreferredBackBufferHeight / 2 + 30), Color.White);
                    spriteBatch.Draw(smoke, new Vector2(0, 0), new Rectangle(0, 0, 2000, 2000), Color.FromNonPremultiplied(235, 235, 220, 75));
                spriteBatch.End();
            }
            if (gameState == states.main1)
            {
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone);
                    spriteBatch.DrawString(font, "WELCOME TO JENGA", new Vector2(25, 25), Color.White);
                    spriteBatch.DrawString(font, "The #1 (and only) Jenga Simulator in the WORLD!", new Vector2(25,55), Color.White);
                    spriteBatch.DrawString(font, "Press Space to Begin", 
                        new Vector2((graphics.PreferredBackBufferWidth / 2) - 105, graphics.PreferredBackBufferHeight / 2 + 30), Color.White);
                spriteBatch.End();
            } 
            if (gameState == states.instructions)
            {
                int startInstruction = 105;
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone);
                    spriteBatch.DrawString(font, "INSTRUCTIONS", new Vector2((graphics.PreferredBackBufferWidth / 2) - 65, graphics.PreferredBackBufferHeight / 10 + 25), Color.White);
                    spriteBatch.Draw(smoke,
                        new Rectangle((int)(graphics.PreferredBackBufferWidth / 10), (int)(graphics.PreferredBackBufferHeight/10), 
                            (int)(graphics.PreferredBackBufferWidth - (graphics.PreferredBackBufferWidth / 5)),
                            (int)(graphics.PreferredBackBufferHeight - (graphics.PreferredBackBufferHeight / 5))),
                        new Rectangle(0,0,1000,1000), 
                        Color.FromNonPremultiplied(235, 235, 220, 175));
                    spriteBatch.DrawString(font, "Use W/A/S/D to rotate camera", new Vector2(graphics.PreferredBackBufferWidth / 9, startInstruction + 25), Color.White, 0f, Vector2.Zero, 0.55f, SpriteEffects.None, 0f);
                    spriteBatch.DrawString(font, "Use Up/Down/Left/Right to move camera", new Vector2(graphics.PreferredBackBufferWidth / 9, startInstruction + 45), Color.White, 0f, Vector2.Zero, 0.55f, SpriteEffects.None, 0f);
                    spriteBatch.DrawString(font, "Use Q/E to move forward and backward", new Vector2(graphics.PreferredBackBufferWidth / 9, startInstruction + 65), Color.White, 0f, Vector2.Zero, 0.55f, SpriteEffects.None, 0f);
                    spriteBatch.DrawString(font, "X to reset camera", new Vector2(graphics.PreferredBackBufferWidth / 9, startInstruction + 85), Color.White, 0f, Vector2.Zero, 0.55f, SpriteEffects.None, 0f);
                    spriteBatch.DrawString(font, "Z/C Rotate around tower", new Vector2(graphics.PreferredBackBufferWidth / 9, startInstruction + 85), Color.White, 0f, Vector2.Zero, 0.55f, SpriteEffects.None, 0f);
                    
                    spriteBatch.DrawString(font, "G/Shift + G toggle day time", new Vector2(graphics.PreferredBackBufferWidth / 9, startInstruction + 125), Color.White, 0f, Vector2.Zero, 0.55f, SpriteEffects.None, 0f);
                    spriteBatch.DrawString(font, "F/Shift + F toggle fireworks", new Vector2(graphics.PreferredBackBufferWidth / 9, startInstruction + 145), Color.White, 0f, Vector2.Zero, 0.55f, SpriteEffects.None, 0f);
                    spriteBatch.DrawString(font, "B/Shift + B toggle Hand Bounding Boxes", new Vector2(graphics.PreferredBackBufferWidth / 9, startInstruction + 165), Color.White, 0f, Vector2.Zero, 0.55f, SpriteEffects.None, 0f);
                    spriteBatch.DrawString(font, "1/Shift + 1 toggle Rain", new Vector2(graphics.PreferredBackBufferWidth / 9, startInstruction + 185), Color.White, 0f, Vector2.Zero, 0.55f, SpriteEffects.None, 0f);
                    spriteBatch.DrawString(font, "2/Shift + 2 toggle Snow", new Vector2(graphics.PreferredBackBufferWidth / 9, startInstruction + 205), Color.White, 0f, Vector2.Zero, 0.55f, SpriteEffects.None, 0f);
                   
                spriteBatch.DrawString(font, "Space Next/Pause", new Vector2(graphics.PreferredBackBufferWidth / 9, startInstruction + 235), Color.White, 0f, Vector2.Zero, 0.55f, SpriteEffects.None, 0f);
                spriteBatch.End();
            }
            base.Draw(gameTime);
        }

        public void cameraMotion(KeyboardState keyboardState)
        {
            if (keyboardState.IsKeyDown(Keys.X))
            {
                view = Matrix.CreateLookAt(new Vector3(0, 0, DEFAULT_CAMERA_DISTANCE), Vector3.Zero, Vector3.UnitY);
            }
            if (keyboardState.IsKeyDown(Keys.I))
            {
                // view = Matrix.CreateLookAt(new Vector3(0, viewDist++, 10), new Vector3(0, 0, 0), Vector3.UnitY);
                angleX = -0.005f;
                view = view * Matrix.CreateRotationX(angleX);                
            }
            if (keyboardState.IsKeyDown(Keys.K))
            {
                // view = Matrix.CreateLookAt(new Vector3(0, viewDist++, 10), new Vector3(0, 0, 0), Vector3.UnitY);
                angleX = 0.005f;
                view = view * Matrix.CreateRotationX(angleX);                
            }
            if (keyboardState.IsKeyDown(Keys.L))
            {
                // view = Matrix.CreateLookAt(new Vector3(0, viewDist++, 10), new Vector3(0, 0, 0), Vector3.UnitY);
                angleX = 0.005f;
                view = view * Matrix.CreateRotationY(angleX);                
            }
            if (keyboardState.IsKeyDown(Keys.J))
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
            if (keyboardState.IsKeyDown(Keys.Z))
            {
                // view = Matrix.CreateLookAt(new Vector3(0, viewDist++, 10), new Vector3(0, 0, 0), Vector3.UnitY);
                angleX = -0.005f;
                rotation = rotation * Matrix.CreateRotationY(angleX);
            }
            if (keyboardState.IsKeyDown(Keys.C))
            {
                // view = Matrix.CreateLookAt(new Vector3(0, viewDist++, 10), new Vector3(0, 0, 0), Vector3.UnitY);
                angleX = 0.005f;
                rotation = rotation * Matrix.CreateRotationY(angleX);
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
                projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(60), 1280 / 720, 1, 300);
                graphics.PreferredBackBufferWidth = 1280;
                graphics.PreferredBackBufferHeight = 720;
            }

            if (keyboardState.IsKeyDown(Keys.F))
            {
                gameState = states.play;
            }
            if (keyboardState.IsKeyDown(Keys.F) &&
                keyboardState.IsKeyDown(Keys.LeftShift))
            {
                gameState = states.victory;
            }

            if (keyboardState.IsKeyDown(Keys.D1))
            {
                currentWeather = weathers.rain;
            }
            if (keyboardState.IsKeyDown(Keys.D1) &&
                keyboardState.IsKeyDown(Keys.LeftShift))
            {
                currentWeather = weathers.none;
            }
            if (keyboardState.IsKeyDown(Keys.D2))
            {
                currentWeather = weathers.snow;
            }
            if (keyboardState.IsKeyDown(Keys.D2) &&
                keyboardState.IsKeyDown(Keys.LeftShift))
            {
                currentWeather = weathers.none;
            }

            if (gameState == states.main1)
            {
                if (keyboardState.IsKeyDown(Keys.Space) && !pressed)
                {
                    pressed = true;
                    gameState = states.play;
                }
                else
                {
                    previousKey = Keys.D0;
                }
            }
            else if (gameState == states.victory)
            {
                if (keyboardState.IsKeyDown(Keys.Space) && !pressed)
                {
                    pressed = true;
                    gameState = states.main1;
                }            
            }
            else if (gameState == states.pause)
            {
                if (keyboardState.IsKeyDown(Keys.Space) && !pressed)
                {
                    pressed = true;
                    gameState = states.play;
                }
                else if (keyboardState.IsKeyDown(Keys.I))
                {
                    gameState = states.instructions;
                }
            }
            else if (gameState == states.instructions)
            {
                if (keyboardState.IsKeyDown(Keys.Space) && !pressed)
                {
                    pressed = true;
                    gameState = states.pause;
                } 
            }
            else if (gameState == states.play)
            {
                if (keyboardState.IsKeyDown(Keys.Space) && !pressed)
                {
                    pressed = true;
                    gameState = states.pause;
                }
                if (keyboardState.IsKeyDown(Keys.G))
                {
                    state = states.night;
                }
                if (keyboardState.IsKeyDown(Keys.G) && 
                    keyboardState.IsKeyDown(Keys.LeftShift))
                {
                    state = states.day;
                }
            }
            if (keyboardState.IsKeyUp(Keys.Space))
            {
                pressed = false;
            }
        }
    }
}
