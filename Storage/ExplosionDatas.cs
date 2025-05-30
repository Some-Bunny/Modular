﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Gungeon;

namespace ModularMod
{
    internal class StaticExplosionDatas
    {
        public static ExplosionData explosiveRoundsExplosion = Game.Items["explosive_rounds"].GetComponent<ComplexProjectileModifier>().ExplosionData;

        public static ExplosionData genericSmallExplosion = GameManager.Instance.Dungeon.sharedSettingsPrefab.DefaultSmallExplosionData;

        public static ExplosionData genericLargeExplosion = GameManager.Instance.Dungeon.sharedSettingsPrefab.DefaultExplosionData;

        public static ExplosionData customDynamiteExplosion = new ExplosionData
        {
            effect = StaticExplosionDatas.genericLargeExplosion.effect,
            ignoreList = StaticExplosionDatas.genericLargeExplosion.ignoreList,
            ss = StaticExplosionDatas.genericLargeExplosion.ss,
            damageRadius = 5f,
            damageToPlayer = 0f,
            doDamage = true,
            damage = 45f,
            doDestroyProjectiles = true,
            doForce = true,
            debrisForce = 30f,
            preventPlayerForce = true,
            explosionDelay = 0.1f,
            usesComprehensiveDelay = false,
            doScreenShake = true,
            playDefaultSFX = true
        };

        public static ExplosionData nigNukeExplosion = (PickupObjectDatabase.GetById(443) as TargetedAttackPlayerItem).strikeExplosionData;


        public static ExplosionData CopyFields(ExplosionData explosionToCopy)
        {
            ExplosionData explosionData = new ExplosionData();
            explosionData.CopyFrom(explosionToCopy);
            return explosionData;
        }
    }
}