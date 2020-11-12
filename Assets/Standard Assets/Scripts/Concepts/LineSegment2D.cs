using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ClassExtensions;
using System;

[Serializable]
public class LineSegment2D
{
	public LineSegment2D (Vector2 start, Vector2 end)
	{
		Start = start;
		End = end;
		
		float slope = GetSlope();
		A = -slope;
		B = 1;
		C = Start.y - slope * Start.x;
	}
	
	public Vector2 Start { get; private set; }
	public Vector2 End { get; private set; }
	
	public float A { get; private set; }
	public float B { get; private set; }
	public float C { get; private set; }
	
	public override string ToString ()
	{
		return "[" + Start + "], [" + End + "]";
	}
	
	public float GetSlope ()
	{
		return (End.y - Start.y) / (End.x - Start.x);
	}
	
	public float GetFacingAngle ()
	{
		return (End - Start).GetFacingAngle();
	}
	
	public Vector2 GetIntersection (LineSegment2D other)
	{
		Vector2 output = VectorExtensions.NULL;
		float delta = A * other.B - other.A * B;
		if (delta != 0)
		{
			output.x = (other.B * C - B * other.C) / delta;
			output.y = (A * other.C - other.A * C) / delta;
			if (!ContainsPoint(output) || !other.ContainsPoint(output))
				output = VectorExtensions.NULL;
		}
		return output;
	}
	
	public bool ContainsPoint (Vector2 point)
	{
		return Vector2.Distance(point, Start) + Vector2.Distance(point, End) == Vector2.Distance(Start, End);
	}
	
	public LineSegment2D Move (Vector2 movement)
	{
		return new LineSegment2D(Start + movement, End + movement);
	}
	
	public LineSegment2D Rotate (Vector2 pivotPoint, float degrees)
	{
		LineSegment2D output = this;
		Vector2 outputStart = Start;
		outputStart = outputStart.Rotate(pivotPoint, degrees);
		Vector2 outputEnd = End;
		outputEnd = outputEnd.Rotate(pivotPoint, degrees);
		output = new LineSegment2D(outputStart, outputEnd);
		return output;
	}

	public Vector2 ClosestPoint (Vector2 point)
	{
		Vector2 output;
		float directedDistanceAlongParallel = GetDirectedDistanceAlongParallel(point);
		if (directedDistanceAlongParallel > 0 && directedDistanceAlongParallel < GetLength())
			output = GetPointWithDirectedDistance(directedDistanceAlongParallel);
		else if (directedDistanceAlongParallel >= GetLength())
			output = End;
		else
			output = Start;
		return output;
	}

	public LineSegment2D GetPerpendicular ()
	{
		return Rotate(GetMidpoint(), 90);
	}

	public Vector2 GetMidpoint ()
	{
		return (Start + End) / 2;
	}

	public float GetDirectedDistanceAlongParallel (Vector2 point)
	{
		float rotateLineBy = -GetFacingAngle();
		LineSegment2D rotatedLine = Rotate(Vector2.zero, rotateLineBy);
		point = point.Rotate(rotateLineBy);
		return point.x - rotatedLine.Start.x;
	}

	public Vector2 GetPointWithDirectedDistance (float directedDistance)
	{
		return Start + (GetDirection() * directedDistance);
	}

	public float GetLength ()
	{
		return Vector2.Distance(Start, End);
	}

	public Vector2 GetDirection ()
	{
		return (End - Start).normalized;
	}
}