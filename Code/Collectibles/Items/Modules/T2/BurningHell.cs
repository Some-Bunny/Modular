using Alexandria.ItemAPI;
using Alexandria.Misc;
using Dungeonator;
using JuneLib.Items;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace ModularMod
{
    public class BurningHell : DefaultModule
    {
        public static ItemTemplate template = new ItemTemplate(typeof(BurningHell))
        {
            Name = "Burning Hell",
            Description = "Exchange Rate",
            LongDescription = "Halves Clip size and Rate Of Fire. Projectiles will stick to terrain and enemies and create an area that hurts and burns enemies. (+Burning Radius And Damage per stack.)" + "\n\n" + "Tier:\n" + DefaultModule.ReturnTierLabel(DefaultModule.ModuleTier.Tier_2),
            ManualSpriteCollection = StaticCollections.Module_T2_Collection,
            ManualSpriteID = StaticCollections.Module_T2_Collection.GetSpriteIdByName("burninghell_t2_module"),
            Quality = ItemQuality.SPECIAL,
            PostInitAction = PostInit
        };
        public static void PostInit(PickupObject v)
        {
            var h = (v as DefaultModule);
            h.AltSpriteID = StaticCollections.Module_T2_Collection.GetSpriteIdByName("burninghell_t2_module_alt");
            h.Tier = ModuleTier.Tier_2;
            h.LabelName = "Burning Hell " + h.ReturnTierLabel();
            h.LabelDescription = "Halves Rate Of Fire and increases accuracy.\nProjectiles will stick to terrain and enemies and\ncreate an area that hurts and burns enemies.\n(" + StaticColorHexes.AddColorToLabelString("+Burning Radius And Damage.", StaticColorHexes.Light_Orange_Hex) + ")";
            h.SetTag("modular_module");
            h.AddColorLight(Color.green);
            h.Offset_LabelDescription = new Vector2(0.25f, -1.125f);
            h.Offset_LabelName = new Vector2(0.25f, 1.875f);
            h.EnergyConsumption = 2;

            h.AddModuleTag(BaseModuleTags.STICKY);
            h.AddModuleTag(BaseModuleTags.TRADE_OFF);
            h.AddModuleTag(BaseModuleTags.DAMAGE_OVER_TIME);

            h.AddToGlobalStorage();

            ModulePrinterCore.ModifyForChanceBullets += h.ChanceBulletsModify;
            ID = h.PickupObjectId;
        }
        public static int ID;

        public override void ChanceBulletsModify(ModulePrinterCore modulePrinterCore, Projectile p, float f, PlayerController player)
        {
            if (UnityEngine.Random.value > 0.01f) { return; }
            p.baseData.speed *= 1.66f;
            p.pierceMinorBreakables = true;
            p.UpdateSpeed();

            var mod = p.gameObject.GetOrAddComponent<StickyProjectileModifier>();
            mod.stickyContexts.Add(new StickyProjectileModifier.StickyContext() { CanStickToTerrain = true, CanStickEnemies = true });
            mod.OnStick += H_S;
            mod.OnStickyDestroyed += H2;
        }
    
        public static tk2dSpriteAnimation mineAnimation;


        public void H_S(GameObject stick, StickyProjectileModifier comp, tk2dBaseSprite sprite, PlayerController p)
        {
            var obj = UnityEngine.Object.Instantiate((PickupObjectDatabase.GetById(328) as Gun).DefaultModule.chargeProjectiles[0].Projectile.hitEffects.overrideMidairDeathVFX, stick.transform.position, Quaternion.identity);
            Destroy(obj, 2);
            GameManager.Instance.StartCoroutine(DoTimer(stick,2.5f, 10));
        }

        public override void OnFirstPickup(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            this.stickyContext = new StickyProjectileModifier.StickyContext()
            {
                CanStickToTerrain = true,
                CanStickEnemies = true
            };
            this.gunStatModifier = new ModuleGunStatModifier()
            {
                Name = "BurningHell",
                Accuracy_Process = PFR,
                FireRate_Process = FireRate,
                ChargeSpeed_Process = FireRate,
            };
            modulePrinter.ProcessGunStatModifier(this.gunStatModifier);
            modulePrinter.stickyContexts.Add(this.stickyContext);

            modulePrinter.OnProjectileStickAction += H;
            modulePrinter.OnStickyDestroyAction += H2;
            modulePrinter.OnPostProcessProjectile += PPP;
        }


        public void PPP(ModulePrinterCore modulePrinterCore, Projectile p, float f, PlayerController player, bool IsCrit)
        {
            p.baseData.speed *= 1.5f;
            p.pierceMinorBreakables = true;

            p.UpdateSpeed();
        }
        public void H(GameObject stick, StickyProjectileModifier comp, tk2dBaseSprite sprite, PlayerController p)
        {
            var obj = UnityEngine.Object.Instantiate((PickupObjectDatabase.GetById(328) as Gun).DefaultModule.chargeProjectiles[0].Projectile.hitEffects.overrideMidairDeathVFX, stick.transform.position, Quaternion.identity);
            Destroy(obj, 2);
            GameManager.Instance.StartCoroutine(DoTimer(stick, 2.5f + this.Stack(), 10));
        }

        //This code fucking sucks lmfaoooo
        public IEnumerator DoTimer(GameObject sticky,float radius = 2, float DetTime = 10)
        {
            RoomHandler room = sticky.transform.position.GetAbsoluteRoom();
            float e = 0;
            float asdf = 0;

            while (e < 1)
            {
                if (sticky == null)
                {
                    break; 
                }
                if (room != null)
                {
                    if (asdf > 0.2f)
                    {
                        asdf = 0;
                        GlobalSparksDoer.DoSingleParticle(sticky.transform.PositionVector2() + Toolbox.GetUnitOnCircle(BraveUtility.RandomAngle(), UnityEngine.Random.Range(0.1f, radius)), Vector2.up, null, 3, null, GlobalSparksDoer.SparksType.EMBERS_SWIRLING);
                    }
                    float t = e / 1;
                    float throne1 = Mathf.Sin(t * (Mathf.PI / 2));
                    room.ApplyActionToNearbyEnemies(sticky.transform.position, Mathf.Lerp(0, radius, throne1), Enemies);
                }
                e += BraveTime.DeltaTime;
                asdf += BraveTime.DeltaTime;
                yield return null;
            }
            e = 0;
            asdf = 0;
            while (e < DetTime)
            {
                if (sticky == null)
                {
                    break; 
                }
                if (room != null)
                {
                    if (asdf > 0.25f)
                    {
                        asdf = 0;
                        GlobalSparksDoer.DoSingleParticle(sticky.transform.PositionVector2() + Toolbox.GetUnitOnCircle(BraveUtility.RandomAngle(), UnityEngine.Random.Range(0.1f, radius)), Vector2.up, null, 3, null, GlobalSparksDoer.SparksType.EMBERS_SWIRLING);
                    }
                    room.ApplyActionToNearbyEnemies(sticky.transform.position, radius, Enemies);
                }
                asdf += BraveTime.DeltaTime;
                e += BraveTime.DeltaTime;
                yield return null;
            }

            Destroy(sticky);
            yield break;
        }

        public void Enemies(AIActor aI, float f)
        {
            if (aI)
            {
                aI.healthHaver.ApplyDamage((1.25f * this.Stack()) * BraveTime.DeltaTime, Vector2.zero, "Hellfire", CoreDamageTypes.Fire, DamageCategory.DamageOverTime);
                if (UnityEngine.Random.value < 0.025f)
                {
                    aI.gameActor.ApplyEffect(DebuffStatics.hotLeadEffect);
                }
            }
        }

        public void H2(GameObject stick, StickyProjectileModifier comp, PlayerController p)
        {
            var obj = UnityEngine.Object.Instantiate((PickupObjectDatabase.GetById(328) as Gun).DefaultModule.chargeProjectiles[0].Projectile.hitEffects.overrideMidairDeathVFX, stick.transform.position, Quaternion.identity);
            Destroy(obj, 2);
        }

        public override void OnLastRemoved(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.RemoveGunStatModifier(this.gunStatModifier);

            if (modulePrinter.stickyContexts.Contains(this.stickyContext))
            {
                modulePrinter.stickyContexts.Remove(this.stickyContext);
            }

            modulePrinter.OnProjectileStickAction -= H;
            modulePrinter.OnStickyDestroyAction -= H2;
            modulePrinter.OnPostProcessProjectile -= PPP;
        }

        public override void OnAnyPickup(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player, bool truePickup)
        {
        }

        public float FireRate(float f, ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            return f * 2;
        }
        public float PFR(float f, ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            return f / 2;
        }
    }
}

