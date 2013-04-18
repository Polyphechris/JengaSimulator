using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace JengaSimulator.Human
{
    class Arm
    {
        //Hand linear Constraints
        static float MAX_HEIGHT = -10;
        static float MAX_LATTERAL = 10;
        static float MAX_DEEP = 20;
        static float MIN_DEEP = 10;
        static float MIN_HEIGHT = -30;

        //Hand angular Constraints
        static float MIN_THETA_Z = 0;
        static float MAX_THETA_Z = 3 * (float)Math.PI/2;
        static float MIN_THETA_X = 2f *(float)Math.PI/3;
        static float MAX_THETA_X = 4f *(float)Math.PI/3;

        Wrist wrist;
        Hand hand;

        Model model;
        Matrix world;

        Model BOX;
        Matrix BoxWorld;
        Block collisionBox;

        //linear motion
        public Vector3 position;
        public Vector3 velocity;
        public Vector3 acceleration;

        public Vector3 scale;
        float weight;
        public Vector3 color;
        float alpha;
        ContentManager Content;

        public Arm(ContentManager c)
        {
            alpha = 1f;
            position = new Vector3(0, -20, 0);
            Content = c;
            InitializeWrist();
            collisionBox = new Block(position + new Vector3(0,0,-15), new Vector3(2,1, 5), 1, color, Content.Load<Model>("cube"),true);
            collisionBox.alpha = 0.4f;
            collisionBox.offsetRotation = new Vector3(0, 0, 5f);
        }

        private void InitializeWrist()
        {
            color = new Vector3(0.90f,0.66f,0.60f);

            hand = new Hand();
            hand.color = color;
            hand.model = Content.Load<Model>("hand");

            wrist = new Wrist(hand);
            wrist.color = color;
            wrist.model = Content.Load<Model>("wrist");
        }

        public void update(float time, KeyboardState keyboardState)
        {
            velocity = Vector3.Zero;
            wrist.w = Vector3.Zero;
            hand.w = Vector3.Zero;
            HandleInput(keyboardState);
            position = position + velocity * time / 1000;
            wrist.update(time);
            wrist.position = position;
            collisionBox.velocity = velocity;

            if (hand.d.X < 0) hand.w.Z = -wrist.w.Z;
            else hand.w.Z = wrist.w.Z;
            collisionBox.w = hand.w;
            collisionBox.Update(time);
        }

        public void draw()
        {
            collisionBox.Draw();
            wrist.draw();
        }

        private void HandleInput(KeyboardState keyboardState)
        {
            MouseState mouse = Mouse.GetState();
            if (mouse.LeftButton == ButtonState.Pressed)
            {
                if (position.Z > MIN_DEEP)
                    velocity += new Vector3(0, 0, -10);
            }
            else if (position.Z < MAX_DEEP)
            {
                velocity += new Vector3(0, 0, 10);
            }

            if (mouse.RightButton == ButtonState.Pressed)
            {
                if (keyboardState.IsKeyDown(Keys.A))
                {
                    if (wrist.d.Z > MIN_THETA_Z)
                        wrist.w += new Vector3(0, 0, -5);
                }
                if (keyboardState.IsKeyDown(Keys.W))
                {
                    if (hand.d.Y < MAX_THETA_X)
                        hand.w += new Vector3(0, 5, 0);
                }
                if (keyboardState.IsKeyDown(Keys.S))
                {
                    if (hand.d.Y > MIN_THETA_X)
                        hand.w += new Vector3(0, -5, 0);
                }
                if (keyboardState.IsKeyDown(Keys.D))
                {
                    if (wrist.d.Z < MAX_THETA_Z)
                        wrist.w += new Vector3(0, 0, 5);
                }
            }
            else
            {
                if (keyboardState.IsKeyDown(Keys.A))
                {
                    if (position.X > -MAX_LATTERAL)
                        velocity += new Vector3(-10, 0, 0);
                }
                if (keyboardState.IsKeyDown(Keys.W))
                {
                    if (position.Y < MAX_HEIGHT)
                        velocity += new Vector3(0, 10, 0);
                }
                if (keyboardState.IsKeyDown(Keys.S))
                {
                    if (position.Y > MIN_HEIGHT)
                        velocity += new Vector3(0, -10, 0);
                }
                if (keyboardState.IsKeyDown(Keys.D))
                {
                    if (position.X < MAX_LATTERAL)
                        velocity += new Vector3(10, 0, 0);
                }
            }
            //wrist.d.X = WrapAngle(wrist.d.X);
            //wrist.d.Y = WrapAngle(wrist.d.Y);
            //wrist.d.Z = WrapAngle(wrist.d.Z);
            //hand.d.X = WrapAngle(hand.d.X);
            //hand.d.Y = WrapAngle(hand.d.Y);
            //hand.d.Z = WrapAngle(hand.d.Z);
        }

        private static float WrapAngle(float radians)
        {
            while (radians < -MathHelper.Pi)
            {
                radians += MathHelper.TwoPi;
            }
            while (radians > MathHelper.Pi)
            {
                radians -= MathHelper.TwoPi;
            }
            return radians;
        }
    }
}
