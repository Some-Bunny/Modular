﻿using Alexandria.ItemAPI;
using JuneLib.Items;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace ModularMod
{
    public class LightningNets : DefaultModule
    {
        public static ItemTemplate template = new ItemTemplate(typeof(LightningNets))
        {
            Name = "Lightning Nets",
            Description = "Nicola Would Be Proud",
            LongDescription = "Player projectiles are now tethered with electricity (+Tether range and damage)" + "\n\n" + "Tier:\n" + DefaultModule.ReturnTierLabel(DefaultModule.ModuleTier.Tier_2),
            ManualSpriteCollection = StaticCollections.Module_T2_Collection,
            ManualSpriteID = StaticCollections.Module_T2_Collection.GetSpriteIdByName("voltaicrounds_t2_module"),
            Quality = ItemQuality.SPECIAL,
            PostInitAction = PostInit
        };
        public static void PostInit(PickupObject v)
        {
            var h = (v as DefaultModule);
            h.AltSpriteID = StaticCollections.Module_T2_Collection.GetSpriteIdByName("voltaicrounds_t2_module_alt");
            h.Tier = ModuleTier.Tier_2;
            h.LabelName = "Lightning Nets " + h.ReturnTierLabel();
            h.LabelDescription = "Player Projectiles are now tethered with electricity\n(" + StaticColorHexes.AddColorToLabelString("+Tether range and Damage", StaticColorHexes.Light_Orange_Hex) + ")";
            h.AddToGlobalStorage();
            h.SetTag("modular_module");
            h.AddColorLight(Color.green);
            h.Offset_LabelDescription = new Vector2(0.25f, -1.125f);
            h.Offset_LabelName = new Vector2(0.25f, 1.875f);
            //EncounterDatabase.GetEntry(h.encounterTrackable.EncounterGuid).usesPurpleNotifications = true;
            
            ID = h.PickupObjectId;
            ModulePrinterCore.ModifyForChanceBullets += h.ChanceBulletsModify;

        }
        public static int ID;

        public override void ChanceBulletsModify(ModulePrinterCore modulePrinterCore, Projectile p, float f, PlayerController player)
        {
            if (UnityEngine.Random.value > 0.15f) { return; }
            int stack = 1;
            var chain = p.gameObject.AddComponent<ElectricChainProjectile>();
            chain.Damage = p.baseData.damage;
            chain.Range = 4f + (4 * stack);
            chain.player = player;
            chain.projectile = p.gameObject;
            p.baseData.range += 5;
        }


        public override void OnFirstPickup(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.OnPostProcessProjectile += PPP;
        }
        public override void OnLastRemoved(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.OnPostProcessProjectile -= PPP;
        }
        public void PPP(ModulePrinterCore modulePrinterCore, Projectile p, float f, PlayerController player)
        {
            int stack = this.ReturnStack(modulePrinterCore);
            var chain = p.gameObject.AddComponent<ElectricChainProjectile>();
            chain.Damage = p.baseData.damage;
            chain.Range = 4f + (4 * stack);
            chain.player = player;
            chain.projectile = p.gameObject;
            p.baseData.range += 5;

        }

        public static List<ElectricChainProjectile> allactiveTetherProjectiles = new List<ElectricChainProjectile>();
    }
    public class ElectricChainProjectile : MonoBehaviour
    {
        public void Start()
        {
            LightningNets.allactiveTetherProjectiles.Add(this);
        }

        private Dictionary<GameObject, GameObject> ExtantTethers = new Dictionary<GameObject, GameObject>();
        private float Tick = 0.25f;
        private float Elapsed = 0;


        public void Update()
        {
            Elapsed += BraveTime.DeltaTime;
            if (this.projectile != null)
            {
                List<ElectricChainProjectile> activeProjectiles = LightningNets.allactiveTetherProjectiles;
                if (activeProjectiles != null && activeProjectiles.Count > 0)
                {
                    foreach (ElectricChainProjectile ai in activeProjectiles)
                    {
                        if (ai != null && Vector2.Distance(ai.gameObject.transform.PositionVector2(), this.projectile.GetComponentInChildren<tk2dBaseSprite>().WorldCenter) < Range && ai.gameObject.GetComponent<ElectricChainProjectile>() != null && ai != this.projectile)
                        {
                            if (!ExtantTethers.ContainsKey(ai.gameObject) && !ai.gameObject.GetComponent<ElectricChainProjectile>().ExtantTethers.ContainsKey(ai.gameObject))
                            {
                                GameObject obj = SpawnManager.SpawnVFX(VFXStorage.FriendlyElectricLinkVFX, false).GetComponent<tk2dTiledSprite>().gameObject;
                                ExtantTethers.Add(ai.gameObject, obj);
                            }
                        }
                        if (ai != null && Vector2.Distance(ai.transform.PositionVector2(), this.projectile.GetComponentInChildren<tk2dBaseSprite>().WorldCenter) > Range)
                        {
                            if (ExtantTethers.ContainsKey(ai.gameObject))
                            {
                                GameObject obj;
                                ExtantTethers.TryGetValue(ai.gameObject, out obj);
                                SpawnManager.Despawn(obj);
                                ExtantTethers.Remove(ai.gameObject);
                            }
                        }
                    }
                }
            }
            foreach (var si in ExtantTethers)
            {
                if (this.projectile && si.Value != null && si.Key != null)
                {
                    UpdateLink(this.projectile, si.Value.GetComponent<tk2dTiledSprite>(), si.Key, Elapsed > Tick ? true : false);
                }
                if (si.Key != null && si.Value != null && this.projectile == null)
                {
                    SpawnManager.Despawn(si.Value.gameObject);
                    ExtantTethers.Remove(si.Key);
                    return;
                }
                if (si.Key == null && si.Value != null)
                {
                    SpawnManager.Despawn(si.Value.gameObject);
                    ExtantTethers.Remove(si.Key);
                    return;
                }
            }
        }

        public void OnDestroy()
        {
            foreach (var si in ExtantTethers)
            {
                if (si.Value != null)
                {
                    SpawnManager.Despawn(si.Value.gameObject);
                }
            }
            ExtantTethers.Clear();
            if (LightningNets.allactiveTetherProjectiles.Contains(this))
            {
                LightningNets.allactiveTetherProjectiles.Remove(this);
            }
        }

        private void UpdateLink(GameObject target, tk2dTiledSprite m_extantLink, GameObject actor, bool Damages)
        {
            Vector2 unitCenter = actor.GetComponentInChildren<tk2dSprite>().WorldCenter;
            Vector2 unitCenter2 = target.GetComponentInChildren<tk2dSprite>().WorldCenter;
            m_extantLink.transform.position = unitCenter;
            Vector2 vector = unitCenter2 - unitCenter;
            float num = BraveMathCollege.Atan2Degrees(vector.normalized);
            int num2 = Mathf.RoundToInt(vector.magnitude / 0.0625f);
            m_extantLink.dimensions = new Vector2((float)num2, m_extantLink.dimensions.y);
            m_extantLink.transform.rotation = Quaternion.Euler(0f, 0f, num);
            m_extantLink.UpdateZDepth();
            if (Damages == true)
            {
                Elapsed = 0;
                this.ApplyLinearDamage(unitCenter, unitCenter2);
                this.transform.PositionVector2().GetAbsoluteRoom().ApplyActionToNearbyEnemies(unitCenter, 1.5f, Hit);
                this.transform.PositionVector2().GetAbsoluteRoom().ApplyActionToNearbyEnemies(unitCenter2, 1.5f, Hit);
            }
        }
        public void Hit(AIActor aIActor, float f)
        {
            if (aIActor.State == AIActor.ActorState.Normal)
            {
                aIActor.healthHaver.ApplyDamage(Damage / 4, aIActor.transform.PositionVector2(), "Zap");
            }
        }
        private void ApplyLinearDamage(Vector2 p1, Vector2 p2)
        {
            float num = getCalculateddamage();
            for (int i = 0; i < StaticReferenceManager.AllEnemies.Count; i++)
            {
                AIActor aiactor = StaticReferenceManager.AllEnemies[i];
                if (aiactor && aiactor.HasBeenEngaged && aiactor.IsNormalEnemy && aiactor.specRigidbody)
                {
                    Vector2 zero = Vector2.zero;
                    if (BraveUtility.LineIntersectsAABB(p1, p2, aiactor.specRigidbody.HitboxPixelCollider.UnitBottomLeft, aiactor.specRigidbody.HitboxPixelCollider.UnitDimensions, out zero))
                    {
                        aiactor.healthHaver.ApplyDamage(num, Vector2.zero, "Chain Lightning", CoreDamageTypes.Electric, DamageCategory.Normal, false, null, false);
                    }
                }
            }
        }


        public float getCalculateddamage()
        {
            float ElectricDamage = Damage;
            if (player == null) { return ElectricDamage; }
            float dmg = (player.stats.GetStatValue(PlayerStats.StatType.Damage));
            return ElectricDamage * dmg;
        }
        public float Range = 3.5f;
        public float Damage;
        public PlayerController player;
        public GameObject projectile;
    }
}
