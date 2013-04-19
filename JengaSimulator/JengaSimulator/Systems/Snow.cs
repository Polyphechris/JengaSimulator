using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace JengaSimulator.Systems
{
    class Snow : ParticleSystem
    {
        const int DROP_COUNT = 2000;
        const int DROP_HEIGHT = 80;
        Random random1;   

        public override void initialize()
        {
            spawnTimer = 0;
            random1 = new Random();
            particles = new List<Particle>(DROP_COUNT);
            Bursts = new List<ParticleSystem>();

            for (int i = 0; i < DROP_COUNT; ++i)
            {
                float theta = random1.Next(-DROP_HEIGHT, DROP_HEIGHT);
                float phi = random1.Next(-DROP_HEIGHT, DROP_HEIGHT);
                float scaler = random1.Next(1, 3);
                float h = random1.Next(-30, DROP_HEIGHT);
                float acc = random1.Next(5, 12);
                //compute the points on the sphere                
                double posX = theta;
                double posZ = phi;
                double posY = h;
                Particle newParticle = new Particle();

                Model m = this.content.Load<Model>("star");
                newParticle.Initialize(m, new Vector3((float)posX, (float)posY, (float)posZ), Vector3.Zero, new Vector3(0, -acc, 0),
                    new Vector3(0.75f, 0.75f, 0.75f), 0.7f, new Vector3(0.1f * scaler), 1);
                particles.Add(newParticle);
            }
        }

        private void respawnParticle(Particle p)
        {
            float theta = random1.Next(-DROP_HEIGHT, DROP_HEIGHT);
            float phi = random1.Next(-DROP_HEIGHT, DROP_HEIGHT);
            float acc = random1.Next(5, 12);
            float scaler = random1.Next(1, 3);
            //compute the points on the sphere                
            double posX = theta;
            double posZ = phi;
            double posY = DROP_HEIGHT + acc*10;
            p.position = new Vector3((float)posX, (float)posY, (float)posZ);
            p.velocity = Vector3.Zero;
            p.acceleration = new Vector3(0, -acc, 0);
            p.size = new Vector3(0.1f * scaler);
        }

        public override void update(float time)
        {
            foreach (Particle p in particles)
            {
                p.update(time);
                if (p.position.Y < -40)
                {
                    respawnParticle(p);
                }
            }

        }
    }
}
