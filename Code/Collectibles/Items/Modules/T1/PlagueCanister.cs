using Alexandria.ItemAPI;
using Dungeonator;
using HutongGames.PlayMaker.Actions;
using JuneLib.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Collections;

namespace ModularMod
{
    public class GlassShardBehavior : MonoBehaviour
    {
        public void Start()
        {
            Destroy(this.gameObject, 30);
            var body = this.GetComponent<SpeculativeRigidbody>();
            body.OnPreRigidbodyCollision += AAA;
        }

        public float Damage = 6;

        public void AAA(SpeculativeRigidbody myRigidbody, PixelCollider myPixelCollider, SpeculativeRigidbody otherRigidbody, PixelCollider otherPixelCollider)
        {
            PhysicsEngine.SkipCollision = true;
            if (otherRigidbody.aiActor != null && otherRigidbody.healthHaver != null && otherRigidbody.healthHaver.m_player == null)
            {
                otherRigidbody.healthHaver.ApplyDamage(Damage, Vector2.zero, "Glass Shards");
                Destroy(this.gameObject);
            }
        }
        public void OnDestroy()
        {
            AkSoundEngine.PostEvent("Play_OBJ_glass_shatter_01", this.gameObject);
            var VFX = UnityEngine.Object.Instantiate(PlagueCanister.Glass, this.gameObject.transform.position, Quaternion.identity);
            Destroy(VFX, 2);
        }
    }

    public class PlagueCanister : DefaultModule
    {
        public static ItemTemplate template = new ItemTemplate(typeof(PlagueCanister))
        {
            Name = "Plague Canister",
            Description = "Yah Yeet!",
            LongDescription = "Reloading has a chance to launch 1 (+1 per stack) poison canister that breaks into a pool of poison and glass shards. (+Glass Shards and Goop Radius per stack). Chance increases the emptier your clip is." + "\n\n" + "Tier:\n" + DefaultModule.ReturnTierLabel(DefaultModule.ModuleTier.Tier_1),
            ManualSpriteCollection = StaticCollections.Module_T1_Collection,
            ManualSpriteID = StaticCollections.Module_T1_Collection.GetSpriteIdByName("plaguecanister_tier1_module"),
            Quality = ItemQuality.SPECIAL,
            PostInitAction = PostInit
        };
        public static void PostInit(PickupObject v)
        {
            var h = (v as DefaultModule);
            h.AltSpriteID = StaticCollections.Module_T1_Collection.GetSpriteIdByName("plaguecanister_tier1_module_alt");
            h.Tier = ModuleTier.Tier_1;
            h.LabelName = "Plague Canister " + h.ReturnTierLabel();
            h.AdditionalWeightMultiplier = 0.85f;

            h.LabelDescription = "Reloading has a chance to launch 1 ("+StaticColorHexes.AddColorToLabelString("+1", StaticColorHexes.Light_Orange_Hex) +") poison canisters\nthat breaks into a pool of poison and glass shards.\n(" + StaticColorHexes.AddColorToLabelString("+Glass Shards and Goop Radius", StaticColorHexes.Light_Orange_Hex) + ").\nChance increases the emptier your clip is.";

            h.AddModuleTag(BaseModuleTags.DAMAGE_OVER_TIME);
            h.AddModuleTag(BaseModuleTags.CONDITIONAL);
            h.AddModuleTag(BaseModuleTags.UNIQUE);

            h.AddToGlobalStorage();
            h.SetTag("modular_module");
            h.AddColorLight(Color.cyan);
            h.Offset_LabelDescription = new Vector2(0.25f, -1f);
            h.Offset_LabelName = new Vector2(0.25f, 1.75f);
            //EncounterDatabase.GetEntry(h.encounterTrackable.EncounterGuid).usesPurpleNotifications = true;



            GameObject VFX = new GameObject("Plague Canister");
            FakePrefab.DontDestroyOnLoad(VFX);
            FakePrefab.MarkAsFakePrefab(VFX);
            VFX.SetActive(false);
            var tk2d = VFX.AddComponent<tk2dSprite>();
            tk2d.Collection = StaticCollections.VFX_Collection;
            tk2d.SetSprite(StaticCollections.VFX_Collection.GetSpriteIdByName("warning_003"));
            var tk2dAnim = VFX.AddComponent<tk2dSpriteAnimator>();
            tk2dAnim.Library = Module.ModularAssetBundle.LoadAsset<GameObject>("PoisonVialAnimation").GetComponent<tk2dSpriteAnimation>();
            tk2dAnim.defaultClipId = tk2dAnim.Library.GetClipIdByName("vial_idle");
            tk2dAnim.playAutomatically = true;
            var gooper = VFX.AddComponent<ThrownGoopItem>();
            gooper.goop = JuneLib.Status.EasyGoopDefinitions.PoisonDef;
            gooper.goopRadius = 2.75f;
            gooper.burstAnim = "vial_break";
            Vial = VFX;
             ID = h.PickupObjectId;

            GlassShards.Add(generateGlassShards("glassshards_001"));
            GlassShards.Add(generateGlassShards("glassshards_002"));
            GlassShards.Add(generateGlassShards("glassshards_003"));
            GlassShards.Add(generateGlassShards("glassshards_004"));
            GlassShards.Add(generateGlassShards("glassshards_005"));
            Glass = (PickupObjectDatabase.GetById(223) as Gun).DefaultModule.projectiles[0].hitEffects.enemy.effects[0].effects[0].effect;

        }
        public static GameObject Glass;
        public static int ID;

