using System;
using System.Collections.Generic;
using UnityEngine;

public class FindEntityManager:GridManagerBase {

    public List<Grid> helpGrids = new List<Grid>();

    private List<FightEntity> helpEntity = new List<FightEntity>();


    public FindEntityManager(Map map):base(map) {
        
	}

    protected override void createGrid() {
        this.colNum = this.map.mapData.colNum / 2;
        this.size = this.map.mapData.width / colNum;
        this.rowNum = Mathf.CeilToInt(1f * this.map.mapData.height / this.size);
        base.createGrid();
        Debug.Log("创建格子" + this.size);
    }

    public void createGrid2() {
        this.colNum = int.Parse(TestValue.value);
        this.size = this.map.mapData.width / colNum;
        this.rowNum = Mathf.CeilToInt(1f * this.map.mapData.height / this.size);
        base.createGrid();

        for(int i = 0, len = this.map.players.Count; i < len; i++) {
            this.map.players[i]._oldPosition.setXY(-1, -1);
            this.map.players[i].position = this.map.players[i].position;
        }

        for(int i = 0, len = this.map.beans.Count; i < len; i++) {
            this.map.beans[i]._oldPosition.setXY(-1, -1);
            this.map.beans[i].position = this.map.beans[i].position;
        }

        for(int i = 0, len = this.map.bullets.Count; i < len; i++) {
            this.map.bullets[i]._oldPosition.setXY(-1, -1);
            this.map.bullets[i].position = this.map.bullets[i].position;
        }

        for(int i = 0, len = this.map.others.Count; i < len; i++) {
            this.map.others[i]._oldPosition.setXY(-1, -1);
            this.map.others[i].position = this.map.others[i].position;
        }
    }


    public List<Grid> getGrids(GeomBase geom, List<Grid> grids = null) {
        if(null == grids) {
            this.helpGrids.Clear();
            grids = this.helpGrids;
        }
        if(geom.radius <= 0) return grids;
        if(geom.applyEntity) {
            geom.position.copy(geom.entity.position);
            geom.angle = geom.entity.angle;
        }
        geom.updatePoints();
        Vector2D positon = geom.position;
        int radius = geom.radius;
        int startX = (int)( ( positon.x - radius ) / this.size );
        int endX = (int)( ( positon.x + radius ) / this.size );
        int startY = (int)Math.Max(0, ( positon.y - radius ) / this.size);
        int endY = (int)Math.Min(( positon.y + radius ) / this.size, this.rowNum - 1);

        for(int i = startX; i <= endX; i++) {
            for(int j = startY; j <= endY; j++) {
                Grid grid = this.getGrid(i, j);
                grids.Add(grid);
            }
        }
        return grids;
    }

    ///取得其他玩家，Biggo添加
	public List<PlayerEntity> findPlayer(GeomBase geom, int aim = 1, int sort = 0, bool checkCollision = true) {
        Func<FightEntity, FightEntity, bool> filter = this.getAimFilter(aim);
        List<int> types = new List<int> { ConfigConstant.ENTITY_PLAYER };
        List<FightEntity> players = this.getFightEntitysByRange(geom, types, filter, sort, checkCollision);
        return players.ConvertAll<PlayerEntity>((e) => { return e as PlayerEntity; });
    }

    private Func<FightEntity, FightEntity, bool> getAimFilter(int aim) {
        switch(aim) {
            case -1:
                return null;
            case 0:
                return (Func<FightEntity, FightEntity, bool>)FindEntityManager.getAllyTeamFilter;
            case 1:
                return (Func<FightEntity, FightEntity, bool>)FindEntityManager.getEnemyTeamFilter;
            case 2:
                return (Func<FightEntity, FightEntity, bool>)FindEntityManager.getSelfFilter;
        }
        return null;
    }

