using Alexandria.PrefabAPI;
using ModularMod.Code.Hooks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Dungeonator;

namespace ModularMod.Past.Prefabs.Objects
{
    public class MovingTile
    {
        public static void Init()
        {
            InitTile(1);
            InitTile(2);
        }

        private static void InitTile(int delay = 1)
        {
            GameObject obj = PrefabBuilder.BuildObject("Moving_Tile (Delay: "+ delay.ToString()+")");
            var tk2d = obj.AddComponent<tk2dSprite>();
            tk2d.Collection = StaticCollections.Past_Decorative_Object_Collection;

            tk2d.SetSprite(StaticCollections.Past_Decorative_Object_Collection.GetSpriteIdByName("brick_move_001"));
            tk2d.usesOverrideMaterial = true;
            Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
            mat.mainTexture = tk2d.renderer.material.mainTexture;
            mat.SetColor("_EmissiveColor", new Color32(255, 255, 255, 255));
            mat.SetFloat("_EmissiveColorPower", 0);
            mat.SetFloat("_EmissivePower", 0);
            tk2d.renderer.material = mat;
            var anim = obj.AddComponent<tk2dSpriteAnimator>();
            anim.library = Module.ModularAssetBundle.LoadAsset<GameObject>("MovingTileAnimation").GetComponent<tk2dSpriteAnimation>();
            anim.defaultClipId = anim.library.GetClipIdByName("idle");

            obj.SetLayerRecursively(LayerMask.NameToLayer("FG_Critical"));
            obj.CreateFastBody(CollisionLayer.HighObstacle, new IntVector2(16, 16), new IntVector2(0, 0));
            MovingTileBehavior tile = obj.AddComponent<MovingTileBehavior>();
            tile.Delay = delay;
            
            Alexandria.DungeonAPI.StaticReferences.customObjects.Add("MovingTile_Delay_"+delay.ToString() , obj);
        }

        private class MovingTileBehavior : MonoBehaviour
        {
            public int Delay = 0;
            private tk2dSpriteAnimator animator;
            private RoomHandler currentRoom;
            public void Start()
            {
                animator = this.GetComponent<tk2dSpriteAnimator>();
                currentRoom = this.transform.PositionVector2().GetAbsoluteRoom();
                Actions.OnReinforcementWave += R;
            }
            public void R(RoomHandler r)
            {
                if (currentRoom != null && r == currentRoom)
                {
                    if (Delay > 0) { Delay--; }
                    if (Delay == 0)
                    {
                        if (animator == null) { Destroy(this.gameObject); LootEngine.DoDefaultSynergyPoof(this.transform.PositionVector2()); }
                        else
                        {
                            this.animator.PlayAndDestroyObject("move");
                        }
                    }
                }
            }
            public void OnDestroy()
            {
                Actions.OnReinforcementWave -= R;
            }

        }

    }//BG_Nonsense
}
