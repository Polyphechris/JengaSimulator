using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace JengaSimulator.Human
{  
    class Wrist
    {
        static float WRIST_LENGHT = 10;
        public Model model;
        Matrix world;

        public Vector3 color;
        public Vector3 position;
        public Vector3 d;
        float alpha;
        Hand hand;
        public Vector3 scale;

        public Wrist(Hand h)
        {
            alpha = 1f;
            hand = h;
            scale = Vector3.One;
            h.position = position + new Vector3(0,0,WRIST_LENGHT/2);
        }

        public void update(float time)
        {
            hand.position = position + new Vector3(0, 0, WRIST_LENGHT / 2);
            hand.update(time);
            world = Matrix.CreateScale(scale) * Matrix.CreateFromYawPitchRoll(d.X, d.Y, d.Z) * Matrix.CreateTranslation(position);           
        }

        public void draw()
        {
            hand.draw();
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.SpecularColor = color;
                    effect.DiffuseColor = color;
                    effect.EmissiveColor = color;
                    effect.World = world;
                    effect.View = Game1.view;
                    effect.Projection = Game1.projection;
                    effect.Alpha = alpha;
                }
                mesh.Draw();
            }
        }
    }
}
