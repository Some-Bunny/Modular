using Alexandria.ItemAPI;
using JuneLib.Items;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace ModularMod
{
    public class HeatVectoring : DefaultModule
    {
        public static ItemTemplate template = new ItemTemplate(typeof(HeatVectoring))
        {
            Name = "Heat Vectoring",
            Description = "Knock 'Em Down",
            LongDescription = "Grants 10% (+5% per stack) movement speed. Dodgerolling releases 5 (+3 per stack) projectiles in the direction opposite of where you're moving. Projectiles gain all projectile-based effects. Effect recharges after 6 seconds." + "\n\n" + "Tier:\n" + DefaultModule.ReturnTierLabel(DefaultModule.ModuleTier.Tier_2),
            ManualSpriteCollection = StaticCollections.Module_T2_Collection,
            ManualSpriteID = StaticCollections.Module_T2_Collection.GetSpriteIdByName("heatvectoring_t2_module"),
            Quality = ItemQuality.SPECIAL,
            PostInitAction = PostInit
        };
        public static void PostInit(PickupObject v)
        {
            var h = (v as DefaultModule);
            h.AltSpriteID = StaticCollections.Module_T2_Collection.GetSpriteIdByName("heatvectoring_t2_module_alt");
            h.Tier = ModuleTier.Tier_2;
            h.LabelName = "Heat Vectoring " + h.ReturnTierLabel();
            h.LabelDescription = "Grants 10% (" + StaticColorHexes.AddColorToLabelString("+5%", StaticColorHexes.Light_Orange_Hex) + ") movement speed.\nDodgerolling releases 5 (" + StaticColorHexes.AddColorToLabelString("+4", StaticColorHexes.Light_Orange_Hex) + ") projectiles in the direction\nopposite of where you're moving.\nProjectiles gain all projectile-based effects.\nEffect recharges after 6 seconds.";
            h.AdditionalWeightMultiplier = 0.7f;
            h.EnergyConsumption = 1;


            h.AddModuleTag(BaseModuleTags.DEFENSIVE);
            h.AddModuleTag(BaseModuleTags.UNIQUE);
            h.AddModuleTag(BaseModuleTags.DAMAGE_OVER_TIME);

            h.AddToGlobalStorage();
            h.SetTag("modular_module");
            h.AddColorLight(Color.green);
            h.Offset_LabelDescription = new Vector2(0.25f, -1.125f);
            h.Offset_LabelName = new Vector2(0.25f, 1.875f);
            //EncounterDatabase.GetEntry(h.encounterTrackable.EncounterGuid).usesPurpleNotifications = true;
            ID = h.PickupObjectId;

            var proj_Base = (PickupObjectDatabase.GetById(125) as Gun).DefaultModule.projectiles[0];
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(88) as Gun).DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            FakePrefab.MakeFakePrefab(projectile.gameObject);
            DontDestroyOnLoad(projectile.gameObject);

            projectile.baseData.damage = 18f;
            projectile.shouldRotate = true;
            projectile.baseData.range = 1000f;
            projectile.baseData.speed = 35;
            projectile.baseData.force = 25;
            projectile.baseData.UsesCustomAccelerationCurve = true;
            projectile.baseData.AccelerationCurve = AnimationCurve.Linear(0.1f, 0.2f, 1, 1);
            projectile.AppliesFire = true;
            projectile.FireApplyChance = 1f;
            projectile.fireEffect = DebuffStatics.hotLeadEffect;


            projectile.AnimateProjectileBundle("heatvector_idle", StaticCollections.Projectile_Collection, StaticCollections.Projectile_Animation, "heatvector_idle",
            new List<IntVector2>() { new IntVector2(11, 7), new IntVector2(11, 7), new IntVector2(11, 7), new IntVector2(11, 7), new IntVector2(11, 7), new IntVector2(11, 7), new IntVector2(11, 7), },
            ProjectileToolbox.ConstructListOfSameValues(true, 7), ProjectileToolbox.ConstructListOfSameValues(tk2dBaseSprite.Anchor.MiddleCenter, 7), ProjectileToolbox.ConstructListOfSameValues(true, 7), ProjectileToolbox.ConstructListOfSameValues(false, 7),
            ProjectileToolbox.ConstructListOfSameValues<Vector3?>(null, 7), ProjectileToolbox.ConstructListOfSameValues<IntVector2?>(null, 7), ProjectileToolbox.ConstructListOfSameValues<IntVector2?>(null, 7), ProjectileToolbox.ConstructListOfSameValues<Projectile>(null, 7));
            projectile.objectImpactEventName = (PickupObjectDatabase.GetById(384) as Gun).DefaultModule.projectiles[0].objectImpactEventName;
            projectile.enemyImpactEventName = (PickupObjectDatabase.GetById(384) as Gun).DefaultModule.projectiles[0].enemyImpactEventName;

            projectile.hitEffects.tileMapHorizontal = Toolbox.MakeObjectIntoVFX(proj_Base.hitEffects.enemy.effects.First().effects.First().effect);
            projectile.hitEffects.tileMapVertical = Toolbox.MakeObjectIntoVFX(proj_Base.hitEffects.enemy.effects.First().effects.First().effect);
            projectile.hitEffects.enemy = Toolbox.MakeObjectIntoVFX(proj_Base.hitEffects.enemy.effects.First().effects.First().effect);
            projectile.hitEffects.deathAny = Toolbox.MakeObjectIntoVFX(proj_Base.hitEffects.enemy.effects.First().effects.First().effect);

            Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
            mat.mainTexture = projectile.sprite.renderer.material.mainTexture;
            mat.SetColor("_EmissiveColor", new Color32(255, 255, 255, 255));
            mat.SetFloat("_EmissiveColorPower", 100);
            mat.SetFloat("_EmissivePower", 100);
            projectile.sprite.renderer.material = mat;

            ImprovedAfterImage yes = projectile.gameObject.AddComponent<ImprovedAfterImage>();
            yes.spawnShadows = true;
            yes.shadowLifetime = 0.33f;
            yes.shadowTimeDelay = 0.05f;
            yes.dashColor = new Color(0.8f, 0.2f, 0.0f, 1f);

            proj = projectile;

        }
        public static int ID;
        public static Projectile proj;

        public override void OnFirstPickup(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.VoluntaryMovement_Modifier += ModifySpeed;
            modulePrinter.RollStarted += RollStarted;
        }

        private bool IsOnCooldown = false;

        public void RollStarted(ModulePrinterCore core, PlayerController player, Vector2 direction)
        {
            if (IsOnCooldown == false)
            {
                AkSoundEngine.PostEvent("Play_ENM_Grip_Master_Eject_01", player.gameObject);
                int stacc = this.ReturnStack(core);
                int amount = 1 + (stacc * 4);
                int Backwards = (amount / 2) - amount;

                var vfx = player.PlayEffectOnActor((PickupObjectDatabase.GetById(370) as Gun).muzzleFlashEffects.effects[0].effects[0].effect, new Vector3(1, 0.375f));
                vfx.transform.localRotation = Quaternion.Euler(0, 0, direction.ToAngle() + 180);
                vfx.transform.localScale = Vector3.one * 0.6f;
                Destroy(vfx, 1);
                for (int i = Backwards +1; i < (amount / 2) +1   ; i++)
                {
                    GameObject spawnedBulletOBJ = SpawnManager.SpawnProjectile(proj.gameObject, player.sprite.WorldCenter, Quaternion.Euler(0f, 0f, (direction.ToAngle() + (15 * i)) + 180), true);
                    Projectile component = spawnedBulletOBJ.GetComponent<Projectile>();
                    if (component != null)
                    {
                        component.Owner = player;
                        component.Shooter = player.specRigidbody;
                        player.DoPostProcessProjectile(component);
                    }
                }
                player.StartCoroutine(DoCooldown(player));
            }
        }

        public IEnumerator DoCooldown(PlayerController p)
        {
            IsOnCooldown = true;
            float e = 0;
            while (e < 6)
            {
                e += BraveTime.DeltaTime;
                yield return null;
            }
            AkSoundEngine.PostEvent("Play_ENM_ironmaiden_close_01", p.gameObject);
            for (int I = 0; I < 30; I++)
            {
                GlobalSparksDoer.DoSingleParticle(p.sprite.WorldCenter, Toolbox.GetUnitOnCircle(12 * I, 20), null, 2f, null, GlobalSparksDoer.SparksType.EMBERS_SWIRLING);
            }
            IsOnCooldown = false;
            yield break;
        }

        public Vector2 ModifySpeed(Vector2 currentVelocity, ModulePrinterCore core, PlayerController player)
        {
            return currentVelocity *= 1 +  0.05f + (this.ReturnStack(core) / 20);
        }

        public override void OnLastRemoved(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.VoluntaryMovement_Modifier -= ModifySpeed;
            modulePrinter.RollStarted -= RollStarted;
        }
    }
}

