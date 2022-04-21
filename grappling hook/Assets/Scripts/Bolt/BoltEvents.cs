namespace Bolt
{
    // TODO Rename this visual scripting
    public static class BoltEvents
    {
        // *IMPORTANT* When you update this, you must "regenerate nodes" in Project Settings -> Visual Scripting
        public static readonly string PlayerIsInAttackRange = "PlayerIsInAttackRange";
        public static readonly string PlayerIsNotInAttackRange = "PlayerIsNotInAttackRange";
        public static readonly string CanSeePlayer = "CanSeePlayer";
        public static readonly string CannotSeePlayer = "CannotSeePlayer";
        public static readonly string EnemyIsDead = "EnemyIsDead";
        public static readonly string EnemyIsDestroy = "EnemyIsDestroy";
        
        // DemoBoss
        public static readonly string ActionSelected = "ActionSelected";
        public static readonly string WindUpDone = "WindUpDone";
        public static readonly string PerformActionDone = "PerformActionDone";
    }
}