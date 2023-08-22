using Alexandria.ItemAPI;
using Dungeonator;
using HutongGames.PlayMaker.Actions;
using JuneLib.Items;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace ModularMod
{
    public class TachyonWarp : DefaultModule
    {
        public static ItemTemplate template = new ItemTemplate(typeof(TachyonWarp))
        {
            Name = "Tachyon Warp",
            Description = "WARP ZONE!!!!",
            LongDescription = "Greatly increases accuracy, adds slight homing to your projectiles and slightly increases damage (+Stronger Homing and Damage per stack). Projectiles fire from walls, instead of your gun. Shooting pushes nearby enemies away slightly. (+Stronger push force per stack)\n\n" + "Tier:\n" + DefaultModule.ReturnTierLabel(DefaultModule.ModuleTier.Tier_2),
            ManualSpriteCollection = StaticCollections.Module_T2_Collection,
            ManualSpriteID = StaticCollections.Module_T2_Collection.GetSpriteIdByName("tachyonwarp_t2_module"),
            Quality = ItemQuality.SPECIAL,
            PostInitAction = PostInit
        };
        public static void PostInit(PickupObject v)
        {
            var h = (v as DefaultModule);
            h.AltSpriteID = StaticCollections.Module_T2_Collection.GetSpriteIdByName("tachyonwarp_t2_module_alt");
            h.Tier = ModuleTier.Tier_2;
            h.LabelName = "Tachyon Warp " + h.ReturnTierLabel();
            h.LabelDescription = "Greatly increases accuracy, adds slight homing to your projectiles\nand slightly increases damage (" + StaticColorHexes.AddColorToLabelString("+Stronger Homing and Damage", StaticColorHexes.Light_Orange_Hex) + ").\nProjectiles fire from walls, instead of your gun. Shooting pushes\nnearby enemies away slightly. (" + StaticColorHexes.AddColorToLabelString("+Stronger Push Force", StaticColorHexes.Light_Orange_Hex) + ")";
            h.OverrideScrapCost = 9;
            h.AdditionalWeightMultiplier = 0.8f;
            h.SetTag("modular_module");
            h.AddColorLight(Color.green);
            h.Offset_LabelDescription = new Vector2(0.25f, -1.125f);
            h.Offset_LabelName = new Vector2(0.25f, 1.875f);
            h.IsUncraftable = true;
            h.EnergyConsumption = 2;
            h.AddToGlobalStorage();
            //EncounterDatabase.GetEntry(h.encounterTrackable.EncounterGuid).usesPurpleNotifications = true;
            ID = h.PickupObjectId;
            greenImpact = new VFXPool()
            {
                type = VFXPoolType.Single,
                effects = new VFXComplex[]
                {
                    new VFXComplex()
                    {
                        effects = new VFXObject[]
                        {
                            new VFXObject()
                            {
                                effect = (PickupObjectDatabase.GetById(345) as Gun).DefaultModule.projectiles[0].hitEffects.tileMapHorizontal.effects[0].effects[0].effect
                            }
                        }
                    }
                }
            };
            ModulePrinterCore.ModifyForChanceBulletsOneFrameDelay += h.ChanceBulletsModify;
        }


        public override void ChanceBulletsModify(ModulePrinterCore modulePrinterCore, Projectile p, float f, PlayerController player)
        {
            if (UnityEngine.Random.value > 0.05f) { return; }
            int stack = 1;
            Exploder.DoRadialPush(player.sprite.WorldCenter, 50 * stack, 5);
            Exploder.DoRadialKnockback(player.sprite.WorldCenter, 50 * stack, 5);
            Exploder.DoRadialMinorBreakableBreak(player.sprite.WorldCenter, 5);

            Vector2 unitPosition = p.specRigidbody.Position.UnitPosition;
            Vector2 vector = this.FindExpectedEndPoint(p);
            p.transform.position = vector.ToVector3ZisY(0f);
            p.specRigidbody.Reinitialize();
            p.Direction = (vector - unitPosition).normalized;
            p.SendInDirection(p.Direction * -1f, true, true);
            p.m_distanceElapsed = 0f;
            p.LastPosition = p.transform.position;
            greenImpact.SpawnAtPosition(vector.ToVector3ZisY(0f), 0f, null, null, null, null, false, null, null, false);
            p.baseData.range += 5;
            p.baseData.speed *= 1.5f;
            p.baseData.force *= 0.33f;
            p.UpdateSpeed();
            var homing = p.gameObject.GetOrAddComponent<HomingModifier>();
            homing.AngularVelocity += 90 * stack;
            homing.HomingRadius += 5 * stack;
            p.pierceMinorBreakables = true;
            p.baseData.damage *= 1 + (0.15f * stack);

            p.IgnoreTileCollisionsFor(4f / p.baseData.speed);
            p.UpdateCollisionMask();
        }

        public static VFXPool greenImpact;
        public static int ID;

        public override void OnFirstPickup(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modularGunController.ChangeMuzzleFlash((PickupObjectDatabase.GetById(228) as Gun).muzzleFlashEffects);
            modulePrinter.OnPostProcessProjectileOneFrameDelay += PPP;

            this.gunStatModifier = new ModuleGunStatModifier()
            {
                Accuracy_Process = ProcessAccuracy,
            };
            modulePrinter.ProcessGunStatModifier(this.gunStatModifier);
        }
        public float ProcessAccuracy(float f, ModulePrinterCore modulePrinterCore, ModularGunController modularGunController, PlayerController player)
        {
            return f - (f - (f / (1 + 0.5f)));
        }

        public void PPP(ModulePrinterCore modulePrinterCore, Projectile p, float f, PlayerController player)
        {
            int stack = this.ReturnStack(modulePrinterCore);

            Exploder.DoRadialPush(player.sprite.WorldCenter, 50 * stack, 5);
            Exploder.DoRadialKnockback(player.sprite.WorldCenter, 50 * stack, 5);
            Exploder.DoRadialMinorBreakableBreak(player.sprite.WorldCenter, 5);

            Vector2 unitPosition = p.specRigidbody.Position.UnitPosition;
            Vector2 vector = this.FindExpectedEndPoint(p);
            p.transform.position = vector.ToVector3ZisY(0f);
            p.specRigidbody.Reinitialize();
            p.Direction = (vector - unitPosition).normalized;
            p.SendInDirection(p.Direction * -1f, true, true);
            p.m_distanceElapsed = 0f;
            p.LastPosition = p.transform.position;
            greenImpact.SpawnAtPosition(vector.ToVector3ZisY(0f), 0f, null, null, null, null, false, null, null, false);
            p.baseData.range += 5;
            p.baseData.speed *= 1.25f;
            p.baseData.force *= 0.33f;
            p.UpdateSpeed();
            var homing = p.gameObject.GetOrAddComponent<HomingModifier>();
            homing.AngularVelocity += 90 * stack;
            homing.HomingRadius += 5 * stack;
            p.pierceMinorBreakables = true;
            p.baseData.damage *= 1 + (0.15f * stack);

            p.IgnoreTileCollisionsFor(4f / p.baseData.speed);
            p.UpdateCollisionMask();

        }
        protected Vector2 FindExpectedEndPoint(Projectile projectile)
        {
            Dungeon dungeon = GameManager.Instance.Dungeon;
            Vector2 unitCenter = projectile.specRigidbody.UnitCenter;
            Vector2 b = unitCenter + projectile.Direction.normalized * projectile.baseData.range;
            var m_room = unitCenter.GetAbsoluteRoom();
            bool flag = false;
            Vector2 vector = unitCenter;
            IntVector2 intVector = vector.ToIntVector2(VectorConversions.Floor);
            if (dungeon.data.CheckInBoundsAndValid(intVector))
            {
                flag = dungeon.data[intVector].isExitCell;
            }
            float num = b.x - unitCenter.x;
            float num2 = b.y - unitCenter.y;
            float num3 = Mathf.Sign(b.x - unitCenter.x);
            float num4 = Mathf.Sign(b.y - unitCenter.y);
            bool flag2 = num3 > 0f;
            bool flag3 = num4 > 0f;
            int num5 = 0;
            while (Vector2.Distance(vector, b) > 0.1f && num5 < 10000)
            {
                num5++;
                float num6 = Mathf.Abs((((!flag2) ? Mathf.Floor(vector.x) : Mathf.Ceil(vector.x)) - vector.x) / num);
                float num7 = Mathf.Abs((((!flag3) ? Mathf.Floor(vector.y) : Mathf.Ceil(vector.y)) - vector.y) / num2);
                int num8 = Mathf.FloorToInt(vector.x);
                int num9 = Mathf.FloorToInt(vector.y);
                IntVector2 intVector2 = new IntVector2(num8, num9);
                bool flag4 = false;
                if (!dungeon.data.CheckInBoundsAndValid(intVector2))
                {
                    break;
                }
                CellData cellData = dungeon.data[intVector2];
                if (cellData.nearestRoom != m_room || cellData.isExitCell != flag)
                {
                    break;
                }
                if (cellData.type != CellType.WALL)
                {
                    flag4 = true;
                }
                if (flag4)
                {
                    intVector = intVector2;
                }
                if (num6 < num7)
                {
                    num8++;
                    vector.x += num6 * num + 0.1f * Mathf.Sign(num);
                    vector.y += num6 * num2 + 0.1f * Mathf.Sign(num2);
                }
                else
                {
                    num9++;
                    vector.x += num7 * num + 0.1f * Mathf.Sign(num);
                    vector.y += num7 * num2 + 0.1f * Mathf.Sign(num2);
                }
            }
            return intVector.ToCenterVector2();
        }

        public override void OnLastRemoved(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modularGunController.RevertMuzzleFlash();
            modulePrinter.OnPostProcessProjectileOneFrameDelay -= PPP;
            modulePrinter.RemoveGunStatModifier(this.gunStatModifier);
        }
    }
}

