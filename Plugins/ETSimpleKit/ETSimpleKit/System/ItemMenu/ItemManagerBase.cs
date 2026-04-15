using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ETSimpleKit.ItemSystem
{
    /// <summary>
    /// Role: Manage item menu
    /// How to use: 
    /// - add ItemData to array
    /// </summary>
    public class ItemManagerBase : MonoBehaviour
    {
        public ItemData[] itemDatas;
        public List<ItemDat> itemDats;
        public Transform container;
        public ItemBase pp_item;
        [HideInInspector] public List<ItemBase> listItemBase = new();
        //Example in child class
        //void Start()
        //{
        //    //GenerateItemDats(3);
        //    //GenerateItemMenuUI();
        //}
        /// <summary>
        /// if unlockItemTillExclude = 3, it will unlock 0,1,2
        /// </summary>
        /// <param name="unlockItemTillExclude"></param>
        public void GenerateItemDats(int unlockItemTillExclude = 0)
        {
            itemDats = new List<ItemDat>();
            foreach (var data in itemDatas)
            {
                itemDats.Add(new ItemDat(data));
            }
            for (int i = 0; i < itemDats.Count; i++)
            {
                itemDats[i].id = i;
                if (i< unlockItemTillExclude)
                {
                    itemDats[i].isLocked = false;
                }
                else
                {
                    itemDats[i].isLocked = true;
                }
            }
        }
        public void GenerateItemMenuUI(UnityAction<int> ifIsLockTrueAction = null, UnityAction<int> ifIsLockFalseAction = null)
        {
            listItemBase = new();
            Debug.Log($"Create {itemDats.Count} item");
            foreach (var itemDat in itemDats)
            {
                ItemBase itemBase = Instantiate<ItemBase>(pp_item, container);
                itemBase.Init(itemDat, ifIsLockTrueAction, ifIsLockFalseAction);
                listItemBase.Add(itemBase);
                itemBase.gameObject.SetActive(true);
            }
        }
        public void Sellect(int index)
        {
            foreach (var dat in itemDats)
            {
                if(dat.id == index)
                {
                    dat.isSelect = true;
                }
                else
                {
                    dat.isSelect = false;
                }
            }
            foreach (var itemBase in listItemBase)
            {
                itemBase.UpdateSelect();    
            }
        }
        public void UnlockTill(int indexExcluded)
        {
            for (int i = 0; i < indexExcluded; i++)
            {
                itemDats[i].isLocked = false;
                listItemBase[i].UpdateLock();
            }
        }
    }
}