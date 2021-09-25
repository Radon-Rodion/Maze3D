using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
	public GameObject border;
	public GameObject exit;
	
	public int length, width, height;
	public int borderFrequency = 1;
	public int nHolesInLine = 3;
	int[,,] cellsStats;
	int exitX, exitZ;
	private static Quaternion[] cellBordersRotations = {Quaternion.Euler(90,0,0), Quaternion.Euler(0,0,0), Quaternion.Euler(0,90,0), Quaternion.Euler(0,0,0), Quaternion.Euler(0,90,0), Quaternion.Euler(90,0,0)};
	private static int[] borders = {1, 2, 4, 8, 16, 32};
    
    void Start()
    {
		borderFrequency++; //cause lies in [1, +inf) and norm value is 2
		cellsStats = new int[length,width,height];
        generateMaze(length, width, height);
		//placeBorders(0,0,0,4);
		drawMaze(cellsStats);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
	
	void generateMaze(int length, int width, int height){ //Euler's algorythm
	
		for(int k = 0; k < height; k++){
			
			int[] curMazeString = new int[length];
			int curSet;
			for(int i=0; i<length; i++){ //step 0 - initializing first line
				curMazeString[i] = i+1;
				cellsStats[i,0,k] |= borders[3]; //2^3 - back border
			}
			int maxSetN = length;
			setTopAndBotBorders(length, width, k, k!=height-1);
			if(k!=0) createHolesInBotBorder(length, width, k, nHolesInLine);
			
			for(int j = 0; j < width-1; j++){ //for each line except the last
				cellsStats[0,j,k] |= borders[4]; // 2^4 - left border
				cellsStats[length-1,j,k] |= borders[2]; // 2^2 - right border
			
				for(int i=0; i<length-1;i++){ //step 1 - right borders
					if(curMazeString[i] == curMazeString[i+1]){ //in the same set
						cellsStats[i,j,k] |= borders[2];  //2^2 - right border
					} else {
						int borderRight = Random.Range(0,borderFrequency);
						if(borderRight == 0){ //not creating border
							curMazeString[i+1] = curMazeString[i];
						} else { //creating border
							cellsStats[i,j,k] |= borders[2];
						}
					}
				}
				
				curSet = curMazeString[0];//step 2 - front borders
				
				for(int i=1; i<length; i++){//front borders
					if(curSet != curMazeString[i]){ //cells set № changing
						curSet = curMazeString[i];
					} else {
						int borderFront = Random.Range(0,borderFrequency);
						if(borderFront != 0)
							cellsStats[i,j,k] |= borders[1];
					}
				}
				if(curMazeString[0] == curMazeString[1]){
					int borderFront = Random.Range(0,2);
					if(borderFront == 1){
						cellsStats[0,j,k] |= borders[1];  //placing front border to avoid long left corridor
						cellsStats[1,j,k] &= 63-borders[1]; //and removing front border in the second cell to avoid isolation
					}
				}
				
				if(j != width-2){ //if the line isn't prelast
					for(int i=0; i<length; i++){//step 3 - preparations for the next string
						if((cellsStats[i,j,k] & borders[1]) == borders[1]){ //if front border exists removing cell from its set
							curMazeString[i] = maxSetN + i;
						}
					}
					maxSetN += length;
				}
				
			}
			
			
			for(int i=0; i<length-1; i++){
				if(curMazeString[i] == curMazeString[i+1] && (cellsStats[i,width-2,k]&borders[2]) == borders[2]){ //if cells in the same sets then saving right border
					cellsStats[i,width-1,k] |= borders[2];
				}
				cellsStats[i,width-1,k] |= borders[1];
			}
			cellsStats[0,width-1,k] |= borders[4]; // 2^4 - left border
			cellsStats[length-1,width-1,k] |= borders[2]; // 2^2 - right border
			cellsStats[length-1,width-1,k] |=borders[1];// 2^1 - front border
		}
		
		//creating exit
		exitX = Random.Range(0, length);
		exitZ = Random.Range(0, width);
		cellsStats[exitX, exitZ, height-1] &= 63 - borders[0]; //removing front top right border to place exit here
	}
	
	void setTopAndBotBorders(int length, int width, int k, bool onlyBot){
		for(int j=0; j<width; j++)
			for(int i=0; i<length; i++){
				if(!onlyBot) cellsStats[i,j,k] |= borders[0]; //2^0 - top border
				cellsStats[i,j,k] |= borders[5];//2^5 - bot border
			}
	}
	
	void createHolesInBotBorder(int length, int width, int k, int nHolesInLine){
		for(int j = 0; j<nHolesInLine; j++)
			for(int i = 0; i<nHolesInLine; i++)
				cellsStats[Random.Range(0,length),Random.Range(0,width),k] &= 63-borders[5];
	}
	
	void drawMaze(int[,,] cellsStats){
		for(int k=0; k<height; k++)
			for(int j=0; j<width; j++)
				for(int i=0; i<length; i++)
					placeBorders(i*3,k*3,j*3,cellsStats[i,j,k]);
		Instantiate(exit, new Vector3((exitX)*3, height*3-1.5f, (exitZ)*3), exit.transform.rotation);
	}
	
	void placeBorders(int x, int y, int z, int cellStats){
		int borderSide = 1; //2^0
		for(int i=0; borderSide < 64; i++){ // 64 = 2^6, 6 - number of sides
			if((cellStats & borderSide) > 0)
				Instantiate(border, defineVector(x,y,z,i), cellBordersRotations[i]);
			borderSide <<= 1; 
		}
	}
	
	Vector3 defineVector(float x, float y, float z, int i){
		float ad = 1.5f;
		if(i > 2) ad *= -1;
		if(i % 2 == 0 && i != 0) x += ad;
		if(i % 5 == 0) y += ad;
		if(i % 5 % 2 == 1) z += ad;
		
		return new Vector3(x,y,z);
	}
}
