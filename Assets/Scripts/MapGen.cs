using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UIElements;

public class MapGen
{
    private int[] xpos;
    private int[] ypos;
    private List<KeyValuePair<int, int>> roomspos;
    private Dictionary<KeyValuePair<int, int>, int> ablepos;
    GameObject dungeon;
    GameObject dungeonRoom;
    GameObject dungeonGate;


    public void init()
    {
        roomspos = new List<KeyValuePair<int, int>>();
        ablepos = new Dictionary<KeyValuePair<int, int>, int>();
        //                  U, R, L, D
        xpos = new int[4] { 0, 1, -1, 0 };      
        ypos = new int[4] { 1, 0, 0, -1 };
    }


    public List<GameObject> mapGen(int roomNo, int stageNo)
    {
        List<GameObject> ret = new List<GameObject>();
        KeyValuePair<int, int> target;
        GameObject room = null;
        GameObject tmp = GameObject.Find("Dungeon");
        if (tmp != null)
        {
            GameManager.Resource.Destroy(tmp);
        }

        setParents();
        // Generate StartRoom
        room = startRoomGen();
        addRoom(0,0, room);
        room.GetComponent<Room>().index = 0;
        room.transform.parent = dungeonRoom.transform;
        ret.Add(room);
        // Generate NormalRoom
        for (int i = 0; i < roomNo; i++)
        {
            target = getRandompos();
            room = normalRoomGen();
            addRoom(target.Key, target.Value, room);
            room.GetComponent<Room>().index = i + 1;
            room.transform.parent = dungeonRoom.transform;
            ret.Add(room);
        }

        return ret;
    }

    private void setParents()
    {
        dungeon = new GameObject("Dungeon");
        dungeonRoom = new GameObject("rooms");
        dungeonGate = new GameObject("gates");
        dungeonRoom.transform.parent = dungeon.transform;
        dungeonGate.transform.parent = dungeon.transform;
    }

    // Return created room prefab
    private GameObject startRoomGen()
    {
        GameObject room = GameManager.Resource.Instantiate("Dungeon/StartRoom");
        return room;
    }
    private GameObject normalRoomGen()
    {
        GameObject room = GameManager.Resource.Instantiate("Dungeon/NormalRoom");
        return room;
    }

    // When New Dungeon Create or Next Dungeon Create buffer Clear
    public void clear()
    {
        roomspos.Clear();
        ablepos.Clear();
    }

    // Filled new Room Object in Buffer and able new Position Add
    private void addRoom(int x, int y, GameObject obj)
    {
        // °´Ã¼ x,y Ä­ ¼³Á¤
        obj.GetComponent<Room>().x = x;
        obj.GetComponent<Room>().y = y;
        roomspos.Add(new KeyValuePair<int, int>(x, y));
        for(int i = 0; i < 4; i++)
        {
            KeyValuePair<int, int> target = new KeyValuePair<int, int>(x + xpos[i], y + ypos[i]);
            if(roomspos.IndexOf(new KeyValuePair<int, int>(target.Key, target.Value)) == -1)
            {
                if (!ablepos.ContainsKey(target))
                {
                    ablepos.Add(target, 1);
                }
                else
                {
                    ablepos[target] += 1;
                }
            }
        }
        ablepos.Remove(new KeyValuePair<int, int>(x, y));
    }
    
    // O(n^2) Create Gates between Rooms
    public List<GameObject> setGates(List<GameObject> rms)
    {

        List<GameObject> ret = new List<GameObject>();
        int cnt = rms.Count();
        int gatecnt = 0;
        for (int i = 0; i < cnt -1; i++)
        {
            int currentx = rms.ElementAt(i).GetComponent<Room>().x;
            int currenty = rms.ElementAt(i).GetComponent<Room>().y;
            for(int j = i+1; j < cnt; j++)
            {
                int nextx = rms.ElementAt(j).GetComponent<Room>().x;
                int nexty = rms.ElementAt (j).GetComponent<Room>().y;

                for (int k = 0; k < 4; k++)
                {
                    if (currentx + xpos[k] == nextx && currenty + ypos[k] == nexty)
                    {
                        GameObject gate = GameManager.Resource.Instantiate("Dungeon/Gate");
                        gate.GetComponent<Gate>().index = gatecnt;
                        Char[] strI = rms.ElementAt(i).GetComponent<Room>().gatedir.ToCharArray();
                        Char[] strJ = rms.ElementAt(j).GetComponent<Room>().gatedir.ToCharArray();
                        switch (k)
                        {
                            case 0:
                                rms.ElementAt(i).GetComponent<Room>().gateU = gatecnt;
                                strI[3] = '1';
                                rms.ElementAt(j).GetComponent<Room>().gateD = gatecnt;
                                strJ[2] = '1';
                                break;
                            case 1:
                                rms.ElementAt(i).GetComponent<Room>().gateR = gatecnt;
                                strI[0] = '1';
                                rms.ElementAt(j).GetComponent<Room>().gateL = gatecnt;
                                strJ[1] = '1';
                                break;
                            case 2:
                                rms.ElementAt(i).GetComponent<Room>().gateL = gatecnt;
                                strI[1] = '1';
                                rms.ElementAt(j).GetComponent<Room>().gateR = gatecnt;
                                strJ[0] = '1';
                                break;
                            case 3:
                                rms.ElementAt(i).GetComponent<Room>().gateD = gatecnt;
                                strI[2] = '1';
                                rms.ElementAt(j).GetComponent<Room>().gateU = gatecnt;
                                strJ[3] = '1';
                                break;
                        }

                        gate.GetComponent<Gate>().posx = (float)(currentx + nextx) / 2;
                        gate.GetComponent<Gate>().posy = (float)(currenty + nexty) / 2;

                        rms.ElementAt(i).GetComponent<Room>().gatedir = new string(strI);
                        rms.ElementAt(j).GetComponent<Room>().gatedir = new string(strJ);

                        gate.GetComponent<Gate>().room1 = rms.ElementAt(i).GetComponent<Room>().index;
                        gate.GetComponent<Gate>().room2 = rms.ElementAt(j).GetComponent<Room>().index;
                        gate.transform.parent = dungeonGate.transform;
                        ret.Add(gate);
                        gatecnt++;
                    }

                }
            }
        }
        return ret;
    }

    // return Random Position In abldpos buffer
    private KeyValuePair<int, int> getRandompos()
    {
        int acount = ablepos.Count();
        KeyValuePair<KeyValuePair<int, int>, int> res = ablepos.ElementAt(GameManager.Random.getMapNext(0, acount));
        while (res.Value >= 3)
        {
            res = ablepos.ElementAt(GameManager.Random.getMapNext(0, acount));
        }
        return res.Key;
    }
}
