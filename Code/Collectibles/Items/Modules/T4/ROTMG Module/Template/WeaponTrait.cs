using Alexandria.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ModularMod
{
    public class Trait
    {
        public string TraitName
        {
            get
            {
                return StaticColorHexes.AddColorToLabelString(Override_Name != null ? Override_Name + " " : RarityTraitName, Hex);
            }
        }
        private string Hex
        {
            get
            {
                switch (ThisRarity)
                {

                    case RARITY.COMMON:
                        return StaticColorHexes.White_Hex;
                    case RARITY.RARE:
                        return StaticColorHexes.Green_Hex;
                    case RARITY.LEGENDARY:
                        return StaticColorHexes.Red_Color_Hex;
                }
                return StaticColorHexes.White_Hex;
            }
        }

        public float RarityMultiplier
        {
            get
            {
                switch (ThisRarity)
                {
                    case RARITY.COMMON:
                        return 1;
                    case RARITY.RARE:
                        return 1.3f;
                    case RARITY.LEGENDARY:
                        return 1.85f;
                }
                return 1;
            }
        }

        public int Rarity
        {
            get
            {
                switch (ThisRarity)
                {
                    case RARITY.COMMON:
                        return 0;
                    case RARITY.RARE:
                        return 1;
                    case RARITY.LEGENDARY:
                        return 2;
                }
                return 0;
            }
        }
       
        private string RarityTraitName
        {
            get
            {
                switch (ThisRarity)
                {
                    case RARITY.COMMON:
                        return Common_Name + " ";
                    case RARITY.RARE:
                        return Rare_Name + " ";
                    case RARITY.LEGENDARY:
                        return Legendary_Name + " ";
                }
                return "";
            }
        }

        public void RarityApplier()
        {
            if (ThisRarity != RARITY.ALL) { return; }
            ThisRarity = rarityWeights.SelectByWeight();
        }

        public virtual Color ModifyWeaponColor(Color color) { if (ColorModifier != null) { return ColorModifier(color, this.RarityMultiplier, Rarity); } return color; }
        public virtual float ModifyWeaponSize() { if (WeaponSizeModifier != null) { return WeaponSizeModifier(this.RarityMultiplier); } return 1; }
        public virtual void ModifyProjectilePrimary(Projectile projectile) { if (OnProjectileFired != null) { OnProjectileFired(projectile, this.RarityMultiplier, Rarity); } }
        public virtual float ModifyCooldown(float baseCooldown)
        {
            if (Cooldown != null) { return Cooldown(baseCooldown, this.RarityMultiplier, Rarity); }
            return baseCooldown;
        }
        public virtual float ModifySpread(float baseCooldown)
        {
            if (SpreadCalc != null) { return SpreadCalc(baseCooldown, this.RarityMultiplier, Rarity); }
            return baseCooldown;
        }

        public virtual int ModifyProjectileAmount(int Amount) { if (ProjectileAmount != null) { return ProjectileAmount(Amount, this.RarityMultiplier, Rarity); } return 0; }
        public virtual float ModifyArc(float spread) { if (ProjectileArcSpread != null) { return ProjectileArcSpread(spread, this.RarityMultiplier, Rarity); } return 1; }


        public string Common_Name = "";
        public string Rare_Name = "";
        public string Legendary_Name = "";
        public string Override_Name = null;

        public float Weight = 1;

        public Action<Projectile, float, int> OnProjectileFired;
        public Func<float, float, int, float> Cooldown;
        public Func<float, float, int, float> SpreadCalc;
        public Func<float, float> WeaponSizeModifier;
        public Func<int, float, int, int> ProjectileAmount;
        public Func<float, float, int, float> ProjectileArcSpread;



        public Func<Color, float, int, Color> ColorModifier;



        public enum RARITY
        {
            COMMON,
            RARE,
            LEGENDARY,
            ALL
        }

        public RARITY ThisRarity = RARITY.ALL;

        private static MiscUtility.WeightedTypeCollection<RARITY> rarityWeights = new MiscUtility.WeightedTypeCollection<RARITY>()
        {
            elements = new MiscUtility.WeightedType<RARITY>[]
            {
                        new MiscUtility.WeightedType<RARITY>() { value = RARITY.COMMON, weight = 1 },
                        new MiscUtility.WeightedType<RARITY>() { value = RARITY.RARE, weight = 0.7f },
                        new MiscUtility.WeightedType<RARITY>() { value = RARITY.LEGENDARY, weight = 0.2f }
            }
        };
    }
}
