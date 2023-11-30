using UnityEngine;
using SOEvents;

public class Weapon : MonoBehaviour
{
    [SerializeField] public WeaponSOEvent requestDamageDealerEvent;
    [SerializeField] public WeaponSOEvent requestTargetEvent;
    [SerializeField] public WeaponSOEvent requestCooldownEvent;

    public int weaponTypeID;
    public Rigidbody2D parentBody;
    
    public void TryFire(Rigidbody2D parentBody){}
}


public enum WeaponType
{
    SINGLE,
    ALTERNATING,
    DOUBLE_WIDTH,
    TRIPLE,
    BEAM,
    NONE
}
