using System;
using System.Collections.Generic;
using UnityEngine;

public class GridManagerBase {

    protected List<List<Grid>> grids = new List<List<Grid>>();    

    public Map map;

    protected int colNum;
    protected int rowNum;
    protected int size;

    public GridManagerBase(Map map) {
		this.map = map;
        this.createGrid();
	}

    public Grid getGrid(int col, int row) {
        while(col < 0) col += this.colNum;
        col = col % this.colNum;
        try {
            var cc = this.grids[col][row];
        } catch(Exception e) {
            throw new Exception("没有格子" + col + "  " + row);
        }

        return this.grids[col][row];
    }
    

    protected virtual void createGrid() {
        for(int i = 0; i < this.colNum; i++) {
            this.grids.Add(new List<Grid>());
            for(int j = 0; j < this.rowNum; j++) {
                this.grids[i].Add(new Grid(i, j, this.size, this.map));
            }
        }
    }
    

    public void printGrid() {
        string str = "";
        for(int i = 0, len = this.rowNum; i < len; i++) {
            for(int j = 0, len2 = this.colNum; j < len2; j++) {
                Grid grid = this.getGrid(j, i);
                //str += grid.hasBirthEntity(ConfigConstant.ENTITY_LOOP_BEAN) ? "0" : "X";
                //str += grid.hasBirthEntity(ConfigConstant.ENTITY_PLAYER) ? "1" : "X";
                //str += grid.hasBirthEntity(ConfigConstant.ENTITY_BARRIER) ? "5" : "X";
                str += grid + "|";
            }
            str += "\n";
        }
        Debug.Log(str);
    }

}

