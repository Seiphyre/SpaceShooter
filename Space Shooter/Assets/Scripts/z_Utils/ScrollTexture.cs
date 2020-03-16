using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollTexture : MonoBehaviour
{
    [SerializeField]
    private float _speed = 1f;

    private Renderer _renderer;

    private void Awake()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        _renderer = GetComponent<Renderer>();

        if (_renderer == null)
            Debug.LogWarning("[ScrollTexture] render NULL.");
    }

    // Update is called once per frame
    void Update()
    {
        if (_renderer != null && _renderer.material != null)
            _renderer.material.mainTextureOffset += new Vector2(Time.deltaTime * _speed, 0f);
    }
}