    public List<PersonEntity> getPerson(GeomBase geom, int aim = 1, int sort = 0, bool call = true, bool checkCollision = true) {
        //Biggo修改
        Func<FightEntity, FightEntity, bool> filter = this.getAimFilter(aim);
        List<int> types = call ? new List<int> { ConfigConstant.ENTITY_PLAYER, ConfigConstant.ENTITY_CALL } : new List<int> { ConfigConstant.ENTITY_PLAYER };
        List<FightEntity> persons = this.getFightEntitysByRange(geom, types, filter, sort, checkCollision);
        //把FightEntity类型 转化成personEntity类型。
        return persons.ConvertAll<PersonEntity>((e) => { return e as PersonEntity; });
    }
    
    
    /// 这个里面的entitys 会被复用的 所以 取出来人物的时候 如果 里面还要调用此方法的话，请使用副本。
    public List<FightEntity> getFightEntitysByRange(GeomBase shape, List<int> types, Func<FightEntity, FightEntity, bool> filter = null, int sort = 0, bool checkCollision = true) {
        List<FightEntity> entitys = this.getFightEntitysByGrids(this.getGrids(shape), types);
        for(int i = entitys.Count - 1; i >= 0; i--) {
            //直接过滤就不用检测碰撞了！ 
            //Biggo修改
            if(( filter != null && !filter(shape.entity, entitys[i]) )) {
                entitys.RemoveAt(i);
            } else if(checkCollision){
                CollisionInfo info = Collision.checkCollision(shape, entitys[i].shape, this.map.mapData);
                if(info.isHit) {
                    entitys[i].findDist = info.dist;
                    entitys[i].findDelta.copy(info.deltaPos);
                } else {
                    entitys.RemoveAt(i);
                }

            }
        }
        switch(sort) {
            case 0:
                entitys.Sort((e1, e2) => {
                    double dist1 = e1.findDist;
                    double dist2 = e2.findDist;
                    //这个是否要传递一个deltaPos 还是要传递一个距离？ 还是两个都要传。
                    e1.findDist = dist1;
                    e2.findDist = dist2;
                    if(dist1 - dist2 < 0) {
                        return -1;
                    } else if(dist1 - dist2 > 0) {
                        return 1;
                    } else {
                        return e1.netId - e2.netId;
                    }
                });
                break;
        }

        return entitys;
    }

    public List<FightEntity> getFightEntitysByGrids(List<Grid> grids, List<int> types) {
        this.helpEntity.Clear();
        List<FightEntity> result = this.helpEntity;
        for(int i = 0, len = grids.Count; i < len; i++) {
            for(int j = 0, len2 = types.Count; j < len2; j++) {
                int type = types[j];
                List<FightEntity> entitys = grids[i].getEntity(type);
                for(int k = 0, len3 = entitys.Count; k < len3; k++) {
                    FightEntity entity = entitys[k];
                    if(entity.alived && !result.Contains(entity)) result.Add(entity);
                }
            }
        }
        return result;
    }

    /// 过滤我方排除自己 Biggo增加 
    public static bool noCheckCollision(FightEntity entity1, FightEntity entity2) {
        return true;
    }

    /// 过滤我方排除自己 Biggo增加 
    public static bool getPureAllyTeamFilter(FightEntity entity1, FightEntity entity2) {
        return entity1 != entity2 && entity1.ownerPlayer.teamIndex == entity2.ownerPlayer.teamIndex;
    }
    /// 过滤我方 Biggo增加 
    public static bool getAllyTeamFilter(FightEntity entity1, FightEntity entity2) {
        return entity1.ownerPlayer.teamIndex == entity2.ownerPlayer.teamIndex;
    }
    /// 过滤敌方 Biggo增加 
    public static bool getEnemyTeamFilter(FightEntity entity1, FightEntity entity2) {
        return entity1.ownerPlayer.teamIndex != entity2.ownerPlayer.teamIndex;
    }

    /// 过滤敌方 Biggo增加 
	public static bool getSelfFilter(FightEntity entity1, FightEntity entity2) {
        return entity1 == entity2;
    }

}

