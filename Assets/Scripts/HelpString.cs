﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ManArmy
{
    public class HelpStrings
    {
        public class AI
        {
            // Dictionary
            public const string IS_PLAYER_DEAD = "Is Player Dead";
            public const string IS_PLAYER_IN_SIGHT = "Player in sight";
            public const string CAN_ATTACK_PLAYER = "Can Attack Player";
            public const string NEXT_TO_FOOD = "Next To Food";
            public const string IS_HUNGRY = "Is Hungry";

            // Animator parameters.
            public const string ANIMATOR_PARAM_WALK = "Vertical";
            public const string ANIMATOR_PARAM_RUN = "Vertical";
            public const string ANIMATOR_PARAM_EAT = "Eat";
            public const string ANIMATOR_PARAM_ATTACK = "Attack";
        }
    }
}