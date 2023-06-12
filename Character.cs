using System;
using System.Collections.Generic;
using System.Text;
using Rpg;

namespace DiscordBot
{
    public abstract class Character
    {
        public ulong id { get; private set; }
        protected Stats stats;
        protected Inventory inventory;
        protected Equipment equipment;
        protected SkillBook skills;
    }
}
