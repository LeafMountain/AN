namespace AIBehaviors
{
    public interface IAIBehavior
    {
        public void OnEnter(Character actor);
        public void OnExit(Character actor);
        public void OnUpdate(Character actor);
    }
}
