using Alexandria.ItemAPI;
using Alexandria.Misc;
using JuneLib.Items;
using ModularMod.Code.Controllers;
using ModularMod.Code.Hooks;
using SaveAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Collections;

namespace ModularMod
{
    public class PerformanceCore : PassiveItem
    {
        public static ItemTemplate template = new ItemTemplate(typeof(PerformanceCore))
        {
            Name = "Performance Drive",
            Description = "Brain Power",
            LongDescription = "Overclocks all your systems, everything is faster!\n\nDestroyed when dropped.",
            ManualSpriteCollection = StaticCollections.Item_Collection,//cqc_drive
            ManualSpriteID = StaticCollections.Item_Collection.GetSpriteIdByName("cqc_drive"),
            Quality = ItemQuality.EXCLUDED,
           
            PostInitAction = PostInit
        };
        public static void PostInit(PickupObject v)
        {
            v.gameObject.SetActive(false);
            FakePrefab.MakeFakePrefab(v.gameObject);
            PerformanceDriveID = v.PickupObjectId;
        }
        public override void Pickup(PlayerController player)
        {
            GameStatsManager.Instance.isTurboMode = true;
            base.Pickup(player);
        }
        public override DebrisObject Drop(PlayerController player)
        {
            GameStatsManager.Instance.isTurboMode = false;
            var DebrisObject = base.Drop(player);
            DebrisObject.OnTouchedGround += (obj) =>
            {
                var blast =StaticExplosionDatas.CopyFields(StaticExplosionDatas.explosiveRoundsExplosion);
                blast.ignoreList = new List<SpeculativeRigidbody>() { player.specRigidbody };
                Exploder.Explode(obj.sprite.WorldCenter, blast, obj.sprite.WorldCenter);
                Destroy(obj.gameObject);
            };
            return DebrisObject;
        }

        public override void OnDestroy()
        {
            GameStatsManager.Instance.isTurboMode = false;
            base.OnDestroy();
        }

