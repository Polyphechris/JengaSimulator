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
        Block Ground;
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
            Blocks.Add(new Block(new Vector3(0, -14, blockWidth * 2), new Vector3(blockLength, blockHeight, blockWidth), 1, new Vector3(0.7f, 0.4f, 0.1f), Content.Load<Model>("cube"), false));
            Blocks.Add(new Block(new Vector3(0, -14, -blockWidth * 2), new Vector3(blockLength, blockHeight, blockWidth), 1, new Vector3(0.7f, 0.4f, 0.1f), Content.Load<Model>("cube"), false));
            Blocks.Add(new Block(new Vector3(0, -14, 0), new Vector3(blockLength, blockHeight, blockWidth), 1, new Vector3(0.7f, 0.4f, 0.1f), Content.Load<Model>("cube"), false));
            Blocks.Add(new Block(new Vector3(blockWidth * 2, -10, 0), new Vector3(blockWidth, blockHeight, blockLength), 1, new Vector3(0.7f, 0.4f, 0.1f), Content.Load<Model>("cube"), false));
            Blocks.Add(new Block(new Vector3(0, -10, 0), new Vector3(blockWidth, blockHeight, blockLength), 1, new Vector3(0.7f, 0.4f, 0.1f), Content.Load<Model>("cube"), false));
            Blocks.Add(new Block(new Vector3(-blockWidth * 2, -10, 0), new Vector3(blockWidth, blockHeight, blockLength), 1, new Vector3(0.7f, 0.4f, 0.1f), Content.Load<Model>("cube"), false));
            //Blocks.Add(new Block(new Vector3(0, -6, 3), new Vector3(blockLength, blockHeight, blockWidth), 1, new Vector3(0.7f, 0.4f, 0.1f), Content.Load<Model>("cube"), false));
            //Blocks.Add(new Block(new Vector3(0, -6, -3), new Vector3(blockLength, blockHeight, blockWidth), 1, new Vector3(0.7f, 0.4f, 0.1f), Content.Load<Model>("cube"), false));
            //Blocks.Add(new Block(new Vector3(3, -2, 0), new Vector3(blockWidth, blockHeight, blockLength), 1, new Vector3(0.7f, 0.4f, 0.1f), Content.Load<Model>("cube"), false));
            //Blocks.Add(new Block(new Vector3(-3, -2, 0), new Vector3(blockWidth, blockHeight, blockLength), 1, new Vector3(0.7f, 0.4f, 0.1f), Content.Load<Model>("cube"), false));
            //Blocks.Add(new Block(new Vector3(0, 2, 3), new Vector3(blockLength, blockHeight, blockWidth), 1, new Vector3(0.7f, 0.4f, 0.1f), Content.Load<Model>("cube"), false));
            //Blocks.Add(new Block(new Vector3(0, 2, -3), new Vector3(blockLength, blockHeight, blockWidth), 1, new Vector3(0.7f, 0.4f, 0.1f), Content.Load<Model>("cube"), false));
            //Blocks.Add(new Block(new Vector3(3, 6, 0), new Vector3(blockWidth, blockHeight, blockLength), 1, new Vector3(0.7f, 0.4f, 0.1f), Content.Load<Model>("cube"), false));
            //Blocks.Add(new Block(new Vector3(-3, 6, 0), new Vector3(blockWidth, blockHeight, blockLength), 1, new Vector3(0.7f, 0.4f, 0.1f), Content.Load<Model>("cube"), false));
            //Blocks.Add(new Block(new Vector3(0, 10, 3), new Vector3(blockLength, blockHeight, blockWidth), 1, new Vector3(0.7f, 0.4f, 0.1f), Content.Load<Model>("cube"), false));
            //Blocks.Add(new Block(new Vector3(0, 10, -3), new Vector3(blockLength, blockHeight, blockWidth), 1, new Vector3(0.7f, 0.4f, 0.1f), Content.Load<Model>("cube"), false));
        }

        public void Update(float time, KeyboardState keyboardState)
        {
            arm.update(time, keyboardState);
            if (Game1.systemState == SystemState.Idle)
            {
                bool foundCollision = false;
                foreach (Block finger in arm.fingers)
                {
                    foreach (Block b in Blocks)
                    {
                        if (finger.Collides(b))
                        {
                            Game1.systemState = SystemState.Collision;
                            foundCollision = true;
                            break;
                        }
                    }

                    if (foundCollision)
                    {
                        break;
                    }
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
                    b.Update(time);
                    Collision(b, time);
                }
                Ground.Update(time);
                platform.Update(time);

                bool changeState = false;
                //check velocity of all blocks to see if they are no longer moving (collisions are all done)
                foreach (Block b in Blocks)
                {
                    if (b.position.X != b.previousPosition.X &&
                        b.position.Y != b.previousPosition.Y &&
                        b.position.Z != b.previousPosition.Z)
                    {
                        changeState = true;
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
            foreach (Block b1 in Blocks)
            {
                if (b1 != b)
                {
                    if (b.Collides(b1))
                    {
                        b.ResolveCollision(b1);
                        b1.ResolveCollision(b);
                    }
                }
            }
            if (b.Collides(Ground))
            {
                b.ResolveCollision(Ground);
            }
            if (b.Collides(platform))
            {
                b.ResolveCollision(platform);
            }
        }
        
    }
}
