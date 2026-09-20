using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Cinemachine;
using UnityEngine;

public enum CameraType
{
    None,
    Normal,
    MainWeaponAim,
    SubWeaponAim
}

[Serializable]
public class CameraBinding
{
    public CameraType type;
    public CinemachineCamera camera;
}

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;

    [Header("Camera")]
    [SerializeField] Transform cameraTr;
    [SerializeField] private List<CameraBinding> cameraBindings;

    [Header("Priority")]
    [SerializeField] private int activePriority = 20;
    [SerializeField] private int inactivePriority = 0;

    public Transform CameraTr => cameraTr;

    private Dictionary<CameraType, CinemachineCamera> cameraDict;
    private CameraType currentType = CameraType.None;

    void Awake()
    {
        Instance = this;
        transform.SetParent(null);
        DontDestroyOnLoad(gameObject);

        cameraDict = cameraBindings.ToDictionary(x => x.type, x => x.camera);
    }

    void Start()
    {
        SwitchCamera(CameraType.Normal);
    }

    public void SwitchCamera(CameraType type)
    {
        if (currentType == type)
        {
            return;
        }

        // 이전 카메라
        if (cameraDict.TryGetValue(currentType, out var prevCam))
        {
            prevCam.Priority = inactivePriority;
        }

        // 새로운 카메라
        if (cameraDict.TryGetValue(type, out var newCam))
        {
            newCam.Priority = activePriority;
            currentType = type;
        }
    }
}
