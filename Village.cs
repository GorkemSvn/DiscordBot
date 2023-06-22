using System;
using System.Collections.Generic;
using System.Text;
using Discord.WebSocket;
using Rpg;
using Discord;

namespace DiscordBot
{
    [System.Serializable]
    public class Village
    {
        HashSet<Object> pool;
        public ulong id { get; private set; }
        List<Mob> mobs = new List<Mob>();
        List<string> log = new List<string>();
        public Village(ulong id)
        {
            this.id = id;
            pool = new HashSet<Object>();
            Server.Time += TimeLoop;

            var wolf = Spawner.Coyote(this);
            mobs.Add(wolf);
            /*
            var forest = new Forest(Program.random.Next(50, 150));
            forest.SetVillage(this);
            pool.Add(forest);

            var mine = new Mine();
            mine.SetVillage(this);
            pool.Add(mine);*/
        }

        public void RefreshTimeHook()
        {
            Server.Time += TimeLoop;
        }
        void TimeLoop()
        {

            var standardChance =Math.Pow( 1f / (60f*mobs.Count*mobs.Count),1.0);

            if (Program.random.NextDouble() <standardChance)
            {
                var wolf = Spawner.Coyote(this);
                mobs.Add( wolf);

                SendMessage("A Coyote appeared");
            }
            if (Program.random.NextDouble() < standardChance/2f)
            {
                var bandit = Spawner.Brawler(this);
                mobs.Add(bandit);

                SendMessage("A brawler appeared");
            }
            if (Program.random.NextDouble() < standardChance/5f)
            {
                var bandit = Spawner.Bandit(this);
                mobs.Add(bandit);

                SendMessage("A vile Bandit has appeared");
            }
        }

        public List<Object> GetPool()
        {
            var p = new List<Object>();
            p.AddRange(pool);
            return p;
        }
        public void SendMessage(string m)
        {
            (Bot.instance.client.GetChannel(id) as ISocketMessageChannel)?.SendMessageAsync(m);
        }

        public void AddToLog(string line)
        {
            log.Add(line);
            if (log.Count > 20)
                log.RemoveAt(0);
        }

        public void SendLogs()
        {
            var eb = new EmbedBuilder();
            eb.Title = "Village log:";

            foreach (var item in log)
            {
                eb.Description += item + "\n";
            }

            (Bot.instance.client.GetChannel(id) as ISocketMessageChannel)?.SendMessageAsync("",false,eb.Build());
        }


        [System.Serializable]
        public class Object
        {
            public string name;
            public Village village { get; protected set; }

            public Object()
            {
                Server.Time += ExperienceSecond;
                name = this.GetType().Name;// + Program.random.Next(10, 99);
            }

            protected virtual void ExperienceSecond()
            {
            }

            public void SetVillage(Village To)
            {
                if (village != null && village.pool.Contains(this))
                    village.pool.Remove(this);

                if (To != null && !To.pool.Contains(this))
                    To.pool.Add(this);

                village = To;
            }

            public void Destroy()
            {
                village.pool.Remove(this);
                Server.Time -= ExperienceSecond;
                name += "Destroyed";
                if (village.mobs.Contains(this as Mob))
                    village.mobs.Remove(this as Mob);

                village = null;
            }

            public void RefreshTimeHook()
            {
                Server.Time += ExperienceSecond;
            }
        }
    }

    [System.Serializable]
    public class Forest:Village.Object
    {
        public int maxSize { get; private set; }
        public int treeCount { get; private set; }

        public Forest(int maxSize)
        {
            this.maxSize = maxSize;
            treeCount = maxSize;
            name = "Forest (" + treeCount + " tree)";
        }

        protected override void ExperienceSecond()
        {
            if (Program.random.NextDouble() < 6f / 3600f && treeCount<maxSize)
            {
                treeCount += Math.Max(1, maxSize / 10);
                treeCount = Math.Clamp(treeCount, 0, maxSize);
                village.SendMessage("Sprouts finally grown to trees.");
                name = "Forest (" + treeCount + " tree)";
            }
        }

        public Item Cut()
        {
            if (treeCount > 0)
            {
                treeCount--;
                name = "Forest (" + treeCount + " tree)";
                return ItemGenerator.wood.Duplicate();
            }
            else
                return null;
        }
    }
    [System.Serializable]
    public class Mine:Village.Object
    {
        public float Depth { get; private set; }

        public Dictionary<float, List<Item>> minables;

        public Mine()
        {
            minables = new Dictionary<float, List<Item>>();
            minables.Add(0f, new List<Item>() { ItemGenerator.wood });
            name = "Mountain Mine (" + Depth + "m)";
        }

        public Item Dig()
        {
            var dpeths = new List<float>();
            dpeths.AddRange(minables.Keys);
            dpeths.Sort();
            int layer = 0;
            for (int i = 0; i < dpeths.Count; i++)
            {
                if (Depth >= dpeths[i])
                    layer = i;
            }

            var items = minables[layer];

            return items[Program.random.Next(items.Count)].Duplicate();
        }
    }
    [System.Serializable]
    public class ItemCapsule : Village.Object
    {
        public bool empty { get; private set; }
        Item item;
        int seconds = 0;
        public ItemCapsule(Item item)
        {
            this.item = item;
            if (item != null)
                empty = false;
            else
                empty = true;

            name = item.name;
        }
        protected override void ExperienceSecond()
        {
            seconds++;
            if (seconds >= 60)
                Destroy();
        }

        public Item GiveItem()
        {
            var given = item;
            item = null;
            empty = true;
            Destroy();
            return given;
        }
    }
}
