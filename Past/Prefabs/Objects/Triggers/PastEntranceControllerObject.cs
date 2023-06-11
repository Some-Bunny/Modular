using Alexandria.PrefabAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ModularMod.Past.Prefabs.Objects
{
    public class PastEntranceControllerObject
    {
        public static void Init()
        {
            GameObject obj = PrefabBuilder.BuildObject("Agony");
            //var Controller = obj.AddComponent<EntranceController>();
            //Controller.wallClearanceHeight = 2;
            //Controller.wallClearanceWidth = 1;
            Alexandria.DungeonAPI.StaticReferences.customObjects.Add("EntranceObjectModularPast", obj);
        }
    }
}
