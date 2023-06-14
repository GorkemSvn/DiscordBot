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
            catch(Exception e) { Console.WriteLine(e.Message); }

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
                        obj.RefreshTimeHook();
                    }
                    island.RefreshTimeHook();
                }
                Console.WriteLine("Game data loaded.");
            }
        }
        public static Task Save()
        {
            var path = AppDomain.CurrentDomain.BaseDirectory + "islands";
            try
            {
                DataManager.WriteToFile(villages, path);
            }catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
            Console.WriteLine("Game data written to "+path);
            return Task.CompletedTask;
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
                    await Save();
            }
        }

        public delegate void GameObjectAction();
    }




}
