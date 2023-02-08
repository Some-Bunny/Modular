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
        
        REACH_FLOOR_3_MODULAR,
        BEAT_DRAGUN_AS_MODULAR,
        BEAT_LICH_AS_MODULAR,
        BEAT_OLD_KING_AS_MODULAR,
        BEAT_RAT_AS_MODULAR,
        BEAT_DRAGUN_WITH_6_MODULES_OR_LESS,
        BEAT_LICH_WITH_6_MODULES_OR_LESS,
        LEAD_GOD_AS_MODULAR,
        BOSS_RUSH_AS_MODULAR,
        STYLISHLY_KILL_AN_ENEMY
    }
}
