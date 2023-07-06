using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

[Serializable]
public class AssetReferenceAudioClip : AssetReferenceT<AudioClip>
{
    public AssetReferenceAudioClip(string guid) : base(guid){}
}

public class AddressableManager : MonoBehaviour
{
    [SerializeField] private AssetReference playerArmatureAssetReference;
    [SerializeField] private AssetReference musicAssetReference;
    [SerializeField] private AssetReferenceTexture2D unityLogoAssetReference;
    [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;

    // UI Components
    [SerializeField] private RawImage rawImageUnityLogo;

    private GameObject playerController;
    
    void Start()
    {
        // Logger.Instance.LogInfo("Initializing Addressable...");
        Debug.Log("Initializing Addressable...");
        Addressables.InitializeAsync().Completed += AddressableManager_Completed;
    }

    private void AddressableManager_Completed(AsyncOperationHandle<IResourceLocator> obj)
    {
        // Logger.Instance.LogInfo("Initialized Addressable...");
        Debug.Log("Initialized Addressable...");

        playerArmatureAssetReference.InstantiateAsync().Completed += (go) =>
        {
            playerController = go.Result;
            cinemachineVirtualCamera.Follow = playerController.transform.Find("PlayerCameraRoot");
            
            // Logger.Instance.LogInfo("Instantiated the player controller...");
            Debug.Log("Instantiated the player controller...");
        };

        musicAssetReference.LoadAssetAsync<AudioClip>().Completed += (clip) =>
        {
            var audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = clip.Result;
            audioSource.playOnAwake = false;
            audioSource.loop = true;
            audioSource.Play();
            
            // Logger.Instance.LogInfo("Loaded the audio clip...");
            Debug.Log("Loaded the audio clip...");
        };

        unityLogoAssetReference.LoadAssetAsync<Texture2D>();
        
        // Logger.Instance.LogInfo("Loaded Assets...");
        Debug.Log("Loaded Assets...");
    }

    void Update()
    {
        if (unityLogoAssetReference.Asset != null && rawImageUnityLogo.texture == null)
        {
            rawImageUnityLogo.texture = unityLogoAssetReference.Asset as Texture2D;
            Color currentColor = rawImageUnityLogo.color;
            currentColor.a = 1.0f;
            rawImageUnityLogo.color = currentColor;
            
            // Logger.Instance.LogInfo("Logo loaded as an asset and associated with raw image...");
            Debug.Log("Logo loaded as an asset and associated with raw image...");
        }
    }

    // private void OnDestroy()
    // {
    //     playerArmatureAssetReference.ReleaseInstance(playerController);
    //     unityLogoAssetReference.ReleaseAsset();
    // }
}
