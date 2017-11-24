using UnityEngine;
using System;
using System.Collections;
using TreeSharpPlus;

using RootMotion.FinalIK;

public enum AnimationLayer
{
    Hand,
    Body,
    Face,
}

public class BehaviorMecanim : MonoBehaviour
{
    [HideInInspector]
    public CharacterMecanim Character = null;

    void Awake() { this.Initialize(); }

    protected void Initialize()
    {
        this.Character = this.GetComponent<CharacterMecanim>();
    }

    protected void StartTree(
        Node root,
        BehaviorObject.StatusChangedEventHandler statusChanged = null)
    {
    }

    #region Helper Nodes

    #region Navigation
    /// <summary>
    /// Approaches a target
    /// </summary>
    public Node Node_GoTo(Val<Vector3> targ)
    {
        return new LeafInvoke(
            () => this.Character.NavGoTo(targ),
            () => this.Character.NavStop());
    }
    public Node Node_ChooseNextStall()
    {
        return new LeafInvoke(
                () => this.Character.ChooseNextStall());
    }
    public Node Node_GoToNextStall()
    {
        return new LeafInvoke(
                () => this.Character.NavGoToNextStall(),
                () => this.Character.NavStop()
            );
    }
//	public Node Node_WalkToward(GameObject target){
//		return new LeafInvoke (
//			()=>this.Character.WalkToward(target)
//
//		);
//	}
    public Node Node_OrientTowardsAgent()
    {
        return new LeafInvoke(
            () => this.Character.NavTurnTowardsAgent(),
            () => this.Character.NavOrientBehavior(OrientationBehavior.LookForward)
            );
    }
    public Node Node_WalkToToilet()
    {
        return new Sequence(
            new LeafInvoke(
                () => this.Character.NavGoToToilet(),
                () => this.Character.NavStop()
            ));
    }
    public Node Node_Conversation()
    {
        return new Sequence(
            new Sequence(
                    new LeafInvoke(() => this.Character.LookAtEachOther()),
                    PooperHandMovement("WAVE", 2000),
                    AgentHandMovement("WAVE", 2000)
                ),
            new Sequence(
                    new LeafInvoke(() => this.Character.LookAtEachOther()),
                    PooperHandMovement("BEINGCOCKY", 2000),
                    AgentHandMovement("SURPRISED", 2000)
                ),
            new Sequence(
                   new LeafInvoke(() => this.Character.LookAtEachOther()),
                   PooperHandMovement("CALLOVER", 2000)
               ),
            new Sequence(
                    new LeafInvoke(() => this.Character.StopLookingAtEachOther())
            ));

    }
    public Node Node_CloggingConversation()
    {
        return new Sequence(
           new LeafInvoke(() => {
               this.Character.TurnOnSignal();
           }),
           new Sequence(
                   new LeafInvoke(() => this.Character.LookAtEachOther()),
                   PooperHandMovement("WAVE", 2000),
                   AgentHandMovement("WAVE", 2000)
               ),
           new Sequence(
                   new LeafInvoke(() => this.Character.LookAtEachOther()),
                   PooperHandMovement("BEINGCOCKY", 2000),
                   AgentHandMovement("SURPRISED", 2000)
               ),
           new Sequence(
                  new LeafInvoke(() => this.Character.LookAtEachOther()),
                  PooperHandMovement("CALLOVER", 2000)
              ),
           new Sequence(
                   new LeafInvoke(() => this.Character.StopLookingAtEachOther())
           ));
    }
    public Node PooperHandMovement(Val<string> gestureName, Val<long> duration)
    {
        return new DecoratorCatch(
            () => this.Character.HandAnimation(gestureName, false),
            new Sequence(
                Node_HandAnimation(gestureName, true),
                new LeafWait(duration),
                Node_HandAnimation(gestureName, false)));
    }
    public Node AgentHandMovement(Val<string> gestureName, Val<long> duration)
    {
        return new DecoratorCatch(
            () => this.Character.AgentHandAnimation(gestureName, false),
            new Sequence(
                Node_AgentHandAnimation(gestureName, true),
                new LeafWait(duration),
                Node_AgentHandAnimation(gestureName, false)));
    }
    public Node Node_Poop()
    {
        return new Sequence(
                new LeafInvoke(()=>this.Character.TurnAround()),
                new LeafInvoke(() => this.Character.SitDown()),
                PooperHandMovement("THINK", 2000),
                PooperHandMovement("CHEER", 2000),
                new LeafWait(1000),
                new LeafInvoke(() => this.Character.StandUp())
            );
        
    }
    public Node Node_Clog()
    {
        return new Sequence(
               new LeafInvoke(() => this.Character.TurnAround()),
               new LeafInvoke(() => this.Character.SitDown()),
               PooperHandMovement("THINK", 2000),
               PooperHandMovement("CHEER", 2000),
               new LeafWait(1000)
           );
    }
    public Node Node_SayByeToAgent()
    {
        return new Sequence(
                new LeafInvoke(() => this.Character.WalkToMeetPoint()),
                new LeafInvoke(() => this.Character.NavTurnTowardsAgent()),
                new Sequence(
                    new LeafInvoke(() => this.Character.LookAtEachOther()),
                    PooperHandMovement("WAVE", 500),
                    AgentHandMovement("WAVE", 500)
                ),
                new Sequence(
                    new LeafInvoke(() => this.Character.StopLookingAtEachOther())),
                new LeafWait(UnityEngine.Random.Range(500, 2000))
            
            );
    }

