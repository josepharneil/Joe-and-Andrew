using UnityEngine;

public class EntityBlock : MonoBehaviour
{
    private bool _isBlocking = false;
    [SerializeField] private int blockDamageReductionFactor = 2;

    public bool blockHasBeenBroken = false;
    [SerializeField] private float blockBreakDuration = 1.0f;
    private float _blockTimer = 0.0f;
    public float blockSpeedModifier = 0.25f;

    public void BreakBlock()
    {
        blockHasBeenBroken = true;
        _blockTimer = blockBreakDuration;
    }

    public bool IsBlocking()
    {
        return _isBlocking;
    }

    public void SetBlocking(bool value)
    {
        _isBlocking = value;
    }

    public void ReduceDamage(ref int damage)
    {
        damage /= blockDamageReductionFactor;
    }
    
    /// <summary>
    /// Todo maybe unify all timers into one single update function in one script
    /// </summary>
    private void Update()
    {
        if (!blockHasBeenBroken) return;
        
        _blockTimer -= Time.deltaTime;
        if (_blockTimer <= 0)
        {
            blockHasBeenBroken = false;
        }
    }
}
