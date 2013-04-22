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
        static float MAX_HEIGHT = 0;
        static float MAX_LATTERAL = 10;
        static float MAX_DEEP = 27;
        static float MIN_DEEP = 10;
        static float MIN_HEIGHT = -28;

        //Hand angular Constraints
        static float MIN_THETA_Z = 0;
        static float MAX_THETA_Z = 3 * (float)Math.PI/2;
        static float MIN_THETA_X = 2f *(float)Math.PI/3;
        static float MAX_THETA_X = 4f *(float)Math.PI/3;

        Wrist wrist;
        Hand hand;

        Model model;
        Matrix world;

        //Palm + fingers
        Matrix BoxWorld;
        public Block collisionBox;
        public List<Block> fingers;

        //linear motion
        public Vector3 position;
        public Vector3 velocity;
        public Vector3 acceleration;

        public Vector3 scale;
        float weight;
        public Vector3 color;
        float alpha;
        static public bool showBoxes;
        ContentManager Content;

        public Arm(ContentManager c)
        {
            showBoxes = false;
            alpha = 1f;
            position = new Vector3(0, -10, MAX_DEEP);
            Content = c;
            InitializeWrist();
            
            collisionBox = new Block(position + new Vector3(0,0, -12), new Vector3(2.25f,1.25f, 3), 1, color, Content.Load<Model>("cube"),false);
            collisionBox.alpha = 0.4f;
            collisionBox.onHand = true;
            collisionBox.offsetRotation = new Vector3(0, 0, 1.5f);

            fingers = new List<Block>();
            Block b1 = new Block(position + new Vector3(-1.5f, 0, -17), new Vector3(0.5f, 0.5f, 3), 1, color, Content.Load<Model>("cube"), false);
            b1.alpha = 0.4f;
            b1.onHand = true;
            b1.offsetRotation = new Vector3(1.5f, 0, 7.5f);
            fingers.Add(b1);
            Block b2 = new Block(position + new Vector3(0, 0, -17), new Vector3(0.5f, 0.5f, 3.2f), 1, color, Content.Load<Model>("cube"), false);
            b2.alpha = 0.4f;
            b2.onHand = true;
            b2.offsetRotation = new Vector3(0, 0, 7.5f);
            fingers.Add(b2);
            Block b3 = new Block(position + new Vector3(1.5f, 0, -17), new Vector3(0.5f, 0.5f, 3), 1, color, Content.Load<Model>("cube"), false);
            b3.alpha = 0.4f;
            b3.onHand = true;
            b3.offsetRotation = new Vector3(-1.5f, 0, 7.5f);
            fingers.Add(b3);
        }

        private void InitializeWrist()
        {
            color = new Vector3(0.90f,0.66f,0.60f);

            hand = new Hand();
            hand.color = color;
            hand.model = Content.Load<Model>("hand1");

            wrist = new Wrist(hand);
            wrist.color = color;
            wrist.model = Content.Load<Model>("wrist1");
        }

        public void update(float time, KeyboardState keyboardState)
        {

            if (keyboardState.IsKeyDown(Keys.B))
            {
                showBoxes = false;
            }
            if (keyboardState.IsKeyDown(Keys.B) &&
                keyboardState.IsKeyDown(Keys.LeftShift))
            {
                showBoxes = true;
            }
            velocity = Vector3.Zero;
            wrist.w = Vector3.Zero;
            hand.w = Vector3.Zero;
            HandleInput(keyboardState);
            position = position + velocity * time / 1000;
            wrist.update(time);
            wrist.position = position;

            collisionBox.velocity = velocity;
            if (hand.d.X < 0) hand.w.Z = wrist.w.Z;
            else hand.w.Z = -wrist.w.Z;
            collisionBox.w = hand.w;
            collisionBox.Update(time);
            foreach (Block b in fingers)
            {
                b.velocity = velocity;
                if (hand.d.X < 0) hand.w.Z = wrist.w.Z;
                else hand.w.Z = -wrist.w.Z;
                b.w = hand.w;
                b.Update(time);
            }
        }

        public void draw()
        {
            if (showBoxes)
            {
                collisionBox.Draw();
                foreach (Block b in fingers)
                {
                    b.Draw();
                }
            }
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
