using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillPart : Square
{
	public Sprite singlePillSprite;

	private SpriteRenderer spriteRenderer;

    private PillHolder pillHolder;
    private bool single;
	
	void Start()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
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

    void OnDestroy()
    {
        pillHolder.OnPillPartDestroyed(this);
    }


}
