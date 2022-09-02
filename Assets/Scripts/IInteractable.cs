using Core;

public interface IInteractable
{
    void Interact(Actor interactor);
    string GetPrompt();
}
