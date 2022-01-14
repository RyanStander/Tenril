using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private void OnEnable()
    {
        EventManager.currentManager.Subscribe(EventType.SaveData, OnSaveData);
        EventManager.currentManager.Subscribe(EventType.LoadData, OnLoadData);
    }

    private void OnDisable()
    {
        EventManager.currentManager.Unsubscribe(EventType.SaveData, OnSaveData);
        EventManager.currentManager.Unsubscribe(EventType.LoadData, OnLoadData);
    }

    void Start()
    {

        //Get the game data
        PlayerData playerData = SaveManager.LoadPlayer();

        //Check if the scene matches the existing one
        if (SceneManager.GetActiveScene().name ==playerData.currentScene)
        {
            //Transfer it over to respective classes
            EventManager.currentManager.AddEvent(new LoadPlayerCharacterData(playerData));
        }
        else
        {
            Debug.Log("The saved scene does not match the active scene, to avoid any testing issues, nothing will happen");
        }


    }


    //TODO: if there is a failure in the actual build, it should notify the player of the error
    private void OnSaveData(EventData eventData)
    {
        if (eventData is SaveData)
        {
            #region Get Player Character Data
            GameObject player = GameObject.FindGameObjectWithTag("Player");

            //Find player stats
            PlayerStats playerStats;
            if (!player.TryGetComponent<PlayerStats>(out playerStats))
            {
                Debug.Log("Could not find PlayerStats on the player, cannot save data.");
                return;
            }

            //Find player inventory
            PlayerInventory playerInventory;
            if (!player.TryGetComponent<PlayerInventory>(out playerInventory))
            {
                Debug.Log("Could not find PlayerInventory on the player, cannot save data.");
                return;
            }
            #endregion

            SaveManager.SavePlayer(playerStats, playerInventory, SceneManager.GetActiveScene().name);
        }
        else
        {
            throw new System.Exception("Error: EventData class with EventType.SaveData was received but is not of class SaveData.");
        }
    }

    private void OnLoadData(EventData eventData)
    {
        if (eventData is LoadData)
        {
            //fetch player data
            PlayerData playerData = SaveManager.LoadPlayer();

            //load scene
            SceneManager.LoadScene(playerData.currentScene);
        }
        else
        {
            throw new System.Exception("Error: EventData class with EventType.LoadData was received but is not of class LoadData.");
        }
    }    
}
