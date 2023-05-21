using System.Reflection;
using MonoMod.RuntimeDetour;
using UnityEngine;
using Dungeonator;
using Gungeon;
using System.Collections;
using Brave.BulletScript;
using System;
using System.Collections.Generic;
using Planetside;
using static SpawnEnemyOnDeath;
using Alexandria.Misc;
using UnityEngine.UI;
using FullInspector;
using Alexandria.NPCAPI;
using Alexandria.ItemAPI;
using static BossFinalRogueLaunchShips1;

namespace ModularMod
{
    public static class Hooks
    {
        public class  ChooseModuleController : MonoBehaviour
        {
            public static Func<int, int> AdditionalOptionsModifier;

            public int Count = 4;
            public Gun g;
            public bool isAlt = false;

            public DefaultModule SelectModule(GenericLootTable table)
            {
                var mod = table.SelectByWeightNoExclusions().GetComponent<DefaultModule>();
                switch (mod.Tier)
                {
                    case DefaultModule.ModuleTier.Tier_1:
                        if (UnityEngine.Random.value < 0.0001) { AkSoundEngine.PostEvent("Play_BOSS_queenship_emerge_01", g.gameObject); return GlobalModuleStorage.ReturnRandomModule(DefaultModule.ModuleTier.Tier_Omega); }
                        return mod;
                    case DefaultModule.ModuleTier.Tier_2:
                        if (UnityEngine.Random.value < 0.0002) { AkSoundEngine.PostEvent("Play_BOSS_queenship_emerge_01", g.gameObject); return GlobalModuleStorage.ReturnRandomModule(DefaultModule.ModuleTier.Tier_Omega); }
                        return mod;
                    case DefaultModule.ModuleTier.Tier_3:
                        if (UnityEngine.Random.value < 0.00035) {
                            AkSoundEngine.PostEvent("Play_BOSS_queenship_emerge_01", g.gameObject); return GlobalModuleStorage.ReturnRandomModule(DefaultModule.ModuleTier.Tier_Omega);
                        }
                        return mod;
                    default: return mod;
                }
            }
            public void AlterCount()
            {
                if (g.quality == PickupObject.ItemQuality.B | g.quality == PickupObject.ItemQuality.A) { Count = 5; }
                if (g.quality == PickupObject.ItemQuality.S) { Count = 6; }
            }


            public void Start()
            {
                g = this.GetComponent<Gun>();
                var light = g.gameObject.GetOrAddComponent<AdditionalBraveLight>();
                light.LightColor = TierColor();
                AlterCount();
                selectableModules = new List<ModuleUICarrier>();
                GenericLootTable tableToUse = GlobalModuleStorage.SelectTable(g.quality);

                if (AdditionalOptionsModifier != null) { Count = AdditionalOptionsModifier(Count); }

                float Arc = 30 + (Count * 15);

                for (int i = 0; i < Count; i++)
                {
                    selectableModules.Add(new ModuleUICarrier()
                    {
                        controller = this,
                        defaultModule = SelectModule(tableToUse),
                        EndPosition = Toolbox.GetUnitOnCircle(Toolbox.SubdivideArc(Vector2.up.ToAngle() + (Arc * -1), Arc * 2, Count, i) , 2f),   
                        isUsingAlternate = isAlt
                    });
                }
                foreach(var r in selectableModules)
                {
                    r.Start();
                }
                g.StartCoroutine(LerpLight(g));
            }
            private IEnumerator LerpLight(Gun g)
            {
                bool emergtencyCheck = false;
                var light = g.gameObject.GetOrAddComponent<AdditionalBraveLight>();
                float elapsed = 0f;
                while (elapsed < 1f)
                {
                    if (g == null) { yield break; }
                    elapsed += BraveTime.DeltaTime;
                    float t = elapsed / 1f;
                    if (emergtencyCheck == false)
                    {
                        light.LightIntensity = Mathf.Lerp(0, 10, t);
                        light.LightRadius = Mathf.Lerp(0, 1.5f, t);
                    }
                    yield return null;
                }
                yield break;
            }


            public void DestroyAllOthers()
            {
                for (int i = 0; i < selectableModules.Count; i++)
                {
                    if (selectableModules[i].extantModule)
                    {
                        if (selectableModules[i].HasDropped == false)
                        {
                            selectableModules[i].DoDestroy(selectableModules[i].extantModule.GetComponent<DefaultModule>());
                        }
                    }
                }
                g.StartCoroutine(I_DoDestroy(g));
            }

