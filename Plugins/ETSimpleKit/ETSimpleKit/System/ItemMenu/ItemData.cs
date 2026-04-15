using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETSimpleKit.ItemSystem
{
    [CreateAssetMenu(fileName = "ItemData", menuName = "Game/Data/ItemData")]
    public class ItemData: ScriptableObject
    {
        //info
        public string id = "item0";
        public string title = "Item";
        public string des = "This is an item";
        //UI
        public Sprite smallThumb = null;
        public Sprite largeThumb = null;
    }
}