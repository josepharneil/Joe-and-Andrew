namespace Enemy
{
    public interface IPatroller
    {
        /// <summary>
        /// Update patrolling.
        /// </summary>
        public void UpdatePatrol();
    }

    public interface IChaser
    {
        /// <summary>
        /// Update chasing.
        /// </summary>
        public void UpdateChase();
        
        /// <summary>
        /// Can this enemy detect the target? This can be through sight, hearing etc.
        /// </summary>
        public bool CanDetectTarget();
    }

    public interface IAttacker
    {
        /// <summary>
        /// Update attacking.
        /// </summary>
        public void UpdateAttack();

        /// <summary>
        /// Can enemy enter attack state?
        /// </summary>
        public bool IsInAttackRange();
    }
}