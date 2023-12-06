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
using static tk2dSpriteCollectionDefinition;


namespace ModularMod
{
    public class EternalWrap : DefaultModule
    {
        public static ItemTemplate template = new ItemTemplate(typeof(EternalWrap))
        {
            Name = "Eternal Wrap",
            Description = "Devourer",
            LongDescription = "Acts as 1 (+1 per stack) of every module you will own." + "\n\n" + "Tier:\n" + DefaultModule.ReturnTierLabel(DefaultModule.ModuleTier.Tier_3),
            ManualSpriteCollection = StaticCollections.Module_T4_Collection,
            ManualSpriteID = StaticCollections.Module_T4_Collection.GetSpriteIdByName("CORE_OF_GOD"),
            Quality = ItemQuality.EXCLUDED,
            PostInitAction = PostInit
        };

        public static void PostInit(PickupObject v)
        {
            var h = (v as DefaultModule);
            //h.AltSpriteID = StaticCollections.Module_T3_Collection.GetSpriteIdByName("swarmer_t3_module_alt");
            h.Tier = ModuleTier.Tier_Omega;
            h.LabelName = "ETERNAL WRAP " + h.ReturnTierLabel();
            h.LabelDescription = "PROJECTILES WRAP AROUND ROOMS\nAND GAIN STRENGTH WHEN THEY DO.\n(" + StaticColorHexes.AddColorToLabelString("+MORE WRAP AROUNDS AND DAMAGE", StaticColorHexes.Light_Orange_Hex) + ")";
            h.powerConsumptionData = new PowerConsumptionData()
            {
                FirstStack = 0,
                AdditionalStacks = 0,
                OverridePowerDescriptionLabel = "USES NO POWER."
            };
            h.AddToGlobalStorage();
            h.SetTag("modular_module");
            h.AddColorLight(Color.red);
            h.AdditionalWeightMultiplier = 0.9f;
            h.Offset_LabelDescription = new Vector2(0.25f, -1.125f);
            h.Offset_LabelName = new Vector2(0.25f, 1.875f);
            h.Label_Background_Color_Override = new Color32(255, 10, 10, 100);

            ID = h.PickupObjectId;
        }
        public static int ID;

        public static string SFX = (PickupObjectDatabase.GetById(169) as Gun).DefaultModule.projectiles[0].enemyImpactEventName;

        public override void OnFirstPickup(ModulePrinterCore printer, ModularGunController modularGunController, PlayerController player)
        {
            printer.OnPostProcessProjectile += PPP;
        }

        public void PPP(ModulePrinterCore modulePrinterCore, Projectile p, float f, PlayerController player, bool IsCrit)
        {
            var wrap = p.gameObject.GetOrAddComponent<WraparoundProjectile>();
            wrap.Cap = 2 + this.ReturnStack(modulePrinterCore);
            wrap.OnWrappedAround = (projectile, pos1, pos2) =>
            {
                if (projectile != null)
                {
                    GameObject gameObject1 = UnityEngine.Object.Instantiate<GameObject>((PickupObjectDatabase.GetById(169) as Gun).DefaultModule.projectiles[0].hitEffects.tileMapHorizontal.effects.First().effects.First().effect, pos1, Quaternion.identity);
                    GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>((PickupObjectDatabase.GetById(169) as Gun).DefaultModule.projectiles[0].hitEffects.tileMapHorizontal.effects.First().effects.First().effect, pos2, Quaternion.identity);
                    Destroy(gameObject1, 2);
                    Destroy(gameObject2, 2);
                    projectile.baseData.damage *= 1f + (0.25f * this.ReturnStack(modulePrinterCore));
                    projectile.baseData.speed *= 1f + (0.1f * this.ReturnStack(modulePrinterCore));
                    projectile.UpdateSpeed();
                    AkSoundEngine.PostEvent("Play_WPN_" + SFX + "_impact_01", projectile.gameObject);
                }
            };
        }
        public override void OnLastRemoved(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.OnPostProcessProjectile -= PPP;
        }
    }


    public class WraparoundProjectile : MonoBehaviour
    {
        public void Start()
        {
            this.projectile = base.GetComponent<Projectile>();
            projectile.baseData.range *= RangeMultiplier;
            this.projectile.BulletScriptSettings = new BulletScriptSettings()
            {
                surviveTileCollisions = true
            };
            SpeculativeRigidbody specRigidbody = this.projectile.specRigidbody;
            specRigidbody.OnTileCollision += (obj1) =>
            {
                if (projectile && Warps < Cap)
                {
                    projectile.IgnoreTileCollisionsFor(3f / projectile.baseData.speed);
                    projectile.UpdateCollisionMask();
                    WoopShoop(this.projectile, obj1.Normal);
                }

            };
            /*
            specRigidbody.OnPreTileCollision = (SpeculativeRigidbody.OnPreTileCollisionDelegate)Delegate.Combine(specRigidbody.OnPreTileCollision, new SpeculativeRigidbody.OnPreTileCollisionDelegate(delegate (SpeculativeRigidbody myRigidbody, PixelCollider myPixelCollider, PhysicsEngine.Tile tile, PixelCollider tilePixelCollider)
            {

                projectile.IgnoreTileCollisionsFor(3f / projectile.baseData.speed);
                projectile.UpdateCollisionMask();
                if (Warps < Cap)
                {
                    var dungeonData = GameManager.Instance.Dungeon.data;
                    CellData cell = dungeonData.cellData[tile.X][tile.Y];
                    if (cell.type == CellType.WALL)
                    {
                        c

                        if (dungeonData.isLeftSideWall(tile.X, tile.Y))
                        {
                            WoopShoop(this.projectile, Vector2.right);
                            return;
                        }
                        if (dungeonData.isRightSideWall(tile.X, tile.Y))
                        {
                            WoopShoop(this.projectile, Vector2.left);
                            return;
                        }
                        if (dungeonData.isFaceWallLower(tile.X, tile.Y) | dungeonData.isFaceWallHigher(tile.X, tile.Y))
                        {
                            WoopShoop(this.projectile, Vector2.down);
                            return;
                        }
                        if (dungeonData.isFaceWallLower(tile.X, tile.Y) | dungeonData.isWallDownRight(tile.X, tile.Y))
                        {
                            WoopShoop(this.projectile, Vector2.up);
                            return;
                        }

                    }

                }
            }));
            */
        }


        private int Warps = 0;
        private float RangeMultiplier = 4;

        public void WoopShoop(Projectile p, Vector2 direction)
        {
            Warps++;
            int rayMask = CollisionMask.LayerToMask(CollisionLayer.HighObstacle);
            if (p == null) { return; }
            var cast = Toolbox.ReturnRaycast(p.sprite.WorldCenter + (direction * 0.5f), direction, rayMask, 1000, null);
            var Position = cast.Contact;
            if (p && Position != null)
            {
                if (OnWrappedAround != null)
                {
                    OnWrappedAround(p, p.transform.PositionVector2(), Position);
                }
                p.transform.position = Position;
                p.specRigidbody.Reinitialize();
            }
        }




        public void Update()
        {
            if (Warps >= Cap && this.projectile)
            {
                if (this.projectile.BulletScriptSettings != null)
                {
                    this.projectile.BulletScriptSettings.surviveTileCollisions = false;
                }
            }
        }
        public Action<Projectile, Vector2, Vector2> OnWrappedAround;


        public int Cap = 1;

        private Projectile projectile;
    }

}

