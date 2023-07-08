using Alexandria.CharacterAPI;
using Alexandria.ItemAPI;
using Dungeonator;
using HutongGames.PlayMaker.Actions;
using JuneLib.Items;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using static BossFinalRogueLaunchShips1;
using static dfMaterialCache;
using static DwarfEventListener;
using static tk2dSpriteCollectionDefinition;


namespace ModularMod
{
    public class STRING_EMPTY : DefaultModule
    {
        public static ItemTemplate template = new ItemTemplate(typeof(STRING_EMPTY))
        {
            Name = "STRING_EMPTY",
            Description = "One For Each Finger",
            LongDescription = "Slightly reduces Rate Of Fire, and reduces Damage by 33%. Shoot 5 times the projectiles (+2 Projectiles per stack)." + "\n\n" + "Tier:\n" + DefaultModule.ReturnTierLabel(DefaultModule.ModuleTier.Tier_3),
            ManualSpriteCollection = StaticCollections.Module_T3_Collection,
            ManualSpriteID = StaticCollections.Module_T3_Collection.GetSpriteIdByName("error_mod_t3_module"),
            Quality = ItemQuality.SPECIAL,
            PostInitAction = PostInit
        };
        public static void PostInit(PickupObject v)
        {
            var h = (v as DefaultModule);
            h.AltSpriteID = StaticCollections.Module_T3_Collection.GetSpriteIdByName("error_mod_t3_module_alt");
            h.Tier = ModuleTier.Tier_3;
            h.LabelName = "String.EMPTY " +StaticColorHexes.AddColorToLabelString("sprite Tier_3_Label", StaticColorHexes.Blue_Color_Hex);
            h.LabelDescription = "[Primary_Description_Placeholder]\n[secondaryEffect_Placeholder]\n"+StaticColorHexes.AddColorToLabelString("NullReferenceException: (999+)\nObject not set to an instance of an object.\nDefaultModule.ReturnTeritaryEffectText()\nTileBreaker.ReturnDescriptionLabel()", StaticColorHexes.Red_Color_Hex);
            h.SetTag("modular_module");
            h.AddColorLight(Color.white);
            h.AdditionalWeightMultiplier = 0.4f;
            h.Offset_LabelDescription = new Vector2(0.25f, -1.125f);
            h.Offset_LabelName = new Vector2(0.25f, 1.875f);
            h.OverrideScrapCost = 15;
            h.IsUncraftable = true;
            h.powerConsumptionData.OverridePowerDescriptionLabel = "Uses DefaultModule.PowerConsumption(ModuleQuality.Tier_3, -1)\n(" + StaticColorHexes.AddColorToLabelString("DefaultModule.AdditionalStackPowerConsumption(ModuleQuality.Tier_3, -1)", StaticColorHexes.Orange_Hex) + ")";
            h.EnergyConsumption = 0;
            h.AddToGlobalStorage();


            GameObject VFX = new GameObject("DummyTileSprite");
            FakePrefab.DontDestroyOnLoad(VFX);
            FakePrefab.MarkAsFakePrefab(VFX);
            var tk2d = VFX.AddComponent<tk2dSprite>();
            VFX.CreateFastBody(CollisionLayer.Projectile, new IntVector2(16, 16), new IntVector2(0, 0));
            DummySpriteObject = VFX;
            ID = h.PickupObjectId;
        }
        public static int ID;
        public static GameObject DummySpriteObject;

        public override bool CanBeDisabled(ModulePrinterCore modulePrinter, ModularGunController modularGunController)
        {
            return false;
        }

