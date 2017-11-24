using UnityEngine;
using System;
using System.Collections;
using TreeSharpPlus;
using UnityEngine.AI;

public class BehaviorTree1 : MonoBehaviour
{
	public GameObject savior;
	private GameObject selectedObject;

	private BehaviorAgent behaviorAgent;
    private BehaviorMecanim mec(GameObject g)
    {
        return g.GetComponent<BehaviorMecanim>();
    }
    private bool buttonPressed = false;
	private bool fightPressed = false;
	private bool dancePressed=false;
	private bool someoneDied=false;
	// Use this for initialization
	void Start ()
	{
		behaviorAgent = new BehaviorAgent (this.BuildTreeRoot ());
		BehaviorManager.Instance.Register (behaviorAgent);
		behaviorAgent.StartBehavior ();
       
	}

	char Clicked=' ';
	
	// Update is called once per frame
	void Update ()
	{

		if(Input.GetKeyDown(KeyCode.K)){
			buttonPressed = true;
		}
		if (Input.GetKeyDown (KeyCode.F)) {
			fightPressed = true;
		}
		if (Input.GetKeyDown (KeyCode.H)) {
			dancePressed = true;
		}
			
    }
    protected Node BuildTreeRoot()
    {
        DesignateClogger();
		return new SequenceParallel(new DecoratorLoop(DirectPooperNode()),new DecoratorLoop(suicide()),new DecoratorLoop(dance()),new DecoratorLoop(fight()));
    }
    protected void DesignateClogger()
    {
        GameObject[] poopers = GameObject.FindGameObjectsWithTag("Pooper");
        int i = UnityEngine.Random.Range(0, poopers.Length - 1);
        poopers[i].tag = "Clogger";
    }
    protected Node DirectPooperNode()
    {
        GameObject[] poopers = GameObject.FindGameObjectsWithTag("Pooper");
        GameObject[] clogger = GameObject.FindGameObjectsWithTag("Clogger");
        return new SequenceParallel(
                new ForEach<GameObject>(SpecialPoopArc, poopers),
                new ForEach<GameObject>(SpecialClogArc, clogger)
            );
    }
    protected bool IsPooperActive(GameObject pooper)
    {
        bool active = pooper.GetComponent<PooperMeta>().IsActiveForTree();
        return active;
    }
    protected Node SpecialPoopArc(GameObject pooper)
    {
        return new Sequence(
                new DecoratorForceStatus(
                    RunStatus.Success,
                    new SequenceParallel(
                        new DecoratorLoop(new LeafAssert (()=> IsPooperActive(pooper))),
                        RepeatedPoopArc(pooper)
                    )
                ),
			new LeafInvoke(() => pooper.GetComponent<NavMeshAgent>().ResetPath()),
			new LeafInvoke(() => pooper.GetComponent<NavMeshAgent>().Resume()),
			mec(pooper).Node_GoTo(RandomNavmeshLocation()),
			testClick()

            );
    }
    protected Node SpecialClogArc(GameObject clogger)
    {
        return new Sequence(
                new DecoratorForceStatus(
                    RunStatus.Success,
                    new SequenceParallel(
                        new DecoratorLoop(new LeafAssert(() => IsPooperActive(clogger))),
                        RepeatedClogArc(clogger)
                    )
                )
            );
    }
    protected Node RepeatedClogArc(GameObject clogger)
    {
        int iterations = UnityEngine.Random.Range(1, 3);
        return new Sequence(
                new DecoratorLoop(iterations,PoopArc(clogger)),
                ClogArc(clogger)
            );
    }
    protected Node RepeatedPoopArc(GameObject pooper)
    {
		return new DecoratorLoop(PoopArc(pooper));
    }

	protected Node ConditionH(){
		Debug.Log(dancePressed);
		return new Sequence(
			new LeafAssert (
				()=>dancePressed==true
			),
			new LeafInvoke(() => pooperCheer())
			
		);
	}

	protected Node ConditionF(){
		return new Sequence(
			new LeafAssert (
				()=>fightPressed==true
			),
			new LeafInvoke(() => pooperCry())

		);
	}


	protected Node ConditionK(){
		return new Sequence(
			new LeafAssert (
				()=>buttonPressed==true
			),
			new LeafInvoke(() => pooperDie())

		);
	}

	protected bool pooperCheer(){
		Debug.Log ("poopercheer");
		GameObject[] poopers = GameObject.FindGameObjectsWithTag("Pooper");
		Debug.Log (poopers.Length);
		foreach (GameObject pooper in poopers) {
			pooper.GetComponent<Animator> ().SetBool ("H_Cheer",true);
		}
		return true;
	}

