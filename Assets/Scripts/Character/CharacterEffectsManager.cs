using System;
using UnityEngine;

namespace Character
{
    public class CharacterEffectsManager : MonoBehaviour
    {
        public WeaponFx rightWeaponFx;
        public WeaponFx leftWeaponFx;

        public virtual void PlayerWeaponFx()
        {
            if (rightWeaponFx!=null)
            {
                rightWeaponFx.PlayWeaponFX();
            }
            if (leftWeaponFx!=null)
            {
                leftWeaponFx.PlayWeaponFX();
            }
        }

        public virtual void StopWeaponFx()
        {
            if (rightWeaponFx!=null)
            {
                rightWeaponFx.StopWeaponFX();
            }
            if (leftWeaponFx!=null)
            {
                leftWeaponFx.StopWeaponFX();
            }
        }
    }
}
