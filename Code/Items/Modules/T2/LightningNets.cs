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
        }
        public static int ID;


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
            chain.Damage = p.baseData.damage * (0.33f * stack);
            chain.Range = 4f + (4 * stack);
            chain.player = player;
            chain.projectile = p.gameObject;
            p.baseData.range += 5;

        }

        public static List<GameObject> allactiveTetherProjectiles = new List<GameObject>();
    }
    internal class ElectricChainProjectile : MonoBehaviour
    {
        public void Start()
        {
            LightningNets.allactiveTetherProjectiles.Add(this.gameObject);
        }

        private Dictionary<GameObject, GameObject> ExtantTethers = new Dictionary<GameObject, GameObject>();
        private HashSet<AIActor> m_damagedEnemies = new HashSet<AIActor>();

        public void Update()
        {
            if (this.projectile != null)
            {
                List<GameObject> activeProjectiles = LightningNets.allactiveTetherProjectiles;
                if (activeProjectiles != null && activeProjectiles.Count > 0)
                {
                    foreach (GameObject ai in activeProjectiles)
                    {
                        bool flag8 = ai && ai != null && Vector2.Distance(ai.transform.PositionVector2(), this.projectile.GetComponentInChildren<tk2dBaseSprite>().WorldCenter) < Range && ai.gameObject.GetComponent<ElectricChainProjectile>() != null && ai != this.projectile;
                        if (flag8)
                        {
                            if (!ExtantTethers.ContainsKey(ai))
                            {
                                GameObject obj = SpawnManager.SpawnVFX(VFXStorage.FriendlyElectricLinkVFX, false).GetComponent<tk2dTiledSprite>().gameObject;
                                ExtantTethers.Add(ai, obj);
                            }
                        }
                        bool fuckoff = ai && ai != null && Vector2.Distance(ai.transform.PositionVector2(), this.projectile.GetComponentInChildren<tk2dBaseSprite>().WorldCenter) > Range;
                        if (fuckoff)
                        {
                            if (ExtantTethers.ContainsKey(ai))
                            {
                                GameObject obj;
                                ExtantTethers.TryGetValue(ai, out obj);
                                SpawnManager.Despawn(obj);
                                ExtantTethers.Remove(ai);
                            }
                        }
                    }
                }
            }
            foreach (var si in ExtantTethers)
            {
                if (this.projectile && si.Value != null && si.Key != null)
                {
                    UpdateLink(this.projectile, si.Value.GetComponent<tk2dTiledSprite>(), si.Key);
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
            if (LightningNets.allactiveTetherProjectiles.Contains(this.gameObject))
            {
                LightningNets.allactiveTetherProjectiles.Remove(this.gameObject);
            }
        }

        private void UpdateLink(GameObject target, tk2dTiledSprite m_extantLink, GameObject actor)
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
            this.ApplyLinearDamage(unitCenter, unitCenter2);

        }
        private void ApplyLinearDamage(Vector2 p1, Vector2 p2)
        {
            float num = getCalculateddamage();
            for (int i = 0; i < StaticReferenceManager.AllEnemies.Count; i++)
            {
                AIActor aiactor = StaticReferenceManager.AllEnemies[i];
                if (!this.m_damagedEnemies.Contains(aiactor))
                {
                    if (aiactor && aiactor.HasBeenEngaged && aiactor.IsNormalEnemy && aiactor.specRigidbody)
                    {
                        Vector2 zero = Vector2.zero;
                        if (BraveUtility.LineIntersectsAABB(p1, p2, aiactor.specRigidbody.HitboxPixelCollider.UnitBottomLeft, aiactor.specRigidbody.HitboxPixelCollider.UnitDimensions, out zero))
                        {
                            aiactor.healthHaver.ApplyDamage(num, Vector2.zero, "Chain Lightning", CoreDamageTypes.Electric, DamageCategory.Normal, false, null, false);
                            GameManager.Instance.StartCoroutine(this.HandleDamageCooldown(aiactor));
                        }
                    }
                }
            }
        }

        private IEnumerator HandleDamageCooldown(AIActor damagedTarget)
        {
            this.m_damagedEnemies.Add(damagedTarget);
            yield return new WaitForSeconds(0.25f);
            this.m_damagedEnemies.Remove(damagedTarget);
            yield break;
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

