using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class CameraBounds : MonoBehaviour
{
    private float _height;
    private float _width;
    private int _lastScreenWidth;
    private int _lastScreenHeight;

    private Rigidbody2D[] _bodies;
    private float _refreshTimer;
    private const float BodyRefreshInterval = 2f;

    [SerializeField] private float wrapCooldown = 1f;
    private readonly Dictionary<Rigidbody2D, float> _cooldowns = new();

    private void Start()
    {
        UpdateBounds();
        RefreshBodies();
        _lastScreenWidth = Screen.width;
        _lastScreenHeight = Screen.height;
    }

    private void Update()
    {
        if (Screen.width != _lastScreenWidth || Screen.height != _lastScreenHeight)
        {
            UpdateBounds();
            _lastScreenWidth = Screen.width;
            _lastScreenHeight = Screen.height;
        }

        _refreshTimer += Time.deltaTime;
        if (_refreshTimer >= BodyRefreshInterval)
        {
            RefreshBodies();
            _refreshTimer = 0f;
        }
    }

    private void FixedUpdate()
    {
        TickCooldowns();
        WrapBodies();
    }

    private void UpdateBounds()
    {
        Camera cam = Camera.main;
        _height = cam.orthographicSize;
        _width = _height * cam.aspect;
    }

    private void RefreshBodies()
    {
        _bodies = FindObjectsByType<Rigidbody2D>(FindObjectsSortMode.None);

        // Clean up any destroyed bodies from the cooldown table
        var keys = new List<Rigidbody2D>(_cooldowns.Keys);
        foreach (var key in keys)
            if (key == null) _cooldowns.Remove(key);
    }

    private void TickCooldowns()
    {
        var keys = new List<Rigidbody2D>(_cooldowns.Keys);
        foreach (var key in keys)
        {
            if (key == null) { _cooldowns.Remove(key); continue; }
            _cooldowns[key] -= Time.fixedDeltaTime;
            if (_cooldowns[key] <= 0f) _cooldowns.Remove(key);
        }
    }

    private void WrapBodies()
    {
        foreach (Rigidbody2D rb in _bodies)
        {
            if (rb == null) continue;
            if (_cooldowns.ContainsKey(rb)) continue; // still on cooldown

            Vector2 pos = rb.position;
            bool wrapped = false;

            if (pos.x > _width)
            {
                pos.x = -_width + (pos.x - _width);
                wrapped = true;
            }
            else if (pos.x < -_width)
            {
                pos.x = _width + (pos.x + _width);
                wrapped = true;
            }

            if (pos.y > _height)
            {
                pos.y = -_height + (pos.y - _height);
                wrapped = true;
            }
            else if (pos.y < -_height)
            {
                pos.y = _height + (pos.y + _height);
                wrapped = true;
            }

            if (wrapped)
            {
                Vector2 vel = rb.velocity;
                rb.position = pos;
                rb.velocity = vel;
                _cooldowns[rb] = wrapCooldown;
            }
        }
    }
}