using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace JengaSimulator
{
    public class Particle
    {
        public Model model;

        public Vector3 position;
        public Vector3 velocity;
        public Vector3 acceleration;
        public Vector3 color;

        public Vector3 size;
        public float alpha;

        private float age;
        private float lifespan;

        public static int particleCount;

        public Particle()
        {
            ++particleCount;
        }

        public void Initialize(Model m, Vector3 p, Vector3 v, Vector3 ac, Vector3 c, float a, Vector3 s, float l)
        {
            model = m;
            position = p;
            velocity = v;
            acceleration = ac;
            color = c;
            size = s;
            lifespan = l;
            alpha = a;
            age = 0;
        }

        public void update(float time)
        {
            velocity += acceleration * time/1000;
            position += velocity * time/1000;
            age += time/1000;
        }

        public bool isDead()
        {
            return (age > lifespan);
        }
        
        public void draw(Matrix view, Matrix projection)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.DiffuseColor = color;
                    effect.EmissiveColor = color;
                    effect.World = Matrix.CreateScale(size) * Matrix.CreateTranslation(position);
                    effect.View = view;
                    effect.Projection = projection;
                    effect.Alpha = alpha;
                }
                mesh.Draw();
            }
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.DiffuseColor = color;
                    effect.EmissiveColor = Vector3.Zero;
                    effect.World = Matrix.CreateScale(size * 1.9f) * Matrix.CreateTranslation(position);
                    effect.View = view;
                    effect.Projection = projection;
                    effect.Alpha = alpha;
                    //  effect.TextureEnabled = true;
                    //  effect.Texture = texture;
                }
                mesh.Draw();
            }
        }
    }
}
