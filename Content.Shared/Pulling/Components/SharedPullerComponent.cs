﻿namespace Content.Shared.Pulling.Components
{
    [RegisterComponent]
    [Friend(typeof(SharedPullingStateManagementSystem))]
    public sealed class SharedPullerComponent : Component
    {
        // Before changing how this is updated, please see SharedPullerSystem.RefreshMovementSpeed
        public float WalkSpeedModifier => Pulling == default ? 1.0f : 0.75f;

        public float SprintSpeedModifier => Pulling == default ? 1.0f : 0.75f;

        [ViewVariables]
        public EntityUid? Pulling { get; set; }

        protected override void Shutdown()
        {
            EntitySystem.Get<SharedPullingStateManagementSystem>().ForceDisconnectPuller(this);
            base.Shutdown();
        }

        protected override void OnRemove()
        {
            if (Pulling != default)
            {
                // This is absolute paranoia but it's also absolutely necessary. Too many puller state bugs. - 20kdc
                Logger.ErrorS("c.go.c.pulling", "PULLING STATE CORRUPTION IMMINENT IN PULLER {0} - OnRemove called when Pulling is set!", Owner);
            }
            base.OnRemove();
        }
    }
}
