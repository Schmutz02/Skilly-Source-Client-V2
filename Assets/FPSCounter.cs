using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FPSCounter : MonoBehaviour
{
    private TextMeshProUGUI _text;

    private float deltaTime;
    
    private void Awake()
    {
        _text = GetComponent<TextMeshProUGUI>();
    }

    void Update () {
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        var fps = 1.0f / deltaTime;
        _text.text = Mathf.Ceil(fps).ToString();
    }
}
