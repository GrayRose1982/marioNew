using UnityEngine;

public class TakeDamageBoss : MonoBehaviour, IAttackedable
{
    [SerializeField] private IAttackedable _main;

    protected IAttackedable Main
    {
        get { return _main ?? (_main = transform.parent.GetComponent<IAttackedable>()); }
    }

    public void Attacked(AttackType type, IAttackedable attacker = null)
    {
        if (Main != null)
            Main.Attacked(type, attacker);
    }
}