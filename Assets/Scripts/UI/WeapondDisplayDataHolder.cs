using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeapondDisplayDataHolder : MonoBehaviour
{
    [SerializeField] private Image weaponIcon;
    [SerializeField] private TMP_Text weaponName;
    [SerializeField] private GameObject unselectedHighlight;

    public void SetValues(Sprite weaponIcon, string weaponName)
    {
        if (this.weaponIcon!=null)
            this.weaponIcon.sprite = weaponIcon;

        if (this.weaponName!=null)
            this.weaponName.text = weaponName;
    }

    public void DeselectWeapon()
    {
        if (unselectedHighlight!=null)
            unselectedHighlight.SetActive(true);
    }

    public void SelectWeapon()
    {
        if (unselectedHighlight != null)
            unselectedHighlight.SetActive(false);
    }
}
