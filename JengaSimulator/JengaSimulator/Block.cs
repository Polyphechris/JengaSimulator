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
        public Vector3 previousPosition;
        public Vector3 velocity;
        public Vector3 acceleration;

        //hack for rotation abotu an axis
        public Vector3 offsetRotation;

        public Vector3 scale;
        List<Vector4> impulses;
        List<Vector3> forces;
        float weight;
        public Vector3 color;
        public float alpha;
        
        //angular speeds
        public Vector3 w;
        Vector3 a;
        public Vector3 d;
        bool isStatic;
        //on hand bounding boxes
        public bool onHand;
        bool resting;
        public float specular;

        Vector3[][][] vertex;

        public Block(Vector3 p, Vector3 s, float mass, Vector3 c, Model m, bool i)
        {
            specular = 10;
            onHand = false;
            offsetRotation = Vector3.Zero;
            isStatic = i;
            resting = i;
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
            impulses = new List<Vector4>();
            forces = new List<Vector3>();

            world = Matrix.CreateScale(scale) * Matrix.CreateFromYawPitchRoll(WrapAngle(d.X), WrapAngle(d.Y), WrapAngle(d.Z)) * Matrix.CreateTranslation(position);
        }

        public void Update(float time)
        {
            Vector3 totalA = acceleration;
            foreach (Vector3 f in forces)
            {
                totalA += f;
            }
            for (int i = 0; i < impulses.Count; ++i)
            {
                impulses[i] += new Vector4(0, 0, 0, time);
                if (impulses[i].W > 50)
                {
                    impulses.RemoveAt(i);
                }
                else
                {
                    Vector3 J = new Vector3(impulses[i].X, impulses[i].Y, impulses[i].Z);
                    totalA += J;
                }
            }

            w = w + a * time / 1000;
            d = d + w * time / 1000;
            velocity = velocity + totalA * time / 1000;

            previousPosition = new Vector3(position.X, position.Y, position.Z);
            position = position + velocity * time/1000;

            world = Matrix.CreateScale(scale) * Matrix.CreateTranslation(-offsetRotation) * 
                Matrix.CreateFromYawPitchRoll(WrapAngle(d.X), WrapAngle(d.Y), WrapAngle(d.Z)) * Matrix.CreateTranslation(offsetRotation) * 
                Matrix.CreateTranslation(position);

            forces = new List<Vector3>();
        }

        public void Draw()
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.SpecularColor = color;
                    effect.SpecularPower = specular;
                    effect.DiffuseColor = color;
                    effect.EmissiveColor = color;
                    if (onHand)
                    {
                        effect.World = world;
                    }
                    else
                    {
                        effect.World = world * Game1.rotation;
                    }
                    effect.View = Game1.view;
                    effect.Projection = Game1.projection;
                    effect.Alpha = alpha;
                }
                mesh.Draw();
            }
        }

        public bool Collides(Block b1)
        {
            if(UpdateBoundingBox(model, world).Intersects(UpdateBoundingBox(b1.model, b1.world)))
            {
                return true;
            }
            return false;
        }

        public void ResolveCollision(Block block)
        {
            if (!isStatic)
            {
                //Force can be one of 4 directions
                //Figure out which face of the cube we hit
                 float blockRight = position.X + scale.X;
                float blockLeft = position.X - scale.X;
                float blockTop = position.Y + scale.Y;
                float blockBottom = position.Y - scale.Y;
                float blockFront = position.Z + scale.Z;
                float blockBack = position.Z - scale.Z;

                float wallRight = block.position.X + block.scale.X;
                float wallLeft = block.position.X - block.scale.X;
                float wallTop = block.position.Y + block.scale.Y;
                float wallBottom = block.position.Y - block.scale.Y;
                float wallFront = block.position.Z + block.scale.Z;
                float wallBack = block.position.Z - block.scale.Z;

                if (previousPosition != Vector3.Zero)
                {
                    block.position = block.previousPosition;
                }

                Vector4 newImpulse = Vector4.Zero;
                //float magnitude = velocity.Length();
                float magnitude = velocity.Length();

                if (block.isStatic || block.resting)
                {
                    magnitude = -acceleration.Y;
                    forces.Add(new Vector3(0, 1 * magnitude, 0));
                    velocity = Vector3.Zero;
                    //resting = true;
                }

                if (blockRight >= wallRight && blockLeft <= wallRight)
                {
                    newImpulse = new Vector4(1, 0, 0, 0);
                    impulses.Add(newImpulse * magnitude);
                }
                if (blockLeft <= wallLeft && blockRight >= wallLeft)
                {
                    newImpulse = new Vector4(-1, 0, 0, 0);
                    impulses.Add(newImpulse * magnitude);
                }
                if (blockTop >= wallTop && blockBottom <= wallTop)
                {
                    newImpulse = new Vector4(0, 1, 0, 0);
                    impulses.Add(newImpulse * magnitude);
                }
                if (blockTop >= wallBottom && blockBottom <= wallBottom)
                {
                    newImpulse = new Vector4(0, -1, 0, 0);
                    impulses.Add(newImpulse * magnitude);
                }
                if (blockFront >= wallFront && blockBack <= wallFront)
                {
                    newImpulse = new Vector4(0, 0, 1, 0);
                    impulses.Add(newImpulse * magnitude);
                }
                if (blockFront >= wallBack && blockBack <= wallBack)
                {
                    newImpulse = new Vector4(0, 0, -1, 0);
                    impulses.Add(newImpulse * magnitude);
                }
            }
        }

        //Method obtained from online discussion boards at Stack Overflow
        //http://gamedev.stackexchange.com/questions/2438/how-do-i-create-bounding-boxes-with-xna-4-0
        protected BoundingBox UpdateBoundingBox(Model model, Matrix worldTransform)
        {
            // Initialize minimum and maximum corners of the bounding box to max and min values
            Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            // For each mesh of the model
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                {
                    // Vertex buffer parameters
                    int vertexStride = meshPart.VertexBuffer.VertexDeclaration.VertexStride;
                    int vertexBufferSize = meshPart.NumVertices * vertexStride;

                    // Get vertex data as float
                    float[] vertexData = new float[vertexBufferSize / sizeof(float)];
                    meshPart.VertexBuffer.GetData<float>(vertexData);

                    // Iterate through vertices (possibly) growing bounding box, all calculations are done in world space
                    for (int i = 0; i < vertexBufferSize / sizeof(float); i += vertexStride / sizeof(float))
                    {
                        Vector3 transformedPosition = Vector3.Transform(new Vector3(vertexData[i], vertexData[i + 1], vertexData[i + 2]), worldTransform);

                        min = Vector3.Min(min, transformedPosition);
                        max = Vector3.Max(max, transformedPosition);
                    }
                }
            }

            // Create and return bounding box
            return new BoundingBox(min, max);
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
    }
}
