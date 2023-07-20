using Alexandria.PrefabAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ModularMod.Past.Prefabs.Objects
{
    public class Sprite_Decals
    {
        public static void Init()
        {
            Toolbox.BuildSpriteObject("vent_cover_1", "Vent_Cover_2X2"); // _MDLR
            Toolbox.BuildSpriteObject("vent_cover_2", "Vent_Cover_2X1");
            Toolbox.BuildSpriteObject("vent_cover_3", "Vent_Cover_1X1");

            Toolbox.BuildSpriteObject("electric_ahead", "Electricity_Ahead");

            Toolbox.BuildSpriteObject("big_panel", "BIG_PANEL");

            Toolbox.BuildSpriteObject("cyan_leading_line_001", "Cyan_Line_001");
            Toolbox.BuildSpriteObject("cyan_leading_line_002", "Cyan_Line_002");
            Toolbox.BuildSpriteObject("cyan_leading_line_003", "Cyan_Line_003");
            Toolbox.BuildSpriteObject("cyan_leading_line_004", "Cyan_Line_004");
            Toolbox.BuildSpriteObject("cyan_leading_line_005", "Cyan_Line_005");
            Toolbox.BuildSpriteObject("cyan_leading_line_006", "Cyan_Line_006");
            Toolbox.BuildSpriteObject("cyan_leading_line_007", "Cyan_Line_007");
            Toolbox.BuildSpriteObject("cyan_leading_line_008", "Cyan_Line_008");
            Toolbox.BuildSpriteObject("cyan_leading_line_009", "Cyan_Line_009");
            Toolbox.BuildSpriteObject("cyan_leading_line_010", "Cyan_Line_010");

            Toolbox.BuildSpriteObject("green_leading_line_001", "green_Line_001");
            Toolbox.BuildSpriteObject("green_leading_line_002", "green_Line_002");
            Toolbox.BuildSpriteObject("green_leading_line_003", "green_Line_003");
            Toolbox.BuildSpriteObject("green_leading_line_004", "green_Line_004");
            Toolbox.BuildSpriteObject("green_leading_line_005", "green_Line_005");
            Toolbox.BuildSpriteObject("green_leading_line_006", "green_Line_006");
            Toolbox.BuildSpriteObject("green_leading_line_007", "green_Line_007");
            Toolbox.BuildSpriteObject("green_leading_line_008", "green_Line_008");
            Toolbox.BuildSpriteObject("green_leading_line_009", "green_Line_009");
            Toolbox.BuildSpriteObject("green_leading_line_010", "green_Line_010");

            Toolbox.BuildSpriteObject("orange_leading_line_001", "orange_Line_001");
            Toolbox.BuildSpriteObject("orange_leading_line_002", "orange_Line_002");
            Toolbox.BuildSpriteObject("orange_leading_line_003", "orange_Line_003");
            Toolbox.BuildSpriteObject("orange_leading_line_004", "orange_Line_004");
            Toolbox.BuildSpriteObject("orange_leading_line_005", "orange_Line_005");
            Toolbox.BuildSpriteObject("orange_leading_line_006", "orange_Line_006");
            Toolbox.BuildSpriteObject("orange_leading_line_007", "orange_Line_007");
            Toolbox.BuildSpriteObject("orange_leading_line_008", "orange_Line_008");
            Toolbox.BuildSpriteObject("orange_leading_line_009", "orange_Line_009");
            Toolbox.BuildSpriteObject("orange_leading_line_010", "orange_Line_010");

            Toolbox.BuildSpriteObject("tile_past_decal_001", "Tiled_Line_001", false);
            Toolbox.BuildSpriteObject("tile_past_decal_002", "Tiled_Line_002", false);
            Toolbox.BuildSpriteObject("tile_past_decal_003", "Tiled_Line_003", false);
            Toolbox.BuildSpriteObject("tile_past_decal_004", "Tiled_Line_004", false);
            Toolbox.BuildSpriteObject("tile_past_decal_005", "Tiled_Line_005", false);
            Toolbox.BuildSpriteObject("tile_past_decal_006", "Tiled_Line_006", false);
            Toolbox.BuildSpriteObject("tile_past_decal_007", "Tiled_Line_007", false);
            Toolbox.BuildSpriteObject("tile_past_decal_008", "Tiled_Line_008", false);
            Toolbox.BuildSpriteObject("tile_past_decal_009", "Tiled_Line_009", false);
            Toolbox.BuildSpriteObject("tile_past_decal_010", "Tiled_Line_010", false);
            Toolbox.BuildSpriteObject("tile_past_decal_011", "Tiled_Line_011", false);

            
            Alexandria.DungeonAPI.StaticReferences.customPlaceables.Add("NutsAndBolts",
            Toolbox.GenerateDungeonPlaceable(
            new Dictionary<GameObject, float>()
            {
              {  Toolbox.BuildSpriteObject_FuckingSHit("bolts1", "Bolt_1"), 1 },
              {  Toolbox.BuildSpriteObject_FuckingSHit("bolts2", "Bolt_2"), 1 },
              {  Toolbox.BuildSpriteObject_FuckingSHit("bolts3", "Bolt_3"), 1 },
            }));
            
        }
    }
}
