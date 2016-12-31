using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillPart : Square
{
    private PillHolder pillHolder;
    private bool single;

    public void SetPillHolder(PillHolder pillHolder)
    {
        this.pillHolder = pillHolder;
    }

    public void SetSingle()
    {
        single = true;
        // TODO update the sprite to a single pill instead of pill-half
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
