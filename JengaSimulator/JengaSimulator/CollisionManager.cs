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
            }
            Ground.Update(time);
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
    }
}
