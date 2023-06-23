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
using SaveAPI;

namespace ModularMod
{
    public static class Hooks
    {
        public class ShittyVFXAttacher : MonoBehaviour
        {
            public PlayerController mainPlayer;
            public GameObject gameObj = VFXStorage.VFX_Modulable;
            public tk2dSpriteAnimator VFX;
            public PickupObject g;
            public void Start()
            {
                g = this.GetComponent<PickupObject>();
                VFX = UnityEngine.Object.Instantiate(gameObj, g.sprite.WorldTopLeft, Quaternion.identity).GetComponent<tk2dSpriteAnimator>();
                VFX.gameObject.transform.parent = g.gameObject.transform;
                VFX.Play(mainPlayer.IsUsingAlternateCostume ? "start_alt" : "start");
            }

            public void OnDestroy()
            {
                VFX.PlayAndDestroyObject(mainPlayer.IsUsingAlternateCostume ? "break_alt" : "break");
            }
        }


        public class  ChooseModuleController : MonoBehaviour
        {
            public static Func<int, int> AdditionalOptionsModifier;

            public int Count = 4;
            public Gun g;
            public bool isAlt = false;
            public Dictionary<tk2dTiledSprite, ModuleUICarrier> tk2DTiledSprites = new Dictionary<tk2dTiledSprite, ModuleUICarrier>();

            public Vector2 CalculateAdditionalOffset(float angle)
            {
                return Toolbox.GetUnitOnCircle(angle - 90, 0.5f);
            }

            public void Nudge(PlayerController p)
            {
                if (isNudgeable == false) { return; }
                AkSoundEngine.PostEvent("Play_ENM_rubber_bounce_01", g.gameObject);
                GameManager.Instance.StartCoroutine(NudgeToPlayer(p));
            }

            public IEnumerator NudgeToPlayer(PlayerController p)
            {
                isNudgeable = false;
                Vector2 modPosition = g.transform.PositionVector2();
                float elapsed = 0f;
                while (elapsed < 0.7f)
                {
                    if (g == null) { yield break; }
                    elapsed += BraveTime.DeltaTime;
                    float t = elapsed / 1;
                    if (elapsed > 0.35f) { elapsed = 1; }
                    g.gameObject.transform.position = Vector3.Lerp(modPosition, p.SpriteBottomCenter, Toolbox.SinLerpTValueFull(t / 2));
                    yield return null;
                }
                elapsed = 0f;
                while (elapsed < 0.65f)
                {
                    if (g == null) { yield break; }
                    elapsed += BraveTime.DeltaTime;
                    yield return null;
                }
                isNudgeable = true;
                yield break;
            }
            private bool isNudgeable = true;

            public DefaultModule SelectModule(GenericLootTable table)
            {
                
                var mod = table.SelectByWeightNoExclusions().GetComponent<DefaultModule>();
                switch (mod.Tier)
                {
                    case DefaultModule.ModuleTier.Tier_1:
                        foreach (PlayerController p in GameManager.Instance.AllPlayers)
                        {if (p.PlayerHasCore() != null & p.HasPickupID(815)) 
                            {
                                return GlobalModuleStorage.SelectTable(PickupObject.ItemQuality.B).SelectByWeightNoExclusions().GetComponent<DefaultModule>();
                            }
                            }
                        if (UnityEngine.Random.value < 0.00005f) { AkSoundEngine.PostEvent("Play_BOSS_queenship_emerge_01", g.gameObject); return GlobalModuleStorage.ReturnRandomModule(DefaultModule.ModuleTier.Tier_Omega); }
                        return mod;
                    case DefaultModule.ModuleTier.Tier_2:
                        if (UnityEngine.Random.value < 0.0001f) { AkSoundEngine.PostEvent("Play_BOSS_queenship_emerge_01", g.gameObject); return GlobalModuleStorage.ReturnRandomModule(DefaultModule.ModuleTier.Tier_Omega); }
                        return mod;
                    case DefaultModule.ModuleTier.Tier_3:
                        if (UnityEngine.Random.value < 0.000175f) {
                            AkSoundEngine.PostEvent("Play_BOSS_queenship_emerge_01", g.gameObject); return GlobalModuleStorage.ReturnRandomModule(DefaultModule.ModuleTier.Tier_Omega);
                        }
                        return mod;
                    default: return mod;
                }
            }
            public void AlterCount()
            {
                if (g.quality == PickupObject.ItemQuality.B | g.quality == PickupObject.ItemQuality.A)
                {
                    Count++;
                }
                if (g.quality == PickupObject.ItemQuality.S)
                {
                    Count += 2;
                }
            }


