using System.Collections;
using UnityEngine;

public class MicrobeeSlot : MonoBehaviour
{
    public MicrobeeBehaviour myMicrobee; /*{ get; private set; }*/

    [Header("Placement Curve")]
    [SerializeField] private float travelDuration = 0.8f;

    [SerializeField]
    private AnimationCurve xCurve = new AnimationCurve(
        new Keyframe(0f, 0f, 0f, 2f),   
        new Keyframe(1f, 1f, 0f, 0f)    
    );
    [SerializeField]
    private AnimationCurve yCurve = new AnimationCurve(
        new Keyframe(0f, 0f, 0f, 0f),   
        new Keyframe(1f, 1f, 2f, 0f)    
    );

    public bool IsOccupied() => myMicrobee != null;

    public void SetMicrobee(MicrobeeBehaviour mb)
    {
        myMicrobee = mb;
        StartCoroutine(BringMicrobee());
    }

    private IEnumerator BringMicrobee()
    {
        myMicrobee.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        Vector3 startPos = myMicrobee.transform.position;  
        Vector3 endPos = transform.position;
        float elapsed = 0f;

        while (elapsed < travelDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / travelDuration);
            float x = Mathf.LerpUnclamped(startPos.x, endPos.x, xCurve.Evaluate(t));
            float y = Mathf.LerpUnclamped(startPos.y, endPos.y, yCurve.Evaluate(t));

            myMicrobee.transform.position = new Vector2(x, y);
            yield return null;
        }

        myMicrobee.transform.position = endPos;
        myMicrobee.SetSimulated(false);
    }

    private void Clear(MicrobeeBehaviour mb)
    {
        if (mb == myMicrobee) 
            myMicrobee = null;
    }

    private void OnEnable()
    {
        PointerBehaviour.OnRemoval += Clear; 
    }

    private void OnDisable()
    {
        PointerBehaviour.OnRemoval -= Clear;
    }
}