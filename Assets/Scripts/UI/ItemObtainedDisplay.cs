using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemObtainedDisplay : MonoBehaviour
{
    public TMP_Text itemNameText;
    public Image itemImage;

    [SerializeField] private float popUpDuration=3;

    private void Start()
    {
        //Remove the object after a set time
        Destroy(gameObject, popUpDuration);
    }
}
