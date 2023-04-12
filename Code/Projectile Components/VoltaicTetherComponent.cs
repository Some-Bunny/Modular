using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ModularMod
{
    public class VoltaicTetherComponent : MonoBehaviour
    {
        public static List<GameObject> AllTethers = new List<GameObject>();

        public float PlayerRange = 25f;
        public float PylonRange = 7.5f;
        private Dictionary<GameObject, GameObject> ExtantTethers = new Dictionary<GameObject, GameObject>();
        public float DPS = 2.5f;
        public tk2dBaseSprite sprite;
        public void Start()
        {
            sprite = this.GetComponentInChildren<tk2dBaseSprite>();
            AllTethers.Add(this.gameObject);
        }


        public void Update()
        {
            if (this.gameObject != null)
            {
                List<GameObject> activeObjects = VoltaicTetherComponent.AllTethers;
                if (activeObjects != null && activeObjects.Count > 0)
                {
                    foreach (GameObject ai in activeObjects)
                    {
                        ProcessGameObject(ai, PylonRange);
                    }
                }
                foreach (PlayerController p in GameManager.Instance.AllPlayers)
                {
                    ProcessGameObject(p.gameObject, PlayerRange);
                }

            }
            foreach (var si in ExtantTethers)
            {
                if (this.gameObject && si.Value != null && si.Key != null)
                {
                    UpdateLink(this.gameObject, si.Value.GetComponent<tk2dTiledSprite>(), si.Key);
                }
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
            }
        }

        public void ProcessGameObject(GameObject ai, float Distance = 35)
        {
            bool flag8 = ai && ai != null && Vector2.Distance(ai.transform.PositionVector2(), this.sprite.WorldTopCenter) < Distance && ai != this.gameObject;
            if (flag8)
            {
                if (!ExtantTethers.ContainsKey(ai))
                {
                    GameObject obj = SpawnManager.SpawnVFX(VFXStorage.FriendlyElectricLinkVFX, false).GetComponent<tk2dTiledSprite>().gameObject;
                    ExtantTethers.Add(ai, obj);
                }
            }
            if (ai && ai != null && Vector2.Distance(ai.transform.PositionVector2(), this.sprite.WorldTopCenter) > Distance)
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
        private void UpdateLink(GameObject target, tk2dTiledSprite m_extantLink, GameObject actor)
        {
            Vector2 unitCenter = actor.GetComponent<PlayerController>() != null ? actor.GetComponent<PlayerController>().sprite.WorldCenter : actor.GetComponentInChildren<tk2dBaseSprite>().sprite.WorldTopCenter;

            Vector2 unitCenter2 = target.GetComponentInChildren<tk2dBaseSprite>().sprite.WorldTopCenter;
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
            float num = DPS;
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
        private HashSet<AIActor> m_damagedEnemies = new HashSet<AIActor>();
        private void OnDestroy()
        {
            if (VoltaicTetherComponent.AllTethers.Contains(gameObject))
            {
                VoltaicTetherComponent.AllTethers.Remove(gameObject);
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
