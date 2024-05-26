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
    public class PaceBreaker : DefaultModule
    {
        public static ItemTemplate template = new ItemTemplate(typeof(PaceBreaker))
        {
            Name = "Pace Breaker",
            Description = "See The Use",
            LongDescription = "Adds a new mode to your starting active item. Using an active item now releases an electromagnetic pulse who's strength scales off of how long the active item takes to charge. (+More Power per stack).\n\n" + "Tier:\n" + DefaultModule.ReturnTierLabel(DefaultModule.ModuleTier.Tier_2),
            ManualSpriteCollection = StaticCollections.Module_T2_Collection,
            ManualSpriteID = StaticCollections.Module_T2_Collection.GetSpriteIdByName("pacebreaker_t2_module"),
            Quality = ItemQuality.SPECIAL,
            PostInitAction = PostInit
        };
        public static void PostInit(PickupObject v)
        {
            var h = (v as DefaultModule);
            h.AltSpriteID = StaticCollections.Module_T2_Collection.GetSpriteIdByName("pacebreaker_t2_module_alt");
            h.Tier = ModuleTier.Tier_2;
            h.LabelName = "Pace Breaker " + h.ReturnTierLabel();
            h.LabelDescription =  StaticColorHexes.AddColorToLabelString("Adds a new mode to your starting active item", StaticColorHexes.Lime_Green_Hex) +".\nUsing an active item now releases an electromagnetic pulse\nwho's strength scales off of how long the active item takes to charge.\n("+StaticColorHexes.AddColorToLabelString("More Power", StaticColorHexes.Light_Orange_Hex)+").";
            h.EnergyConsumption = 1;
            h.AppearsFromBlessedModeRoll = false;

            h.AddModuleTag(BaseModuleTags.DEFENSIVE);
            h.AddModuleTag(BaseModuleTags.UNIQUE);
            h.AdditionalWeightMultiplier = 0.65f;

            h.AddToGlobalStorage();
            h.SetTag("modular_module");
            h.AddColorLight(Color.green);
            h.Offset_LabelDescription = new Vector2(0.25f, -1.125f);
            h.Offset_LabelName = new Vector2(0.25f, 1.875f);
            ID = h.PickupObjectId;
        }
        public static int ID;

        public Scrapper.ActiveItemMode PulseMode = new Scrapper.ActiveItemMode()
        {
            Removable = true,
            spriteCollection = StaticCollections.Item_Collection,
            Sprite_Name = "pulse",
            OnUsed = OnUsed
        };

        public static void OnUsed(PlayerController p, Scrapper scrapper)
        {
            float f = BraveUtility.RandomAngle();
            for (int i =0; i < 6; i++)
            {
                GameObject spawnedBulletOBJ = SpawnManager.SpawnProjectile((PickupObjectDatabase.GetById(13) as Gun).DefaultModule.projectiles[0].gameObject, p.sprite.WorldCenter, Quaternion.Euler(0f, 0f, (60* i)+f), true);
                Projectile component = spawnedBulletOBJ.GetComponent<Projectile>();
                if (component != null)
                {
                    component.baseData.damage = 8;
                    component.Owner = p;
                    component.Shooter = p.specRigidbody;
                    p.DoPostProcessProjectile(component);
                    var b = p.gameObject.GetOrAddComponent<BounceProjModifier>();
                    b.numberOfBounces += 3;
                    b.damageMultiplierOnBounce = 1.25f;
                }
            }
        }

        public override void OnFirstPickup(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            foreach (var item in player.activeItems)
            {
                if (item is Scrapper scrapper)
                {
                    scrapper.AddMode(PulseMode);
                }
            }
            player.OnUsedPlayerItem += this.DoEffect;
        }

        public override void OnLastRemoved(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            foreach (var item in player.activeItems)
            {
                if (item is Scrapper scrapper)
                {
                    scrapper.RemoveMode(PulseMode);
                }
            }
            player.OnUsedPlayerItem -= this.DoEffect;
        }
        private void DoEffect(PlayerController usingPlayer, PlayerItem usedItem)
        {
            AkSoundEngine.PostEvent("Play_BOSS_lichC_zap_01", usingPlayer.gameObject);

            int s = this.ReturnStack(Stored_Core);
            float Power = 10;

            if (ConfigManager.DoVisualEffect == true)
            {
                for (int i = 0; i < 4; i++)
                {
                    GameObject breakVFX = UnityEngine.Object.Instantiate<GameObject>((PickupObjectDatabase.GetById(156) as Gun).DefaultModule.projectiles[0].hitEffects.tileMapVertical.effects[0].effects[0].effect, usingPlayer.sprite.WorldCenter + new Vector2(UnityEngine.Random.Range(1.25f, -1.25f), UnityEngine.Random.Range(0.625f, -0.625f)), Quaternion.identity);
                    tk2dBaseSprite component = breakVFX.GetComponent<tk2dBaseSprite>();
                    component.PlaceAtPositionByAnchor(usingPlayer.sprite.WorldCenter + new Vector2(UnityEngine.Random.Range(1.25f, -1.25f), UnityEngine.Random.Range(0.625f, -1.25f)), tk2dBaseSprite.Anchor.MiddleCenter);
                    component.HeightOffGround = 35f;
                    component.UpdateZDepth();
                    Destroy(breakVFX, 3);
                }
            }


            Power *= 1 + ((usedItem.timeCooldown / 5) * s);
            Power *= 1 + ((usedItem.roomCooldown / 2) * s);
            Power *= 1 + ((usedItem.damageCooldown / 50) * s);
            Power *= usedItem.consumable == true ? usedItem.numberOfUses + 0.5f : 1;

            float Range = (Power % 30) + 3;
            Exploder.DoRadialPush(usingPlayer.sprite.WorldCenter, Power, Range);
            Exploder.DoRadialKnockback(usingPlayer.sprite.WorldCenter, Power, Range);
            Exploder.DoRadialMinorBreakableBreak(usingPlayer.sprite.WorldCenter, Range);

            ApplyActionToNearbyEnemies(usingPlayer.sprite.WorldCenter, 5, usingPlayer.CurrentRoom, Power);
            Exploder.DoDistortionWave(usingPlayer.sprite.WorldCenter, (Power / 75) * ConfigManager.DistortionWaveMultiplier, 0.25f * ConfigManager.DistortionWaveMultiplier, Range, 0.3f + (Range / 100));
        }
        public void ApplyActionToNearbyEnemies(Vector2 position, float radius, RoomHandler room, float Power)
        {
            List<AIActor> a = new List<AIActor>();
            float num = radius * radius;
            if (room.activeEnemies != null)
            {
                for (int i = 0; i < room.activeEnemies.Count; i++)
                {
                    if (room.activeEnemies[i])
                    {
                        AIActor aI = room.activeEnemies[i];
                        bool flag = radius < 0f;
                        Vector2 vector = room.activeEnemies[i].CenterPosition - position;
                        if (!flag)
                        {
                            flag = (vector.sqrMagnitude < num);
                        }
                        if (flag)
                        {
                            if (aI.behaviorSpeculator)
                            {
                                if (aI.behaviorSpeculator.ImmuneToStun == false && Power > 50) { aI.behaviorSpeculator.Stun((0.1f + (Power/ 100))); }
                            }
                            aI.healthHaver.ApplyDamage((5f * (Power / 6.66f)), aI.transform.PositionVector2(), "Vent", CoreDamageTypes.Fire);
                        }
                    }
                }
            }
        }

    }
}

