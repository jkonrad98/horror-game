using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadBobNoise : MonoBehaviour
{
    private CinemachineVirtualCamera virtualCamera;
    private CinemachineBasicMultiChannelPerlin noise;
    private Rigidbody rb;

    [SerializeField, Range(0, 2f)] private float noiseGain = 1f; 

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
        noise = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }
    void FixedUpdate()
    {
        if (rb.velocity.magnitude > 0.1f)
        {
            noise.m_AmplitudeGain = noiseGain;
            noise.m_FrequencyGain = noiseGain;
        }
        else
        {
            noise.m_AmplitudeGain = Mathf.Lerp(noise.m_AmplitudeGain, 0f, Time.deltaTime * 10f);
        }
    }
}
