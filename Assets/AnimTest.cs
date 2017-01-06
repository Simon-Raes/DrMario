using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimTest : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

        Animation anim = GetComponent<Animation>();
        AnimationCurve curve;

        // create a new AnimationClip
        AnimationClip clip = new AnimationClip();
        clip.legacy = true;

        // create a curve to move the GameObject and assign to the clip
        Keyframe[] xMovementKeys;
        xMovementKeys = new Keyframe[3];
        xMovementKeys[0] = new Keyframe(0.0f, 4f);
        xMovementKeys[1] = new Keyframe(1.0f, 2f);
        xMovementKeys[2] = new Keyframe(2.0f, 0.0f);
        curve =  AnimationCurve.Linear(0f, 0f, 2f, 2f);
        clip.SetCurve("", typeof(Transform), "localPosition.x", curve);

        // update the clip to a change the red color
        // curve = AnimationCurve.Linear(0.0f, 1.0f, 2.0f, 0.0f);
        // clip.SetCurve("", typeof(Material), "_Color.r", curve);

        // now animate the GameObject    
        anim.AddClip(clip, clip.name);
        anim.Play(clip.name);

    }

    // Update is called once per frame
    void Update()
    {

    }
}
