using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace JengaSimulator.Systems
{
    class Fireworks : ParticleSystem
    {
        const int BURST_COUNT = 20;
        const int INITIAL_UP_VELOCITY = 8;
        const int INITIAL_Y_ACC = -10;
        Random random;

        public Fireworks(Vector3 s)
        {
            trails = new List<ParticleSystem>(BURST_COUNT);
            source = s;
            particles = new List<Particle>(BURST_COUNT);
        }
        public override void initialize()
        {
            Bursts = new List<ParticleSystem>();
            spawBurst(BURST_COUNT);
        }

        public void spawBurst(int count)
        {
            random = new Random();
            for (int i = 0; i < count; ++i)
            {                
                particles.Add(InitializeRocket(new Particle()));
                trails.Insert(particles.Count-1, new FireworksTrail());
            }
        }

        public void resetBurst(int index)
        {
            InitializeRocket(particles.ElementAt(index));

            Particle.particleCount -= trails.ElementAt(index).particles.Count;
            trails.RemoveAt(index);
            trails.Insert(index, new FireworksTrail());
        }

        public Particle InitializeRocket(Particle p)
        {
            double r = random.Next(0, 179);
            double r2 = random.Next(4, 7);
            Vector3 initialV = new Vector3((float)Math.Cos(r) * INITIAL_UP_VELOCITY/2, INITIAL_UP_VELOCITY * (float)r2, (float)Math.Sin(r) * INITIAL_UP_VELOCITY/2);
            //initialV = initialV * INITIAL_UP_VELOCITY;

            Model m = content.Load<Model>("star");
            p.Initialize(m, source, initialV, new Vector3(0, INITIAL_Y_ACC, 0), new Vector3(1, 0.85f, 0.2f), 1, new Vector3(0.1f), 1);

            return p;
        }

        public override void update(float time)
        {
            random = new Random();
            for (int j = 0; j < Bursts.Count; ++j)
            {
                ParticleSystem currentPS = Bursts.ElementAt(j);
                if (currentPS.particles.Count == 0)
                    Bursts.RemoveAt(j);                
                else
                    currentPS.update(time);
            }

            for(int i = 0; i < particles.Count; ++i)
            {
                Particle currentP = particles.ElementAt(i);

                //Update and add a particle
                Particle trail = new Particle();
                trail.Initialize(particles.ElementAt(i).model, currentP.position, Vector3.Zero, Vector3.Zero, new Vector3(0.5f, 0.5f, 0.5f), 1, new Vector3(0.1f), 1);
                trails.ElementAt(i).AddParticle(trail);
                trails.ElementAt(i).update(time);

                //Update and explode the rockets when needed
                particles.ElementAt(i).update(time);
                if (currentP.velocity.Y <= 0.1f)
                {
                    FireworksBurst fb = new FireworksBurst(currentP.position, content);
                    Bursts.Add(fb);
                    resetBurst(i);    
                }
            }
        }
    }
}
