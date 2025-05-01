public interface IAbility
{
    void Activate();
    void Deactivate(); // Optional for continuous abilities like movement
    void Tick();       // Called every frame (for movement, cooldowns, etc.)
    bool IsActive { get; }
}