        public override void OnAnyEverObtainedNonActivation(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            AkSoundEngine.PostEvent("Play_Glitch_Y", player.gameObject);
            modulePrinter.PowerModule(this);
            var fx = player.PlayEffectOnActor(EmergencyResponse.WarnVFX, new Vector3(UnityEngine.Random.Range(-1, 1), UnityEngine.Random.Range(-1, 1)));
            fx.GetComponent<tk2dSpriteAnimator>().PlayAndDestroyObject("warn");
            var sprite = fx.GetComponent<tk2dBaseSprite>();
            sprite.usesOverrideMaterial = true;
            Material material = sprite.renderer.material;
            material.shader = ShaderCache.Acquire("Brave/Internal/Glitch");
            material.SetFloat("_GlitchInterval", 0.1f);
            material.SetFloat("_DispProbability", 0.6f);
            material.SetFloat("_DispIntensity", 0.024f);
            material.SetFloat("_ColorProbability", 0.7f);
            material.SetFloat("_ColorIntensity", 0.1f);
        }

        private tk2dTileMap currentTileMap;
        public override void OnFirstPickup(ModulePrinterCore printer, ModularGunController modularGunController, PlayerController player)
        {
            currentTileMap = GameManager.Instance.Dungeon.MainTilemap;
            printer.OnNewFloorStarted += ONFS;
            printer.PlayerEnteredAnyRoom += EnteredRoom;
            printer.OnFrameUpdate += FrameUpdate;
            printer.OnPostProcessProjectile += PPP;
        }

        public void PPP(ModulePrinterCore modulePrinterCore, Projectile p, float f, PlayerController player)
        {
            if (UnityEngine.Random.value < this.ReturnStack(modulePrinterCore) / 300)
            {
                modulePrinterCore.DoChanceBulletProc(p, f);
            }
            if (UnityEngine.Random.value < this.ReturnStack(modulePrinterCore) / 200)
            {
                var gl = p.gameObject.AddComponent<GlitchProjectileBehavior>();
                gl.stack = this.ReturnStack(modulePrinterCore);
            }
        }

        public override void OnLastRemoved(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.OnNewFloorStarted -= ONFS;
            modulePrinter.PlayerEnteredAnyRoom -= EnteredRoom;
            modulePrinter.OnFrameUpdate -= FrameUpdate;
            modulePrinter.OnPostProcessProjectile -= PPP;
        }
        private float ela;
        private float ela_2;
        private List<RoomHandler> enteredRooms = new List<RoomHandler>();
        public void FrameUpdate(ModulePrinterCore core, PlayerController p)
        {
            if (GameManager.Instance.IsLoadingLevel == true) { return; }

            ela += BraveTime.DeltaTime;
            ela_2 += BraveTime.DeltaTime;

            if (ela > 1f)
            {
                ela = 0;
                var l = allglitchedTiles.Where(self => self.currentRoom == p.CurrentRoom);
                var list = l.ToList();
                if (list.Count > 0)
                {
                    foreach (var entry in list)
                    {
                        entry.Spread();
                    }
                }
            }
            if (ela_2 > 120f)
            {
                ela_2 = 0;
                for (int i = core.ModuleContainers.Count - 1; i > -1; i--)
                {
                    var moduleContainer = core.ModuleContainers[i];
                    var shitfuck = core.ModuleContainers.Where(self => self.LabelName != this.LabelName && self.ActiveCount > 0);
                    if (shitfuck.Count() > 0)
                    {
                        var mod = BraveUtility.RandomElement<ModulePrinterCore.ModuleContainer>(shitfuck.ToList()).defaultModule;
                        p.StartCoroutine(DoFlashyVFX(p, mod, this));
                        core.RemoveModule(mod, 1);
                        core.AddModule(this, p);
                        core.PowerModule(this);
                    }
                }
            }
        }

