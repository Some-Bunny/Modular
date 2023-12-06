using Alexandria.ItemAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ModularMod
{
    public class MarkedEffect : GameActorSpeedEffect
    {

        public static GameObject MarkedVFX;

        public static GameObject BuildVFX()
        {
            GameObject VFX = new GameObject("Marked For Death");
            FakePrefab.DontDestroyOnLoad(VFX);
            FakePrefab.MarkAsFakePrefab(VFX);
            var tk2d = VFX.AddComponent<tk2dSprite>();
            tk2d.Collection = StaticCollections.VFX_Collection;
            tk2d.SetSprite(StaticCollections.VFX_Collection.GetSpriteIdByName("critSeeker_006"));
            var tk2dAnim = VFX.AddComponent<tk2dSpriteAnimator>();

            tk2dAnim.Library = Module.ModularAssetBundle.LoadAsset<GameObject>("GenericVFXAnimation").GetComponent<tk2dSpriteAnimation>();
            tk2dAnim.playAutomatically = true;
            tk2dAnim.defaultClipId = tk2dAnim.Library.GetClipIdByName("Crit_Start");

            tk2d.usesOverrideMaterial = true;
            tk2d.renderer.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTiltedCutoutEmissive");
            tk2d.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_ON");
            tk2d.renderer.material.SetFloat("_EmissivePower", 4);
            tk2d.renderer.material.SetFloat("_EmissiveColorPower", 4);
            MarkedVFX = VFX;
            return MarkedVFX;
        }

        public override void ApplyTint(GameActor actor)
        {

        }
        public override void OnEffectApplied(GameActor actor, RuntimeGameActorEffectData effectData, float partialAmount = 1f)
        {
            HunterSeeker.lockedEnemies.Add(actor as AIActor);
            base.OnEffectApplied(actor, effectData, partialAmount);
            actor.healthHaver.AllDamageMultiplier += M;
            actor.healthHaver.OnDeath += (obj1) =>
            {
                if (HunterSeeker.lockedEnemies.Contains(actor as AIActor))
                {
                    HunterSeeker.lockedEnemies.Remove(actor as AIActor);
                }
            };
        }

        public float M = 0.15f;


        public override void OnEffectRemoved(GameActor actor, RuntimeGameActorEffectData effectData)
        {
            HunterSeeker.lockedEnemies.Remove(actor as AIActor);

            actor.healthHaver.AllDamageMultiplier -= M;
            base.OnEffectRemoved(actor, effectData);
            actor.healthHaver.OnPreDeath -= effectData.OnActorPreDeath;
        }
    }

}
