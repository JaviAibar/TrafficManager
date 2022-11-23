using BezierSolution;
using UnityEngine;

namespace Level
{
	[RequireComponent(typeof(Rigidbody2D))]
	public class BezierWalkerWithSpeedVariant : BezierWalkerWithSpeed
	{
		Rigidbody2D rb;
		private void Start()
		{
			rb = GetComponent<Rigidbody2D>();
		}
		public override void Execute(float deltaTime)
		{
			float targetSpeed = (isGoingForward) ? speed : -speed;

			Vector3 targetPos = spline.MoveAlongSpline(ref m_normalizedT, targetSpeed*Time.deltaTime);

			//rb.MovePosition(targetPos);
			//transform.position = targetPos;
			//transform.position = Vector3.Lerp( transform.position, targetPos, movementLerpModifier * deltaTime );
			rb.MovePosition(Vector2.Lerp(rb.position, (Vector2)targetPos, movementLerpModifier));
			bool movingForward = MovingForward;

			if (lookAt == LookAtMode.Forward)
			{
				BezierSpline.Segment segment = spline.GetSegmentAt(m_normalizedT);
				Quaternion targetRotation;
				if (movingForward)
					targetRotation = Quaternion.LookRotation(segment.GetTangent(), segment.GetNormal());
				else
					targetRotation = Quaternion.LookRotation(-segment.GetTangent(), segment.GetNormal());

				//transform.rotation = Quaternion.Lerp( transform.rotation, targetRotation, rotationLerpModifier * deltaTime );
				//transform.rotation = targetRotation;
				transform.up = segment.GetTangent();
			}
			else if (lookAt == LookAtMode.SplineExtraData)
				transform.rotation = Quaternion.Lerp(transform.rotation, spline.GetExtraData(m_normalizedT, extraDataLerpAsQuaternionFunction), rotationLerpModifier * deltaTime);

			if (movingForward)
			{
				if (m_normalizedT >= 1f)
				{
					if (travelMode == TravelMode.Once)
						m_normalizedT = 1f;
					else if (travelMode == TravelMode.Loop)
						m_normalizedT -= 1f;
					else
					{
						m_normalizedT = 2f - m_normalizedT;
						isGoingForward = !isGoingForward;
					}

					if (!onPathCompletedCalledAt1)
					{
						onPathCompletedCalledAt1 = true;
#if UNITY_EDITOR
						if (UnityEditor.EditorApplication.isPlaying)
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
				if (m_normalizedT <= 0f)
				{
					if (travelMode == TravelMode.Once)
						m_normalizedT = 0f;
					else if (travelMode == TravelMode.Loop)
						m_normalizedT += 1f;
					else
					{
						m_normalizedT = -m_normalizedT;
						isGoingForward = !isGoingForward;
					}

					if (!onPathCompletedCalledAt0)
					{
						onPathCompletedCalledAt0 = true;
#if UNITY_EDITOR
						if (UnityEditor.EditorApplication.isPlaying)
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
	}
}
