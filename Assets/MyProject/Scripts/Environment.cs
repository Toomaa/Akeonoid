using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Environment : MonoBehaviour
{
    [Serializable]
    private struct EnvironmentState
    {
        public float SunIntensity;
        public float SunSize;
        public Color SunColor;
        public float AtmoshpereThickness;
        public Color AmbientColor;
    }

    [SerializeField] private float _darknessDuration;
    [SerializeField] private float _sunScaleSpeed;
    [SerializeField] private Light _sun;
    [SerializeField] private Material _skybox;
    [SerializeField] private EnvironmentState _day;
    [SerializeField] private EnvironmentState _night;

    [Header("Curves")]
    [SerializeField] private AnimationCurve _sunScaleCurve;
    [SerializeField] private AnimationCurve _sunRotateCurve;

    private GameObject _sunCollider;

    private static WaitForEndOfFrame _waitForEndOfFrame = new WaitForEndOfFrame();
    private static WaitForSeconds _waitForOneSecond = new WaitForSeconds(1f);

    private const string SKY_SUN_SIZE = "_SunSize";
    private const string SKY_ATMOSPHERE_THICKNESS = "_AtmosphereThickness";

    public UnityEvent OnDarknessPass;

    private void Start()
    {
        _sunCollider = _sun.transform.GetChild(0).gameObject;
        Revert();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Revert();
        }
    }

    public void EnterTheDarkness()
    {
        StartCoroutine(EnterCoroutine());
    }

    private IEnumerator EnterCoroutine()
    {
        float currentTime = 0;
        float time = 0f;

        // transition to night

        // 1. scale down the sun
        while (time < 1f)
        {
            time += _sunScaleSpeed * Time.deltaTime;
            time = Mathf.Clamp01(time);
            AnimateTheSky(time);
            yield return _waitForEndOfFrame;
        }

        yield return _waitForOneSecond;

        while (currentTime < _darknessDuration)
        {
            currentTime += Time.deltaTime;
            yield return null;
        }

        // transition back to day
        time = 1f;
        while (time > 0)
        {
            time -= _sunScaleSpeed * Time.deltaTime;
            time = Mathf.Clamp01(time);
            AnimateTheSky(time);
            yield return _waitForEndOfFrame;
        }

        OnDarknessPass?.Invoke();

        ActiveteSunCollider();
    }

    public void Revert()
    {
        AnimateTheSky(0f);
        ActiveteSunCollider();
    }

    private void ActiveteSunCollider()
    {
        _sunCollider.gameObject.SetActive(true);
    }

    private void AnimateTheSky(float time)
    {
        float eval = _sunScaleCurve.Evaluate(time);

        // skybox sun
        _skybox.SetFloat(SKY_SUN_SIZE, Mathf.Lerp(_day.SunSize, _night.SunSize, eval));

        // decrease sun intensity
        _sun.intensity = Mathf.Lerp(_day.SunIntensity, _night.SunIntensity, eval);

        // sun color
        _sun.color = Color.Lerp(_day.SunColor, _night.SunColor, eval);

        // atmosphere thickness
        _skybox.SetFloat(SKY_ATMOSPHERE_THICKNESS,
            Mathf.Lerp(_day.AtmoshpereThickness, _night.AtmoshpereThickness, eval));

        // ambient light
        RenderSettings.ambientLight = Color.Lerp(_day.AmbientColor, _night.AmbientColor, eval);
    }

}