        private static IEnumerator DoFlashyVFX(PlayerController player, DefaultModule properties, DefaultModule self)
        {
            Vector2 playerPos = player.sprite.WorldCenter;

            var VFX_Object = UnityEngine.Object.Instantiate(VFXStorage.VFX_SpriteAppear, playerPos, Quaternion.identity).GetComponent<tk2dBaseSprite>();
            VFX_Object.SetSprite(GlobalModuleStorage.ReturnModule(properties).sprite.collection, GlobalModuleStorage.ReturnModule(properties).sprite.spriteId);

            var light = VFX_Object.GetComponent<AdditionalBraveLight>();
            light.LightColor = GlobalModuleStorage.ReturnModule(properties).BraveLight.LightColor;

            Vector2 offset = Toolbox.GetUnitOnCircle(BraveUtility.RandomAngle(), UnityEngine.Random.Range(2f, 3.0f));
            float e = 0;
            while (e < 1)
            {
                float t = Toolbox.SinLerpTValue(e);

                VFX_Object.transform.position = Vector2.Lerp(playerPos, playerPos + offset, t);
                VFX_Object.renderer.material.SetFloat("_Fade", t);
                light.LightIntensity = Mathf.Lerp(0, 2.5f, t);
                light.LightRadius = Mathf.Lerp(0, 2, t);
                e += BraveTime.DeltaTime;
                yield return null;
            }
            e = 0;
            while (e < 0.25f)
            {
                e += BraveTime.DeltaTime;
                yield return null;
            }
            AkSoundEngine.PostEvent("Play_Glitch_Y", player.gameObject);
            VFX_Object.usesOverrideMaterial = true;
            Material material = VFX_Object.renderer.material;
            material.shader = ShaderCache.Acquire("Brave/Internal/Glitch");
            material.SetFloat("_GlitchInterval", 0.1f);
            material.SetFloat("_DispProbability", 0.4f);
            material.SetFloat("_DispIntensity", 0.024f);
            material.SetFloat("_ColorProbability", 0.7f);
            material.SetFloat("_ColorIntensity", 0.1f);
            e = 0;
            while (e < 1f)
            {
                e += BraveTime.DeltaTime;
                yield return null;
            }
            VFX_Object.SetSprite(GlobalModuleStorage.ReturnModule(self).sprite.collection, GlobalModuleStorage.ReturnModule(self).sprite.spriteId);
            material.shader = Shader.Find("Brave/Internal/SimpleAlphaFadeUnlit");
            material.SetFloat("_Fade", 0f);
            e = 0;
            while (e < 0.6f)
            {
                e += BraveTime.DeltaTime;
                yield return null;
            }
            e = 0;
            Vector2 p = VFX_Object.transform.PositionVector2();
            float d = UnityEngine.Random.Range(0.7f, 1.5f);
            while (e < d)
            {
                float t = Toolbox.SinLerpTValue(e / d);
                VFX_Object.transform.position = Vector2.Lerp(p, player.sprite.WorldCenter, t);
                light.LightIntensity = Mathf.Lerp(2.5f, 1, t);
                light.LightRadius = Mathf.Lerp(0, 2, t);
                VFX_Object.renderer.material.SetFloat("_Fade", 1 - t);

                e += BraveTime.DeltaTime;
                yield return null;
            }
            LootEngine.DoDefaultSynergyPoof(player.sprite.WorldCenter);
            UnityEngine.Object.Destroy(VFX_Object.gameObject);
            yield break;
        }


        public void ONFS(ModulePrinterCore core, PlayerController p)
        { currentTileMap = GameManager.Instance.Dungeon.MainTilemap; }

        public static List<GlitchTileBehavior> allglitchedTiles = new List<GlitchTileBehavior>();

