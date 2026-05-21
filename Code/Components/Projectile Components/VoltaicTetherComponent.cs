using Brave.BulletScript;
using HutongGames.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using static ETGMod;

namespace ModularMod
{
    public class VoltaicTetherComponent : MonoBehaviour
    {
        public static List<VoltaicTetherComponent> AllTethers = new List<VoltaicTetherComponent>();

        public float PlayerRange = 12.5f;
        public float PylonRange = 10f;
        private Dictionary<tk2dBaseSprite, tk2dTiledSprite> ExtantTethers = new Dictionary<tk2dBaseSprite, tk2dTiledSprite>();
        public float DPS = 3f;
        public tk2dBaseSprite sprite;
        public void Start()
        {
            sprite = this.GetComponentInChildren<tk2dBaseSprite>();
            AllTethers.Add(this);
        }


        public void Update()
        {
            if (this.gameObject != null)
            {
                List<VoltaicTetherComponent> activeObjects = VoltaicTetherComponent.AllTethers;
                if (activeObjects != null && activeObjects.Count > 0)
                {
                    foreach (VoltaicTetherComponent ai in activeObjects)
                    {
                        ProcessGameObject(ai, ai.sprite, PylonRange, true);
                    }
                }
                foreach (PlayerController p in GameManager.Instance.AllPlayers)
                {
                    ProcessGameObject(null, p.sprite, PlayerRange);
                }

            }
            foreach (var si in ExtantTethers)
            {
                if (si.Key != null && si.Value != null && this.gameObject == null)
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
                if (this.gameObject && si.Value != null && si.Key != null)
                {
                    UpdateLink(this.sprite, si.Value, si.Key ,  true);
                }
            }
        }

        public void ProcessGameObject(VoltaicTetherComponent voltaicTetherComponent, tk2dBaseSprite ai, float Distance = 35, bool isTether = false)
        {
            if (ExtantTethers.Count > 3)
            {
                var thing = ReturnLongestDistance(Distance);
                if (thing != null)
                {
                    tk2dTiledSprite obj_Clear;
                    ExtantTethers.TryGetValue(thing, out obj_Clear);
                    SpawnManager.Despawn(obj_Clear.gameObject);
                    ExtantTethers.Remove(thing);
                }
            }

            if (!ExtantTethers.ContainsKey(ai))
            {
                if (ai != null && Vector2.Distance(ai.sprite.WorldCenter, this.sprite.WorldTopCenter) <= Distance && ai != this.gameObject && ExtantTethers.Count < 4)
                {
                    {
                        if (isTether == true)
                        {
                            if (!voltaicTetherComponent.ExtantTethers.ContainsKey(ai))
                            {

                                tk2dTiledSprite obj = SpawnManager.SpawnVFX(VFXStorage.FriendlyElectricLinkVFX, false).GetComponent<tk2dTiledSprite>();
                                ExtantTethers.Add(ai, obj);
                            }
                        }
                        else
                        {

                            tk2dTiledSprite obj = SpawnManager.SpawnVFX(VFXStorage.FriendlyElectricLinkVFX, false).GetComponent<tk2dTiledSprite>();
                            ExtantTethers.Add(ai, obj);
                        }
                    }
                }
            }
            else
            {

                if (ai != null && Vector2.Distance(ai.transform.PositionVector2(), this.sprite.WorldTopCenter) > Distance)
                {
                    {
                        tk2dTiledSprite obj;
                        ExtantTethers.TryGetValue(ai, out obj);
                        SpawnManager.Despawn(obj.gameObject);
                        ExtantTethers.Remove(ai);
                    }

                }
            }
        }
        private void UpdateLink(tk2dBaseSprite target, tk2dTiledSprite m_extantLink, tk2dBaseSprite actor, bool Damages)
        {
            Vector2 unitCenter = actor.sprite.WorldCenter;

            Vector2 unitCenter2 = target.transform.PositionVector2();
            m_extantLink.transform.position = unitCenter;
            Vector2 vector = unitCenter2 - unitCenter;
            float num = BraveMathCollege.Atan2Degrees(vector.normalized);   
            int num2 = Mathf.RoundToInt(vector.magnitude * 16);
            m_extantLink.dimensions = new Vector2((float)num2, m_extantLink.dimensions.y);
            m_extantLink.transform.rotation = Quaternion.Euler(0f, 0f, num);
            m_extantLink.UpdateZDepth();
            if (Damages)
            {
                this.transform.PositionVector2().GetAbsoluteRoom().ApplyActionToNearbyEnemies(unitCenter, 2f, Hit);
                //this.transform.PositionVector2().GetAbsoluteRoom().ApplyActionToNearbyEnemies(unitCenter2, 2f, Hit);
                this.ApplyLinearDamage(unitCenter, unitCenter2);
            }
        }

