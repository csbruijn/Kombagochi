using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;

public class Floater : MonoBehaviour
{
    private bool touched = false;
    private float lifeSpawnTotal = 5f;
    private float lifeRemaining;

    private void FixedUpdate()
    {
        if (!touched) return;
        if (lifeRemaining > 0f)
        {
            lifeRemaining -= Time.deltaTime;
        }
        else Destroy(gameObject);

    }

    public void HandleRipplePush()
    {
        if (touched) return;

        CircleCollider2D Col2D = GetComponent<CircleCollider2D>();
        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        Col2D.isTrigger = false;
        rb.gravityScale = 1f;
        rb.drag = .5f;
    }

}
