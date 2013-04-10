using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PatrickModafferiA2.Systems
{
    class FireworksTrail : ParticleSystem
    {
        public FireworksTrail()
        { initialize(); }
        public override void initialize()
        {
            particles = new List<Particle>();
        }

        public override void update(float time)
        {
            for (int i = 0; i < particles.Count; ++i)
            {
                particles.ElementAt(i).alpha -= 0.05f;
                if (particles.ElementAt(i).alpha <= 0)
                {
                    particles.RemoveAt(i);
                }
            }
        }
    }
}
