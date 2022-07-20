using System;
using UnityEngine;

namespace Character
{
    public class CharacterEffectsManager : MonoBehaviour
    {
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
    }
}
