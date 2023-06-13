using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Rpg;


namespace DiscordBot
{
    public static class Server
    {
        //world should have propagate times. Invoke time events
        //hold every object
        public static event GameObjectAction Time;

        public static Dictionary<ulong, Hero> players;
        public static Dictionary<ulong, Village> villages;

        static Task playth;
        public static int worldTime { get; private set; }

        public static void Load()
        {
            //islands include players so we only load that and create players dict from that
            var path= AppDomain.CurrentDomain.BaseDirectory + "islands";
            try
            {
                villages = DataManager.ReadFromFile(path) as Dictionary<ulong, Village>;
            }
            catch { }

            if (villages == null)
            {
                players = new Dictionary<ulong, Hero>();
                villages = new Dictionary<ulong, Village>();
                Console.WriteLine("Game data created.");
            }
            else
            {
                players = new Dictionary<ulong, Hero>();
                foreach (var island in villages.Values)
                {
                    var objs = island.GetPool();
                    foreach (var obj in objs)
                    {
                        if(obj is Hero)
                        {
                            Hero chr = (Hero)obj;
                            players.Add(chr.id, chr);
                        }    
                    }
                }
                Console.WriteLine("Game data loaded.");
            }
        }
        public static void Save()
        {
            var path = AppDomain.CurrentDomain.BaseDirectory + "islands";
            DataManager.WriteToFile(villages, path);
            Console.WriteLine("Game data written to "+path);
        }

        public static void SetActive(bool play)
        {
            if (play && playth==null)//if wants to play but it's not running
            {
                playth = Play();
            }

            if(!play && playth!=null)//if wants to stop but it's running
            {
                playth.Dispose();
                playth = null;
            }
        }
        private static async Task Play()
        {
            while (true)
            {
                await Task.Delay(1000);
                Time?.Invoke();

                if (++worldTime % 10 == 0)
                    Save();
            }
        }

        public delegate void GameObjectAction();
    }


    public class Village
    {
        HashSet<Object> pool;
        public ulong id { get; private set; }
        public Village(ulong id)
        {
            this.id = id;
            pool = new HashSet<Object>();
            Server.Time += TimeLoop;
        }

        void TimeLoop()
        {
        }

        public List<Object> GetPool()
        {
            var p = new List<Object>();
            p.AddRange(pool);
            return p;
        }
        public void SendMessage(string m)
        {
            (DiscordBot.Bot.instance.client.GetChannel(id) as Discord.WebSocket.ISocketMessageChannel).SendMessageAsync(m);
        }
        public class Object
        {
            public string name { get; private set; }
            public Village village { get; protected set; }

            public Object()
            {
                Server.Time += ExperienceSecond;
            }

            protected virtual void ExperienceSecond()
            {
            }

            public void SetVillage(Village To)
            {
                if (village != null && village.pool.Contains(this))
                    village.pool.Remove(this);

                if(To!=null && !To.pool.Contains(this))
                    To.pool.Add(this);

                village = To;
            }

        }
    }


}