            public Color TierColor()
            {
                switch (g.quality)
                {
                    case PickupObject.ItemQuality.D:
                        return new Color(0.6f, 0.3f, 0);
                    case PickupObject.ItemQuality.C:
                        return Color.blue;
                    case PickupObject.ItemQuality.B:
                        return new Color(0.4f, 0.8f, 0.09f);
                    case PickupObject.ItemQuality.A:
                        return Color.red;
                    case PickupObject.ItemQuality.S:
                        return Color.white;
                    default: return Color.cyan;
                }     
            }

            public bool isBeingDestroyed = false;
            private IEnumerator I_DoDestroy(Gun g)
            {
                isBeingDestroyed = true;
                bool emergtencyCheck = false;
                var light = g.gameObject.GetOrAddComponent<AdditionalBraveLight>();
                light.LightColor = TierColor();
                g.sprite.renderer.material.shader = StaticShaders.Displacer_Beast_Shader;
                g.sprite.renderer.material.SetTexture("_MainTex", g.sprite.renderer.material.mainTexture);
                float elapsed = 0f;
                while (elapsed < 0.5f)
                {
                    if (g == null) { yield break; }
                    elapsed += BraveTime.DeltaTime;
                    float t = elapsed / 0.5f;
                    g.sprite.renderer.material.SetFloat("_BurnAmount", t);
                    if (emergtencyCheck == false)
                    {
                        light.LightIntensity = Mathf.Lerp(10, 50, t);
                        light.LightRadius = Mathf.Lerp(1.5f, 0, t);
                    }
                    SpriteOutlineManager.RemoveOutlineFromSprite(g.sprite, false);
                    yield return null;
                }
                Destroy(g.gameObject);
                yield break;
            }


            private void OnDestroy()
            {
                for (int i = 0; i < selectableModules.Count; i++)
                {
                    if (selectableModules[i].extantModule)
                    {
                        if (selectableModules[i].HasDropped == false && selectableModules[i].BeingDestroyed == false)
                        {
                            selectableModules[i].DoDestroy(selectableModules[i].extantModule.GetComponent<DefaultModule>());
                        }
                    }
                }
            }


            public List<ModuleUICarrier> selectableModules = new List<ModuleUICarrier>();

            public class ModuleUICarrier : MonoBehaviour
            {
                public void Start()
                {
                    HasStoppedMoving = false;
                    extantModule = UnityEngine.Object.Instantiate<GameObject>(defaultModule.gameObject, controller.g.sprite.WorldCenter, Quaternion.identity);
                    var DefMod = extantModule.GetComponent<DefaultModule>();
                    DefMod.ChangeShader(StaticShaders.Hologram_Shader);
                    DefMod.sprite.renderer.material.SetFloat("_IsGreen", isUsingAlternate == true ? 1 : 0);

                    DefMod.PreInteractLogic += PreInteract;
                    DefMod.StartCoroutine(this.DoMovement(1f, DefMod));
                    DefMod.StartCoroutine(LerpLight(DefMod, 4, 0, 2, 0));
                }

                public bool HasDropped = false;
                public bool BeingDestroyed = false;


                public bool PreInteract(DefaultModule DefMod, PlayerController p)
                {
                    if (HasStoppedMoving == true && HasDropped == false)
                    {
                        DebrisObject orAddComponent = DefMod.gameObject.GetOrAddComponent<DebrisObject>();
                        orAddComponent.shouldUseSRBMotion = true;
                        orAddComponent.angularVelocity = 0f;
                        orAddComponent.Priority = EphemeralObject.EphemeralPriority.Critical;
                        orAddComponent.sprite.UpdateZDepth();
                        orAddComponent.Trigger(Vector3.up.WithZ(2f), 1, 1f);
                        DefMod.OnEnteredRange(p);
                        DefMod.EnteredRange -= Entered;
                        DefMod.ExitedRange -= Exited;
                        DefMod.ChangeShader(StaticShaders.Default_Shader);
                        HasDropped = true;
                        controller.DestroyAllOthers();
                        DefMod.StartCoroutine(LerpLight(DefMod, 0, 7, 0, 3));
                        Destroy(this);
                        return false;
                    }
                    return (HasStoppedMoving);
                }
                private bool HasStoppedMoving;

