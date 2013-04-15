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
        static float WRIST_LENGHT = -12f;
        public Model model;
        Matrix world;

        public Vector3 color;
        public Vector3 position;
        public Vector3 d;
        public Vector3 w;
        float alpha;
        Hand hand;
        public Vector3 scale;

        public Wrist(Hand h)
        {
            w = Vector3.Zero;
            alpha = 1f;
            hand = h;
            scale = new Vector3(0.65f,0.65f,2);
            d.Z = (float)Math.PI / 2;
            hand.d.Z = -d.Z;
            h.position = position + new Vector3(0,0,WRIST_LENGHT/2);
        }

        public void update(float time)
        {
            d = d + w * time / 1000;

            hand.position = position + new Vector3(0, 0, WRIST_LENGHT / 2);
            if (hand.d.X < 0) hand.w.Z = w.Z;
            else hand.w.Z = -w.Z;
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
                    effect.AmbientLightColor = new Vector3(0.3f);
                   // effect.SpecularColor = color;
                    effect.DiffuseColor = color;
                   // effect.EmissiveColor = color;
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