        public static int PerformanceDriveID;
    }
    public class AllocationCore : PassiveItem
    {
        public static ItemTemplate template = new ItemTemplate(typeof(AllocationCore))
        {
            Name = "Allocation Drive",
            Description = "Switched Around",
            LongDescription = "More power drawn to your weapon! Just make sure you land your shots.\n\nDestroyed when dropped.",
            ManualSpriteCollection = StaticCollections.Item_Collection,
            ManualSpriteID = StaticCollections.Item_Collection.GetSpriteIdByName("allocation_drive"),
            Quality = ItemQuality.EXCLUDED,
            PostInitAction = PostInit
        };
        public static void PostInit(PickupObject v)
        {
            v.gameObject.SetActive(false);
            FakePrefab.MakeFakePrefab(v.gameObject);
            v.AddPassiveStatModifier(PlayerStats.StatType.ProjectileSpeed, 1.1f, StatModifier.ModifyMethod.MULTIPLICATIVE);
            v.AddPassiveStatModifier(PlayerStats.StatType.AdditionalClipCapacityMultiplier, 0.5f, StatModifier.ModifyMethod.MULTIPLICATIVE);
            v.AddPassiveStatModifier(PlayerStats.StatType.Damage, 1.2f, StatModifier.ModifyMethod.MULTIPLICATIVE);
            AllocationCoreID = v.PickupObjectId;
        }
        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
        }
        public override DebrisObject Drop(PlayerController player)
        {
            var DebrisObject = base.Drop(player);
            DebrisObject.OnTouchedGround += (obj) =>
            {
                var blast = StaticExplosionDatas.CopyFields(StaticExplosionDatas.explosiveRoundsExplosion);
                blast.ignoreList = new List<SpeculativeRigidbody>() { player.specRigidbody };
                Exploder.Explode(obj.sprite.WorldCenter, blast, obj.sprite.WorldCenter);
                Destroy(obj.gameObject);
            };
            return DebrisObject;
        }
        public static int AllocationCoreID;
    }
    public class CloseQuartersCore : PassiveItem
    {
        public static ItemTemplate template = new ItemTemplate(typeof(CloseQuartersCore))
        {
            Name = "Optics Drive",
            Description = "The World Looks Red",
            LongDescription = "Calibrated systems, at the cost of some visual distortion.\n\nDestroyed when dropped.",
            ManualSpriteCollection = StaticCollections.Item_Collection,
            ManualSpriteID = StaticCollections.Item_Collection.GetSpriteIdByName("performance_drive"),
            Quality = ItemQuality.EXCLUDED,
            PostInitAction = PostInit
        };
        public static void PostInit(PickupObject v)
        {
            v.gameObject.SetActive(false);
            FakePrefab.MakeFakePrefab(v.gameObject);
            v.AddPassiveStatModifier(PlayerStats.StatType.Accuracy, 0.8f, StatModifier.ModifyMethod.MULTIPLICATIVE);
            v.AddPassiveStatModifier(PlayerStats.StatType.DamageToBosses, 1.05f, StatModifier.ModifyMethod.MULTIPLICATIVE);
            v.AddPassiveStatModifier(PlayerStats.StatType.RateOfFire, 1.05f, StatModifier.ModifyMethod.MULTIPLICATIVE);
            v.AddPassiveStatModifier(PlayerStats.StatType.Damage, 1.05f, StatModifier.ModifyMethod.MULTIPLICATIVE);
            CQCID = v.PickupObjectId;
        }
        public override void Pickup(PlayerController player)
        {
            GameManager.Instance.StartCoroutine(DoFade(1, 0.65f));
            base.Pickup(player);
        }

        public IEnumerator DoFade(float S_V, float E_V)
        {
            float e = 0;
            float t = 0;
            while (e < 0.45f)
            {
                t = e / 0.45f;
                Fade = Mathf.Lerp(S_V, E_V, t);
                e += BraveTime.DeltaTime;
                yield return null;
            }
            yield break;
        }
        public IEnumerator DoFadeReal(float S_V, float E_V)
        {
            float e = 0;
            float t = 0;
            while (e < 0.45f)
            {
                t = e / 0.45f;
                Pixelator.Instance.fade = Mathf.Lerp(S_V, E_V, t);
                e += BraveTime.DeltaTime;
                yield return null;
            }
            yield break;
        }
        public float Fade = 1;

        public override void Update()
        {
            if (base.Owner)
            {
                Pixelator.Instance.fade = Fade;
                Pixelator.Instance.FadeColor = Color.red;
            }
            base.Update();
        }
        public override DebrisObject Drop(PlayerController player)
        {
            GameManager.Instance.StartCoroutine(DoFadeReal(Pixelator.Instance.fade, 1));
            var DebrisObject = base.Drop(player);
            DebrisObject.OnTouchedGround += (obj) =>
            {
                var blast = StaticExplosionDatas.CopyFields(StaticExplosionDatas.explosiveRoundsExplosion);
                blast.ignoreList = new List<SpeculativeRigidbody>() { player.specRigidbody };
                Exploder.Explode(obj.sprite.WorldCenter, blast, obj.sprite.WorldCenter);
                Destroy(obj.gameObject);
            };
            return DebrisObject;
        }
        public override void OnDestroy()
        {
            GameManager.Instance.StartCoroutine(DoFadeReal(Pixelator.Instance.fade, 1));
            base.OnDestroy();
        }
        public static int CQCID;
    }

    public class HighValueInfo : PassiveItem
    {
        public static ItemTemplate template = new ItemTemplate(typeof(HighValueInfo))
        {
            Name = "High-Value Information",
            Description = "Worth A Lot",
            LongDescription = "A pocket drive with all of Hegemony Mechanic's hidden military caches. This will go for a pretty penny on the outside.\n\nVERY fragile.\n\nDestroyed when dropped.",
            ManualSpriteCollection = StaticCollections.Item_Collection,
            ManualSpriteID = StaticCollections.Item_Collection.GetSpriteIdByName("highValueInfo"),
            Quality = ItemQuality.EXCLUDED,
            PostInitAction = PostInit
        };
        public static void PostInit(PickupObject v)
        {
            v.gameObject.SetActive(false);
            FakePrefab.MakeFakePrefab(v.gameObject);
            CoolInfoID = v.PickupObjectId;
        }


        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            player.OnReceivedDamage += Player_OnReceivedDamage;
        }

        private void Player_OnReceivedDamage(PlayerController obj)
        {
            obj.RemovePassiveItem(this.PickupObjectId);
            Exploder.Explode(obj.sprite.WorldCenter, StaticExplosionDatas.explosiveRoundsExplosion, obj.sprite.WorldCenter);
            obj.OnReceivedDamage -= Player_OnReceivedDamage;
        }

        public override DebrisObject Drop(PlayerController player)
        {
            player.OnReceivedDamage -= Player_OnReceivedDamage;
            var DebrisObject = base.Drop(player);
            DebrisObject.OnTouchedGround += (obj) =>
            {
                var blast = StaticExplosionDatas.CopyFields(StaticExplosionDatas.explosiveRoundsExplosion);
                blast.ignoreList = new List<SpeculativeRigidbody>() { player.specRigidbody };
                Exploder.Explode(obj.sprite.WorldCenter, blast, obj.sprite.WorldCenter);
                Destroy(obj.gameObject);
            };
            return DebrisObject;
        }
        public static int CoolInfoID;
    }
}

