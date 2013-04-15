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
        static float MAX_HEIGHT = 0;
        static float MAX_LATTERAL = 10;
        static float MAX_DEEP = 10;
        static float MIN_HEIGHT = -40;
        Wrist wrist;
        Hand hand;

        Model model;
        Matrix world;

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
        }

        private void InitializeWrist()
        {
            hand = new Hand();
            hand.model = Content.Load<Model>("hand");
            wrist = new Wrist(hand);
            wrist.model = Content.Load<Model>("wrist");
        }

        public void update(float time, KeyboardState keyboardState)
        {
            MouseState mouse = Mouse.GetState();
            velocity = Vector3.Zero;
            if (mouse.LeftButton == ButtonState.Pressed)
            {
                if (position.Z > -MAX_DEEP)
                    velocity += new Vector3(0, 0, -10);
            }
            else if (position.Z < MAX_DEEP)
            {
                velocity += new Vector3(0, 0, 10);
            }
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

            position = position + velocity * time / 1000;
            wrist.update(time);
            wrist.position = position;
        }

        public void draw()
        {
            wrist.draw();
        }

    }
}
