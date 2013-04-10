using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace PatrickModafferiA3.Systems
{
    class StarsShooting : ParticleSystem
    {
        const int BURST_COUNT = 12;
        const int INITIAL_UP_VELOCITY = 70;
        const int INITIAL_Y_ACC = -1;

        Random random;

        public StarsShooting(Vector3 s, ContentManager c)
        {
            trails = new List<ParticleSystem>();
            random = new Random();
            content = c;
            particles = new List<Particle>();
            source = s;
            spawnStar(source);
        }

        public void spawnStar(Vector3 position)
        {
            Particle p = new Particle();
            double r = random.Next(0, 179);
            Vector3 unitV = new Vector3(position.X, position.Y, position.Z);
            unitV.Normalize();
            unitV = new Vector3(1 - Math.Abs(unitV.X), 1 - Math.Abs(unitV.Y), 1 - Math.Abs(unitV.Z));
            Model m = content.Load<Model>("star");
            p.Initialize(m, position, unitV * INITIAL_UP_VELOCITY, new Vector3(0, INITIAL_Y_ACC, 0), new Vector3(1, 1, 1), 1, new Vector3(0.25f), 3);
            particles.Add(p);

            trails.Insert(particles.Count - 1, new FireworksTrail());            
        }

        public override void initialize()
        {
        }

        public override void update(float time)
        {
            for (int i = 0; i < particles.Count; ++i)
            {
                Particle currentP = particles.ElementAt(i);

                //Update and add a particle
                Particle trail = new Particle();
                trail.Initialize(particles.ElementAt(i).model, currentP.position, Vector3.Zero, Vector3.Zero, new Vector3(0.5f, 0.5f, 0.5f), 3, new Vector3(0.2f), 1);
                trails.ElementAt(i).AddParticle(trail);
                trails.ElementAt(i).update(time);

                //Update and explode the rockets when needed
                particles.ElementAt(i).update(time);
                if ( currentP.isDead() )
                {
                    particles.RemoveAt(i);
                    --Particle.particleCount;
                    Particle.particleCount -= trails.ElementAt(i).particles.Count;
                    trails.RemoveAt(i);
                }
            }
        }
    }
}
