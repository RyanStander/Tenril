using UnityEngine;

public class WeaponFx : MonoBehaviour
{
    [Header("Weapon FX")] public ParticleSystem normalWeaponTrail;
    //TODO: Add more weapon trails types (for example when enchanting weapon with fire damage

    public void PlayWeaponFX()
    {
        //Makes sure that the weapon effect doesnt act strangely when playing
        normalWeaponTrail.Stop();

        if (normalWeaponTrail.isStopped)
        {
            normalWeaponTrail.Play();
        }
    }
    
    public void StopWeaponFX()
    {
        normalWeaponTrail.Stop();
    }
}
