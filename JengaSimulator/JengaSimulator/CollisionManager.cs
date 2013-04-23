using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JengaSimulator.Human;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace JengaSimulator
{
    class CollisionManager
    {
        public List<Block> Blocks;
        public Block Ground;
        Block platform;
        ContentManager Content;
        Arm arm;

        public CollisionManager(ContentManager c)
        {
            Content = c;
            InitializeGround();
            InitializeTower();
            arm = new Arm(Content);
        }

        private void InitializeGround()
        {
            Ground = new Block(new Vector3(0,-50,0), new Vector3(70, 10, 70),1, new Vector3(1f, 1f, 1f), Content.Load<Model>("grass"), true);
            platform = new Block(new Vector3(0, -50, 0), new Vector3(10, 20, 10), 1, new Vector3(0.2f, 0.2f, 0.2f), Content.Load<Model>("cube"), true);
        }

        private void InitializeTower()
        {
            float blockLength = 5f;
            float blockHeight = 1f/5f * blockLength;
            float blockWidth = 1f/3f * blockLength;

            Blocks = new List<Block>();

            if (Game1.resetWithOneBlock)
            {
                Blocks.Add(new Block(new Vector3(0, -14, blockWidth * 2), new Vector3(blockLength, blockHeight, blockWidth), 1, new Vector3(0.7f, 0.4f, 0.1f), Content.Load<Model>("cube"), false));
            }
            else
            {
                Blocks.Add(new Block(new Vector3(0, -14, blockWidth * 2), new Vector3(blockLength, blockHeight, blockWidth), 1, new Vector3(0.7f, 0.4f, 0.1f), Content.Load<Model>("cube"), false));
                Blocks.Add(new Block(new Vector3(0, -14, -blockWidth * 2), new Vector3(blockLength, blockHeight, blockWidth), 1, new Vector3(0.7f, 0.4f, 0.1f), Content.Load<Model>("cube"), false));
                Blocks.Add(new Block(new Vector3(0, -14, 0), new Vector3(blockLength, blockHeight, blockWidth), 1, new Vector3(0.7f, 0.4f, 0.1f), Content.Load<Model>("cube"), false));
                Blocks.Add(new Block(new Vector3(blockWidth * 2, -10, 0), new Vector3(blockWidth, blockHeight, blockLength), 1, new Vector3(0.7f, 0.4f, 0.1f), Content.Load<Model>("cube"), false));
                Blocks.Add(new Block(new Vector3(0, -10, 0), new Vector3(blockWidth, blockHeight, blockLength), 1, new Vector3(0.7f, 0.4f, 0.1f), Content.Load<Model>("cube"), false));
                Blocks.Add(new Block(new Vector3(-blockWidth * 2, -10, 0), new Vector3(blockWidth, blockHeight, blockLength), 1, new Vector3(0.7f, 0.4f, 0.1f), Content.Load<Model>("cube"), false));
                Blocks.Add(new Block(new Vector3(0, -6, 3), new Vector3(blockLength, blockHeight, blockWidth), 1, new Vector3(0.7f, 0.4f, 0.1f), Content.Load<Model>("cube"), false));
                Blocks.Add(new Block(new Vector3(0, -6, -3), new Vector3(blockLength, blockHeight, blockWidth), 1, new Vector3(0.7f, 0.4f, 0.1f), Content.Load<Model>("cube"), false));
                Blocks.Add(new Block(new Vector3(3, -2, 0), new Vector3(blockWidth, blockHeight, blockLength), 1, new Vector3(0.7f, 0.4f, 0.1f), Content.Load<Model>("cube"), false));
                Blocks.Add(new Block(new Vector3(-3, -2, 0), new Vector3(blockWidth, blockHeight, blockLength), 1, new Vector3(0.7f, 0.4f, 0.1f), Content.Load<Model>("cube"), false));
                Blocks.Add(new Block(new Vector3(0, 2, 3), new Vector3(blockLength, blockHeight, blockWidth), 1, new Vector3(0.7f, 0.4f, 0.1f), Content.Load<Model>("cube"), false));
                Blocks.Add(new Block(new Vector3(0, 2, -3), new Vector3(blockLength, blockHeight, blockWidth), 1, new Vector3(0.7f, 0.4f, 0.1f), Content.Load<Model>("cube"), false));
                Blocks.Add(new Block(new Vector3(3, 6, 0), new Vector3(blockWidth, blockHeight, blockLength), 1, new Vector3(0.7f, 0.4f, 0.1f), Content.Load<Model>("cube"), false));
                Blocks.Add(new Block(new Vector3(-3, 6, 0), new Vector3(blockWidth, blockHeight, blockLength), 1, new Vector3(0.7f, 0.4f, 0.1f), Content.Load<Model>("cube"), false));
                Blocks.Add(new Block(new Vector3(0, 10, 3), new Vector3(blockLength, blockHeight, blockWidth), 1, new Vector3(0.7f, 0.4f, 0.1f), Content.Load<Model>("cube"), false));
                Blocks.Add(new Block(new Vector3(0, 10, -3), new Vector3(blockLength, blockHeight, blockWidth), 1, new Vector3(0.7f, 0.4f, 0.1f), Content.Load<Model>("cube"), false));
            }
        }

        public void Update(float time, KeyboardState keyboardState)
        {
            arm.update(time, keyboardState);
            bool foundCollision = false;
            foreach (Block finger in arm.fingers)
            {
                foreach (Block b in Blocks)
                {
                    if (finger.Collides(b))
                    {
                        Game1.systemState = SystemState.Collision;
                        foundCollision = true;
                        b.ResolveCollision(finger);
                        break;
                    }
                }
            }

            if (Game1.systemState == SystemState.Idle)
            {
                if (foundCollision)
                {
                    Game1.systemState = SystemState.Collision;
                }
            }

            if (Game1.systemState == SystemState.Collision)
            {
                foreach (Block b in Blocks)
                {
                    if (b.acceleration.X.Equals(float.NaN))
                    {
                        b.color = Vector3.Zero;
                    }
                    Collision(b, time);
                    UpdatePenetration(b);
                    b.ApplyFriction(b.previousVelocity);
                    b.Update(time);
                }
                Ground.Update(time);
                platform.Update(time);

                bool changeState = true;
                //check velocity of all blocks to see if they are no longer moving (collisions are all done)
                foreach (Block b in Blocks)
                {
                    if (b.velocity.Length() >= 0.16f || !b.resting)
                    {
                        changeState = false;
                        break;
                    }
                }

                if (changeState)
                {
                    Game1.systemState = SystemState.Idle;
                }
            }
        }

        public void Draw()
        {
            arm.draw();
            foreach (Block b in Blocks)
            {
                b.Draw();
            }
            Ground.Draw();
            platform.Draw();
        }

        private void Collision(Block b, float time)
        {
            bool resting = false;

            foreach (Block b1 in Blocks)
            {
                if (b1 != b)
                {
                    if (b.Collides(b1))
                    {
                        b.ResolveCollision(b1);
                        if (b.IsResting(b1))
                        {
                            resting = true;
                        }
                        //b1.ResolveCollision(b);
                    }
                }
            }

            b.resting = resting;

            if (b.Collides(Ground))
            {
                b.ResolveCollision(Ground);
            }
            if (b.Collides(platform))
            {
                b.ResolveCollision(platform);
            }
        }
        
        private void UpdatePenetration(Block b)
        {
            foreach (Block b1 in Blocks)
            {
                if (b1 != b)
                {
                    if (b.Collides(b1))
                    {
                        if (b.previousPosition != Vector3.Zero &&
                            b.previousVelocity != Vector3.Zero)
                        {
                           //b.position = b.previousPosition;
                           //b1.position = b1.previousPosition;
                           //b.velocity = b.previousVelocity;
                        }
                    }
                }
            }
        }
    }
}
