using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FirstController : MonoBehaviour, ISceneController, IUserAction {

    public CCActionManager actionManager { get; set; }
    public GameObject move1, move2;
    private GameObject boat;
    private GameObject l_ground, r_ground;
    private Vector3 boatPosition = new Vector3(1, -2, -20);
    private Vector3[] groundPosition = new Vector3[] { new Vector3(10.5f, -4, -20), new Vector3(-10.5f, -4, -20) };
    private Vector3[] priestsPosition = new Vector3[] { new Vector3(12f, 0.65f, -20), new Vector3(10.5f, 0.65f, -20), new Vector3(9f, 0.65f, -20) };
    private Vector3[] devilsPosition = new Vector3[] { new Vector3(7.5f, 0.65f, -20), new Vector3(6f, 0.65f, -20), new Vector3(4.5f, 0.65f, -20) };

    public bool isMoving;
    private bool boatOnLeft = true;
    // Values:GameObject.GetInstanceID()
    public int[] boatRole = new int[2] { 0, 0 };
    // Keys:GameObject.GetInstanceID()    Values:RoleMessage
    public Dictionary<int, RoleMessage> roleDict = new Dictionary<int, RoleMessage>();

    // the first scripts
    void Awake () {
		SSDirector director = SSDirector.getInstance ();
        director.setFPS (60);
		director.currentSceneController = this;
        director.currentSceneController.LoadResources ();
        Debug.Log ("awake FirstController!");
	}

	// loading resources for first scence
	public void LoadResources () {
        boat = GameObject.Instantiate(Resources.Load("Prefabs/Boat", typeof(GameObject))) as GameObject;
        boat.transform.position = boatPosition;

        l_ground = GameObject.Instantiate(Resources.Load("Prefabs/Ground", typeof(GameObject))) as GameObject;
		r_ground = GameObject.Instantiate(Resources.Load("Prefabs/Ground", typeof(GameObject))) as GameObject;
		l_ground.transform.position = groundPosition[0];
        r_ground.transform.position = groundPosition[1];

        isMoving = false;

        for (int i = 0; i < 3; i++)
        {
            GameObject devil = GameObject.Instantiate(Resources.Load("Prefabs/Devil", typeof(GameObject))) as GameObject;
            GameObject priest = GameObject.Instantiate(Resources.Load("Prefabs/Priest", typeof(GameObject))) as GameObject;
            devil.transform.position = devilsPosition[i];
            priest.transform.position = priestsPosition[i];
            roleDict.Add(devil.GetInstanceID(), new RoleMessage(i + 4, devil, "devil"));
            roleDict.Add(priest.GetInstanceID(), new RoleMessage(i + 1, priest, "priest"));
        }
    }

	public void Pause ()
	{
		throw new System.NotImplementedException ();
	}

	public void Resume ()
	{
		throw new System.NotImplementedException ();
	}

    public void Check()
    {
        int l_devils = 0;
        int l_priests = 0;
        int r_devils = 0;
        int r_priests = 0;

        foreach (var key in roleDict.Keys)
        {
            RoleMessage mes = roleDict[key];
            if (mes.rolePosition == 1)
            {
                if (mes.role == "devil")
                {
                    l_devils++;
                }
                else
                {
                    l_priests++;
                }
            }
            else if (mes.rolePosition == -1)
            {
                if (mes.role == "devil")
                {
                    r_devils++;
                }
                else
                {
                    r_priests++;
                }
            }
        }

        // Debug.Log(string.Format("l_devils: {0},l_priests: {1},r_devils: {2},r_priests: {3}", l_devils, l_priests, r_devils, r_priests));
        if ((r_priests == 3 && r_devils == 3))
        {
            GameObject winCanvas = GameObject.Instantiate(Resources.Load("Prefabs/WinCanvas", typeof(GameObject))) as GameObject;
        }
        if ((l_devils > l_priests && l_priests != 0) || (r_devils > r_priests && r_priests != 0))
        {
            GameObject loseCanvas = GameObject.Instantiate(Resources.Load("Prefabs/LoseCanvas", typeof(GameObject))) as GameObject;
        }
    }

    #region IUserAction implementation
    public void MoveBoat()
    {
        if (isMoving)
        {
            return;
        }

        isMoving = true;

        int targetX = 0;
        if (boatOnLeft)
        {
            targetX = -1;
            boatOnLeft = false;
        }
        else
        {
            targetX = 1;
            boatOnLeft = true;
        }
        
        CCMoveToAction moveAction = CCMoveToAction.GetSSAction(new Vector3(targetX, -2, -20), 5);
        actionManager.RunAction(boat, moveAction, actionManager);
        for (int i = 0; i < 2; i++)
        {
            if (boatRole[i] != 0)
            {
                GameObject role = roleDict[boatRole[i]].roleModel;
                moveAction = CCMoveToAction.GetSSAction(new Vector3(role.transform.position.x + targetX * 2, -0.85f, -20), 5);
                actionManager.RunAction(role, moveAction, actionManager);
            }
        }

        Check();
    }

    public void MoveRole(GameObject role)
    {
        if (isMoving)
        {
            return;
        }
        // Judge if the priest or the devil can get on board
        if ((boatOnLeft && role.transform.position.x < -3) || (!boatOnLeft && role.transform.position.x > 3))
        {
            return;
        }

        isMoving = true;

        float targetX = 0;
        float targetY = 0.65f;

        // The priest or the devil is located on the shore
        if (role.transform.position.y == 0.65f)
        {
            targetY = -0.85f;
            if (boatRole[0] == 0)
            {
                if (boatOnLeft)
                {
                    targetX = 2;
                }
                boatRole[0] = role.GetInstanceID();
                roleDict[boatRole[0]].rolePosition = 0;
            }
            else if (boatRole[1] == 0)
            {
                if (!boatOnLeft)
                {
                    targetX = -2;
                }
                boatRole[1] = role.GetInstanceID();
                roleDict[boatRole[1]].rolePosition = 0;
            }
            else
            {
                return;
            }
        }
        else
        {
            int index = roleDict[role.GetInstanceID()].sequenceIndex;
            if (index > 3)
            {
                targetX = devilsPosition[index - 4].x;
            }
            else
            {
                targetX = priestsPosition[index - 1].x;
            }
            if (boatOnLeft)
            {
                if (role.transform.position.x == 2)
                {
                    roleDict[boatRole[0]].rolePosition = 1;
                    boatRole[0] = 0;
                }
                else
                {
                    roleDict[boatRole[1]].rolePosition = 1;
                    boatRole[1] = 0;
                }
            }
            else
            {
                targetX *= -1;
                if (role.transform.position.x == 0)
                {
                    roleDict[boatRole[0]].rolePosition = -1;
                    boatRole[0] = 0;
                }
                else
                {
                    roleDict[boatRole[1]].rolePosition = -1;
                    boatRole[1] = 0;
                }
            }
        }
        CCMoveToAction moveUp = CCMoveToAction.GetSSAction(new Vector3(role.transform.position.x, 2.65f, -20), 5);
        CCMoveToAction moveFowaard = CCMoveToAction.GetSSAction(new Vector3(targetX, 2.65f, -20), 10);
        CCMoveToAction moveDown = CCMoveToAction.GetSSAction(new Vector3(targetX, targetY, -20), 5);
        CCSequenceAction ccs = CCSequenceAction.GetSSAction(1, 0, new List<SSAction> { moveUp, moveFowaard, moveDown });
        actionManager.RunAction(role, ccs, actionManager);

        Check();
    }

    public void GameOver ()
	{
        SSDirector.getInstance ().NextScene ();
	}
	#endregion

	// Use this for initialization
	void Start () {
        //give advice first
    }
	
	// Update is called once per frame
	void Update () {
        //give advice first
	}

}
