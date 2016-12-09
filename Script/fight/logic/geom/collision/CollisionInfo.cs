using System;
using System.Collections.Generic;

public class CollisionInfo {

    public bool rockBack;

    public bool isHit;
	public Vector2D pos1 = Vector2D.createVector();
	public Vector2D pos2 = Vector2D.createVector();

	public Vector2D deltaPos = Vector2D.createVector();

    public double overlap = 0;
    public double dist;

    public CollisionInfo() {
        
    }

	public void reset() {
        this.rockBack = this.isHit = false;
        this.overlap = 0;
    }
}
