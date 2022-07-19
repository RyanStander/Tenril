using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Manager that handles footsteps belonging to different types of ground textures.
/// </summary>
public class FootstepTerrainSoundManager : MonoBehaviour
{
    /// <summary>
    /// The layer related to ground detection.
    /// </summary>
    [SerializeField]
    private LayerMask FloorLayer;

    /// <summary>
    /// The texture sounds currently active in the manager.
    /// </summary>
    [SerializeField]
    private TextureSound[] TextureSounds;

    /// <summary>
    /// If the sounds should be blended.
    /// </summary>
    [SerializeField]
    private bool BlendTerrainSounds;

    /// <summary>
    /// The active character controller.
    /// </summary>
    private CharacterController Controller;

    /// <summary>
    /// The active audio source.
    /// </summary>
    private AudioSource AudioSource;

    /// <summary>
    /// Get relevant components.
    /// </summary>
    private void Awake()
    {
        Controller = GetComponent<CharacterController>();
        AudioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// Start the courotine to check the ground.
    /// </summary>
    private void Start() => StartCoroutine(CheckGround());

    /// <summary>
    /// Courotine for checking the ground and playing audio clips.
    /// </summary>
    private IEnumerator CheckGround()
    {
        // Continuously run the ground check.
        while (true)
        {
            // Helper variables for ease of calculation.
            Vector3 origin = transform.position - new Vector3(0, 0.5f * Controller.height + 0.5f * Controller.radius, 0);
            bool groundRay = Physics.Raycast(origin, Vector3.down, out RaycastHit hit, 1f, FloorLayer);

            // If grounded and a collision was found.
            if (Controller.isGrounded && Controller.velocity != Vector3.zero && groundRay)
            {
                // Play a sound based on the type of ground detected.
                if (hit.collider.TryGetComponent(out Terrain terrain))
                {
                    yield return StartCoroutine(PlayFootstepSoundFromTerrain(terrain, hit.point));
                }
                else if (hit.collider.TryGetComponent(out Renderer renderer))
                {
                    yield return StartCoroutine(PlayFootstepSoundFromRenderer(renderer));
                }
            }
            yield return null;
        }
    }

    /// <summary>
    /// Plays an audio clip based on the given renderer.
    /// </summary>
    /// <param name="Terrain">The terrain being checked.</param>
    /// <param name="HitPoint">The point on the terrain being checked.</param>
    /// <returns>On the completion of the audio clip.</returns>
    private IEnumerator PlayFootstepSoundFromTerrain(Terrain Terrain, Vector3 HitPoint)
    {
        // Helper variables for positioning.
        Vector3 terrainPosition = HitPoint - Terrain.transform.position;
        Vector3 splatMapPosition = new Vector3(terrainPosition.x / Terrain.terrainData.size.x, 0, terrainPosition.z / Terrain.terrainData.size.z);

        // Calculate the terrain coordinate data.
        int xPosition = Mathf.FloorToInt(splatMapPosition.x * Terrain.terrainData.alphamapWidth);
        int yPosition = Mathf.FloorToInt(splatMapPosition.z * Terrain.terrainData.alphamapHeight);
        float[,,] alphaMap = Terrain.terrainData.GetAlphamaps(xPosition, yPosition, 1, 1);

        // Execute sound logic based on blending setting.
        if (!BlendTerrainSounds)
        {
            int primaryIndex = 0;
            for (int i = 1; i < alphaMap.Length; i++)
            {
                if (alphaMap[0, 0, i] > alphaMap[0, 0, primaryIndex])
                {
                    primaryIndex = i;
                }
            }

            foreach (TextureSound textureSound in TextureSounds)
            {
                if (textureSound.albedo == Terrain.terrainData.terrainLayers[primaryIndex].diffuseTexture)
                {
                    AudioClip clip = textureSound.GetRandomClip();
                    AudioSource.PlayOneShot(clip);
                    yield return new WaitForSeconds(clip.length);
                    break;
                }
            }
        }
        else
        {
            List<AudioClip> clips = new List<AudioClip>();
            int clipIndex = 0;
            for (int i = 0; i < alphaMap.Length; i++)
            {
                if (alphaMap[0, 0, i] > 0)
                {
                    foreach (TextureSound textureSound in TextureSounds)
                    {
                        if (textureSound.albedo == Terrain.terrainData.terrainLayers[i].diffuseTexture)
                        {
                            AudioClip clip = textureSound.GetRandomClip();
                            AudioSource.PlayOneShot(clip, alphaMap[0, 0, i]);
                            clips.Add(clip);
                            clipIndex++;
                            break;
                        }
                    }
                }
            }

            float longestClip = clips.Max(clip => clip.length);
            yield return new WaitForSeconds(longestClip);
        }
    }

    /// <summary>
    /// Plays an audio clip based on the given renderer.
    /// </summary>
    /// <param name="Renderer">The renderer being used.</param>
    /// <returns>On the completion of the audio clip.</returns>
    private IEnumerator PlayFootstepSoundFromRenderer(Renderer Renderer)
    {
        // Iterate over all of the texture sounds.
        foreach (TextureSound textureSound in TextureSounds)
        {
            // Check if one of the albedos match the renderers texture.
            if (textureSound.albedo == Renderer.material.GetTexture("_MainTex"))
            {
                // Get the audio clip.
                AudioClip clip = textureSound.GetRandomClip();

                // Play the audio clip and wait until it's complete before returning.
                AudioSource.PlayOneShot(clip);
                yield return new WaitForSeconds(clip.length);
                break;
            }
        }
    }
}
