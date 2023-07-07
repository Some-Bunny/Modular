using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ModularMod
{
    public class BaseRealmWeapon
    {
        public virtual IEnumerator DoWeaponFire(tk2dBaseSprite orbitalSprite, float aimAngle, float cooldown)
        {

            if (Attack_VFX)
            {
                var vfx = UnityEngine.Object.Instantiate(Attack_VFX, orbitalSprite.WorldCenter, Quaternion.identity);
                vfx.transform.parent = orbitalSprite.transform;
                vfx.transform.localRotation = Quaternion.Euler(0, 0, aimAngle);
                vfx.transform.localScale *= Attack_VFX_Scale;
            }
            if (Attack_SFX != null) AkSoundEngine.PostEvent(Attack_SFX, orbitalSprite.gameObject);
            if (SpriteID_Attack != null) orbitalSprite.SetSprite(SpriteData, SpriteID_Attack.Value);
            yield return new WaitForSeconds(Mathf.Min(0.25f, cooldown / 3));
            if (SpriteID_Idle != null) orbitalSprite.SetSprite(SpriteData, SpriteID_Idle.Value);
            yield break;
        }

        public virtual void Startup()
        {
            ThisWeaponType = Toolbox.RandomEnum<WEAPONTYPE>();
            switch (ThisWeaponType)
            {
                case WEAPONTYPE.SWORD:
                    BaseCooldown = 0.9f;
                    Spread = 2;
                    BaseName = "Sword";
                    SpriteID = SpriteData.GetSpriteIdByName("blade_fx");
                    Projectile = LootMadness.SwordProj;
                    SpriteID_Idle = SpriteData.GetSpriteIdByName("blade_anim_001");
                    SpriteID_Attack = SpriteData.GetSpriteIdByName("blade_anim_002");
                    Attack_SFX = "Play_WPN_blasphemy_shot_01";
                    Attack_VFX = (PickupObjectDatabase.GetById(417) as Gun).muzzleFlashEffects.effects[0].effects[0].effect;
                    Attack_VFX_Scale = 0.5f;
                    Projectile_Arc_Spread = 22;
                    return;
                case WEAPONTYPE.STAFF:
                    BaseCooldown = .35f;
                    Spread = 16;
                    BaseName = "Staff";
                    SpriteID = StaticCollections.VFX_Collection.GetSpriteIdByName("wand_fx");
                    Projectile = LootMadness.StaffProj;
                    SpriteID_Idle = SpriteData.GetSpriteIdByName("staff_anim_001");
                    SpriteID_Attack = SpriteData.GetSpriteIdByName("staff_anim_002");
                    Attack_SFX = "Play_WPN_skullgun_shot_01";
                    Attack_VFX = (PickupObjectDatabase.GetById(61) as Gun).muzzleFlashEffects.effects[0].effects[0].effect;
                    Projectile_Arc_Spread = 11;
                    return;
                case WEAPONTYPE.BOW:
                    BaseCooldown = 0.65f;
                    BaseName = "Bow";
                    Spread = 9;
                    SpriteID = StaticCollections.VFX_Collection.GetSpriteIdByName("bow_fx");
                    Projectile = LootMadness.ArrowProj;
                    SpriteID_Idle = SpriteData.GetSpriteIdByName("bow_anim_001");
                    SpriteID_Attack = SpriteData.GetSpriteIdByName("bow_anim_002");
                    Attack_SFX = "Play_WPN_woodbow_shot_01";
                    Attack_VFX = (PickupObjectDatabase.GetById(17) as Gun).muzzleFlashEffects.effects[0].effects[0].effect;
                    Projectile_Arc_Spread = 3;
                    return;
            }
        }
        public enum WEAPONTYPE
        {
            SWORD,
            BOW,
            STAFF
        }


        public WEAPONTYPE ThisWeaponType;

        public int Projectile_Amount = 1;
        public float Projectile_Arc_Spread = 8;


        public float BaseCooldown;
        public float Cooldown;
        public Projectile Projectile;
        public tk2dSpriteCollectionData SpriteData = StaticCollections.VFX_Collection;
        public int SpriteID;

        public GameObject Attack_VFX;
        public float Attack_VFX_Scale = 1;

        public int? SpriteID_Idle;
        public int? SpriteID_Attack;

        public string Attack_SFX = null;

        public string BaseName;
        public float Spread;
    }
}
