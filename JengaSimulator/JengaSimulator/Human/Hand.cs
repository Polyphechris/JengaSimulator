using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace JengaSimulator.Human
{
    class Hand
    {
        static float FINGER_LENGTH = 10;
        public Model model;
        Matrix world;

        public Vector3 color;
        float alpha;
        //angular speeds
        public Vector3 position;
        public Vector3 d;
        Vector3 w;

        Vector3 fingerTip;
        public Vector3 scale;

        public Hand()
        {
            alpha = 1f;
            scale = Vector3.One;
        }

        public void update(float time)
        {
            d = d + w * time / 1000;
            world = Matrix.CreateScale(scale) * Matrix.CreateFromYawPitchRoll(d.X, d.Y, d.Z) * Matrix.CreateTranslation(position);
        }
        
        public void draw()
        {
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

        public Vector3 GetFingerTip()
        {
            return position + new Vector3(0, 0, FINGER_LENGTH);
        }
    }
}
