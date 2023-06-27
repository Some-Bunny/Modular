using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Collections;

namespace ModularMod.Code.Components
{
    public class ExpandReticleRiserEffect : MonoBehaviour
    {
        public ExpandReticleRiserEffect()
        {
            this.NumRisers = 4;
            this.RiserHeight = 1f;
            this.RiseTime = 1.5f;
            this.UpdateSpriteDefinitions = false;
            this.CurrentSpriteName = string.Empty;
        }

        private void Start()
        {
            this.m_sprite = base.GetComponent<tk2dSprite>();
            this.m_sprite.usesOverrideMaterial = true;


            //GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(base.gameObject);
            //gameObject.GetComponent<tk2dSprite>().renderer.material.shader = StaticShaders.TransparencyShader;//ShaderCache.Acquire("tk2d/BlendVertexColorUnlitTilted");

            //UnityEngine.Object.Destroy(gameObject.GetComponent<ExpandReticleRiserEffect>());


            this.m_risers = new tk2dSprite[this.NumRisers];
            //this.m_risers[0] = gameObject.GetComponent<tk2dSprite>();
            for (int i = 0; i < this.NumRisers; i++)
            {
                var obj = UnityEngine.Object.Instantiate<GameObject>(gameObject);
                UnityEngine.Object.Destroy(obj.GetComponent<tk2dSpriteAnimator>());
                UnityEngine.Object.Destroy(obj.GetComponent<ExpandReticleRiserEffect>());
                obj.GetComponent<tk2dSprite>().renderer.material.shader = StaticShaders.TransparencyShader;//ShaderCache.Acquire("tk2d/BlendVertexColorUnlitTilted");
                this.m_risers[i] = obj.GetComponent<tk2dSprite>();
            }
            this.OnSpawned();
        }
        private void OnSpawned()
        {
            this.m_localElapsed = 0f;
            if (this.m_risers != null)
            {
                for (int i = 0; i < this.m_risers.Length; i++)
                {
                    this.m_risers[i].transform.parent = base.transform;
                    this.m_risers[i].transform.localPosition = Vector3.zero;
                    this.m_risers[i].transform.localRotation = Quaternion.identity;
                    this.m_risers[i].usesOverrideMaterial = true;
                    this.m_risers[i].gameObject.SetLayerRecursively(LayerMask.NameToLayer("FG_Critical"));
                }
            }
        }

        public void Stop()
        {
            Stopped = true;
            for (int i = 0; i < this.m_risers.Count(); i++)
            {
                var riser = this.m_risers[i];
                if (riser)
                {
                    riser.StartCoroutine(Zap(riser));
                }
            }
        }

        public IEnumerator Zap(tk2dSprite s)
        {
            if (s == null) { yield break; }
            float e = s.renderer.material.GetFloat("_Fade");
            while (e > 0)
            {
                float y = Mathf.Lerp(0f, this.RiserHeight, e);
                s.transform.localPosition = Vector3.zero;
                s.transform.position += Vector3.zero.WithY(y * 0.66f);
                s.renderer.material.SetFloat("_Fade", Mathf.Max(0, 1 - e));
                e += BraveTime.DeltaTime * 1.5f;
                yield return null;
            }
            Destroy(s.gameObject);
            yield break;
        }

        private bool Stopped = false;

        private void Update()
        {
            if (!this.m_sprite || Stopped == true)
            {
                return;
            }
            this.m_localElapsed += BraveTime.DeltaTime;
            this.m_sprite.ForceRotationRebuild();
            this.m_sprite.UpdateZDepth();
            if (this.m_risers != null)
            {
                for (int i = 0; i < this.m_risers.Length; i++)
                {
                    if (this.UpdateSpriteDefinitions && !string.IsNullOrEmpty(this.CurrentSpriteName))
                    {
                        this.m_risers[i].SetSprite(this.CurrentSpriteName);
                    }
                    float t = Mathf.Max(0f, this.m_localElapsed - this.RiseTime / (float)this.NumRisers * (float)i) % this.RiseTime / this.RiseTime;
                    float y = Mathf.Lerp(0f, this.RiserHeight, t);
                    this.m_risers[i].transform.localPosition = Vector3.zero;
                    this.m_risers[i].transform.position += Vector3.zero.WithY(y);
                    this.m_risers[i].ForceRotationRebuild();
                    this.m_risers[i].UpdateZDepth();
                    this.m_risers[i].sprite.renderer.material.SetFloat("_Fade", Mathf.Max(0, 1 - t));
                    if (this.UpdateSpriteDefinitions && !string.IsNullOrEmpty(this.CurrentSpriteName))
                    {
                        this.m_risers[i].SetSprite(this.CurrentSpriteName);
                    }

                }
            }
        }
        public bool UpdateSpriteDefinitions;
        public string CurrentSpriteName;
        public int NumRisers;
        public float RiserHeight;
        public float RiseTime;
        private tk2dSprite m_sprite;
        private tk2dSprite[] m_risers;
        private float m_localElapsed;
    }

}
