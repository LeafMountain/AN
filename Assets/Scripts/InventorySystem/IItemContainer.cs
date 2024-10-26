namespace InventorySystem
{
    public interface IItemContainer
    {
        public InventoryHandle InventoryHandle { get; set; }
        // public void DepositImplementation(ItemHandle itemAccessId);
        // public void WithdrawImplementation(ItemHandle itemAccessId);
    }
}