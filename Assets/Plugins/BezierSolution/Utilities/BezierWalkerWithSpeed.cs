﻿using UnityEngine;
using UnityEngine.Events;

namespace BezierSolution
{
	[AddComponentMenu( "Bezier Solution/Bezier Walker With Speed" )]
	[HelpURL( "https://github.com/yasirkula/UnityBezierSolution" )]
	public class BezierWalkerWithSpeed : BezierWalker
	{
		public BezierSpline spline;
		public TravelMode travelMode;

		public float speed = 0f;
		[SerializeField]
		[Range( 0f, 1f )]
		protected float m_normalizedT = 0f;

		public override BezierSpline Spline {get { return spline; } }

		public override float NormalizedT
		{
			get { return m_normalizedT; }
			set { m_normalizedT = value; }
		}

		public float movementLerpModifier = 10f;
		public float rotationLerpModifier = 10f;

		public LookAtMode lookAt = LookAtMode.Forward;

		protected bool isGoingForward = true;
		public override bool MovingForward { get { return ( speed > 0f ) == isGoingForward; } }

		public UnityEvent onPathCompleted = new UnityEvent();
		protected bool onPathCompletedCalledAt1 = false;
		protected bool onPathCompletedCalledAt0 = false;

		private void FixedUpdate()
		{
			Execute( /*Time.deltaTime */ Time.fixedDeltaTime);
		}

		public override void Execute( float deltaTime )
		{
			float targetSpeed = ( isGoingForward ) ? speed : -speed;

			Vector3 targetPos = spline.MoveAlongSpline( ref m_normalizedT, targetSpeed * deltaTime );

			transform.position = targetPos;
			//transform.position = Vector3.Lerp( transform.position, targetPos, movementLerpModifier * deltaTime );

			bool movingForward = MovingForward;

			if( lookAt == LookAtMode.Forward )
			{
				BezierSpline.Segment segment = spline.GetSegmentAt( m_normalizedT );
				Quaternion targetRotation;
				if( movingForward )
					targetRotation = Quaternion.LookRotation( segment.GetTangent(), segment.GetNormal() );
				else
					targetRotation = Quaternion.LookRotation( -segment.GetTangent(), segment.GetNormal() );

				//transform.rotation = Quaternion.Lerp( transform.rotation, targetRotation, rotationLerpModifier * deltaTime );
				//transform.rotation = targetRotation;
				transform.up = segment.GetTangent();
			}
			else if( lookAt == LookAtMode.SplineExtraData )
				transform.rotation = Quaternion.Lerp( transform.rotation, spline.GetExtraData( m_normalizedT, extraDataLerpAsQuaternionFunction ), rotationLerpModifier * deltaTime );

			if( movingForward )
			{
				if( m_normalizedT >= 1f )
				{
					if( travelMode == TravelMode.Once )
						m_normalizedT = 1f;
					else if( travelMode == TravelMode.Loop )
						m_normalizedT -= 1f;
					else
					{
						m_normalizedT = 2f - m_normalizedT;
						isGoingForward = !isGoingForward;
					}

					if( !onPathCompletedCalledAt1 )
					{
						onPathCompletedCalledAt1 = true;
#if UNITY_EDITOR
						if( UnityEditor.EditorApplication.isPlaying )
#endif
							onPathCompleted.Invoke();
					}
				}
				else
				{
					onPathCompletedCalledAt1 = false;
				}
			}
			else
			{
				if( m_normalizedT <= 0f )
				{
					if( travelMode == TravelMode.Once )
						m_normalizedT = 0f;
					else if( travelMode == TravelMode.Loop )
						m_normalizedT += 1f;
					else
					{
						m_normalizedT = -m_normalizedT;
						isGoingForward = !isGoingForward;
					}

					if( !onPathCompletedCalledAt0 )
					{
						onPathCompletedCalledAt0 = true;
#if UNITY_EDITOR
						if( UnityEditor.EditorApplication.isPlaying )
#endif
							onPathCompleted.Invoke();
					}
				}
				else
				{
					onPathCompletedCalledAt0 = false;
				}
			}
		}

		public void SetAtStart()
		{
			transform.position = spline.GetPoint(0);
		}
	}
}