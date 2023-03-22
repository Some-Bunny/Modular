using Alexandria.CharacterAPI;
using Alexandria.ItemAPI;
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
    public class CloakPlating : DefaultModule
    {
        public static ItemTemplate template = new ItemTemplate(typeof(CloakPlating))
        {
            Name = "Cloak Plating",
            Description = "+60%",
            LongDescription = "Entering combat cloaks the player for 5 (+2.5 per stack) seconds. Uncloaking forcefully grants a 4x (+1 per stack) damage multiplier that degrades fast." + "\n\n" + "Tier:\n" + DefaultModule.ReturnTierLabel(DefaultModule.ModuleTier.Tier_2),
            ManualSpriteCollection = StaticCollections.Module_T2_Collection,
            ManualSpriteID = StaticCollections.Module_T2_Collection.GetSpriteIdByName("cloakup_t2_module"),
            Quality = ItemQuality.SPECIAL,
            PostInitAction = PostInit
        };
        public static void PostInit(PickupObject v)
        {
            var h = (v as DefaultModule);
            h.AltSpriteID = StaticCollections.Module_T2_Collection.GetSpriteIdByName("cloakup_t2_module_alt");
            h.Tier = ModuleTier.Tier_2;
            h.LabelName = "Cloak Plating " + h.ReturnTierLabel();
            h.LabelDescription = "Entering combat cloaks the player for 5 ("+ StaticColorHexes.AddColorToLabelString("+2.5", StaticColorHexes.Light_Orange_Hex) + ") seconds.\nForcefully uncloaking grants a\n" +
                "4x (" + StaticColorHexes.AddColorToLabelString("+1", StaticColorHexes.Light_Orange_Hex) + ") damage multiplier that degrades fast.";
            h.SetTag("modular_module");
            h.AddColorLight(Color.green);
            h.Offset_LabelDescription = new Vector2(0.25f, -1.125f);
            h.Offset_LabelName = new Vector2(0.25f, 1.875f);
            h.EnergyConsumption = 1;
            h.AddToGlobalStorage();

            //EncounterDatabase.GetEntry(h.encounterTrackable.EncounterGuid).usesPurpleNotifications = true;
            ID = h.PickupObjectId;
        }
        public static int ID;

        public override void OnFirstPickup(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.OnEnteredCombat += OEC;
            modulePrinter.OnPostProcessProjectile += PPP;
        }
        public override void OnLastRemoved(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.OnEnteredCombat -= OEC;
            modulePrinter.OnPostProcessProjectile -= PPP;
        }

        public float DamageMax = 5;
        public float CloakTime = 5;

        public override void OnAnyPickup(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player, bool truePickup)
        {
            int stack = this.ReturnStack(modulePrinter);
            DamageMax = 3 + (stack);
            CloakTime = 2.5f + (stack * 2.5f);
        }

        public void OEC(ModulePrinterCore modulePrinterCore, RoomHandler room, PlayerController p)
        {
            p.StartCoroutine(HandleStealth(p));
        }

        private IEnumerator HandleStealth(PlayerController user)
        {
            AkSoundEngine.PostEvent("Play_cloak", user.gameObject);
            Hooks.Stencility_Enabled = false;
            float elapsed = 0f;
            user.sprite.usesOverrideMaterial = true;
            user.sprite.renderer.material.shader = StaticShaders.TransparencyShader;
            user.sprite.renderer.material.SetFloat("_Fade", 1f);
            for (int i = 0; i < user.healthHaver.bodySprites.Count; i++)
            {
                user.healthHaver.bodySprites[i].usesOverrideMaterial = true;
                user.healthHaver.bodySprites[i].renderer.material.shader = StaticShaders.TransparencyShader;
                user.healthHaver.bodySprites[i].renderer.material.SetFloat("_Fade", 1f);
            }
            if (user.primaryHand && user.primaryHand.sprite)
            {
                user.primaryHand.sprite.usesOverrideMaterial = true;
                user.primaryHand.sprite.renderer.material.shader = StaticShaders.TransparencyShader;
                user.primaryHand.sprite.renderer.material.SetFloat("_Fade", 1f);
            }
            if (user.secondaryHand && user.secondaryHand.sprite)
            {
                user.secondaryHand.sprite.usesOverrideMaterial = true;
                user.secondaryHand.sprite.renderer.material.shader = StaticShaders.TransparencyShader;
                user.secondaryHand.sprite.renderer.material.SetFloat("_Fade", 1f);
            }
            currentState = Cloak_State.Active;

            user.SetIsStealthed(true, "smoke");
            user.specRigidbody.AddCollisionLayerIgnoreOverride(CollisionMask.LayerToMask(CollisionLayer.EnemyHitBox, CollisionLayer.EnemyCollider));
            user.OnDidUnstealthyAction += this.BreakStealth;
            while (elapsed < CloakTime)
            {
                if (currentState != Cloak_State.Active) { yield break; }
                AlterShader(user, Mathf.Max(0.2f, Mathf.Lerp(1, 0, elapsed * 1.25f)));
                elapsed += BraveTime.DeltaTime;
                if (!user.IsStealthed)
                {
                    break;
                }
                yield return null;
            }
            user.OnDidUnstealthyAction -= this.BreakStealth;
            user.StartCoroutine(DoMultiplierFade(user, false));
            user.specRigidbody.RemoveCollisionLayerIgnoreOverride(CollisionMask.LayerToMask(CollisionLayer.EnemyHitBox, CollisionLayer.EnemyCollider));
            user.SetIsStealthed(false, "smoke");
            yield break;
        }

        public void AlterShader(PlayerController user, float f)
        {
            if (user.sprite.renderer.material.shader != StaticShaders.TransparencyShader) { return; }
            user.sprite.renderer.material.SetFloat("_Fade", f);
            for (int i = 0; i < user.healthHaver.bodySprites.Count; i++)
            {
                user.healthHaver.bodySprites[i].renderer.material.SetFloat("_Fade", f);
            }
            if (user.primaryHand && user.primaryHand.sprite)
            {
                user.primaryHand.sprite.renderer.material.SetFloat("_Fade", f);
            }
            if (user.secondaryHand && user.secondaryHand.sprite)
            {
                user.secondaryHand.sprite.renderer.material.SetFloat("_Fade", f);
            }
        }

        private IEnumerator DoMultiplierFade(PlayerController user, bool affectsDamage = true)
        {
            if (affectsDamage == true)
            {
                AkSoundEngine.PostEvent("Play_BOSS_cyborg_storm_01", user.gameObject);
                user.PlayEffectOnActor(VFXStorage.MachoBraceDustupVFX, new Vector3(-1f, -1f));
            }
            currentState = Cloak_State.Disabling;
            float elapsed = 0f;
            while (elapsed < 2.5f)
            {
                elapsed += BraveTime.DeltaTime;
                if (currentState == Cloak_State.Active) { Mult = 1; yield break; }
                AlterShader(user, Mathf.Lerp(0.2f, 1, elapsed * 2f));    
                if (affectsDamage == true)
                {
                    Mult = Mathf.Lerp(DamageMax, 1, elapsed / 2.5f);
                }
                yield return null;
            }
            Mult = 1;
            user.sprite.renderer.material = user.gameObject.GetComponent<CustomCharacter>().data.glowMaterial;
            Hooks.Stencility_Enabled = true;
            currentState = Cloak_State.Inactive;
            yield break;
        }

        public Cloak_State currentState = Cloak_State.Inactive;
        public enum Cloak_State
        {
            Inactive,
            Active,
            Disabling
        }

        private void BreakStealth(PlayerController obj)
        {
            obj.OnDidUnstealthyAction -= this.BreakStealth;
            obj.StartCoroutine(DoMultiplierFade(obj));
            obj.specRigidbody.RemoveCollisionLayerIgnoreOverride(CollisionMask.LayerToMask(CollisionLayer.EnemyHitBox, CollisionLayer.EnemyCollider));
            obj.SetIsStealthed(false, "smoke");
        }
        private float Mult = 1;
        public void PPP(ModulePrinterCore modulePrinterCore, Projectile p, float f, PlayerController player)
        {    
            p.baseData.damage *= Mult;
            p.AdditionalScaleMultiplier *= Mathf.Min(Mult, 2.5f);
        }
    }
}

