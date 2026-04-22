namespace SABI
{
    [System.Serializable]
    public class TransitionToStateBasedOnLODTargetTag
    {
        public string lodTargetTag;
        public State_Base stateToTransition_OnVisanEnter;
        public State_Base stateToTransition_OnVisanExit;
    }
}
