
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CAssets
{
    public static class AssetHelper
    {
        public static Sprite Texture2DToSprite(Texture2D texture2D)
        {
            return Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), Vector2.zero);
        }
    }
}