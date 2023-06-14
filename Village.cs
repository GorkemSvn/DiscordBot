using System;
using System.Collections.Generic;
using System.Text;
using Discord.WebSocket;
using Rpg;

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

            var wolf = Spawner.Wolf(this);
            mobs.Add(wolf);

            var forest = new Forest(Program.random.Next(50, 150));
            forest.SetVillage(this);
            pool.Add(forest);
        }

        public void RefreshTimeHook()
        {
            Server.Time += TimeLoop;
        }
        void TimeLoop()
        {
            if (mobs.Count >= 10)
                return;

            if (Program.random.NextDouble() < 1f / (60f*(mobs.Count+1)))
            {
                var wolf = Spawner.Wolf(this);
                mobs.Add( wolf);

                SendMessage("A wolf appeared");
            }
            if (Program.random.NextDouble() < 1f / (60f * (mobs.Count + 3)))
            {
                var bandit = Spawner.Bandit(this);
                mobs.Add(bandit);

                SendMessage("A bandit appeared");
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

        public List<string> ShowLog()
        {
            return log;
        }


        [System.Serializable]
        public class Object
        {
            public string name;
            public Village village { get; protected set; }

            public Object()
            {
                Server.Time += ExperienceSecond;
                name = this.GetType().Name + Program.random.Next(10, 99);
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
        }

        protected override void ExperienceSecond()
        {
            if (Program.random.NextDouble() < 6f / 3600f && treeCount<maxSize)
            {
                treeCount += Math.Max(1, maxSize / 10);
                treeCount = Math.Clamp(treeCount, 0, maxSize);
                village.SendMessage("Sprouts finally grown to trees.");
            }
        }

        public Item Cut()
        {
            var wood = new Item();
            wood.name = "Wood";
            wood.quantity = 1;
            return wood;
        }
    }

    [System.Serializable]
    public class ItemCapsule : Village.Object
    {
        public bool empty { get; private set; }
        Item item;

        public ItemCapsule(Item item)
        {
            this.item = item;
            if (item != null)
                empty = false;
            else
                empty = true;

            name = item.name;
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
