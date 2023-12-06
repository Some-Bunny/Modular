using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ModularMod
{
    public static class GunInt
    {
        public static void SetupSprite(this Gun gun, tk2dSpriteCollectionData collection = null, string defaultSprite = null)
        {
            if ((object)collection == null)
            {
                collection = ETGMod.Databases.Items.WeaponCollection;
            }
            if (defaultSprite != null)// && !GunSpriteDefs.Contains(gun))
            {
                GunSpriteDefs.Add(collection.GetSpriteDefinition(defaultSprite));

                //AddSpriteToCollection(collection.GetSpriteDefinition(defaultSprite), ammonomiconCollection);
                gun.encounterTrackable.journalData.AmmonomiconSprite = defaultSprite;
            }
            gun.emptyAnimation = null;


            tk2dBaseSprite sprite = gun.GetSprite();
            tk2dSpriteCollectionData newCollection = collection;
            int newSpriteId = (gun.DefaultSpriteID = collection.GetSpriteIdByName(gun.encounterTrackable.journalData.AmmonomiconSprite));
            sprite.SetSprite(newCollection, newSpriteId);
        }

        private static List<tk2dSpriteDefinition> GunSpriteDefs = new List<tk2dSpriteDefinition>();

        private static void AddSpritesToCollection(tk2dSpriteCollectionData collection)
        {
            tk2dSpriteDefinition[] spriteDefinitions = collection.spriteDefinitions;
            tk2dSpriteDefinition[] array = spriteDefinitions.Concat(GunSpriteDefs.ToArray()).ToArray<tk2dSpriteDefinition>();
            
            collection.spriteDefinitions = array;
            for (int i = 0; i < collection.spriteDefinitions.Length; i++)
            {
                collection.spriteNameLookupDict[collection.spriteDefinitions[i].name] = i;
            }
        }


        public static void FinalizeSprites()
        {
            AddSpritesToCollection(ammonomiconCollection);
        }

        public static tk2dSpriteCollectionData ammonomiconCollection = AmmonomiconController.ForceInstance.EncounterIconCollection;
    }
}
