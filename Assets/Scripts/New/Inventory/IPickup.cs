public interface IPickup
{
    string ItemID { get; }
    void OnPickedUp();
}