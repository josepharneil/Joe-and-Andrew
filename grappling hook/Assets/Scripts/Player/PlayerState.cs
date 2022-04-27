using UnityEngine;
using Entity;

namespace Player
{
    public class PlayerState
    {
        public MoveState MoveState;
        public Vector2 Velocity;
        public FacingDirection FacingDirection;
        public AttackDirection AttackDirection;
    }
}