        public static GameObject Vial;
        public static List<GameObject> GlassShards = new List<GameObject>();

        public static GameObject generateGlassShards(string shardName = "glassshards_001")
        {
            GameObject VFX = new GameObject("Glass_Shard");
            FakePrefab.MarkAsFakePrefab(VFX);
            FakePrefab.DontDestroyOnLoad(VFX);
            VFX.SetActive(false);

            var tk2d = VFX.AddComponent<tk2dSprite>();
            tk2d.Collection = StaticCollections.VFX_Collection;
            tk2d.SetSprite(StaticCollections.VFX_Collection.GetSpriteIdByName(shardName));
            DebrisObject component = VFX.AddComponent<DebrisObject>();
            SpeculativeRigidbody body = VFX.AddComponent<SpeculativeRigidbody>();
            body.CollideWithOthers = true;
            body.CollideWithTileMap = false;
            body.PixelColliders = new List<PixelCollider>();
            body.PixelColliders.Add(new PixelCollider
            {
                ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual,
                CollisionLayer = CollisionLayer.Projectile,
                IsTrigger = false,
                BagleUseFirstFrameOnly = false,
                SpecifyBagelFrame = string.Empty,
                BagelColliderNumber = 0,
                ManualOffsetX = 0,
                ManualOffsetY = -0,
                ManualWidth = 6,
                ManualHeight = 6,
                ManualDiameter = 0,
                ManualLeftX = 0,
                ManualLeftY = 0,
                ManualRightX = 0,
                ManualRightY = 0,
            });
            VFX.AddComponent<GlassShardBehavior>();
            return VFX;
        }


        public override void OnFirstPickup(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.OnGunReloaded += OGR;
        }
        public override void OnLastRemoved(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.OnGunReloaded -= OGR;
        }
        public override void OnAnyPickup(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player, bool truePickup)
        {
            int stack = this.ReturnStack(modulePrinter);
        }

        public IEnumerator TossCanisters(int stack, PlayerController user)
        {
            for (int i = 0; i < stack; i++)
            {
                Vector3 vector = user.unadjustedAimPoint - user.LockedApproximateSpriteCenter;
                Vector3 vector2 = user.specRigidbody.UnitCenter;
                if (vector.y > 0f)
                {
                    vector2 += Vector3.up * 0.25f;
                }
                GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(Vial, vector2, Quaternion.identity);
                tk2dBaseSprite component4 = gameObject2.GetComponent<tk2dBaseSprite>();
                if (component4)
                {
                    component4.PlaceAtPositionByAnchor(vector2, tk2dBaseSprite.Anchor.MiddleCenter);
                }

                Vector2 vector3 = user.unadjustedAimPoint - user.LockedApproximateSpriteCenter;
                DebrisObject debrisObject = LootEngine.DropItemWithoutInstantiating(gameObject2, gameObject2.transform.position, vector3, 6.5f + (stack * 2f), false, false, true, false);
                if (vector.y > 0f && debrisObject)
                {
                    debrisObject.additionalHeightBoost = -1f;
                    if (debrisObject.sprite)
                    {
                        debrisObject.sprite.UpdateZDepth();
                    }
                }
                gameObject2.GetComponent<ThrownGoopItem>().goopRadius = 2.25f + (stack / 2);
                debrisObject.IsAccurateDebris = true;
                debrisObject.Priority = EphemeralObject.EphemeralPriority.Critical;
                debrisObject.bounceCount = 0;
                debrisObject.OnGrounded += (obj) =>
                {
                    for (int e = 0; e < 2 + stack; e++)
                    {
                        GameObject shard = UnityEngine.Object.Instantiate<GameObject>(BraveUtility.RandomElement<GameObject>(GlassShards), debrisObject.transform.position, Quaternion.identity);
                        shard.GetComponent<DebrisObject>().Trigger(Toolbox.GetUnitOnCircle(BraveUtility.RandomAngle(), UnityEngine.Random.Range(0.5f, 4 + stack)), 1);
                        shard.GetComponent<GlassShardBehavior>().Damage += stack * 0.5f;
                    }
                };
                yield return new WaitForSeconds(user.CurrentGun.reloadTime / stack);
            }
            yield break;
        }


        public void OGR(ModulePrinterCore modulePrinterCore, PlayerController user, Gun g)
        {
            if (UnityEngine.Random.value > g.PercentageOfClipLeft())
            {
                user.StartCoroutine(TossCanisters(this.ReturnStack(modulePrinterCore), user));
               
            }
        }  
    }
}

