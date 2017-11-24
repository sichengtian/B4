using UnityEngine;
using System;
using System.Collections;
using TreeSharpPlus;

public class BehaviorTree1 : MonoBehaviour
{
	private BehaviorAgent behaviorAgent;
    private BehaviorMecanim mec(GameObject g)
    {
        return g.GetComponent<BehaviorMecanim>();
    }
    private GameObject[] images;
	// Use this for initialization
	void Start ()
	{	
		images = GameObject.FindGameObjectsWithTag("image");
		behaviorAgent = new BehaviorAgent (this.BuildTreeRoot ());
		BehaviorManager.Instance.Register (behaviorAgent);
		behaviorAgent.StartBehavior ();
       
	}
	
	// Update is called once per frame
	void Update ()
	{
       
    }
    protected Node BuildTreeRoot()
    {
        DesignateClogger();
        return new Sequence(DirectPooperNode(),new DecoratorLoop(new LeafWait(1000)));
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
                new ForEach<GameObject>(RepeatedPoopArc, poopers),
                new ForEach<GameObject>(RepeatedClogArc, clogger)
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
        return new DecoratorLoop(4, PoopArc(pooper));
    }
    /*
    protected Node disableImage(){
    	//GameObject[] image = GameObject.FindGameObjectsWithTag("image");
    	return new SequenceParallel(
    		new ForEach<GameObject>(disable,images)
    	);
    }

    protected Node disable(GameObject image){
    	return new LeafInvoke(
            () => image.SetActive(false));
    }

    protected Node enbleImage(){
    	//GameObject[] images = GameObject.FindGameObjectsWithTag("image");
    	return new SequenceParallel(
    		new ForEach<GameObject>(reenable,images)
    	);
    }

    protected Node reenable(GameObject image){
    	return new LeafInvoke(
            () => image.SetActive(true));
    }
    */
    protected Node PoopArc(GameObject pooper)
    {
        return new Sequence(
                mec(pooper).Node_ChooseNextStall(),
                mec(pooper).Node_DisableImage(),
                mec(pooper).Node_GoToNextStall(),
                mec(pooper).Node_OrientTowardsAgent(),
                mec(pooper).Node_EnableImage(),
                mec(pooper).Node_FightEachOther(),
                mec(pooper).Node_PistolAimEachOther(),
                mec(pooper).Node_Conversation(),
                mec(pooper).Node_WalkToToilet(),
                mec(pooper).Node_DisableImage(),
                mec(pooper).Node_Poop(),
                mec(pooper).Node_SayByeToAgent(),
                mec(pooper).Node_LeaveStall(),
                mec(pooper).Node_EnableImage()
            );
    }
    protected Node ClogArc(GameObject clogger)
    {
        return new Sequence(
                mec(clogger).Node_ChooseNextStall(),
                mec(clogger).Node_DisableImage(),
                mec(clogger).Node_GoToNextStall(),
                mec(clogger).Node_OrientTowardsAgent(),
                mec(clogger).Node_EnableImage(),
                mec(clogger).Node_FightEachOther(),
                mec(clogger).Node_PistolAimEachOther(),
                mec(clogger).Node_Conversation(),
                mec(clogger).Node_WalkToToilet(),
                mec(clogger).Node_DisableImage(),
                mec(clogger).Node_Clog(),
                mec(clogger).Node_CloggingConversation(),
                mec(clogger).Node_EnableImage(),
                mec(clogger).Node_Explosion()
            );
    }
    
}
