namespace Bolt
{
    public static class BoltEvents
    {
        // *IMPORTANT* When you update this, you must update the unit options in tools/bolt
        public static readonly string PlayerIsInAttackRange = "PlayerIsInAttackRange";
        public static readonly string PlayerIsNotInAttackRange = "PlayerIsNotInAttackRange";
        public static readonly string CanSeePlayer = "CanSeePlayer";
        public static readonly string CannotSeePlayer = "CannotSeePlayer";
        public static readonly string EnemyIsDead = "EnemyIsDead";
    }
}