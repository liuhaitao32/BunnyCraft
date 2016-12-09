using System;
using System.Collections.Generic;
using UnityEngine;

public class Collision {

    private static CollisionInfo help = new CollisionInfo();

	private delegate CollisionInfo CollisionHandler(GeomBase geom1, GeomBase geom2);

	private static CollisionHandler[] handlerMap = new CollisionHandler[ConfigConstant.GEOM_RECT | ConfigConstant.GEOM_CIRCLE | ConfigConstant.GEOM_LINE | ConfigConstant.GEOM_SECTOR];

    public Collision() {
        
    }

	public static void init(){
		handlerMap[ConfigConstant.GEOM_CIRCLE | ConfigConstant.GEOM_CIRCLE] = circleToCircle;
        handlerMap[ConfigConstant.GEOM_CIRCLE | ConfigConstant.GEOM_RECT]   = circleToRect;
        handlerMap[ConfigConstant.GEOM_CIRCLE | ConfigConstant.GEOM_LINE]   = circleToLine;
        handlerMap[ConfigConstant.GEOM_CIRCLE | ConfigConstant.GEOM_SECTOR] = circleToSector;
        handlerMap[ConfigConstant.GEOM_RECT   | ConfigConstant.GEOM_RECT]   = rectToRect;
    }

    private static CollisionInfo circleToRect(GeomBase geom1, GeomBase geom2) {
        GeomCircle circle = (GeomCircle)geom1;
        GeomRect rect = (GeomRect)geom2;

        Vector2D unRotated = circle.center.clone().subtract(rect.center);
        unRotated.angle -= rect.angle;
        unRotated = unRotated.add(rect.center);

        Vector2D closest = Vector2D.createVector();
        if(unRotated.x < rect.x) {
            closest.x = rect.x;
        } else if(unRotated.x > rect.right) {
            closest.x = rect.right;
        } else {
            closest.x = unRotated.x;
        }

        if(unRotated.y < rect.y) {
            closest.y = rect.y;
        } else if(unRotated.y > rect.bottom) {
            closest.y = rect.bottom;
        } else {
            closest.y = unRotated.y;
        }
        help.isHit = unRotated.dist(closest) < circle.radius;
        unRotated.clear();
        closest.clear();
        return help;
    }



    private static CollisionInfo circleToSector(GeomBase geom1, GeomBase geom2) {
        GeomCircle circle = (GeomCircle)geom1;
        GeomSector sector = (GeomSector)geom2;
        GeomLine line1 = sector.line1;
        GeomLine line2 = sector.line2;
        
        //这个因为有需求所以就这么直接写一下了。
        bool result = !( 0 != sector.inRadius && sector.center.dist(circle.center) <= sector.inRadius/* + circle.radius*/) &&//不能在内圆
                    ( ( line1.checkPoint(circle.center) >= 0 && line2.checkPoint(circle.center) <= 0 ) ||//在夹角内
                        circleToLine(circle, line1).isHit || circleToLine(circle, line2).isHit );//或者与线相交
        help.isHit = result;
        return help;
    }

    private static CollisionInfo circleToLine(GeomBase geom1, GeomBase geom2) {
        GeomCircle circle = (GeomCircle)geom1;
        GeomLine line = (GeomLine)geom2;

        Vector2D v1 = circle.center.clone().subtract(line.point1);
        Vector2D v2 = line.vector2D;

        double len = v2.length;
        v2.normalize();
        double u = v1.dotProd(v2);

        Vector2D v0 = Vector2D.createVector();
        if(u <= 0) {
            v0.copy(line.point1);
        } else if(u >= len) {
            v0.copy(line.point2);
        } else {
            v0.copy(v2.multiply(u).add(line.point1));
        }
        help.isHit = v0.dist(circle.center) <= circle.radius;
        v0.clear();
        v1.clear();
        v2.clear();
        return help;
    }

    private static CollisionInfo rectToRect(GeomBase geom1, GeomBase geom2) {
        GeomRect rect1 = (GeomRect)geom1;
        GeomRect rect2 = (GeomRect)geom2;

        Vector2D centerDistanceVertor = rect1.center.clone().subtract(rect2.center);
        Vector2D[] axes = new Vector2D[] {
            rect1.axisX,
            rect1.axisY,
            rect2.axisX,
            rect2.axisY,
        };
        help.isHit = true;
        for(int i = 0, len = axes.Length; i < len; i++) {
            if(rect1.getProjectionRadius(axes[i]) + rect2.getProjectionRadius(axes[i])
                <= Math.Abs(centerDistanceVertor.dotProd(axes[i]))) {
                help.isHit = false;
                break;
            }
        }
        centerDistanceVertor.clear();
        return help;
    }


    private static CollisionInfo circleToCircle(GeomBase geom1, GeomBase geom2) {
        help.dist = geom1.position.dist(geom2.position);
        help.overlap = help.dist - (geom1.radius + geom2.radius);
        help.isHit = help.overlap < 0;
        return help;
    }


    public static CollisionInfo checkCollision(GeomBase geom1, GeomBase geom2, MapData mapData) {
		//把大的放在前面！ 
		if(geom1.type > geom2.type){
            GeomBase temp = geom1;
			geom1 = geom2;
			geom2 = temp;
		}

        if(geom1.applyEntity) {
            geom1.position.copy(geom1.entity.position);
            geom1.angle = geom1.entity.angle;
        }
        if(geom2.applyEntity) {
            geom2.position.copy(geom2.entity.position);
            geom2.angle = geom2.entity.angle;
        }

        CollisionInfo result = realPosition(geom1.position, geom2.position, mapData);
        geom1.position.copy(result.pos1);
        geom2.position.copy(result.pos2);
        geom1.updatePoints();
		geom2.updatePoints();

        int type = geom1.type | geom2.type;
			
		result = handlerMap[1](geom1, geom2);

		//非圆形 就再判断一次！
		if (result.isHit && type != 1) result = handlerMap[type](geom1, geom2);
			
		return result;
	}

    


    public static CollisionInfo realPosition(Vector2D pos1, Vector2D pos2, MapData mapData) {
		help.reset();
		help.pos1.copy (pos1);
		help.pos2.copy (pos2);
		bool rockBack = Math.Abs(pos2.x - pos1.x) >= mapData.widthHalf;

        if(rockBack) {
			if (help.pos1.x > mapData.widthHalf) {
				help.pos1.x -= mapData.width;
			}
			if (help.pos2.x > mapData.widthHalf) {
				help.pos2.x -= mapData.width;
			}
			help.rockBack = rockBack;
        }

        Vector2D v = help.pos2.clone().subtract(help.pos1);
        help.deltaPos.copy(v);
        v.clear();
        return help;
    }
    

    public static GeomBase createGeom(FightEntity entity, object[] datas, object[] offset = null) {
        GeomBase result = null;
        switch(datas.Length) {
            case 1:
                result = new GeomCircle(entity.map);
                break;
            case 2:
                result = new GeomRect(entity.map);
                break;
            case 3:
            case 4:
                result = new GeomSector(entity.map);
                break;
        }
        result.entity = entity;
        result.parseData(datas, offset);
        return result;
    }

}