        public void Hit(AIActor aIActor, float f)
        {
            if (!this.m_damagedEnemies_AOE.Contains(aIActor) )
            {
                if (aIActor.State == AIActor.ActorState.Normal && aIActor.CanTargetPlayers == true && aIActor.CanTargetEnemies == false) 
                {
                    aIActor.healthHaver.ApplyDamage(DPS * 0.25f, aIActor.transform.PositionVector2(), "Zap");
                    GameManager.Instance.StartCoroutine(this.HandleDamageCooldown(aIActor, m_damagedEnemies_AOE));
                }
            }
        }

        private void ApplyLinearDamage(Vector2 p1, Vector2 p2)
        {
            float num = DPS;
            for (int i = 0; i < StaticReferenceManager.AllEnemies.Count; i++)
            {
                AIActor aiactor = StaticReferenceManager.AllEnemies[i];
                if (!this.m_damagedEnemies.Contains(aiactor))
                {
                    if (aiactor && aiactor.HasBeenEngaged && aiactor.specRigidbody && aiactor.CanTargetPlayers == true && aiactor.CanTargetEnemies == false)
                    {
                        Vector2 zero = Vector2.zero;
                        if (BraveUtility.LineIntersectsAABB(p1, p2, aiactor.specRigidbody.HitboxPixelCollider.UnitBottomLeft, aiactor.specRigidbody.HitboxPixelCollider.UnitDimensions, out zero))
                        {
                            aiactor.healthHaver.ApplyDamage(num, Vector2.zero, "Chain Lightning", CoreDamageTypes.Electric, DamageCategory.Normal, false, null, false);
                            GameManager.Instance.StartCoroutine(this.HandleDamageCooldown(aiactor, m_damagedEnemies));
                        }
                    }
                }
                    
            }
        }
        private IEnumerator HandleDamageCooldown(AIActor damagedTarget, HashSet<AIActor> list)
        {
            list.Add(damagedTarget);
            yield return new WaitForSeconds(0.25f);
            list.Remove(damagedTarget);
            yield break;
        }

        private HashSet<AIActor> m_damagedEnemies = new HashSet<AIActor>();
        private HashSet<AIActor> m_damagedEnemies_AOE = new HashSet<AIActor>();



        public tk2dBaseSprite ReturnLongestDistance(float h)
        {
            List<float> distances = new List<float>();
            Dictionary<float, tk2dBaseSprite> distances_V = new Dictionary<float, tk2dBaseSprite>();
            for (int i = ExtantTethers.Count - 1; i > -1; i--)
            {
                var entry = ExtantTethers.ElementAt(i);
                if (entry.Key != null && entry.Value != null)
                {
                    float d = Vector2.Distance(entry.Key.sprite.WorldCenter, entry.Value.transform.PositionVector2());
                    if (!distances_V.ContainsKey(d))
                    {
                        distances_V.Add(d, entry.Key);
                        distances.Add(d);
                    }
                }
            }
            distances.Sort();    
            if (distances == null | distances.Count() == 0) { return null; }

            return h > distances.Last() ? null : distances_V[distances.Last()];
        }

        private void OnDestroy()
        {
            if (VoltaicTetherComponent.AllTethers.Contains(this))
            {
                VoltaicTetherComponent.AllTethers.Remove(this);
            }
            foreach (var si in ExtantTethers)
            {
                if (si.Value != null)
                {
                    SpawnManager.Despawn(si.Value.gameObject);
                }
            }
            ExtantTethers.Clear();
        }
    }
}
