using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace JengaSimulator
{
    class CollisionManager
    {
        public List<Block> Blocks;
        Block Ground;
        Block platform;
        ContentManager Content;

        public CollisionManager(ContentManager c)
        {
            Content = c;
            InitializeGround();
            InitializeTower();

        }

        private void InitializeGround()
        {
            Ground = new Block(new Vector3(0,-50,0), new Vector3(70, 10, 70),1, new Vector3(1f, 1f, 1f), Content.Load<Model>("grass"), true);
            platform = new Block(new Vector3(0, -50, 0), new Vector3(10, 20, 10), 1, new Vector3(0.2f, 0.2f, 0.2f), Content.Load<Model>("cube"), true);
        }

        private void InitializeTower()
        {
            Blocks = new List<Block>();
            Blocks.Add(new Block(new Vector3(0, -10, 0), new Vector3(2, 1, 1), 1, new Vector3(0.8f, 0.5f, 0.2f), Content.Load<Model>("cube"), false));
        }

        public void Update(float time)
        {
            foreach (Block b in Blocks)
            {
                b.Update(time);
                Collision(b, time);
            }
            Ground.Update(time);
            platform.Update(time);
        }

        public void Draw()
        {
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
