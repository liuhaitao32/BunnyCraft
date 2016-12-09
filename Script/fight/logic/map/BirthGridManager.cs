using System;
using System.Collections.Generic;
using UnityEngine;

public class BirthGridManager:GridManagerBase {

    ///一个二维的快表 用于查找当前抛出去障碍物之后剩下的格子。一维是row 二维是col。
    public List<List<Grid>> birthPlayerGrids = new List<List<Grid>>();
    ///另一个快表。障碍物的半径-1. 用于豆生成。
    public List<List<Grid>> birthBeanGrids = new List<List<Grid>>();

	public BirthGridManager(Map map):base(map) {

	}

    public Grid getRandomBirthGrid(List<Vector2D> range, List<int> exclude, bool isBean = false) {
        List<List<Grid>> gridsList = isBean ? this.birthBeanGrids : this.birthPlayerGrids;
        List<Grid> emptys = new List<Grid>();
        for(int i = 0, len = range.Count; i < len; i++) {
            int yMin = (int)range[i].x;
            int yMax = (int)range[i].y;
            for(int j = yMin; j < yMax; j++) {
                List<Grid> grids = gridsList[j];
                for(int k = 0, len2 = grids.Count; k < len2; k++) {
                    Grid grid = grids[k];
                    //判断当前格子里是否有此type  都没有type的话 添加到空列表里。
                    bool exists = exclude.Exists((int type) => { return grid.hasBirthEntity(type); });
                    if(!exists) {
                        emptys.Add(grid);
                    }
                }
            }
        }
        return emptys.Count > 0 ? emptys[(int)( this.map.getRandom() * emptys.Count )] : null;
    }

    protected override void createGrid() {
        this.colNum = this.map.mapData.colNum;
        this.rowNum = this.map.mapData.rowNum;
        this.size = ConfigConstant.MAP_GRID_SIZE;
        base.createGrid();
        for(int i = 0; i < this.rowNum; i++) {
            this.birthPlayerGrids.Add(new List<Grid>());
            this.birthBeanGrids.Add(new List<Grid>());
            for(int j = 0; j < this.colNum; j++) {
                Grid grid = this.grids[j][i];
                this.birthPlayerGrids[i].Add(grid);
                this.birthBeanGrids[i].Add(grid);
            }
        }
    }

    public void createBirthGrids() {
        for(int j = 0; j < this.rowNum; j++) {
            for(int i = this.colNum - 1; i >= 0; i--) {
                Grid grid = this.birthPlayerGrids[j][i];
                //倒叙移除掉有障碍物的格子。 因为障碍物从此就不用了！
                if(grid.hasBirthEntity(ConfigConstant.ENTITY_BARRIER)) {
                    this.birthPlayerGrids[j].RemoveAt(i);
                }
            }
        }
        //重新生成小的障碍物。 给item用！
        for(int i = 0, len = this.map.others.Count; i < len; i++) {
            BarrierEntity entity = this.map.others[i] as BarrierEntity;
            if(null != entity) entity.generateBirthGrids(-1);
        }

        for(int j = 0; j < this.rowNum; j++) {
            for(int i = this.colNum - 1; i >= 0; i--) {
                Grid grid = this.birthBeanGrids[j][i];
                //倒叙移除掉有障碍物的格子。 因为障碍物从此就不用了！
                if(grid.hasBirthEntity(ConfigConstant.ENTITY_BARRIER)) {
                    this.birthBeanGrids[j].RemoveAt(i);
                }
            }
        }
    }

}

