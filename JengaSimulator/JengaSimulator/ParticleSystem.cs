using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace JengaSimulator
{
    public abstract class ParticleSystem
    {
        public List<Particle> particles { get; protected set; }
        protected Vector3 source;
        public ContentManager content;

        //Rockets fired from some point
        protected List<ParticleSystem> Bursts;
        protected List<ParticleSystem> trails;
        protected float spawnRate;
        protected float spawnTimer;

        public abstract void initialize();
        public abstract void update(float time);

        public void AddParticle(Particle p)
        {
            particles.Add(p);
        }

        public void draw(Matrix view, Matrix projection)
        {
            foreach (Particle p in particles)
            {
                p.draw(view, projection);
            }
            if (Bursts != null)
            {
                foreach (ParticleSystem ps in Bursts)
                {
                    ps.draw(view, projection);
                }
            }
            if (trails != null)
            {
                foreach (ParticleSystem ps in trails)
                {
                    ps.draw(view, projection);
                }
            }
        }
    }
}
