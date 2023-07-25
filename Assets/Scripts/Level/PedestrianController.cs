using System;
using UnityEngine;
using static Level.GameEngine;

namespace Level
{
    [System.Serializable]
    public class PedestrianController : RoadUser
    {
        private Animator anim;
        private SpriteRenderer renderer;
        public Sprite ranOverSprite;

        protected override void Awake()
        {
            anim = GetComponent<Animator>();
            base.Awake(); // This is not at first line in Awake because on restart, it caused visual glitch
            renderer = GetComponent<SpriteRenderer>();
        }
        public override void LoopStarted()
        {
            base.LoopStarted();
            anim.enabled = true;
        }

        // Called both when the pedestrian reaches a collider or when the trafficlight changes
        public override void CheckMovingConditions()
        {
            PrintInfoCheckMovingConditions();
            
            if (CanMove && trafficArea) // This flag takes offsetTime condition into account
            {
                if (!MovingConditions()) return; // If moving conditions not fulfilled
                if (MustStop())
                    Stop();

                else if (MustRun())
                    Run();

                else
                    NormalMove();
            }
        }

        #region Movement methods
        private void NormalMove()
        {
            //bool worthCallMoving = SwitchStopped(normalSpeed);
            ChangeSpeed(normalSpeed);
            /*if (worthCallMoving)*/
            Moving(true);

            anim.speed = 1f;
            anim.SetBool("isWalking", true);
        }

        private void Run()
        {
            //bool worthCallMoving = SwitchStopped(runningSpeed);
            ChangeSpeed(runningSpeed);
            /*if (worthCallMoving)*/
            Moving(true);

            anim.speed = 1.7f;
            anim.SetBool("isWalking", true);
        }

        private void Stop()
        {
            //    bool worthCallMoving = SwitchStopped(0);
            ChangeSpeedImmediately(0);
            /*if (worthCallMoving)*/
            Moving(false);
            anim.SetBool("isWalking", false);
        }
        #endregion

        #region Condition checks

        public bool MustStop() =>
            trafficArea.StopArea && trafficArea.SameDirection(transform.up) && (trafficLight.IsGreen || trafficLight.IsYellow);

        public bool MustRun() => trafficLight.IsGreen && trafficArea.IsCenter;
        public bool MovingConditions() => trafficArea && respectsTheRules && hasStartedMoving;
        #endregion

        public override void GameSpeedChanged(GameSpeed state)
        {
            base.GameSpeedChanged(state);
            anim.speed = (float)state;
        }

        internal void BeRunOver()
        {
            anim.enabled = false;
            renderer.sprite = ranOverSprite;
        }

        private void PrintInfoCheckMovingConditions()
        {
            string moreInfo = (trafficArea
                ? $"After the method the vehicle will be {(MustStop() ? "Stopped" : MustRun() ? "Running" : "at Walking speed")}"
                  + $"\nExplanation:\nIs an stop area ({trafficArea.StopArea}) affecting us (same dir)({trafficArea.SameDirection(RoadUserDir)}) in red ({trafficLight.IsRed})(then must stop)? -> {MustStop()}.\n"
                  + $"Is yellow ({trafficLight.IsYellow}) and middle ({trafficArea.IsCenter}) OR before ({trafficArea.SameDirection(RoadUserDir)}) a cross OR IsRed ({trafficLight.IsRed} and Center ({trafficArea.IsCenter})? -> {MustRun()}.\n"
                  + $"Otherwise? {!MustStop() && !MustRun()}"
                : "");
            Print($"[{name}] [CheckMovingConditions] MovingConditions? {MovingConditions()}: "
                  + $"(hasStartedMoving?: {hasStartedMoving} respectsRules?: {respectsTheRules}  {(trafficArea ? $"has ({trafficArea.name})" : "doesn't have ")}a traffic area)\n"
                  + moreInfo, VerboseEnum.Speed);
        }
    }
}
