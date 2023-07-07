using Alexandria.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ModularMod
{
    public class GlobalTraitList
    {
        public static void InitTraits()
        {
            InitPrimaries();
            InitSecondaries();
        }

        public static void AddTo_1(Trait trait)
        {
            MiscUtility.WeightedType<Trait> weapon = new MiscUtility.WeightedType<Trait>();
            weapon.value = trait;
            weapon.weight = trait.Weight;
            Primarytraits.elements = Primarytraits.elements.Concat(new MiscUtility.WeightedType<Trait>[] { weapon }).ToArray();
        }
        public static void AddTo_2(Trait trait)
        {
            MiscUtility.WeightedType<Trait> weapon = new MiscUtility.WeightedType<Trait>();
            weapon.value = trait;
            weapon.weight = trait.Weight;
            Secondarytraits.elements = Secondarytraits.elements.Concat(new MiscUtility.WeightedType<Trait>[] { weapon }).ToArray();
        }
        public static void InitPrimaries()
        {
            AddTo_1(new Trait()
            {
                OnProjectileFired = (proj, r_M, rarity_Int) =>
                {
                    proj.baseData.range = 100;
                    proj.baseData.damage *= 0.65f;
                    proj.OverrideMotionModule = new OrbitProjectileMotionModule();
                },
                Cooldown = (f, r_M, rarity_Int) =>
                {
                    return f / 2f;
                },
                WeaponSizeModifier = (r_M) =>
                {
                    return 1.15f;
                },
                ColorModifier = (Colour, r_M, rarity_Int) =>
                {
                    Colour.r += 0.12f;
                    Colour.g += 0.05f;
                    Colour.b += 0.2f;

                    return Colour;
                },
                ThisRarity = Trait.RARITY.RARE,
                Override_Name = "Celestial"
            });
            AddTo_1(new Trait()
            {
                OnProjectileFired = (proj, r_M, rarity_Int) =>
                {
                    proj.AdditionalScaleMultiplier *= 1.2f * r_M;
                },
                Cooldown = (f, r_M, rarity_Int) =>
                {
                    return f *= r_M;
                },
                WeaponSizeModifier = (r_M) =>
                {
                    return 1.2f * r_M;
                },
                Common_Name = "Large",
                Rare_Name = "Very Large",
                Legendary_Name = "Extremely Large"
            });
            AddTo_1(new Trait()
            {
                ProjectileAmount = (f, r_M, rarity_Int) =>
                {
                    return (1 + rarity_Int);
                },
                Cooldown = (f, r_M, rarity_Int) =>
                {
                    return f *= 1.2f;
                },
                WeaponSizeModifier = (r_M) =>
                {
                    return 1.2f * r_M;
                },
                Common_Name = "Double",
                Rare_Name = "Triple",
                Legendary_Name = "Quadruple",
                Weight = 0.8f
            });

            AddTo_1(new Trait()
            {
                OnProjectileFired = (proj, r_M, rarity_Int) =>
                {
                    proj.AdditionalScaleMultiplier *= 1.2f * r_M;
                    var blast = proj.gameObject.AddComponent<ExplosiveModifier>();
                    blast.explosionData = StaticExplosionDatas.explosiveRoundsExplosion;
                },
                Override_Name = "Explosive",
                ThisRarity = Trait.RARITY.RARE,
                ColorModifier = (Colour, r_M, rarity_Int) =>
                {
                    Colour.r += 0.25f;
                    Colour.g += 0.04f;

                    return Colour;
                }

            });
            AddTo_1(new Trait()
            {
                Cooldown = (f, f1, int1) =>
                {
                    return f *= 5f;
                },
                WeaponSizeModifier = (f) =>
                {
                    return 2.5f;
                },
                OnProjectileFired = (proj, r_M, rarity_Int) =>
                {
                    proj.AdditionalScaleMultiplier *= 1.5f * r_M;
                    ImprovedAfterImage yes = proj.gameObject.AddComponent<ImprovedAfterImage>();
                    yes.spawnShadows = true;
                    yes.shadowLifetime = 0.5f;
                    yes.shadowTimeDelay = 0.025f;
                    yes.dashColor = new Color(0.1f, 1f, 0.1f, 1f);
                    var blast = proj.gameObject.AddComponent<ExplosiveModifier>();
                    blast.explosionData = StaticExplosionDatas.nigNukeExplosion;
                },
                Override_Name = "Nuclear",
                ThisRarity = Trait.RARITY.LEGENDARY,
                Weight = 0.2f,
                ColorModifier = (Colour, r_M, rarity_Int) =>
                {
                    Colour.r += 0.01f;
                    Colour.g += 0.3f;
                    Colour.b += 0.02f;
                    return Colour;
                }

            });
            AddTo_1(new Trait()
            {
                OnProjectileFired = (proj, r_M, rarity_Int) =>
                {
                    var mod = proj.gameObject.AddComponent<HomingModifier>();
                    mod.AngularVelocity = 150 * r_M;
                    mod.HomingRadius = 9 * r_M;
                },
                SpreadCalc = (f, r_M, rarity_Int) =>
                {
                    return f *= r_M + 0.25f;
                },
                Common_Name = "Searching",
                Rare_Name = "Seeking",
                Legendary_Name = "Targeting"
            });

            AddTo_1(new Trait()
            {
                Cooldown = (f, r_M, rarity_Int) => { return f *= (0.85f / r_M); },
                Common_Name = "Fast",
                Rare_Name = "Very Fast",
                Legendary_Name = "Extremely Fast"
            });
            AddTo_1(new Trait()
            {
                WeaponSizeModifier = (r_M) =>
                {
                    return 1 / r_M;
                },
                OnProjectileFired = (proj, r_M, rarity_Int) => {

                    proj.AdditionalScaleMultiplier /= r_M;
                },
                SpreadCalc = (f, r_M, rarity_Int) => { return f /= r_M; },
                Common_Name = "Small",
                Rare_Name = "Tiny",
                Legendary_Name = "Miniscule"
            });
            AddTo_1(new Trait()
            {
                Override_Name = "Lightning Fast",
                Cooldown = (f, r_M, rarity_Int) => { return f *= 0.4f; },
                OnProjectileFired = (proj, r_M, rarity_Int) => {

                    proj.baseData.speed *= 1.3f;
                    proj.baseData.damage *= 0.75f;
                    proj.UpdateSpeed();
                },
                ThisRarity = Trait.RARITY.LEGENDARY,
            });
            AddTo_1(new Trait()
            {
                WeaponSizeModifier = (r_M) =>
                {
                    return 1.15f;
                },
                Cooldown = (f, r_M, rarity_Int) => { return f *= 1.15f * r_M; },
                OnProjectileFired = (proj, r_M, rarity_Int) =>
                {
                    proj.baseData.damage *= 1.2f * r_M;
                },
                ThisRarity = Trait.RARITY.ALL,
                Common_Name = "Heavy",
                Rare_Name = "Very Heavy",
                Legendary_Name = "Extremely Heavy"

            });
            AddTo_1(new Trait()
            {
                Override_Name = "Sluggish",
                Cooldown = (f, r_M, rarity_Int) => { return f *= 1.2f; },
                ThisRarity = Trait.RARITY.COMMON
            });
            AddTo_1(new Trait()
            {
                SpreadCalc = (f, r_M, rarity_Int) => { return f *= (0.75f / r_M); },
                Common_Name = "Accurate",
                Rare_Name = "Very Accurate",
                Legendary_Name = "Extremely Accurate"
            });
            AddTo_1(new Trait()
            {
                Override_Name = "Pinpoint",
                SpreadCalc = (f, r_M, rarity_Int) => { return f *= 0.01f; },
                ThisRarity = Trait.RARITY.LEGENDARY,
            });
            AddTo_1(new Trait()
            {
                SpreadCalc = (f, r_M, rarity_Int) => { return f *= 2.5f + r_M; },
                OnProjectileFired = (proj, r_M, rarity_Int) => {

                    proj.baseData.speed *= 1.25f + r_M;
                    proj.baseData.damage *= 1.33f * r_M;
                    proj.UpdateSpeed();
                },
                ThisRarity = Trait.RARITY.ALL,
                Common_Name = "Wild",
                Rare_Name = "Quite Wild",
                Legendary_Name = "Insanely Wild",
                Weight = 0.5f,
            });
            AddTo_1(new Trait()
            {
                SpreadCalc = (f, r_M, rarity_Int) => { return f *= 1.1f; },
                OnProjectileFired = (proj, r_M, rarity_Int) => {

                    proj.baseData.damage *= 1.1f;
                    proj.baseData.UsesCustomAccelerationCurve = true;
                    proj.baseData.AccelerationCurve = AnimationCurve.Linear(0.1f, 0.5f, 2, 2);
                    proj.UpdateSpeed();
                },
                ThisRarity = Trait.RARITY.COMMON,
                Override_Name = "Accelerating",
                Weight = 0.5f,
            });
        }
        public static void InitSecondaries()
        {
            AddTo_2(new Trait()
            {
                OnProjectileFired = (proj, r_M, rarity_Int) =>
                {
                    proj.baseData.force *= (3 * r_M);
                },
                Common_Name = "Force",
                Rare_Name = "Strong Force",
                Legendary_Name = "Powerful Force",
            });
            AddTo_2(new Trait()
            {
                OnProjectileFired = (proj, r_M, rarity_Int) =>
                {
                    var pierce = proj.gameObject.AddComponent<PierceProjModifier>();
                    pierce.penetration = 1 + rarity_Int;
                    proj.baseData.speed *= 0.25f + r_M;
                },
                Common_Name = "Bypass",
                Rare_Name = "Cleaving",
                Legendary_Name = "The Phantasm",
            });
            AddTo_2(new Trait()
            {
                OnProjectileFired = (proj, r_M, rarity_Int) =>
                {
                    proj.baseData.speed *= 2 + r_M;
                    proj.baseData.range *= 1 + r_M;
                },
                ThisRarity = Trait.RARITY.ALL,
                Common_Name = "The Wind",
                Rare_Name = "Powerful Wind",
                Legendary_Name = "Cyclonic Wind",
                ColorModifier = (Colour, r_M, rarity_Int) =>
                {
                    Colour.r += 0.2f * r_M;
                    Colour.g += 0.2f * r_M;
                    Colour.b += 0.2f * r_M;
                    return Colour;
                }
            });
            AddTo_2(new Trait()
            {
                Cooldown = (time, r_M, rarity_Int) =>
                {
                    return time / (1.5f * r_M);
                },
                ThisRarity = Trait.RARITY.ALL,
                Common_Name = "Fast Casting",
                Rare_Name = "Rapid Casting",
                Legendary_Name = "Lightning Fast Casting",
            });
            AddTo_2(new Trait()
            {
                Cooldown = (time, r_M, rarity_Int) =>
                {
                    return time * r_M;
                },
                OnProjectileFired = (proj, r_M, rarity_Int) =>
                {
                    proj.baseData.speed /= r_M + 0.25f;
                    proj.AdditionalScaleMultiplier *= 0.375f + r_M;
                },
                WeaponSizeModifier = (r_M) =>
                {
                    return (0.375f + r_M);
                },
                ThisRarity = Trait.RARITY.ALL,
                Common_Name = "The Colossus",
                Rare_Name = "The Great Colossus",
                Legendary_Name = "The Legendary Colossus",
                Weight = 0.7f
            });
            AddTo_2(new Trait()
            {
                OnProjectileFired = (proj, r_M, rarity_Int) =>
                {
                    proj.AppliesFire = true;
                    proj.fireEffect = rarity_Int == 2 ? DebuffStatics.greenFireEffect : DebuffStatics.hotLeadEffect;
                    proj.FireApplyChance = r_M / 0.55f;
                },
                ThisRarity = Trait.RARITY.ALL,
                Common_Name = "Sparking",
                Rare_Name = "Burning",
                Legendary_Name = "The Eternal Flame",
                Weight = 0.7f,
                ColorModifier = (Colour, r_M, rarity_Int) =>
                {
                    Colour.r += 0.4f * r_M;
                    Colour.g += 0.15f * r_M;
                    return Colour;
                }
            });
            AddTo_2(new Trait()
            {
                OnProjectileFired = (proj, r_M, rarity_Int) =>
                {
                    proj.AppliesFreeze = true;
                    proj.freezeEffect = DebuffStatics.chaosBulletsFreeze;
                    proj.FreezeApplyChance = 0.1f + (0.35f * rarity_Int);
                },
                
                ThisRarity = Trait.RARITY.ALL,
                Common_Name = "The Cold",
                Rare_Name = "Freezing",
                Legendary_Name = "The Frost Lords",
                Weight = 0.7f,
                ColorModifier = (Colour, r_M, rarity_Int) =>
                {
                    Colour.g += 0.02f * r_M;
                    Colour.b += 0.25f * r_M;

                    return Colour;
                }

            });
            AddTo_2(new Trait()
            {
                Cooldown = (time, r_M, rarity_Int) =>
                {
                    return 0.2f;
                },
                SpreadCalc = (f, r_M, rarity_Int) =>
                {
                    return f *= 4;
                },
                ThisRarity = Trait.RARITY.RARE,
                Override_Name = "Silliness",
                Weight = 0.9f
            });
            AddTo_2(new Trait()
            {
                OnProjectileFired = (proj, r_M, rarity_Int) =>
                {
                    proj.baseData.speed *= 1.1f;
                    proj.baseData.damage *= 1.05f;
                },
                Cooldown = (time, r_M, rarity_Int) =>   
                {
                    return time * 0.9f;
                },
                SpreadCalc = (f, r_M, rarity_Int) =>
                {
                    return f *= 0.9f;
                },
                ThisRarity = Trait.RARITY.COMMON,
                Override_Name = "Basic Efficiency",
                Weight = 1.1f
            });
        }

        public static MiscUtility.WeightedTypeCollection<Trait> Primarytraits = new MiscUtility.WeightedTypeCollection<Trait>() { elements = new MiscUtility.WeightedType<Trait>[] { } };
        public static MiscUtility.WeightedTypeCollection<Trait> Secondarytraits = new MiscUtility.WeightedTypeCollection<Trait>() { elements = new MiscUtility.WeightedType<Trait>[] { } };
    }
}
