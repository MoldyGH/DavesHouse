using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class VirtualRoom
{

    public int sequentialId;    // Set when created
	public int Width;
	public int Height;
	public CellLocation leftCorner;
	public List<CellLocation> doors;
	public List<CellLocation> cells;
	public List<CellLocation> corridorDoors;
	public List<CellLocation> roomDoors;
	public int doorsNumber;
	public int corridorExit;
	public List<int> connectedRoomsIds;

    public struct Dimensions
    {
        public int start_x;
        public int width_x;
        public int start_y;
        public int width_y;
        public int storey; // Written by the specific virtual map it belongs to
    }
	
	public VirtualRoom (int w, int h, CellLocation l)
	{
		Width = w;
		Height = h;
		leftCorner = l;
		
		doorsNumber = 0;
		doors = new List<CellLocation> ();
		corridorDoors = new List<CellLocation> ();
		roomDoors = new List<CellLocation> ();
		cells = new List<CellLocation> ();
		
		connectedRoomsIds = new List<int> ();
		
		corridorExit = 0;
	}

	public bool containsLocation (CellLocation l)
	{
		return cells.Contains (l);
	}

	// Inside the room (tiles)
	public bool IsInRoom (CellLocation l)
	{
//		Debug.Log (l + " corn: " + leftCorner);
		return (l.x >= leftCorner.x && l.x <=  leftCorner.x + 2 * Width - 2 && l.y >= leftCorner.y && l.y <= leftCorner.y + 2 * Height - 2);
	}
	
	// On the border
	public bool IsInBorder (CellLocation l)
	{
		CellLocation langle = new CellLocation (leftCorner.x - 1, leftCorner.y - 1);
		if ((l.y == langle.y && l.x >= langle.x && l.x <= langle.x + 2 * Width) || (l.x == langle.x && l.y >= langle.y && l.y <= langle.y + 2 * Height))
			return true;
		
		langle = new CellLocation (langle.x + 2 * Width, langle.y + 2 * Height);
		if ((l.y == langle.y && l.x <= langle.x && l.x >= langle.x - 2 * Width) || (l.x == langle.x && l.y <= langle.y && l.y >= langle.y - 2 * Height))
			return true;
		return false;
	}
	
	// Returns true if the passage has another door close to this
//	public bool hasNearbyDoor (Location l)
//	{
//		return doors.Contains (new Location (l.x, l.y)) || doors.Contains (new Location (l.x, l.y - 2)) || 
//			doors.Contains (new Location (l.x, l.y + 2)) || doors.Contains (new Location (l.x - 2, l.y)) 
//		    || doors.Contains (new Location (l.x + 2, l.y)) ;
//           
//	}
//	
//	// Returns true if the passage leads to another door nearby on other rooms
//	public bool hasConnectedDoor (Location l)
//	{
//		foreach (VirtualRoom r in connectedRoomsIds) {
//			if (r.hasNearbyDoor (l))
//				return true;
//		}
//		return false;
//	}
	
	public void AddDoorToAnotherRoom(CellLocation passage){
		doorsNumber++;
		doors.Add (passage);
		roomDoors.Add (passage);	
	}
	
	public bool IsAlreadyConnectedToARoomAt(CellLocation l, VirtualMap virtualMap){
		foreach(int index in this.connectedRoomsIds){
            VirtualRoom r = virtualMap.GetRoom(index);
            Debug.Log("CHECK: " + index);
			if (r.containsLocation(l)) {
//				Debug.Log("Room " + this + " is already connected to room " + r);
				return true;		
			}
		}
		return false;
	}

    public void ConnectRoom(VirtualRoom otherRoom)
    {
        this.connectedRoomsIds.Add(otherRoom.sequentialId);
    }

	public bool IsConnectedToCorridor(){
		return this.corridorExit > 0;	
	}
	
	public List<CellLocation> getBorders ()
	{
		List<CellLocation> result = new List<CellLocation> ();
		CellLocation langle = new CellLocation (leftCorner.x - 1, leftCorner.y - 1);
		int x = langle.x + 1;
		while (x<=langle.x+2*Width) {
			result.Add (new CellLocation (x, langle.y));
			x += 2;
		}
		int y = langle.y + 1;
		while (y<=langle.y+2*Height) {
			result.Add (new CellLocation (langle.x, y));
			y += 2;
		}
		langle = new CellLocation (langle.x + 2 * Width, langle.y + 2 * Height);
		x = langle.x - 1;
		while (x>=langle.x-2*Width) {
			result.Add (new CellLocation (x, langle.y));
			x -= 2;
		}
		y = langle.y - 1;
		while (y>=langle.y-2*Height) {
			result.Add (new CellLocation (langle.x, y));
			y -= 2;
		}
		return result;
	}
	
	override public string ToString(){
		return this.leftCorner.ToString();
	}


    public Dimensions GetDimensions()
    {
        Dimensions dim = new Dimensions();
        dim.start_x = this.leftCorner.x;
        dim.start_y = this.leftCorner.y;
        dim.width_x = this.Width;
        dim.width_y = this.Height;
        return dim;
    }
}

	
