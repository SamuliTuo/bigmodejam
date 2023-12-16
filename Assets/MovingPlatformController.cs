using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformController : MonoBehaviour
{
    public Vector3 dir = Vector3.up;
    public float dist = 1;
    public float stopTime = 1;
    public float speed = 1;

    private float t = 0;
    private Vector3 startPos, endPos;
    private enum platformStates
    {
        up, goDown, down, goUp
    }
    private platformStates state = platformStates.down;

    private void Start()
    {
        startPos = transform.position;
        endPos = transform.position + dir * dist;
    }

    private void FixedUpdate()
    {

        switch (state)
        {
            case platformStates.goUp:
                if (t < 1)
                {
                    float perc = t * t * (3f - 2f * t);
                    transform.position = Vector3.Lerp(startPos, endPos, perc);
                    t += Time.fixedDeltaTime * speed;
                }
                else
                {
                    t = 0;
                    transform.position = endPos;
                    state = platformStates.up;
                }
                break;
            case platformStates.up:
                if (t < stopTime)
                {
                    t += Time.fixedDeltaTime;
                }
                else
                {
                    t = 0;
                    state = platformStates.goDown;
                }
                break;
            case platformStates.goDown:
                if (t < 1)
                {
                    float perc = t * t * (3f - 2f * t);
                    transform.position = Vector3.Lerp(endPos, startPos, perc);
                    t += Time.fixedDeltaTime * speed;
                }
                else
                {
                    t = 0;
                    transform.position = startPos;
                    state = platformStates.down;
                }
                break;
            case platformStates.down:
                if (t < stopTime)
                {
                    t += Time.fixedDeltaTime;
                }
                else
                {
                    t = 0;
                    state = platformStates.goUp;
                }
                break;
            default:
                break;
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.SetParent(transform);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.SetParent(null);
        }
    }
}
