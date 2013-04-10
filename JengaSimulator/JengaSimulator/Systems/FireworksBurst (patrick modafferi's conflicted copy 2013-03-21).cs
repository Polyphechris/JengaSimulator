using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace PatrickModafferiA2.Systems
{
    //Bursts that explode eventually in it's own set of particles
    // Explosion location is the source of this new particle system
    class FireworksBurst : ParticleSystem
    {
        const int BURST_COUNT = 200;
        const int INITIAL_UP_VELOCITY = 5;
        const int INITIAL_Y_ACC = -5;

        public FireworksBurst(Vector3 s, ContentManager c)
        {
            content = c;
            source = s;
            particles = new List<Particle>();
            initialize();
        }

        public override void initialize()
        {
            spawBurst(BURST_COUNT);
        }

        public void spawBurst(int count)
        {
            Random random = new Random();
            for (int i = 0; i <= count; ++i)
            {
                float phi = random.Next(0, 179);
                float theta = random.Next(0, 179);
                //compute the points on the sphere
                double posX = INITIAL_UP_VELOCITY * Math.Cos(theta) * Math.Sin(phi);
                double posZ = INITIAL_UP_VELOCITY * Math.Sin(theta) * Math.Sin(phi);
                double posY = INITIAL_UP_VELOCITY * Math.Cos(phi);

                phi = random.Next(0, 179);
                theta = random.Next(0, 179);
                //compute the random colors
                double R = Math.Cos(theta) * Math.Sin(phi);
                double G = Math.Sin(theta) * Math.Sin(phi);
                double B = Math.Cos(phi);                 

                Vector3 initialV = new Vector3((float)posX, (float)posY, (float)posZ);
                initialV = initialV * INITIAL_UP_VELOCITY;
                Particle p = new Particle();

                Model m = content.Load<Model>("star");
                p.Initialize(m, source, initialV, initialV / INITIAL_Y_ACC + new Vector3(0,INITIAL_Y_ACC,0) , 
                    new Vector3((float)R, (float)G, (float)B), 1, new Vector3(0.12f + (float)(G/10)), 1 + (float)R);
                particles.Add(p);
            }
        }

        public override void update(float time)
        {
            for (int i = 0; i < particles.Count; ++i)
            {
                particles.ElementAt(i).update(time);
                if (particles.ElementAt(i).isDead())
                {
                    particles.RemoveAt(i);
                }
            }
        }
    }
}
