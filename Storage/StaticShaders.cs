﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ModularMod
{
    public class StaticShaders
    {

        public static void GetDisplacerMat()
        {
            Displacer_Beast_Shader = EnemyDatabase.GetOrLoadByGuid("45192ff6d6cb43ed8f1a874ab6bef316").sprite.renderer.material.shader;


            var orLoadByName = DungeonDatabase.GetOrLoadByName("Finalscenario_Soldier");
            FloorTileMaterial =  new Material(orLoadByName.tileIndices.dungeonCollection.materials[0]);
            FloorTileMaterial_Transparency = new Material(orLoadByName.tileIndices.dungeonCollection.materials[1]);

            orLoadByName = null;
        }

        public static Shader Hologram_Shader = ShaderCache.Acquire("Brave/Internal/HologramShader");
        public static Shader Default_Shader = ShaderCache.Acquire("Sprites/Default");
        public static Shader Space_Fog_Shader = ShaderCache.Acquire("Brave/Internal/SpaceFogShader");
        public static Shader Gonner_Shader = ShaderCache.Acquire("tk2d/CutoutVertexColorTiltedGonner");
        public static Shader Displacer_Beast_Shader = ShaderCache.Acquire("Brave/DisplacerBeast");
        public static Shader Default_UI_Shader = ShaderCache.Acquire("Daikon Forge/Default UI Shader");
        public static Shader MagicCircle_Shader = ShaderCache.Acquire("Brave/MagicCircle");
        public static Shader Dragun_Spotlight_Shader = ShaderCache.Acquire("Brave/Internal/GBuffer_Custom_DragunSpotlight");
        public static Shader Player_Shader = ShaderCache.Acquire("Brave/PlayerShader");
        public static Shader DarkPortal_Shader = ShaderCache.Acquire("Brave/Internal/DarkPortalShader");
        public static Shader SpaceFog_Shader = ShaderCache.Acquire("Brave/Internal/SpaceFogShader");
        public static Shader TransparencyShader = Shader.Find("Brave/Internal/SimpleAlphaFadeUnlit");
        public static Material FloorTileMaterial;
        public static Material FloorTileMaterial_Transparency;

    }
}