        public void EnteredRoom(ModulePrinterCore modulePrinter, PlayerController player, RoomHandler room)
        {
            if (enteredRooms.Contains(room)) { return; }
            enteredRooms.Add(room);
            int stack = this.ReturnStack(modulePrinter);
            List<IntVector2> cells = new List<IntVector2>();
            cells.AddRange(room.Cells);
            cells.Shuffle();
            int c = Mathf.Max((room.Cells.Count / 400) + stack, stack);

            for (int I = 0; I < c; I++)
            {
                var tM = GameManager.Instance.Dungeon.data.cellData[cells[I].x][cells[I].y].positionInTilemap;
                var sprite = UnityEngine.Object.Instantiate(DummySpriteObject, tM.ToCenterVector3(5), Quaternion.identity).GetComponent<tk2dBaseSprite>();
                sprite.SetSprite(currentTileMap.spriteCollection, UnityEngine.Random.Range(0, currentTileMap.spriteCollection.spriteDefinitions.Count()));
                sprite.usesOverrideMaterial = true;
                Material material = sprite.renderer.material;
                material.shader = ShaderCache.Acquire("Brave/Internal/Glitch");
                material.SetFloat("_GlitchInterval", 0.1f);
                material.SetFloat("_DispProbability", 0.4f);
                material.SetFloat("_DispIntensity", 0.024f);
                material.SetFloat("_ColorProbability", 0.7f);
                material.SetFloat("_ColorIntensity", 0.1f);
                var tile = sprite.gameObject.AddComponent<GlitchTileBehavior>();
                tile.position = tM;
                tile.currentRoom = room;
                tile.Damage = 7 + (stack*2);
                tile.DelayTimer = 1.5f + stack;

            }
        }

        public class GlitchTileBehavior : MonoBehaviour
        {
            public IntVector2 position;
            private tk2dTileMap map;

            public RoomHandler currentRoom;
            private SpeculativeRigidbody body;

            public int Damage = 10;
            public float DelayTimer = 1.5f;

            public void Start()
            {
                map = GameManager.Instance.Dungeon.MainTilemap;
                body = this.GetComponent<SpeculativeRigidbody>();
                body.OnPreRigidbodyCollision += OPC;
                allglitchedTiles.Add(this);
            }

            public void OPC(SpeculativeRigidbody myBody, PixelCollider myCollider, SpeculativeRigidbody otherBody, PixelCollider otherCollider)
            {
                PhysicsEngine.SkipCollision = true;
                if (otherBody.aiActor != null)
                {
                    otherBody.aiActor.healthHaver.ApplyDamage(Damage, this.transform.PositionVector2(), "ERROR", CoreDamageTypes.Void);
                    myBody.RegisterTemporaryCollisionException(otherBody, 0.666f);
                }
            }
            private float e = 0;
            public void Update()
            {
                e += BraveTime.DeltaTime;
                if (e > 0.2f)
                {
                    e = 0;
                    this.GetComponent<tk2dBaseSprite>().SetSprite(map.spriteCollection, UnityEngine.Random.Range(0, map.spriteCollection.spriteDefinitions.Count()));
                }
            }

            public void Spread()
            {
                if (HasPlaced == true) { return; }
                IntVector2 pain = directionReturn();
                if (pain == new IntVector2(-69, -69)) { return; }
                var c = GameManager.Instance.Dungeon.data.cellData[pain.x][pain.y];
                if (c == null) { return; }
                var tM = GameManager.Instance.Dungeon.data.cellData[pain.x][pain.y].positionInTilemap;

                var sprite = UnityEngine.Object.Instantiate(DummySpriteObject, tM.ToCenterVector3(5), Quaternion.identity).GetComponent<tk2dBaseSprite>();
                sprite.SetSprite(map.spriteCollection, UnityEngine.Random.Range(0, map.spriteCollection.spriteDefinitions.Count()));
                sprite.usesOverrideMaterial = true;
                Material material = sprite.renderer.material;
                material.shader = ShaderCache.Acquire("Brave/Internal/Glitch");
                material.SetFloat("_GlitchInterval", 0.1f);
                material.SetFloat("_DispProbability", 0.4f);
                material.SetFloat("_DispIntensity", 0.024f);
                material.SetFloat("_ColorProbability", 0.7f);
                material.SetFloat("_ColorIntensity", 0.1f);
                var tile = sprite.gameObject.AddComponent<GlitchTileBehavior>();
                tile.position = tM;
                tile.currentRoom = currentRoom;
                tile.Damage = this.Damage;
                tile.DelayTimer = this.DelayTimer;

                Destroy(this.gameObject, DelayTimer);
                HasPlaced = true;
            }

            public bool HasPlaced = false;

