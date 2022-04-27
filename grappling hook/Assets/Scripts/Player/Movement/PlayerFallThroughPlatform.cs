using System;
using Physics;
using UnityEngine;

namespace Player
{
    [Serializable] public class PlayerFallThroughPlatform
    {
        private bool _hasFallenThroughPlatform;
        private int _fallThroughPlatformTimer;

        public void Update(BoxRayCollider2D boxRayCollider2D, ref bool ref_isJumpInput, bool verticalInputIsDown)
        {
            TryFallThroughPlatform(boxRayCollider2D, ref ref_isJumpInput, verticalInputIsDown);
            ContinueOrStopFallingThroughPlatform(boxRayCollider2D);
        }

        private void ContinueOrStopFallingThroughPlatform(BoxRayCollider2D boxRayCollider2D)
        {
            // JA:29/03/22 Not sure if you should use frame counting for this instead of a timer...???
            // TODO Look into this
            if (_hasFallenThroughPlatform && _fallThroughPlatformTimer < 20)
            {
                _fallThroughPlatformTimer += 1;
            }
            else
            {
                _hasFallenThroughPlatform = false;
                _fallThroughPlatformTimer = 0;
                boxRayCollider2D.SetFallThroughPlatform(false);
            }
        }

        private void TryFallThroughPlatform(BoxRayCollider2D boxRayCollider2D, ref bool ref_isJumpInput,
            bool verticalInputIsDown)
        {
            if (ref_isJumpInput && boxRayCollider2D.CheckIfOneWayPlatform() && verticalInputIsDown)
            {
                ref_isJumpInput = false;
                boxRayCollider2D.SetFallThroughPlatform(true);
                _hasFallenThroughPlatform = true;
            }
        }
    }
}