	protected bool pooperCry(){
		Debug.Log ("poopercry");
		GameObject[] poopers = GameObject.FindGameObjectsWithTag("Pooper");
		Debug.Log (poopers.Length);
		foreach (GameObject pooper in poopers) {
			pooper.GetComponent<Animator> ().SetTrigger ("H_Cry");
		}
		return true;
	}

	protected bool pooperDie(){
		Debug.Log ("poopercry");
		GameObject[] poopers = GameObject.FindGameObjectsWithTag("Pooper");
		Debug.Log (poopers.Length);
		foreach (GameObject pooper in poopers) {
			pooper.GetComponent<Animator> ().SetTrigger ("B_Dying");
		}
		return true;
	}

	protected Node testClick(){
		return new Selector (
			ConditionF(),
			ConditionH(),
			ConditionK()
		);
	}

	protected Node suicide(){
		return new LeafAssert(() => suicideOrNot());
	}

	protected bool suicideOrNot(){
		if(buttonPressed){
			Debug.Log("do DYING");
			savior.GetComponent<Animator>().SetTrigger("B_Dying");
			GameObject[] poopers = GameObject.FindGameObjectsWithTag("Pooper");
			foreach(GameObject pooper in poopers){
				pooper.GetComponent<PooperMeta>().StillActive=false;
			}
		}
		return true;
	}

	protected Node dance(){
		return new LeafAssert(() => danceOrNot());
	}

	protected bool danceOrNot(){
		if (dancePressed) {
			//check for nearby agent
			Collider[] hitColliders=Physics.OverlapSphere(savior.transform.position,3);
			int i = 0;
			bool foundPeter = false;
			while (i < hitColliders.Length) {
				if (hitColliders [i].name.Contains ("Peter")) {
					foundPeter = true;
					savior.GetComponent<Animator> ().SetTrigger ("B_Breakdance");
					hitColliders [i].GetComponent<Animator> ().SetBool ("H_Cheer",true);
					GameObject[] poopers = GameObject.FindGameObjectsWithTag("Pooper");
					foreach(GameObject pooper in poopers){
						pooper.GetComponent<PooperMeta>().StillActive=false;
					}

				}
				i++;
			}
			if (foundPeter == false) {
				dancePressed = false;
			}
		}
		return true;
	}

	protected Node fight(){
		return new LeafAssert(() => fightOrNot());
	}

	protected bool fightOrNot(){
		if (fightPressed) {
			//check for nearby agent
			Collider[] hitColliders=Physics.OverlapSphere(savior.transform.position,3);
			int i = 0;
			bool foundPeter = false;
			while (i < hitColliders.Length) {
				if (hitColliders [i].name.Contains ("Daniel")) {
					foundPeter = true;
					savior.GetComponent<Animator> ().SetBool ("B_Idle_Fight",true);
					hitColliders [i].GetComponent<Animator> ().SetBool ("B_Dying",true);
				}
				i++;
			}
			GameObject[] poopers = GameObject.FindGameObjectsWithTag("Pooper");
			foreach(GameObject pooper in poopers){
				pooper.GetComponent<PooperMeta>().StillActive=false;
			}
			if (foundPeter == false) {
				fightPressed = false;
			}
		}
		return true;
	}

	protected Vector3 RandomNavmeshLocation(){
		Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * 10;
        randomDirection += savior.transform.position;
        NavMeshHit hit;
        Vector3 finalPosition = Vector3.zero;
        if (NavMesh.SamplePosition(randomDirection, out hit, 10, 1)) {
            finalPosition = hit.position;            
        }
        return finalPosition;
	}
    
    protected Node PoopArc(GameObject pooper)
    {
        return new Sequence(
                mec(pooper).Node_ChooseNextStall(),
                mec(pooper).Node_GoToNextStall(),
                mec(pooper).Node_OrientTowardsAgent(),
                mec(pooper).Node_Conversation(),
                mec(pooper).Node_WalkToToilet(),
                mec(pooper).Node_Poop(),
                mec(pooper).Node_SayByeToAgent(),
                mec(pooper).Node_LeaveStall()
            );
    }
    protected Node ClogArc(GameObject clogger)
    {
        return new Sequence(
                mec(clogger).Node_ChooseNextStall(),
                mec(clogger).Node_GoToNextStall(),
                mec(clogger).Node_OrientTowardsAgent(),
                mec(clogger).Node_Conversation(),
                mec(clogger).Node_WalkToToilet(),
                mec(clogger).Node_Clog(),
                mec(clogger).Node_CloggingConversation(),
                mec(clogger).Node_Explosion()
            );
    }
    
}