            private IntVector2 directionReturn()
            {
                IntVector2 fuck = new IntVector2(-69, -69);
               int c = 10;
                while (c > 0)
                {
                    var vec = CanWeNotHaveSwitchCasesThatBreakMethods();
                    IntVector2 pain = new IntVector2(position.x + ((int)vec.x), position.y + ((int)vec.y));
                    if (allglitchedTiles.Where(self => self.position == pain).Count() == 0)
                    {
                        if (pain.ToCenterVector2().GetAbsoluteRoom() == this.currentRoom)
                        {
                            fuck = pain;
                            c = -1;
                        }
                    }
                    c--;
                }
                return fuck;
            }
            private Vector2 CanWeNotHaveSwitchCasesThatBreakMethods()
            {
                
                switch (UnityEngine.Random.Range(1, 5))
                {
                    case 1:
                        return Vector2.up;
                    case 2:
                        return Vector2.down;
                    case 3:
                        return Vector2.left;
                    case 4:
                        return Vector2.right;
                    default:
                        return Vector2.zero;
                }
            }

            public void OnDestroy()
            {
                if (allglitchedTiles.Contains(this)) { allglitchedTiles.Remove(this); }
            }
        }

        public class GlitchProjectileBehavior : BraveBehaviour
        {
            public int stack = 1;
            public void Start()
            {
                projectile.sprite.usesOverrideMaterial = true;
                Material material = projectile.sprite.renderer.material;
                material.shader = ShaderCache.Acquire("Brave/Internal/Glitch");
                material.SetFloat("_GlitchInterval", 0.1f);
                material.SetFloat("_DispProbability", 0.4f);
                material.SetFloat("_DispIntensity", 0.024f);
                material.SetFloat("_ColorProbability", 0.7f);
                material.SetFloat("_ColorIntensity", 0.1f);
                projectile.OnHitEnemy += OHE;
            }

            public void OHE(Projectile self, SpeculativeRigidbody enemy, bool fatal)
            {
                if (enemy.aiActor)
                {
                    if (!enemy.aiActor.healthHaver.IsBoss && !enemy.aiActor.healthHaver.IsSubboss && enemy.gameObject.GetComponent<QuickFreeze>() == null)
                    {
                        enemy.gameObject.AddComponent<QuickFreeze>();
                        GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(ResourceCache.Acquire("Global VFX/VFX_Synergy_Poof_001"), enemy.transform.position, Quaternion.identity);
                        tk2dBaseSprite component = gameObject.GetComponent<tk2dBaseSprite>();
                        gameObject.transform.localScale *= 2.5f;
                        component.sprite.usesOverrideMaterial = true;
                        Material material = component.sprite.renderer.material;
                        material.shader = ShaderCache.Acquire("Brave/Internal/Glitch");
                        material.SetFloat("_GlitchInterval", 0.1f);
                        material.SetFloat("_DispProbability", 0.4f);
                        material.SetFloat("_DispIntensity", 0.024f);
                        material.SetFloat("_ColorProbability", 0.7f);
                        material.SetFloat("_ColorIntensity", 0.1f);
                        Destroy(gameObject, 1.5f);

                    }
                }
            }
            public IEnumerator DoFreeze(AIActor enemy)
            {
                var f = enemy.LocalTimeScale;
                enemy.LocalTimeScale = 0;
                float e = 0;
                while (e < this.stack)
                {
                    e += BraveTime.DeltaTime;
                    yield return null;
                }
                if (enemy)
                {
                    enemy.LocalTimeScale = f;
                }
                yield break;
            }
        }
        public class QuickFreeze : BraveBehaviour
        {
            public void Start()
            {
                lts = this.aiActor.LocalTimeScale;
                this.aiActor.LocalTimeScale = 0;

            }
            private float lts;
            float ela = 0;
            public void Update()
            {
                ela += BraveTime.DeltaTime;
                if (ela > 1)
                {
                    this.aiActor.LocalTimeScale = lts;
                    Destroy(this, 2);
                }
            }
        }
    }
}

