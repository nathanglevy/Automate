﻿using System;
using System.Collections.Generic;
using System.Linq;
using Automate.Model.Utility;
using UnityEngine;

namespace src.View
{
    static public class GraphicsHandler
    {
        private static Dictionary<string, string> _textureMappingDict = new Dictionary<string, string>()
        {
            { "StrongBox", "SpriteSheets/open_tileset_2x:599" },
            { "RegularBox", "SpriteSheets/open_tileset_2x:641" },
            { "FragileBox", "SpriteSheets/open_tileset_2x:643" },
            { "ContainerLeft", "SpriteSheets/open_tileset_2x:189" },
            { "ContainerRight", "SpriteSheets/open_tileset_2x:190" },
        };
        private static HashSet<string> _cachedSpriteSheets = new HashSet<string>();
        private static Dictionary<string, Sprite> _cachedSprites = new Dictionary<string, Sprite>();
        private static Dictionary<string, Sprite> _spriteNameToSprite = new Dictionary<string, Sprite>();

        public static void SetSpriteByPath(GameObject gameObjectToSet, string spriteSheetPath, List<int> spriteNums, Vector3 dimensions)
        {
            if (dimensions.x + dimensions.y != spriteNums.Count)
                throw new ArgumentException("Incorrect amount of sprites given for dimensions of sprite request");
            var xcount = 0;
            var ycount = 0;
            foreach (int spriteNum in spriteNums)
            {
                //create new sprite block
                GameObject newGameObject = new GameObject();
                SpriteRenderer spriteRenderer = newGameObject.AddComponent<SpriteRenderer>();
                newGameObject.transform.parent = gameObjectToSet.transform;
                newGameObject.transform.Translate(xcount * 64, ycount * 64, 0);

                //set sprite block
                SetSpriteByPath(newGameObject, spriteSheetPath, spriteNum);

                //increment offset
                xcount++;
                if (xcount >= dimensions.x)
                {
                    xcount = 0;
                    ycount++;
                }
            }
            
        }

        public static void SetSpriteByPath(GameObject gameObjectToSet, string spriteSheetPath, int spriteNum)
        {
            string patternFullName = spriteSheetPath + ":" + spriteNum;
            LoadSpriteSheetToCache(spriteSheetPath);
            Sprite newSprite = _cachedSprites[patternFullName];
            gameObjectToSet.GetComponent<SpriteRenderer>().sprite = newSprite;
        }

        public static void SetSpriteByName(GameObject gameObjectToSet, string patternName)
        {
            if (_spriteNameToSprite.ContainsKey(patternName))
            {
                gameObjectToSet.GetComponent<SpriteRenderer>().sprite = _spriteNameToSprite[patternName];
                return;
            }
            string patternPath = _textureMappingDict[patternName];
            string spritePath = patternPath.Split(':')[0];
            int spriteNum = int.Parse(patternPath.Split(':')[1]);
            SetSpriteByPath(gameObjectToSet, spritePath, spriteNum);
        }

        //        private static void LoadSpriteSheetToCache(string spriteSheetPath)
        //        {
        //            
        //        }

        private static void LoadSpriteSheetToCache(string spriteSheetPath)
        {
            if (!_cachedSpriteSheets.Contains(spriteSheetPath))
            {
                _cachedSpriteSheets.Add(spriteSheetPath);
                Sprite[] sprites = Resources.LoadAll<Sprite>(spriteSheetPath);
                foreach (var item in sprites.Select((value, i) => new { i, value })) {
                    _cachedSprites.Add(spriteSheetPath + ":" + item.i, item.value);
                }
            }      
        }
    }
}