                private IEnumerator LerpLight(DefaultModule self, float to, float From, float radTo, float radFrom)
                {
                    if (self.BraveLight == null) { yield break; }
                    float elapsed = 0f;
                    while (elapsed < 0.5f)
                    {
                        elapsed += BraveTime.DeltaTime;
                        float t = elapsed / 0.5f;
                        self.BraveLight.LightIntensity = Mathf.Lerp(From, to, t);
                        self.BraveLight.LightRadius = Mathf.Lerp(radFrom, radTo, t);
                        yield return null;
                    }
                    yield break;
                }


                private IEnumerator DoMovement(float duration, DefaultModule self)
                {
                    float elapsed = 0f;
                    while (elapsed < duration)
                    {
                        elapsed += BraveTime.DeltaTime;
                        float t = elapsed / duration;
                        self.gameObject.transform.position = Vector3.Lerp(controller.g.sprite.WorldCenter, controller.g.sprite.WorldCenter + EndPosition, Toolbox.SinLerpTValue(t));
                        yield return null;
                    }
                    self.EnteredRange += Entered;
                    self.ExitedRange += Exited;
                    HasStoppedMoving = true;
                    yield break;
                }

                public void Entered(DefaultModule DefMod)
                {
                    DefMod.StartCoroutine(LerpLight(DefMod, 7, 4, 3, 2));
                    DefMod.ChangeShader(StaticShaders.Default_Shader);
                }
                public void Exited(DefaultModule DefMod)
                {
                    DefMod.StartCoroutine(LerpLight(DefMod, 4, 7, 2, 3));
                    DefMod.ChangeShader(StaticShaders.Hologram_Shader);
                    DefMod.sprite.renderer.material.SetFloat("_IsGreen", isUsingAlternate == true ? 1 : 0);

                }

                public void DoDestroy(DefaultModule DefMod)
                {
                    DefMod.StartCoroutine(I_DoDestroy(DefMod));
                }

                private IEnumerator I_DoDestroy(DefaultModule DefMod)
                {
                    DefMod.OverrideCanDisplayText(false);
                    bool emergtencyCheck = false;
                    if (DefMod.BraveLight == null) { emergtencyCheck = true; }

                    BeingDestroyed = true;

                    DefMod.PreInteractLogic -= PreInteract;
                    DefMod.PreInteractLogic += PreInteractOverride;
                    DefMod.OverrideEnteredRangeOutline = NoOutline;
                    DefMod.OverrideExitedRangeOutline = NoOutline;
                    DefMod.EnteredRange -= Entered;
                    DefMod.ExitedRange -= Exited;


                    float i = emergtencyCheck == false ? DefMod.BraveLight.LightIntensity : 0;
                    DefMod.ChangeShader(StaticShaders.Displacer_Beast_Shader);
                    DefMod.sprite.renderer.material.SetTexture("_MainTex", DefMod.sprite.renderer.material.mainTexture);     
                    float elapsed = 0f;
                    while (elapsed < 0.66f)
                    {
                        elapsed += BraveTime.DeltaTime;
                        float t = elapsed / 0.66f;
                        DefMod.sprite.renderer.material.SetFloat("_BurnAmount", t);
                        if (emergtencyCheck == false)
                        {
                            DefMod.BraveLight.LightIntensity = Mathf.Lerp(i, 50, t);
                            DefMod.BraveLight.LightRadius = Mathf.Lerp(2, 0, t);
                        }
                        SpriteOutlineManager.RemoveOutlineFromSprite(DefMod.sprite, false);
                        yield return null;
                    }
                    Destroy(DefMod.gameObject);
                    yield break;
                }
                public bool PreInteractOverride(DefaultModule DefMod, PlayerController p)
                {
                    return false;
                }
                public void NoOutline(DefaultModule DefMod){ }

