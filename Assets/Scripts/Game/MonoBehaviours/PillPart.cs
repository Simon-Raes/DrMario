using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// One half of a pill, either controllable or placed on the grid
public class PillPart : Square
{
    public Sprite singlePillSprite;

    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private PillHolder pillHolder;
    private bool single;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    public void SetPillHolder(PillHolder pillHolder)
    {
        this.pillHolder = pillHolder;
    }

    public void SetSingle()
    {
        single = true;

        spriteRenderer.sprite = singlePillSprite;
    }

    public bool IsSingle()
    {
        return single;
    }

    public PillPart GetCounterPart()
    {
        return pillHolder.GetCounterPart(this);
    }

    // todo figure out when to play
    public void PlayDeathAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger("Kill");
        }
    }

    void OnDestroy()
    {
        pillHolder.OnPillPartDestroyed(this);
    }


}
