using System;
using System.Collections.Generic;

public class Grid {
	
	public List<List<FightEntity>> entityMap = new List<List<FightEntity>> (7);

    public int[] birthEntityMap = new int[7];

    public int row;

	public int col;

    public int size;

	public Map map;

	public Grid (int col, int row, int size, Map map) {
        this.col = col;
        this.row = row;
		this.map = map;
        this.size = size;
		for (int i = 0, len = birthEntityMap.Length; i < len; i++) {
			this.entityMap.Add (new List<FightEntity> ());
            this.birthEntityMap[i] = 0;
        }
	}

	public void addEntity(FightEntity entity){
		List<FightEntity> list = this.getEntity (entity.type);
		//if (!list.Contains (entity)) {
			list.Add (entity);
		//}
	}

    public void changeBirthEntity(FightEntity entity, int value) {
        this.birthEntityMap[entity.type - 500] += value;
    }

    public void removeEntity(FightEntity entity){
		List<FightEntity> list = this.getEntity(entity.type);
		list.Remove (entity);
	}
    
    public List<FightEntity> getEntity(int type){
        //entity id 是已500开始的。
		return this.entityMap[type - 500];
	}
    

    public bool hasEntity(int type) {
        return this.getEntity(type).Count > 0;
    }

    public bool hasBirthEntity(int type) {
        return this.birthEntityMap[type - 500] > 0;
    }

    public Vector2D center {
        get { return Vector2D.createVector(( this.col + 0.5f ) * this.size, ( this.row + 0.5f ) * this.size); }
    }

    public Vector2D randomPosition {
        get {
            Vector2D result = this.center;
            Vector2D random = Vector2D.createVector2(this.map.getRandomRange(0, this.size / 2), this.map.getRandomRange(0, Math2.PI2));
            result.add(random);
            random.clear();
            return result;
        }        
    }

    private static Dictionary<string, List<List<int>>> gridArea = new Dictionary<string, List<List<int>>>();

    public static List<Grid> getAreaGrids(Grid center, int gridRadius, Map map, List<List<int>> areaLines) {
        string key = gridRadius.ToString();
        if(!gridArea.ContainsKey(key)) {
            List<List<int>> gridLists = new List<List<int>>();
            gridLists.Add(new List<int>{ 0, 0 });
            int i, j;
            for(i = 1; i <= gridRadius; i++) {
                for(j = 0; j <= i; j++) {
                    //判断一次距离，将可能的4或8个镜像位置都加入到列表
                    if(i * i + j * j <= gridRadius * gridRadius + gridRadius) {
                        if(j == 0) {
                            //仅有4个复制
                            gridLists.Add(new List<int> { i, 0 });
                            gridLists.Add(new List<int> { -i, 0 });
                            gridLists.Add(new List<int> { 0, i });
                            gridLists.Add(new List<int> { 0, -i });
                        } else if(i == j) {
                            //仅有4个复制
                            gridLists.Add(new List<int> { i, i });
                            gridLists.Add(new List<int> { -i, i });
                            gridLists.Add(new List<int> { i, -i });
                            gridLists.Add(new List<int> { -i, -i });
                        } else {
                            //8个复制
                            gridLists.Add(new List<int> { i, j });
                            gridLists.Add(new List<int> { i, -j });
                            gridLists.Add(new List<int> { -i, j });
                            gridLists.Add(new List<int> { -i, -j });
                            gridLists.Add(new List<int> { j, i });
                            gridLists.Add(new List<int> { j, -i });
                            gridLists.Add(new List<int> { -j, i });
                            gridLists.Add(new List<int> { -j, -i });
                        }
                    }
                }
            }
            gridArea[key] = gridLists;

        }

        List<Grid> result = new List<Grid>();
        List<List<int>> grids = gridArea[key];

        for(int i = 0, len = grids.Count; i < len; i++) {
            int row = grids[i][1];
            int col = grids[i][0];
            if(areaLines.Exists((List<int> range) => {//有一个是true 就返回true
                return center.row + row >= range[0] && center.row + row <= range[1];
            })){
                result.Add(map.birthGrids.getGrid(center.col + col, center.row + row));
            }
        }
        return result;
    }
}