                public void OnDestroy()
                {
                    if (controller)
                    {
                        if (controller.selectableModules.Contains(this)) { controller.selectableModules.Remove(this); }
                    }
                }
                public Vector2 EndPosition;
                public ChooseModuleController controller;
                public GameObject extantModule;
                public DefaultModule defaultModule;
                public bool isUsingAlternate = false;

            }
        }
        public static void Init()
        {
            new Hook(typeof(Gun).GetMethod("Pickup", BindingFlags.Instance | BindingFlags.Public), typeof(Hooks).GetMethod("PickupHook"));
            new Hook(typeof(PlayerController).GetMethod("SetStencilVal", BindingFlags.Instance | BindingFlags.NonPublic), typeof(Hooks).GetMethod("SetStencilValHook"));
            new Hook(typeof(PlayerController).GetMethod("UpdateStencilVal", BindingFlags.Instance | BindingFlags.NonPublic), typeof(Hooks).GetMethod("UpdateStencilValHook"));
            new Hook(typeof(PlayerStats).GetMethod("RebuildGunVolleys", BindingFlags.Instance | BindingFlags.Public), typeof(Hooks).GetMethod("RebuildGunVolleysHook"));
            new Hook(typeof(DungeonData).GetMethod("FloodFillDungeonInterior", BindingFlags.Instance | BindingFlags.NonPublic), typeof(Hooks).GetMethod("FloodFillDungeonInteriorHook"));
            new Hook(typeof(RoomHandler).GetMethod("CheckCellArea", BindingFlags.Instance | BindingFlags.NonPublic), typeof(Hooks).GetMethod("CheckCellAreaHook"));


            //new Hook(typeof(BaseShopController).GetMethod("HandleEnter", BindingFlags.Instance | BindingFlags.NonPublic), typeof(Hooks).GetMethod("HandleEnterHook"));
            JuneLib.ItemsCore.AddChangeSpawnItem(ReturnObj);
        }



        public static bool CheckCellAreaHook(Func<RoomHandler, IntVector2, IntVector2, bool> orig, RoomHandler self, IntVector2 basePosition, IntVector2 objDimensions)
        {
            DungeonData data = GameManager.Instance.Dungeon.data;
            bool result = true;
            for (int i = basePosition.x; i < basePosition.x + objDimensions.x; i++)
            {
                for (int j = basePosition.y; j < basePosition.y + objDimensions.y; j++)
                {
                    CellData cellData = data.cellData[i][j];
                    if (cellData != null) 
                    {
                        if (!cellData.IsPassable)
                        {
                            return false;
                        }
                    }
                    
                }
            }
            return result;
        }


