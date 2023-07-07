using Alexandria.Misc;
using Brave.BulletScript;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace ModularMod
{
    public class ModifiedRealmWeapon : BaseRealmWeapon
    {
        public override void Startup()
        {
            base.Startup();

            ThisPrimaryTrait = GlobalTraitList.Primarytraits.SelectByWeight();
            ThisSecondaryTrait = GlobalTraitList.Secondarytraits.SelectByWeight();

            this.ThisPrimaryTrait.RarityApplier();
            this.ThisSecondaryTrait.RarityApplier();

            ThisWeaponType = Toolbox.RandomEnum<WEAPONTYPE>();
            this.BaseCooldown = ThisPrimaryTrait.ModifyCooldown(this.BaseCooldown);
            this.BaseCooldown = ThisSecondaryTrait.ModifyCooldown(this.BaseCooldown);

            this.Spread = ThisPrimaryTrait.ModifySpread(this.Spread);
            this.Spread = ThisSecondaryTrait.ModifySpread(this.Spread);


            this.Projectile_Amount += ThisPrimaryTrait.ModifyProjectileAmount(Projectile_Amount);
            this.Projectile_Amount += ThisSecondaryTrait.ModifyProjectileAmount(Projectile_Amount);

            color = UnityEngine.Random.ColorHSV(0f, 1f, SaturationMult(), SaturationMult(), ValueMult(), ValueMult());


            color = ThisPrimaryTrait.ModifyWeaponColor(color);
            color = ThisSecondaryTrait.ModifyWeaponColor(color);
            color.a = Mathf.Min(color.a, 1);

            Name = ThisPrimaryTrait.TraitName + ReturunColoredText(this.BaseName + " of ") + ThisSecondaryTrait.TraitName;
        }

        public float ReturnTotalSizeMult()
        {
            float e = 1;
            e *= this.ThisPrimaryTrait.ModifyWeaponSize();
            e *= this.ThisSecondaryTrait.ModifyWeaponSize();
            return e;
        }


        public int TotalRarity
        {
            get
            {
                return ThisPrimaryTrait.Rarity + ThisSecondaryTrait.Rarity;
            }
        }
        private string ReturunColoredText(string Text)
        {
            switch (TotalRarity)
            {
                case 0:
                    return Text;
                case 1:
                    return StaticColorHexes.AddColorToLabelString(Text, StaticColorHexes.Light_Blue_Color_Hex);
                case 2:
                    return StaticColorHexes.AddColorToLabelString(Text, StaticColorHexes.Green_Hex);
                case 3:
                    return StaticColorHexes.AddColorToLabelString(Text, StaticColorHexes.Yellow_Hex);
                case 4:
                    return StaticColorHexes.AddColorToLabelString(Text, StaticColorHexes.Red_Color_Hex);
                default:
                    return StaticColorHexes.AddColorToLabelString(Text, StaticColorHexes.Light_Blue_Color_Hex);
            }
        }
        private float ValueMult()
        {
            switch (TotalRarity)
            {
                case 0:
                    return 0.7f;
                case 1:
                    return 0.8f;
                case 2:
                    return 0.9f;
                case 3:
                    return 1.0f;
                case 4:
                    return 1.1f;
                default:
                    return 0.8f;
            }
        }
        private float SaturationMult()
        {
            switch (TotalRarity)
            {
                case 0:
                    return 0.8f;
                case 1:
                    return 0.85f;
                case 2:
                    return 0.9f;
                case 3:
                    return 0.95f;
                case 4:
                    return 1f;
                default:
                    return 0.8f;
            }
        }
        public void Pickup(PlayerController p, float extraRange = 0)
        {
            AkSoundEngine.PostEvent("Play_OBJ_key_pickup_01", p.gameObject);

            PlayerOrbital guon = PlayerOrbitalItem.CreateOrbital(p, LootMadness.orbitalPrefab, false).GetComponent<PlayerOrbital>();
            guon.m_orbitalTier = UnityEngine.Random.Range(20, 100);
            guon.orbitRadius = UnityEngine.Random.Range(1.2f, 3.1f + extraRange);
            guon.orbitDegreesPerSecond = UnityEngine.Random.Range(90, 240);
            guon.shouldRotate = false;
            guon.Initialize(p);
            guon.gameObject.transform.parent = p.transform;
            guon.sprite.SetSprite(SpriteData, SpriteID_Idle.Value);
            guon.sprite.usesOverrideMaterial = true;
            guon.sprite.renderer.material.SetColor("_OverrideColor", this.color);
            guon.gameObject.transform.localScale *= this.ThisPrimaryTrait.ModifyWeaponSize();
            guon.gameObject.transform.localScale *= this.ThisSecondaryTrait.ModifyWeaponSize();

            extantWeapon = guon;

            GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(ResourceCache.Acquire("Global VFX/VFX_Synergy_Poof_001"), guon.transform.position, Quaternion.identity);
            tk2dBaseSprite component = gameObject.GetComponent<tk2dBaseSprite>();
            component.HeightOffGround = 35f;
            component.UpdateZDepth();
            gameObject.transform.parent = guon.transform;
            gameObject.transform.localScale *= 1 + ReturnTotalSizeMult();
            tk2dSpriteAnimator component2 = component.GetComponent<tk2dSpriteAnimator>();
            if (component2 != null)
            {
                component2.sprite.usesOverrideMaterial = true;
                component2.sprite.renderer.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive");
                component2.sprite.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_ON");
                component2.sprite.renderer.material.SetFloat("_EmissivePower", 2);
                component2.sprite.renderer.material.SetFloat("_EmissiveColorPower", 2f);
                component2.sprite.renderer.material.SetColor("_OverrideColor", color);
                component2.sprite.renderer.material.SetColor("_EmissiveColor", color);
            }

            var maanger = guon.gameObject.AddComponent<WeaponManager>();
            maanger.controller = this;
            maanger.StartUp(p);
            UnityEngine.Object.Destroy(gameObject, 1.5f);
        }


        private void AimAt(Vector2 point, bool instant = false)
        {
            this.m_currentAimTarget = (point - extantWeapon.sprite.WorldCenter).ToAngle();
            if (instant)
            {
                extantWeapon.transform.localRotation = Quaternion.Euler(0f, 0f, m_currentAimTarget);
            }

        }

        public void Update()
        {
            DecrementTick();
        }

        public void DecrementTick()
        {
            if (extantWeapon)
            {
                AimAt(extantWeapon.m_owner.unadjustedAimPoint.XY());
                extantWeapon.transform.localRotation = Quaternion.Euler(0f, 0f, Mathf.MoveTowardsAngle(extantWeapon.transform.localRotation.eulerAngles.z, this.m_currentAimTarget, 360 * BraveTime.DeltaTime));
                bool flag2 = extantWeapon.transform.localRotation.eulerAngles.z > 90f && extantWeapon.transform.localRotation.eulerAngles.z < 270f;
                if (flag2 && !extantWeapon.sprite.FlipY)
                {
                    extantWeapon.sprite.FlipY = true;
                }
                else if (!flag2 && extantWeapon.sprite.FlipY)
                {
                    extantWeapon.sprite.FlipY = false;
                }
            }

            if (this.Cooldown < BaseCooldown)
            {
                this.Cooldown += BraveTime.DeltaTime;
            }
        }


        public void OnFire(PlayerController player)
        {
            if (this.Cooldown < BaseCooldown) { return; }
            this.Cooldown = 0;

            for (int i = 0; i < Projectile_Amount; i++)
            {
                Debug.Log(1);
                GameObject spawnedBulletOBJ = SpawnManager.SpawnProjectile(this.Projectile.gameObject, extantWeapon.sprite.WorldCenter, Quaternion.Euler(0f, 0f, m_currentAimTarget + UnityEngine.Random.Range(-this.Spread, this.Spread)), true);
                Projectile component = spawnedBulletOBJ.GetComponent<Projectile>();
                if (component != null)
                {
                    component.Owner = player;
                    component.Shooter = player.specRigidbody;
                    component.sprite.renderer.material.SetColor("_OverrideColor", this.color);

                    ThisPrimaryTrait.ModifyProjectilePrimary(component);
                    ThisSecondaryTrait.ModifyProjectilePrimary(component);
                    if (extantWeapon) { extantWeapon.StartCoroutine(this.DoWeaponFire(extantWeapon.sprite, m_currentAimTarget + (extantWeapon.sprite.FlipY ? 180 : 0), BaseCooldown)); }
                }
            }
        }
        protected float SubdivideRange(float startValue, float endValue, int numDivisions, int i, bool offset = false)
        {
            return Mathf.Lerp(startValue, endValue, ((float)i + ((!offset) ? 0f : 0.5f)) / (float)(numDivisions - 1));
        }
        public string Name;
        private Trait ThisPrimaryTrait;
        private Trait ThisSecondaryTrait;

        private PlayerOrbital extantWeapon;
        public Color color;
        private float m_currentAimTarget;

        public class WeaponManager : MonoBehaviour
        {
            public ModifiedRealmWeapon controller;

            public void StartUp(PlayerController p)
            {
                player = p;
                p.PostProcessProjectile += P_PostProcessProjectile;
                p.OnTriedToInitiateAttack += P_OnTriedToInitiateAttack;

            }

            private void P_OnTriedToInitiateAttack(PlayerController obj)
            {
                if (controller != null) controller.OnFire(player);
            }

            private PlayerController player;
            private void P_PostProcessProjectile(Projectile arg1, float arg2)
            {
                if (controller != null) controller.OnFire(player);
            }

            public void Update()
            {
                if (controller != null) controller.DecrementTick();
            }
        }
    }
}
