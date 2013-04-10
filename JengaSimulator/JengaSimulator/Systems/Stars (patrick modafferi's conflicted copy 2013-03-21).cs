using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PatrickModafferiA2.Systems
{
    class Stars : ParticleSystem
    {
        const int STAR_COUNT = 500;
        const float STAR_DISTANCE = 70;
        const float SPAWN_RATE = 1;//in seconds 
   
        Random random1;   
        
        public override void initialize()
        {
            spawnTimer = 0;
            random1 = new Random();
            particles = new List<Particle>(STAR_COUNT);
            Bursts = new List<ParticleSystem>();

            Random random = new Random();
            for (int i = 0; i < STAR_COUNT; ++i)
            {
                float phi = random.Next(0, 179);
                float theta = random.Next(0, 179);
                //compute the points on the sphere
                double posX = STAR_DISTANCE * Math.Cos(theta) * Math.Sin(phi);
                double posZ = STAR_DISTANCE * Math.Sin(theta) * Math.Sin(phi);
                double posY = STAR_DISTANCE * Math.Cos(phi);

                Particle newParticle = new Particle();

                Model m = this.content.Load<Model>("star");
                newParticle.Initialize(m, new Vector3((float)posX, (float)posY, (float)posZ), Vector3.Zero, Vector3.Zero, new Vector3(0.8f, 0.8f, 0.8f), 0.7f, new Vector3(0.05f), 1);
                particles.Add(newParticle);
            }
        }

        public override void update(float time)
        {
            spawnTimer += time/1000;
            if (spawnTimer >= SPAWN_RATE)
            {
                int randomStar = random1.Next(0, particles.Count - 1);
                Bursts.Add(new StarsShooting(particles.ElementAt(randomStar).position, content));
                spawnTimer = 0;
            }
            for (int j = 0; j < Bursts.Count; ++j)
            {
                ParticleSystem currentPS = Bursts.ElementAt(j);
                if (currentPS.particles.Count == 0)
                    Bursts.RemoveAt(j);
                else
                    currentPS.update(time);
            }
        }
    }
}
