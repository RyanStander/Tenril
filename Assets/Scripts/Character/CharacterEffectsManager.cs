using System;
using UnityEngine;

namespace Character
{
    public class CharacterEffectsManager : MonoBehaviour
    {
        [Header("Damage Fx")] public GameObject bloodSplatterFx;
        [Header("Weapon Fx")]
        public WeaponFx rightWeaponFx;
        public WeaponFx leftWeaponFx;

        public virtual void PlayWeaponFx()
        {
            if (rightWeaponFx!=null)
            {
                rightWeaponFx.PlayWeaponFx();
            }
            if (leftWeaponFx!=null)
            {
                leftWeaponFx.PlayWeaponFx();
            }
        }

        public virtual void StopWeaponFx()
        {
            if (rightWeaponFx!=null)
            {
                rightWeaponFx.StopWeaponFx();
            }
            if (leftWeaponFx!=null)
            {
                leftWeaponFx.StopWeaponFx();
            }
        }

        public virtual void PlayBloodSplatterFx(Vector3 bloodSplatterLocation)
        {
            //create blood at hit location
            var blood = Instantiate(bloodSplatterFx, bloodSplatterLocation, Quaternion.identity);
        }
    }
}
