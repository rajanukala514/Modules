using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Modules.AtlasSystem
{
    public class AtlasManager : Singleton.GenericSingleton<AtlasManager> {

        [SerializeField]
        private KeyTexturePair[] allTextures;

        [SerializeField]
        private string resourceFolderPath;

        private Sprite[] allSprites;
        private Dictionary<string, Dictionary<string, Sprite>> mySprites;

        protected override void Awake()
        {
            base.Awake();
            if (resourceFolderPath != String.Empty)
            {
                Initialize();
            }

            else
                Debug.LogError("Resource folder path must need");
        }

        /// <summary>
        /// Initiating all the sprites from the resources which are required load at runtime
        /// </summary>
        private void Initialize()
        {
            mySprites = new Dictionary<string, Dictionary<string, Sprite>>();

            for (int i = 0; i < allTextures.Length; i++)
            {
                StringBuilder path = new StringBuilder(resourceFolderPath);
                path.Append(allTextures[i].texture.name);

                allSprites = Resources.LoadAll<Sprite>(path.ToString());

                if (!mySprites.ContainsKey(allTextures[i].key))
                {
                    Dictionary<string, Sprite> innerDic = new Dictionary<string, Sprite>();
                    mySprites.Add(allTextures[i].key, innerDic);

                    for (int j = 0; j < allSprites.Length; j++)
                    {
                        //todo : Store all sprites to apply
                        if (!innerDic.ContainsKey(allSprites[j].name))
                        {
                            innerDic.Add(allSprites[j].name, allSprites[j]);
                        }
                    }
                }
            }

            GC.Collect();
        }

        /// <summary>
        /// Returns the Sprite with the given key and sprite name from the stored list
        /// </summary>
        /// <param name="key"></param>
        /// <param name="spriteName"></param>
        /// <returns></returns>
        public Sprite GetSprite(string key, string spriteName)
        {
            if (mySprites != null && mySprites.ContainsKey(key) && mySprites[key].ContainsKey(spriteName))
                return mySprites[key][spriteName];
            else
                Debug.LogError("Sprite name can not found in the atlas");

            return null;
        }
    }
}
