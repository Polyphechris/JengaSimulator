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
        static float FINGER_LENGTH = 12;
        public Model model;
        Matrix world;

        public Vector3 color;
        float alpha;
        //angular speeds
        public Vector3 position;
        public Vector3 d;
        public Vector3 w;

        Vector3 fingerTip;
        public Vector3 scale;
        public Vector3 offset;

        List<Vector3> CollisionPoints;
        Model touchPoint;

        public Hand()
        {
            offset = new Vector3(0, 0, -3.75f - 3);
            w = Vector3.Zero;
            d.Y = (float)Math.PI;
            alpha = 1f;
            scale = Vector3.One;
        }

        public void update(float time)
        {
            d = d + w * time / 1000;
            //UpdateRotation(d, time);
            world = Matrix.CreateScale(scale) *
                Matrix.CreateTranslation(-offset) * Matrix.CreateFromYawPitchRoll(WrapAngle(d.X), WrapAngle(d.Y), WrapAngle(d.Z)) * Matrix.CreateTranslation(offset)
                * Matrix.CreateTranslation(position);
        }
        private void UpdateRotation(Vector3 aPreviousRotation, float time)
        {            
            if (!w.Equals(Vector3.Zero))
            {
                d.X = w.X / 10 * time / 1000 + aPreviousRotation.X;
                d.X = RebalanceRotation(d.X);

                //  Condition to fix the yaw/pitch problem
                if (d.X > Math.PI/2 && d.X < 3*Math.PI/2)
                {
                    d.Y = w.Y / 10 * time / 1000 + aPreviousRotation.Y;
                    d.Y = RebalanceRotation(d.Y);
                }
                else
                {
                    d.Y = -w.Y / 10 + aPreviousRotation.Y;
                    d.Y = RebalanceRotation(d.Y);
                }
            }
        }

        /// <summary>
        /// Checks to see if rotations exceed 0-360 degrees
        /// </summary>
        private float RebalanceRotation(float aRotation)
        {
            float rebalancedRotation = aRotation;

            if (aRotation > 2*Math.PI)
            {
                rebalancedRotation -= 2 * (float)Math.PI;
            }
            else if (aRotation < 0.0f)
            {
                rebalancedRotation += 2 * (float)Math.PI;
            }
            return rebalancedRotation;
        }

        public void draw()
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.AmbientLightColor = new Vector3(0.55f);
                   // effect.SpecularColor = color;
                    effect.DiffuseColor = color;
                    //effect.EmissiveColor = color;
                    effect.World = world;
                    effect.View = Game1.view;
                    effect.Projection = Game1.projection;
                    effect.Alpha = alpha;
                }
                mesh.Draw();
            }
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

        public Vector3 GetFingerTip()
        {
            return position + new Vector3(0, 0, FINGER_LENGTH);
        }
    }
}
