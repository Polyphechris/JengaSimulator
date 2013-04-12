using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace JengaSimulator
{
    public class Block
    {
        Model model;
        Matrix world;

        //linear motion
        public Vector3 position;
        public Vector3 velocity;
        public Vector3 acceleration;

        public Vector3 scale;
        List<Vector4> Impulses;
        float weight;
        Vector3 color;
        float alpha;

        //angular speeds
        Vector3 w;
        Vector3 a;
        Vector3 d; 

        public Block(Vector3 p, Vector3 s, float mass, Vector3 c, Model m, bool isStatic)
        {
            model = m;
            position = p;
            alpha = 1;
            color = c;
            weight = mass;
            scale = s;
            velocity = Vector3.Zero;
            if (isStatic)
                acceleration = Vector3.Zero;
            else
                acceleration = new Vector3(0,-10,0);

            d = Vector3.Zero;
            a = Vector3.Zero;
            w = Vector3.Zero;
            Impulses = new List<Vector4>();
        }

        public void Update(float time)
        {
            w = w + a * time / 1000;
            d = d + w * time / 1000;
            velocity = velocity + acceleration * time/1000;
            position = position + velocity * time/1000;

            world =  Matrix.CreateScale(scale) * Matrix.CreateFromYawPitchRoll(d.X, d.Y, d.Z) * Matrix.CreateTranslation(position);
        }

        public void Draw()
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.SpecularColor = color;
                    effect.DiffuseColor = color;
                    effect.EmissiveColor = color;
                    effect.World = Matrix.CreateScale(scale) * Matrix.CreateTranslation(position);
                    effect.View = Game1.view;
                    effect.Projection = Game1.projection;
                    effect.Alpha = alpha;
                }
                mesh.Draw();
            }
        }

        public void ResolveCollision(Block block)
        {

        }
    }
}
