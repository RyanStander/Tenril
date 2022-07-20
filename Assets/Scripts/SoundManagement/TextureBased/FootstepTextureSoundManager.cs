using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Manager that handles footsteps belonging to different types of ground textures.
/// </summary>
public class FootstepTextureSoundManager : MonoBehaviour
{
    /// <summary>
    /// The layer related to ground detection.
    /// </summary>
    [SerializeField]
    private LayerMask floorLayer;

    /// <summary>
    /// If the sounds should be blended.
    /// </summary>
    [SerializeField]
    private bool blendTerrainSounds;

    /// <summary>
    /// Draw a ray to visualize the affected area.
    /// </summary>
    public void Update()
    {
        // Calculate the origin and debug a ray.
        Vector3 origin = transform.position + (Vector3.up / 2);
        Debug.DrawRay(origin, Vector3.down, Color.red);
    }

    /// <summary>
    /// Check for the type of ground below and return a valid audio clip.
    /// <param name="textureSounds">The collection of texture sounds being checked.</param>
    /// </summary>
    /// <returns>An audio clip based on the ground texture.</returns>
    public ClipVolumePair GetFootstepTextureSound(TextureSounds[] textureSounds)
    {
        // Calculate the origin to create a ray from.
        Vector3 origin = transform.position + (Vector3.up / 2);

        // If collision was found.
        if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, 1f, floorLayer))
        {
            // Get a sound based on the type of ground detected.
            if (hit.collider.TryGetComponent(out Terrain terrain))
            {
                return GetFootstepSoundFromTerrain(terrain, hit.point, textureSounds);
            }
            else if (hit.collider.TryGetComponent(out Renderer renderer))
            {
                return GetFootstepSoundFromRenderer(renderer, textureSounds);
            }
        }

        // If no clips are found, return null after debugging a warning.
        Debug.LogWarning("No audio clip could be fetched!");
        return new ClipVolumePair(null, 0);
    }

    /// <summary>
    /// Plays an audio clip based on the given renderer.
    /// </summary>
    /// <param name="terrain">The terrain being checked.</param>
    /// <param name="hitPoint">The point on the terrain being checked.</param>
    /// <param name="textureSounds">The collection of texture sounds being checked.</param>
    /// <returns>An audio clip based on the ground texture.</returns>
    private ClipVolumePair GetFootstepSoundFromTerrain(Terrain terrain, Vector3 hitPoint, TextureSounds[] textureSounds)
    {
        // Helper variables for positioning.
        Vector3 terrainPosition = hitPoint - terrain.transform.position;
        Vector3 splatMapPosition = new Vector3(terrainPosition.x / terrain.terrainData.size.x, 0, terrainPosition.z / terrain.terrainData.size.z);

        // Calculate the terrain coordinate data.
        int xPosition = Mathf.FloorToInt(splatMapPosition.x * terrain.terrainData.alphamapWidth);
        int yPosition = Mathf.FloorToInt(splatMapPosition.z * terrain.terrainData.alphamapHeight);
        float[,,] alphaMap = terrain.terrainData.GetAlphamaps(xPosition, yPosition, 1, 1);

        // Execute sound logic based on blending setting.
        if (!blendTerrainSounds)
        {
            // Iterate over the alpha map and find the most active texture.
            int primaryIndex = 0;
            for (int i = 1; i < alphaMap.Length; i++)
            {
                if (alphaMap[0, 0, i] > alphaMap[0, 0, primaryIndex])
                {
                    primaryIndex = i;
                }
            }

            // Check if one of the albedos match the terrains texture.
            foreach (TextureSounds textureSound in textureSounds)
            {
                if (textureSound.albedos.Contains(terrain.terrainData.terrainLayers[primaryIndex].diffuseTexture))
                {
                    // Get and return a unique audio clip.
                    return new ClipVolumePair(textureSound.GetUniqueRandomClip(), textureSound.audioVolume);
                }
            }
        }
        else
        {
            //List<AudioClip> clips = new List<AudioClip>();
            //int clipIndex = 0;
            //for (int i = 0; i < alphaMap.Length; i++)
            //{
            //    if (alphaMap[0, 0, i] > 0)
            //    {
            //        foreach (TextureSounds textureSound in textureSounds)
            //        {
            //            if (textureSound.albedos.Contains(terrain.terrainData.terrainLayers[i].diffuseTexture))
            //            {
            //                AudioClip clip = textureSound.GetRandomClip();
            //                AudioSource.PlayOneShot(clip, alphaMap[0, 0, i]);
            //                clips.Add(clip);
            //                clipIndex++;
            //                break;
            //            }
            //        }
            //    }
            //}

            //float longestClip = clips.Max(clip => clip.length);
            //yield return new WaitForSeconds(longestClip);
        }

        // If no clips are found, return null after debugging a warning.
        Debug.LogWarning("No audio clip could be fetched for terrain: " + terrain.name);
        return new ClipVolumePair(null, 0);
    }

    /// <summary>
    /// Plays an audio clip based on the given renderer.
    /// </summary>
    /// <param name="textureSounds">The collection of texture sounds being checked.</param>
    /// </summary>
    /// <returns>An audio clip based on the ground texture.</returns>
    private ClipVolumePair GetFootstepSoundFromRenderer(Renderer renderer, TextureSounds[] textureSounds)
    {
        // Locally save the active texture on the renderer.
        Texture renderTexture = renderer.material.mainTexture;

        // Iterate over all of the texture sounds.
        foreach (TextureSounds textureSound in textureSounds)
        {
            // Check if one of the albedos match the renderers texture.
            if (textureSound.albedos.Contains(renderTexture))
            {
                // Get and return a unique audio clip.
                return new ClipVolumePair(textureSound.GetUniqueRandomClip(), textureSound.audioVolume);
            }
        }

        // If no clips are found, return null after debugging a warning.
        Debug.LogWarning("No audio clip could be fetched for texture on " + renderer.name);
        return new ClipVolumePair(null, 0);
    }
}
