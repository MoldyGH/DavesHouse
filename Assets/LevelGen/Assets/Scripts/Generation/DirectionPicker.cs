using System.Collections;
using System.Collections.Generic;
using System;
//using UnityEngine;

// DirectionPicker
public class DirectionPicker
{
	private static bool LINK_DEAD_ENDS_PASSING_ON_ROCKS = false;

	
	private readonly List<VirtualMap.DirectionType> directionsPicked = new List<VirtualMap.DirectionType>();
	public int ChangeDirectionModifier;
	private readonly VirtualMap.DirectionType PreviousDirection;
	private readonly CellLocation PreviousLocation;
	
	public DirectionPicker (VirtualMap map, CellLocation location, int changeDirectionModifier, VirtualMap.DirectionType previousDirection)
	{	
		this.PreviousDirection = previousDirection;
		this.PreviousLocation = location;
		this.ChangeDirectionModifier = changeDirectionModifier;
		
		foreach(VirtualMap.DirectionType dir in map.directions){
			if (map.HasAdjacentCellInDirection(location, dir) && !map.AdjacentCellInDirectionIsVisited(location, dir)){
				directionsPicked.Add(dir);
			}
		}
	}
	
	// Override the constructor for opening dead ends
	public DirectionPicker (VirtualMap map, CellLocation location, VirtualMap.DirectionType previousDirection)
	{	
		this.PreviousDirection = previousDirection;
		this.PreviousLocation = location;
		this.ChangeDirectionModifier = 100; // Always change the direction



		foreach(VirtualMap.DirectionType dir in map.directions){
			if (map.HasAdjacentCellInDirection(location,dir) && (LINK_DEAD_ENDS_PASSING_ON_ROCKS || !map.AdjacentCellInDirectionIsRock(location,dir)) ){
				directionsPicked.Add(dir);
			}
		}
	}
	
	public bool HasNextDirection
	{
		get { return directionsPicked.Count > 0; }
	}
	
	public VirtualMap.DirectionType GetNextDirection (VirtualMap map, CellLocation location)
	{	
		VirtualMap.DirectionType directionPicked = new VirtualMap.DirectionType();
		//right
		if (directionsPicked.Count == 0) //nulla da fare
		{
			map.visitedAndBlockedCells.Add(location);
			return VirtualMap.DirectionType.None;
		}
		else if (directionsPicked.Count == 1) //solo una ne ho, non c'e nbisogno di fare altro
		{
			directionPicked = directionsPicked[0];
		}
		else if (directionsPicked.Contains(PreviousDirection)) //posso cambiare o meno
		{
			if (MustChangeDirection) //se sono qua ho almeno due direzioni, levo quella corrente, perche' un'altra ce ne deve essere
		   	{
					directionsPicked.Remove(PreviousDirection);
					int index = DungeonGenerator.Random.Instance.Next(0, directionsPicked.Count-1);
					directionPicked = directionsPicked[index];
			}
			else
				directionPicked = PreviousDirection;			
		}
		else //devo cambiare per forza
		{
			int index = DungeonGenerator.Random.Instance.Next(0, directionsPicked.Count-1);
			directionPicked = directionsPicked[index];
		}
		
		return directionPicked;
	}
	
	private bool MustChangeDirection
	{
		get
		{
//			Debug.Log("Change dir" + ChangeDirectionModifier);
			return ChangeDirectionModifier > DungeonGenerator.Random.Instance.Next(0, 99);
		}
	}
	
	override public string ToString(){
		return "At loc " + PreviousLocation + " coming from " + PreviousDirection + " that can go to " + directionsPicked.Count + " direction";
	}
}

