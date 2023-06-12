using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
namespace DiscordBot
{
    public class World
    {
        //world should have propagate times. Invoke time events
        public static event GameObjectAction Time;
        private Dictionary<ulong, Character> players;
        private Dictionary<ulong, Island> groups;

        public World()
        {
            Play();
        }

        private async Task Play()
        {
            while (true)
            {
                await Task.Delay(1);
                Time?.Invoke();
            }
        }

        public delegate void GameObjectAction();
    }

    public class GameObject
    {
        public GameObject()
        {
            World.Time += ExperienceSecond;
        }
        ~GameObject()
        {
            World.Time -= ExperienceSecond;
        }

        protected virtual void ExperienceSecond()
        {

        }
    }

    public class Island
    {
        public List<GameObject> pool { get; private set; }

        public Island()
        {
            pool = new List<GameObject>();
            //start a cycle that spawns some items at start and at a frequency into pool
        }
    }
}
