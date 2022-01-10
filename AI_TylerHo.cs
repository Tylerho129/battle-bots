using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AI_TylerHo: MonoBehaviour {

    public CharacterScript mainScript;

    public float[] bombSpeeds;
    public float[] buttonCooldowns;
    public float playerSpeed;
    public int[] beltDirections;
    public float[] buttonLocations;
	public float[]  bombPoints;
	

	public float ourL;
	public float enemyL;


	// Use this for initialization
	void Start () {
        mainScript = GetComponent<CharacterScript>();

        if (mainScript == null)
        {
            print("No CharacterScript found on " + gameObject.name);
            this.enabled = false;
        }

        buttonLocations = mainScript.getButtonLocations();

        playerSpeed = mainScript.getPlayerSpeed();


	}

	// Update is called once per frame
	void Update() {

		//initialize the functions into variables
		buttonCooldowns = mainScript.getButtonCooldowns();
		beltDirections = mainScript.getBeltDirections();
		bombSpeeds = mainScript.getBombSpeeds();
		bombPoints = mainScript.getBombDistances();
		ourL = mainScript.getCharacterLocation();
		bombSpeeds = mainScript.getBombSpeeds();
		int target = 0;
		float goal = 0;
		int tick = 0;
		List<int> hit = new List<int>();
		List<float> dist = new List<float>();

		//checks for bombs
		checkforBomb(hit);

		for(int i = 0; i < beltDirections.Length;  i++){
			dist.Add(Mathf.Abs(buttonLocations[i] - ourL));
        }
		target = findMin(hit, dist);
		tick = estimated(tick);
		int prioty = -1;
		prioty = lowerEvil(prioty, tick, hit);
		if(prioty != -1){

			target = which(target, prioty);
		}

		goal = buttonLocations[target];
		move(goal);
		attack(target, tick);
		


	}


	//moves our character
	void move(float location){
		if (location < ourL){
            mainScript.moveDown();    
        }else{
		
           mainScript.moveUp();
        
        }
	}
	//finds the minimum
	int findMin(List<int> x, List<float> list){
		float minD = 25;
		float curD = list[0];
		int omin = 0;
		for (int i = 0; i < beltDirections.Length; i++){
			curD = list[i];
			if (x[i] == -1){
				if (curD <= minD){
					omin = i;
					minD = curD;
				}
			}
		}
		return omin;
	} 

	//presses the button up to twice a turn.
	//if the robot is near the same location as the enemy
	//move away from the enemy
	void attack(int target, int current){
		if (estimated(0) == estimatedE(0)){
	
			if((target - 1) != -1){
				move(buttonLocations[target - 1]);
			}else if((target + 1) != 9){
				move(buttonLocations[target + 1]);
            }
			
        }
		if(buttonCooldowns[target] <= 0){
			if (Mathf.Abs(ourL - buttonLocations[target]) / playerSpeed + 0.35f < bombPoints[target] / bombSpeeds[target]){
				mainScript.push();
			}
			if (target == current){
				mainScript.push();
			}
		}
		
	}

	//estimates the current place of our character but putsd it into int value
	int estimated(int x){
		float distance = 25;
		for (int i = 0; i < beltDirections.Length; i++){
			if (Mathf.Abs(ourL - buttonLocations[i]) < distance){
				x = i;
				distance = Mathf.Abs(ourL - buttonLocations[i]);
			}
		}
		return x;
	}

	//estimating enemys location 
	int estimatedE(int x){
		float distance = 25;
		for (int i = 0; i < beltDirections.Length; i++){
			if (Mathf.Abs(ourL - buttonLocations[i]) < distance){
				x = i;
				distance = Mathf.Abs(enemyL - buttonLocations[i]);
			}
		}
		return x;
	}

	//checks for bombs either idle or heading in our direction
	List<int> checkforBomb(List<int> x){
		for(int i = 0; i < beltDirections.Length; i++){
			if(beltDirections[i] == -1 || beltDirections[i] == 0){
				x.Add(-1);
			}else{
				x.Add(0);
			}
		}
		return x;
	}

	//shows which point to take prority
	int which(int ori, int ch) {
		if (bombPoints[ch] <5f) {
			return ch;
		}else{
			return ori;
        }
	}

	//looks over what value to pass through
	int lowerEvil(int x, int placement, List<int> hit){
		int count = 0;
		int count2 = 0;
		//checks to see if there are neighbors on both side 
		if((placement + 1) != 8 && (placement - 1 != -1)){	
			if (hit[placement + 1] != null){
				count = placement + 1;
			}
			if (hit[placement - 1] != null){
				count2 = placement - 1;
			}

			//if the bomb distance but if the speed on one of the bombs faster
			//prioritieze that bomb
			if (bombPoints[count] < bombPoints[count2]){
				x= count;
				if (bombSpeeds[count2] > bombSpeeds[count]){
					x = count2;
				}
			}else if (bombPoints[count] > bombPoints[count2]){
				x = count2;
				if (bombSpeeds[count2] < bombSpeeds[count]){
					x = count;
				}
			}   

		}
		return x;
	}
}