        public static void FloodFillDungeonInteriorHook(Action<DungeonData> orig, DungeonData self)
        {
            /*
            Stack<CellData> stack = new Stack<CellData>();
            for (int i = 0; i < self.rooms.Count; i++)
            {
                //ETGModConsole.Log(self.rooms[i].GetRoomName() ?? "NULL");
                //ETGModConsole.Log(stack != null ? "stack" : "NULL");
                //ETGModConsole.Log(self.rooms[i] != null ? "self.rooms[i]" : "NULL");
                if (self.rooms[i] == self.Entrance || self.rooms[i].IsStartOfWarpWing)
                {
                    stack.Push(self[self.rooms[i].GetRandomAvailableCellDumb()]);
                }
            }
            while (stack.Count > 0)
            {
                CellData cellData = stack.Pop();
                if (cellData.type != CellType.WALL)
                {
                    List<CellData> cellNeighbors = self.GetCellNeighbors(cellData, false);
                    cellData.isGridConnected = true;
                    for (int j = 0; j < cellNeighbors.Count; j++)
                    {
                        if (cellNeighbors[j] != null && cellNeighbors[j].type != CellType.WALL && !cellNeighbors[j].isGridConnected)
                        {
                            stack.Push(cellNeighbors[j]);
                        }
                    }
                }
            }
            */
            Stack<CellData> stack = new Stack<CellData>();
            for (int i = 0; i < self.rooms.Count; i++)
            {
                if (self.rooms[i] == self.Entrance || self.rooms[i].IsStartOfWarpWing)
                {
                    Debug.Log(self.rooms[i].GetRoomName());
                    try
                    {
                        stack.Push(self[self.rooms[i].GetRandomAvailableCellDumb()]);
                    }
                    catch (Exception ex)
                    {
                        //ETGModConsole.Log("[ExpandTheGungeon] Warning: Exception caught at DungeonData.FloodFillDungeonInterior!");
                        Debug.LogException(ex);
                    }
                }
            }
            try
            {
                while (stack.Count > 0)
                {
                    CellData cellData = stack.Pop();
                    if (cellData.type != CellType.WALL)
                    {
                        List<CellData> cellNeighbors = self.GetCellNeighbors(cellData, false);
                        cellData.isGridConnected = true;
                        for (int j = 0; j < cellNeighbors.Count; j++)
                        {
                            if (cellNeighbors[j] != null && cellNeighbors[j].type != CellType.WALL && !cellNeighbors[j].isGridConnected)
                            {
                                stack.Push(cellNeighbors[j]);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ETGModConsole.Log("[Modular] Warning: Exception caught at DungeonData.FloodFillDungeonInterior!");
                Debug.LogException(ex);
            }
            
        }


        public static void RebuildGunVolleysHook(Action<PlayerStats, PlayerController> orig, PlayerStats self, PlayerController p)
        {
            orig(self, p);
            GameManager.Instance.StartCoroutine(FrameDelay());
        }
        public static IEnumerator FrameDelay()
        {
            yield return null;
            if (OnRecalculateStats != null) { OnRecalculateStats(); }
            yield break;
        }
        public static Action OnRecalculateStats;

        public static GameObject ReturnObj(PickupObject pickup)
        {
            foreach (var player in GameManager.Instance.AllPlayers)
            {
                if (player.PlayerHasCore() != null) 
                {
                    var HPComp = pickup.GetComponent<HealthPickup>();
                    if (HPComp != null)
                    {
                        if (HPComp.healAmount == 0.5f)
                        {
                            pickup = UnityEngine.Random.value < 0.02f ? PickupObjectDatabase.GetById(CraftingCore.CraftingCoreID) : PickupObjectDatabase.GetById(Scrap.Scrap_ID);
                            pickup.gameObject.SetActive(true);

                        }
                        if (HPComp.healAmount == 1f)
                        {
                            pickup = UnityEngine.Random.value < 0.035f ? PickupObjectDatabase.GetById(CraftingCore.CraftingCoreID) : PickupObjectDatabase.GetById(Scrap.Scrap_ID);
                        }
                    }
                }
            }
            return pickup.gameObject;
        }

        public static void PickupHook(Action<Gun, PlayerController> orig, Gun self, PlayerController player)
        {
            if (player.HasPickupID(ModulePrinterCore.ModulePrinterCoreID) == true)
            {
                var yes = self.gameObject.GetOrAddComponent<ChooseModuleController>();
                yes.isAlt = player.IsUsingAlternateCostume;
            }
            else
            {
                var c = self.gameObject.GetComponent<ChooseModuleController>();
                if (c != null) { if (c.isBeingDestroyed == true) { return; } }
                orig(self, player);
            }
        }
        public static bool Stencility_Enabled = true;
        public static void SetStencilValHook(Action<PlayerController, int> orig, PlayerController player, int i)
        {
            if (player.sprite.renderer.material.shader == StaticShaders.TransparencyShader) { return; }
            if (Stencility_Enabled == false) { return; }
            orig(player, i);
        }
        public static void UpdateStencilValHook(Action<PlayerController> orig, PlayerController player)
        {
            if (player.sprite.renderer.material.shader == StaticShaders.TransparencyShader) { return; }
            if (Stencility_Enabled == false) { return; }
            orig(player);
        }

        public static void HandleEnterHook(Action<BaseShopController, PlayerController> orig, BaseShopController self, PlayerController p)
        {
            if (!self.m_hasBeenEntered && self.baseShopType == BaseShopController.AdditionalShopType.NONE)
            {
                foreach (PlayerController p1 in GameManager.Instance.AllPlayers)
                {
                    if (p1.PlayerHasCore() == true)
                    {
                        ReinitializeHPTOModules(self);
                    }
                }
            }
            orig(self, p);
            
        }

        public static void ReinitializeHPTOModules(BaseShopController self)
        {
            if (self.baseShopType == BaseShopController.AdditionalShopType.NONE)
            {
                for (int i = 0; i < self.m_itemControllers.Count; i++)
                {
                    var HPComp = self.m_itemControllers[i].item.GetComponent<HealthPickup>();
                    if (self.m_itemControllers[i] && self.m_itemControllers[i].item && HPComp != null)
                    {
                        if (HPComp.healAmount == 0.5f)
                        {
                            var g = UnityEngine.Random.value < 0.025f ? PickupObjectDatabase.GetById(CraftingCore.CraftingCoreID).gameObject : PickupObjectDatabase.GetById(Scrap.Scrap_ID).gameObject;
                            self.m_shopItems[i] = g;
                            self.m_itemControllers[i].Initialize(g.GetComponent<PickupObject>(), self);
                        }
                        if (HPComp.healAmount == 1f)
                        {
                            var g = UnityEngine.Random.value < 0.0625f ? PickupObjectDatabase.GetById(CraftingCore.CraftingCoreID).gameObject : PickupObjectDatabase.GetById(Scrap.Scrap_ID).gameObject;
                            self.m_shopItems[i] = g;
                            self.m_itemControllers[i].Initialize(g.GetComponent<PickupObject>(), self);
                        }
                    }
                }
            }
        }
    }
}