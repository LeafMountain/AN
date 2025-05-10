using System;
using System.Collections.Generic;

public interface IAbility {
    bool IsActive { get; }
    void Activate();
    void Deactivate();
    void Tick();
    IEnumerable<System.Type> GetConflictingAbilities() => new List<Type>();
}