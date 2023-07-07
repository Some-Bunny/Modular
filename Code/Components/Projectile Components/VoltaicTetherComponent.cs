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
        private Dictionary<GameObject, GameObject> ExtantTethers = new Dictionary<GameObject, GameObject>();
        public float DPS = 3f;
        public tk2dBaseSprite sprite;
        public void Start()
        {
            sprite = this.GetComponentInChildren<tk2dBaseSprite>();
            AllTethers.Add(this);
        }


        public void Update()
        {
            //Elapsed += BraveTime.DeltaTime;
            if (this.gameObject != null)
            {
                List<VoltaicTetherComponent> activeObjects = VoltaicTetherComponent.AllTethers;
                if (activeObjects != null && activeObjects.Count > 0)
                {
                    foreach (VoltaicTetherComponent ai in activeObjects)
                    {
                        ProcessGameObject(ai.gameObject, PylonRange, true);
                    }
                }
                foreach (PlayerController p in GameManager.Instance.AllPlayers)
                {
                    ProcessGameObject(p.gameObject, PlayerRange);
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
                    UpdateLink(this.gameObject, si.Value.GetComponent<tk2dTiledSprite>(), si.Key);//, Elapsed > Tick ? true : false);
                }
            }
        }

        public void ProcessGameObject(GameObject ai, float Distance = 35, bool isTether = false)
        {
            if (ExtantTethers.Count > 2)
            {
                var thing = ReturnLongestDistance(Distance);
                if (thing != null)
                {
                    GameObject obj_Clear;
                    ExtantTethers.TryGetValue(thing, out obj_Clear);
                    SpawnManager.Despawn(obj_Clear);
                    ExtantTethers.Remove(thing);
                }
            }
            if (ai != null && Vector2.Distance(ai.transform.PositionVector2(), this.sprite.WorldTopCenter) < Distance && ai != this.gameObject && ExtantTethers.Count < 3)
            {
                if (!ExtantTethers.ContainsKey(ai))
                {
                    if (isTether == true)
                    {
                        if(!ai.GetComponent<VoltaicTetherComponent>().ExtantTethers.ContainsKey(ai))
                        {
                           
                            GameObject obj = SpawnManager.SpawnVFX(VFXStorage.FriendlyElectricLinkVFX, false).GetComponent<tk2dTiledSprite>().gameObject;
                            ExtantTethers.Add(ai, obj);
                        }                     
                    }
                    else
                    {

                        GameObject obj = SpawnManager.SpawnVFX(VFXStorage.FriendlyElectricLinkVFX, false).GetComponent<tk2dTiledSprite>().gameObject;
                        ExtantTethers.Add(ai, obj);
                    }
                }
            }
           
            if (ai != null && Vector2.Distance(ai.transform.PositionVector2(), this.sprite.WorldTopCenter) > Distance)
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
        private void UpdateLink(GameObject target, tk2dTiledSprite m_extantLink, GameObject actor)//, bool Damages)
        {
            Vector2 unitCenter = actor.GetComponent<PlayerController>() != null ? actor.GetComponent<PlayerController>().sprite.WorldBottomCenter : actor.GetComponentInChildren<tk2dBaseSprite>().sprite.WorldBottomCenter;

            Vector2 unitCenter2 = target.transform.PositionVector2();
            m_extantLink.transform.position = unitCenter;
            Vector2 vector = unitCenter2 - unitCenter;
            float num = BraveMathCollege.Atan2Degrees(vector.normalized);   
            int num2 = Mathf.RoundToInt(vector.magnitude / 0.0625f);
            m_extantLink.dimensions = new Vector2((float)num2, m_extantLink.dimensions.y);
            m_extantLink.transform.rotation = Quaternion.Euler(0f, 0f, num);
            m_extantLink.UpdateZDepth();
            this.transform.PositionVector2().GetAbsoluteRoom().ApplyActionToNearbyEnemies(unitCenter, 2f, Hit);
            this.transform.PositionVector2().GetAbsoluteRoom().ApplyActionToNearbyEnemies(unitCenter2, 2f, Hit);

            this.ApplyLinearDamage(unitCenter, unitCenter2);
        }

        public void Hit(AIActor aIActor, float f)
        {

            if (!this.m_damagedEnemies_AOE.Contains(aIActor) )
            {
                if (aIActor.State == AIActor.ActorState.Normal && aIActor.CanTargetPlayers == true && aIActor.CanTargetEnemies == false) 
                {
                    aIActor.healthHaver.ApplyDamage(DPS / 5, aIActor.transform.PositionVector2(), "Zap");
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


        public GameObject ReturnLongestDistance(float h)
        {
            List<float> distances = new List<float>();
            Dictionary<float, GameObject> distances_V = new Dictionary<float, GameObject>();
            foreach (var entry in ExtantTethers)
            {
                if (entry.Key && entry.Value)
                {
                    float d = Vector2.Distance(entry.Key.transform.PositionVector2(), entry.Value.transform.PositionVector2());
                    if (!distances_V.ContainsKey(d))
                    {
                        distances_V.Add(d, entry.Key);
                        distances.Add(d);
                    }
                }
            }
            distances.Sort();
           
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