    public Node Node_LeaveStall()
    {
        return new LeafInvoke(
                () => this.Character.NavLeaveStall(),
                () => this.Character.NavStop()
            );

    }
    public Node Node_Explosion()
    {
        return new LeafInvoke(() =>
        {
            GameObject explosion = GameObject.FindGameObjectWithTag("Wasted");
            explosion.transform.GetChild(0).gameObject.SetActive(true);
            explosion.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).gameObject.GetComponent<UnityEngine.Video.VideoPlayer>().Play();
        });
    }
    public Node Node_NudgeTo(Val<Vector3> targ)
    {
        return new LeafInvoke(
            () => this.Character.NavNudgeTo(targ),
            () => this.Character.NavStop());
    }

    //public Node Node_GoAlongPoints(Val<Vector3>[] targ)
    //{
    //    return new LeafInvoke(
    //        () => this.Character.NavAlongPoints(targ),
    //        () => this.Character.NavStop());
    //}

    // TODO: No speed support yet! - AS 5/4/14
    ///// <summary>
    ///// Approaches a target with a certain speed
    ///// </summary>
    //public Node Node_GoTo(Val<Vector3> targ, Val<float> speed)
    //{
    //    this.Character.SetSpeed(speed.Value);
    //    return new LeafInvoke(
    //        () => this.Character.NavGoTo(targ),
    //        () => this.Character.NavStop());
    //}

    /// <summary>
    /// Orient towards a target position
    /// </summary>
    /// <param name="targ"></param>
    /// <returns></returns>
    public Node Node_OrientTowards(Val<Vector3> targ)
    {
        return new LeafInvoke(
            () => this.Character.NavTurn(targ),
            () => this.Character.NavOrientBehavior(
                OrientationBehavior.LookForward));
    }

    /// <summary>
    /// Orient towards a target position
    /// </summary>
    /// <param name="targ"></param>
    /// <returns></returns>
    public Node Node_Orient(Val<Quaternion> direction)
    {
        return new LeafInvoke(
            () => this.Character.NavTurn(direction),
            () => this.Character.NavOrientBehavior(
                OrientationBehavior.LookForward));
    }

    /// <summary>
    /// Approaches a target at a given radius
    /// </summary>
    public Node Node_GoToUpToRadius(Val<Vector3> targ, Val<float> dist)
    {
        Func<RunStatus> GoUpToRadius =
            delegate()
            {
                Vector3 targPos = targ.Value;
                Vector3 curPos = this.transform.position;
                if ((targPos - curPos).magnitude < dist.Value)
                {
                    this.Character.NavStop();
                    return RunStatus.Success;
                }
                return this.Character.NavGoTo(targ);
            };

        return new LeafInvoke(
            GoUpToRadius,
            () => this.Character.NavStop());
    }
    #endregion

    #region Reach
    /// <summary>
    /// Tries to reach target with right or left hand.
    /// </summary>
    public Node Node_StartInteraction(Val<FullBodyBipedEffector> effector, Val<InteractionObject> obj)
    {
        return new LeafInvoke(
            () => this.Character.StartInteraction(effector, obj),
            () => this.Character.StopInteraction(effector));
    }

    /// <summary>
    /// Tries to reach target with right or left hand.
    /// </summary>
    public Node Node_ResumeInteraction(Val<FullBodyBipedEffector> effector)
    {
        return new LeafInvoke(
            () => this.Character.ResumeInteraction(effector),
            () => this.Character.StopInteraction(effector));
    }

    /// <summary>
    /// Stops reaching the target for the specified hand.
    /// </summary>
    public Node Node_StopInteraction(Val<FullBodyBipedEffector> effector)
    {
        return new LeafInvoke(
            () => this.Character.StopInteraction(effector));
    }

    /// <summary>
    /// Waits for a trigger on a specific effector
    /// </summary>
    public Node Node_WaitForTrigger(Val<FullBodyBipedEffector> effector)
    {
        return new LeafInvoke(
            () => this.Character.WaitForTrigger(effector));
    }

    /// <summary>
    /// Waits for a specific effector to finish its interaction
    /// </summary>
    public Node Node_WaitForFinish(Val<FullBodyBipedEffector> effector)
    {
        return new LeafInvoke(
            () => this.Character.WaitForFinish(effector));
    }
    #endregion

    #region HeadLook
    public Node Node_HeadLook(Val<Vector3> targ)
    {
        return new LeafInvoke(
                () => this.Character.HeadLookAt(targ),
                () => this.Character.HeadLookStop());
    }

    public Node Node_HeadLookTurnFirst(Val<Vector3> targ)
    {
        return new Sequence(
            new LeafInvoke(() => this.Character.NavTurn(targ)),
            Node_HeadLook(targ));
    }

    public Node Node_HeadLookStop()
    {
        return new LeafInvoke(
            () => this.Character.HeadLookStop());
    }

    #endregion

    #region Animation
    /// <summary>
    /// A Hand animation is started if the bool is true, the hand animation 
    /// is stopped if the bool is false
    /// </summary>
    public Node Node_HandAnimation(Val<string> gestureName, Val<bool> start)
    {

        return new LeafInvoke(
            () => this.Character.HandAnimation(gestureName, start),
            () => this.Character.HandAnimation(gestureName, false));
    }
    public Node Node_AgentHandAnimation(Val<string> gestureName, Val<bool> start)
    {
        return new LeafInvoke(
            () => this.Character.AgentHandAnimation(gestureName, start),
            () => this.Character.AgentHandAnimation(gestureName, false));
    }
    /// <summary>
    /// A Face animation is started if the bool is true, the face animation 
    /// is stopped if the bool is false
    /// </summary>
    public Node Node_FaceAnimation(Val<string> gestureName, Val<bool> start)
    {
        return new LeafInvoke(
            () => this.Character.FaceAnimation(gestureName, start),
            () => this.Character.FaceAnimation(gestureName, false));
    }

    public Node Node_BodyAnimation(Val<string> gestureName, Val<bool> start)
    {
        return new LeafInvoke(
            () => this.Character.BodyAnimation(gestureName, start),
            () => this.Character.BodyAnimation(gestureName, false));
    }


    #endregion

    #endregion

    #region Helper Subtrees

    /// <summary>
    /// Plays a gesture of a determined type for a given duration
    /// </summary>
    public Node ST_PlayGesture(
        Val<string> gestureName,
        Val<AnimationLayer> layer,
        Val<long> duration)
    {
        switch (layer.Value)
        {
            case AnimationLayer.Hand:
                return this.ST_PlayHandGesture(gestureName, duration);
            case AnimationLayer.Body:
                return this.ST_PlayBodyGesture(gestureName, duration);
            case AnimationLayer.Face:
                return this.ST_PlayFaceGesture(gestureName, duration);
        }
        return null;
    }

    /// <summary>
    /// Plays a hand gesture for a duration in miliseconds
    /// </summary>
    public Node ST_PlayHandGesture(
        Val<string> gestureName, Val<long> duration)
    {
        return new DecoratorCatch(
            () => this.Character.HandAnimation(gestureName, false),
            new Sequence(
                Node_HandAnimation(gestureName, true),
                new LeafWait(duration),
                Node_HandAnimation(gestureName, false)));
    }

    /// <summary>
    /// Plays a body gesture for a duration in miliseconds
    /// </summary>
    public Node ST_PlayBodyGesture(
        Val<string> gestureName, Val<long> duration)
    {
        return new DecoratorCatch(
            () => this.Character.BodyAnimation(gestureName, false),
            new Sequence(
            this.Node_BodyAnimation(gestureName, true),
            new LeafWait(duration),
            this.Node_BodyAnimation(gestureName, false)));
    }

    /// <summary>
    /// Plays a face gesture for a duration in miliseconds
    /// </summary>
    public Node ST_PlayFaceGesture(
        Val<string> gestureName, Val<long> duration)
    {
        return new DecoratorCatch(
            () => this.Character.FaceAnimation(gestureName, false),
            new Sequence(
                Node_FaceAnimation(gestureName, true),
                new LeafWait(duration),
                Node_FaceAnimation(gestureName, false)));
    }

    /// <summary>
    /// Turns to face a target position
    /// </summary>
    public Node ST_TurnToFace(Val<Vector3> target)
    {
        Func<RunStatus> turn =
            () => this.Character.NavTurn(target);

        Func<RunStatus> stopTurning =
            () => this.Character.NavOrientBehavior(
                OrientationBehavior.LookForward);

        return
            new Sequence(
                new LeafInvoke(turn, stopTurning));
    }
    #endregion
}
