using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SaveAPI
{
    public enum CustomDungeonFlags
    {
        //Add your custom flags here
        //You can remove any flags here (except NONE, don't remove it)
        NOLLA,
        NONE, 
        TEST_UNLOCK,
        
        FULL_POWER, //Reach Power Capacity
        FIRST_FLOOR_NO_MODULES, //Beat the first floor boss without any modules active
        BEAT_FLOOR_3, //Beat the 3rd floor
        BEAT_DRAGUN_AS_MODULAR, //Beat Dragun, Duh
        BEAT_LICH_AS_MODULAR, // Beat Lich, Duh
        BEAT_OLD_KING_AS_MODULAR, // Beat Old King
        BEAT_ADVANCED_DRAGUN_AS_MODULAR, // Beat Adv. Dragun
        BEAT_RAT_AS_MODULAR, //Rat
        LEAD_GOD_AS_MODULAR, //Lead God
        BOSS_RUSH_AS_MODULAR, //Boss Rush
        PAST, //TO_DO
        OVERLOADED, //10 Active Modules at once
        BEAT_DRAGUN_WITH_3_ACTIVE_MODULES_OR_LESS, //Self Explanatory
        BEAT_LICH_WITH_4_MODULES_OR_LESS, //Self Explanatory

        PAST_ALT_SKIN,
        CRATE_DROP,

        CHECKED_ALL_ADVICE,

        PAST_MASTERY,

        DO_NOT_CHANGE

        //STYLISHLY_KILL_AN_ENEMY
    }
}
