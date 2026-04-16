namespace ETSimpleKit.ItemSystem
{
    public class ItemDat
    {
        //info
        public int id = 0;
        public ItemData data;
        //properties
        public bool isLocked = true;
        public bool isSelect = false;
        public ItemDat(ItemData data, bool isLocked = true)
        {
            this.data = data;
            this.isLocked = isLocked;
            this.isSelect = false;
        }
    }
}