            public void Start()
            {

                g = this.GetComponent<Gun>();
                var obj = g.gameObject.GetComponent<ShittyVFXAttacher>();
                if (obj)
                {
                    Destroy(obj);
                }
                AkSoundEngine.PostEvent("Play_OBJ_paydaydrill_start_01", g.gameObject);

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
                        EndPosition = Toolbox.GetUnitOnCircle(Toolbox.SubdivideCircle(Vector2.up.ToAngle() + (Arc * -1), Count, i), 2.5f), //, Count, i) , 2f),   
                        isUsingAlternate = isAlt
                    });
                }
                foreach(var r in selectableModules)
                {
                    r.Start();
                    var Extant_Tether = UnityEngine.Object.Instantiate(VFXStorage.VFX_Tether_Modulable, g.sprite.WorldCenter, Quaternion.identity).GetComponent<tk2dTiledSprite>();
                    Extant_Tether.dimensions = new Vector2(1, 16f);
                    Extant_Tether.transform.localRotation = Quaternion.Euler(0, 0, 0);
                    Extant_Tether.GetComponent<tk2dSpriteAnimator>().Play(isAlt ? "chain_alt_start" : "chain_start");
                    tk2DTiledSprites.Add(Extant_Tether, r);
                }

              
                g.StartCoroutine(LerpLight(g));
            }




            public void Update()
            {
                if (isBeingDestroyed == true) { return; }
                foreach (var entry in tk2DTiledSprites)
                {
                    float angle = ((entry.Value.extantModule.transform.PositionVector2() - g.sprite.WorldCenter) + new Vector2(0.5f, 0.5f)).ToAngle();
                    var vec = CalculateAdditionalOffset(angle);
                    entry.Key.dimensions = new Vector2(Vector2.Distance(g.sprite.WorldCenter + CalculateAdditionalOffset(angle) + vec, entry.Value.extantModule.GetComponent<tk2dBaseSprite>().WorldCenter + vec) * 16, 16f);
                    entry.Key.gameObject.transform.localRotation = Quaternion.Euler(0, 0, angle);
                    entry.Key.gameObject.transform.position = g.sprite.WorldCenter + vec;
                }
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
                foreach (var entry in tk2DTiledSprites)
                {
                    entry.Key.GetComponent<tk2dSpriteAnimator>().PlayAndDestroyObject(isAlt ? "chain_alt_break" : "chain_break");
                }

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
                private Vector2 Offset = new Vector2(-0.5f, -0.5f);
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
                        AkSoundEngine.PostEvent("Play_OBJ_metroid_roll_01", DefMod.gameObject);
                        DefMod.EnteredRange -= Entered;
                        DefMod.ExitedRange -= Exited;
                        DefMod.ChangeShader(StaticShaders.Default_Shader);
                        HasDropped = true;
                        controller.DestroyAllOthers();
                        DefMod.StartCoroutine(LerpLight(DefMod, 0, 7, 0, 3));
                        Destroy(this);
                        DefMod.StartCoroutine(this.DoMovementToPlayer(DefMod, p));
                        return false;
                    }
                    return (HasStoppedMoving);
                }

                private IEnumerator DoMovementToPlayer(DefaultModule self, PlayerController p)
                {
                    HasStoppedMoving = false;
                    self.PreInteractLogic -= PreInteract;
                    Vector2 modPosition = self.transform.PositionVector2();
                    float elapsed = 0f;
                    while (elapsed < 0.75f)
                    {
                        if (self == null) { yield break; }
                        elapsed += BraveTime.DeltaTime;
                        float t = elapsed / 1;
                        self.gameObject.transform.position = Vector3.Lerp(modPosition, p.SpriteBottomCenter, Toolbox.SinLerpTValue(t));
                        yield return null;
                    }
                    DebrisObject orAddComponent = self.gameObject.GetOrAddComponent<DebrisObject>();
                    orAddComponent.shouldUseSRBMotion = true;
                    orAddComponent.angularVelocity = 0f;
                    orAddComponent.Priority = EphemeralObject.EphemeralPriority.Critical;
                    orAddComponent.sprite.UpdateZDepth();
                    orAddComponent.Trigger(Vector3.up.WithZ(2f), 1, 1f);
                    self.OnEnteredRange(p);


                    yield break;
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
                        self.gameObject.transform.position = Vector3.Lerp(controller.g.sprite.WorldCenter + Offset, (controller.g.sprite.WorldCenter + EndPosition) + Offset, Toolbox.SinLerpTValue(t));
                        yield return null;
                    }
                    self.EnteredRange += Entered;
                    self.ExitedRange += Exited;
                    HasStoppedMoving = true;
                    while (this.controller)
                    {
                        if (HasStoppedMoving == true && extantModule.gameObject && controller.g)
                        {
                            extantModule.gameObject.transform.position = Vector2.MoveTowards(extantModule.gameObject.transform.position, (controller.g.sprite.WorldCenter + EndPosition) + Offset, 0.04f);
                        }
                        yield return null;
                    }
                    yield break;
                }

                /*
                public void Update()
                {
                    
                    if (controller)
                    {
                        Debug.Log("EX: " + (extantModule != null));
                        Debug.Log("EXOB: " + (extantModule.gameObject != null));
                        Debug.Log("CO: " + (controller != null));
                        Debug.Log("COG: " + (controller.g != null));
                        Debug.Log("COGSPR: " + (controller.g.sprite != null));

                        if (HasStoppedMoving == true && extantModule.gameObject && controller.g)
                        {
                            extantModule.gameObject.transform.position = Vector2.MoveTowards(extantModule.gameObject.transform.position, (controller.g.sprite.WorldCenter + EndPosition) + Offset, 0.075f);
                        }
                    }
                    
                }
                */
                public void Entered(DefaultModule DefMod)
                {
                    AkSoundEngine.PostEvent("Play_UI_menu_select_01", DefMod.gameObject);
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
                    //Extant_Tether.GetComponent<tk2dSpriteAnimator>().PlayAndDestroyObject(controller.isAlt ? "chain_alt_break" : "chain_break");
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
//                private tk2dTiledSprite Extant_Tether;

            }
        }
        public static void Init()
        {
            new Hook(typeof(Gun).GetMethod("Pickup", BindingFlags.Instance | BindingFlags.Public), typeof(Hooks).GetMethod("PickupHook"));
            //new Hook(typeof(Gun).GetMethod("OnEnteredRange", BindingFlags.Instance | BindingFlags.Public), typeof(Hooks).GetMethod("OnEnteredRangeHook"));

            new Hook(typeof(PlayerController).GetMethod("SetStencilVal", BindingFlags.Instance | BindingFlags.NonPublic), typeof(Hooks).GetMethod("SetStencilValHook"));
            new Hook(typeof(PlayerController).GetMethod("UpdateStencilVal", BindingFlags.Instance | BindingFlags.NonPublic), typeof(Hooks).GetMethod("UpdateStencilValHook"));
            new Hook(typeof(PlayerStats).GetMethod("RebuildGunVolleys", BindingFlags.Instance | BindingFlags.Public), typeof(Hooks).GetMethod("RebuildGunVolleysHook"));
            new Hook(typeof(DungeonData).GetMethod("FloodFillDungeonInterior", BindingFlags.Instance | BindingFlags.NonPublic), typeof(Hooks).GetMethod("FloodFillDungeonInteriorHook"));
            new Hook(typeof(RoomHandler).GetMethod("CheckCellArea", BindingFlags.Instance | BindingFlags.NonPublic), typeof(Hooks).GetMethod("CheckCellAreaHook"));
            new Hook(typeof(AIActor).GetMethod("TeleportSomewhere", BindingFlags.Instance | BindingFlags.Public), typeof(Hooks).GetMethod("TeleportationImmunity"));


            new Hook(
                 typeof(RoomHandler).GetMethod("AddProceduralTeleporterToRoom", BindingFlags.Instance | BindingFlags.Public),
                 typeof(Hooks).GetMethod("AddProceduralTeleporterToRoomHook", BindingFlags.Static | BindingFlags.Public)
             );
            JuneLib.ItemsCore.AddChangeSpawnItem(ReturnObj);
            
            
            new Hook(typeof(PickupObject).GetMethod("HandlePickupCurseParticles", BindingFlags.Instance | BindingFlags.NonPublic), typeof(Hooks).GetMethod("HandlePickupCurseParticlesHook"));

        }


        public static void AddProceduralTeleporterToRoomHook(Action<RoomHandler> orig, RoomHandler roomHandler)
        {
            //yes, this is a really cheap and shit way of preventing teleporters on **specificaly** my floor but it should work
            if (GameManager.Instance.Dungeon.BossMasteryTokenItemId == ModulePrinterCore.ModulePrinterCoreID) { return; }
            orig(roomHandler);
        }

        public static void TeleportationImmunity(Action<AIActor, IntVector2?, bool> orig, AIActor self, IntVector2? overrideClearance = null, bool keepClose = false)
        {
            if (self.GetComponent<TeleportationImmunity>() != null) { return; }
            orig(self, overrideClearance, keepClose);
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
                    //Debug.Log(self.rooms[i].GetRoomName());
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
                        bool flga = AdvancedGameStatsManager.Instance.GetFlag(CustomDungeonFlags.PAST);
                        if (HPComp.healAmount == 0.5f)
                        {
                            pickup = UnityEngine.Random.value < 0.02f && flga == true ? PickupObjectDatabase.GetById(CraftingCore.CraftingCoreID) : PickupObjectDatabase.GetById(Scrap.Scrap_ID);
                            pickup.gameObject.SetActive(true);

                        }
                        if (HPComp.healAmount == 1f)
                        {
                            pickup = UnityEngine.Random.value < 0.035f && flga == true ? PickupObjectDatabase.GetById(CraftingCore.CraftingCoreID) : PickupObjectDatabase.GetById(Scrap.Scrap_ID);
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
                var yes = self.gameObject.GetComponent<ChooseModuleController>();
                if (yes == null)
                {
                    yes = self.gameObject.AddComponent<ChooseModuleController>();
                    yes.isAlt = player.IsUsingAlternateCostume;

                }
                else
                {
                    yes.Nudge(player);
                }
            }
            else
            {
                var c = self.gameObject.GetComponent<ChooseModuleController>();
                if (c != null) { if (c.isBeingDestroyed == true) { return; } }
                orig(self, player);
            }
        }

        public static void OnEnteredRangeHook(Action<Gun, PlayerController> orig, Gun self, PlayerController player)
        {
            orig(self, player);
            if (player.PlayerHasCore() != null && self.gameObject.GetComponent<ShittyVFXAttacher>() == null && self.gameObject.GetComponent<ChooseModuleController>() == null)
            {
                var p = self.gameObject.AddComponent<ShittyVFXAttacher>();
                p.mainPlayer = player;
            }
        }


        //AwakeHook
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
                        bool flga = AdvancedGameStatsManager.Instance.GetFlag(CustomDungeonFlags.PAST);

                        if (HPComp.healAmount == 0.5f)
                        {
                            var g = UnityEngine.Random.value < 0.025f && flga == true ? PickupObjectDatabase.GetById(CraftingCore.CraftingCoreID).gameObject : PickupObjectDatabase.GetById(Scrap.Scrap_ID).gameObject;
                            self.m_shopItems[i] = g;
                            self.m_itemControllers[i].Initialize(g.GetComponent<PickupObject>(), self);
                        }
                        if (HPComp.healAmount == 1f)
                        {
                            var g = UnityEngine.Random.value < 0.0625f && flga == true ? PickupObjectDatabase.GetById(CraftingCore.CraftingCoreID).gameObject : PickupObjectDatabase.GetById(Scrap.Scrap_ID).gameObject;
                            self.m_shopItems[i] = g;
                            self.m_itemControllers[i].Initialize(g.GetComponent<PickupObject>(), self);
                        }
                    }
                }
            }
        }

        public static void HandlePickupCurseParticlesHook(Action<PickupObject> orig, PickupObject self)
        {
            orig(self);
            foreach (var player in GameManager.Instance.AllPlayers)
            {
                if (player.PlayerHasCore() != null && self.gameObject.GetComponent<ShittyVFXAttacher>() == null && self.gameObject.GetComponent<ChooseModuleController>() == null && BasicCustomSynergyNotifier.ModularSynergy.isSynergyItem(self.PickupObjectId) == true)
                {
                    var p = self.gameObject.AddComponent<ShittyVFXAttacher>();
                    p.gameObj = VFXStorage.VFX__Synergy;
                    p.mainPlayer = player;
                }
                else if (self is Gun && self.gameObject.GetComponent<ShittyVFXAttacher>() == null && self.gameObject.GetComponent<ChooseModuleController>() == null)
                {
                    var p = self.gameObject.AddComponent<ShittyVFXAttacher>();
                    p.mainPlayer = player;
                }
            }          
        }
    }
}