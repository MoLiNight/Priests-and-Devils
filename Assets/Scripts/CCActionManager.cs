using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CCActionManager : SSActionManager, ISSActionCallback {
	
	private FirstController sceneController;
	private CCMoveToAction moveToA , moveToB, moveToC, moveToD;

	protected new void Start() {
		sceneController = (FirstController)SSDirector.getInstance ().currentSceneController;
		sceneController.actionManager = this;
		moveToA = CCMoveToAction.GetSSAction (new Vector3 (5, 0, 0), 1);
		this.RunAction (sceneController.move1, moveToA, this);
		moveToC = CCMoveToAction.GetSSAction (new Vector3 (-2, -2, -2), 1);
		moveToD = CCMoveToAction.GetSSAction (new Vector3 (3, 3, -3), 1);
		CCSequenceAction ccs = CCSequenceAction.GetSSAction (3, 0, new List<SSAction> {moveToC, moveToD});
		this.RunAction (sceneController.move2, ccs, this);
	}

	// Update is called once per frame
	protected new void Update ()
	{
		base.Update ();
	}

	#region ISSActionCallback implementation
	public void SSActionEvent (SSAction source, SSActionEventType events = SSActionEventType.Competeted, int intParam = 0, string strParam = null, Object objectParam = null)
	{
        sceneController.isMoving = false;
        if (source == moveToA)
        {
            moveToB = CCMoveToAction.GetSSAction(new Vector3(-5, 0, 0), 1);
            this.RunAction(sceneController.move1, moveToB, this);
        }
        else if (source == moveToB)
        {
            moveToA = CCMoveToAction.GetSSAction(new Vector3(5, 0, 0), 1);
            this.RunAction(sceneController.move1, moveToA, this);
        }
    }
    #endregion

}

