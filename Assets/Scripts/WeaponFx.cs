using UnityEngine;

public class WeaponFx : MonoBehaviour
{
    [Header("Weapon FX")] public ParticleSystem normalWeaponTrail;
    //TODO: Add more weapon trails types (for example when enchanting weapon with fire damage

    public void PlayWeaponFx()
    {
        //Makes sure that the weapon effect doesnt act strangely when playing
        normalWeaponTrail.Stop();
        
        normalWeaponTrail.Play();
    }
    
    public void StopWeaponFx()
    {
        normalWeaponTrail.Stop();
    }
}
