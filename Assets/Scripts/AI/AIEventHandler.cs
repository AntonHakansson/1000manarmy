using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ManArmy.AI
{
    public class AIEventHandler : EntityEventHandler
    {
        public Value<bool> isTakingDamage = new Value<bool>(false);

        /// <summary></summary>
		public Activity Patrol = new Activity();

        /// <summary></summary>
        public Activity Chase = new Activity();

        /// <summary></summary>
        public Activity Attack = new Activity();

        /// <summary></summary>
        public Activity RunAway = new Activity();
    }
}