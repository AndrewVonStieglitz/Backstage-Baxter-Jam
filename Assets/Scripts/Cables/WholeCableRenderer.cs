﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Cables
{
    public class WholeCableRenderer : CableRenderer
    {
        private void Update()
        {
            DrawLine();
        }
        
        private void DrawLine()
        {
            var points = new List<Vector2>();

            // Connections between nodes
            for (var nodeIndex = 0; nodeIndex < nodes.Count - 1; nodeIndex++)
            {
                var a = (Vector2) nodes[nodeIndex].transform.position;
                var b = (Vector2) nodes[nodeIndex + 1].transform.position;
                var o = nodes[nodeIndex].Orientation;
                
                points.AddRange(PointsBetweenPositions(a, b, o));
            }

            // Connection between the last node and the player
            points.AddRange(PlayerSegmentPoints(points));

            var points3D = SetZPositionsWithSlope(points).ToList();
            
            lineRenderer.positionCount = points3D.Count;
            lineRenderer.SetPositions(points3D.ToArray());
        }

        // TODO: Convert to a pure static function and move to base class
        // For every pair of points, use the slope to set the Z position
        private IEnumerable<Vector3> SetZPositionsWithSlope(List<Vector2> points)
        {
            var points3D = new List<Vector3>();
            
            for (var pointIndex = 0; pointIndex < points.Count - 1; pointIndex++)
            {
                var point = points[pointIndex];
                var nextPoint = points[pointIndex + 1];

                var node = nodes[Mathf.FloorToInt(pointIndex / (float) pointsBetweenPins)];

                OrientationUtil.OrientedVector2 vector = new OrientationUtil.OrientedVector2(nextPoint - point);
            
                float vectorSlope;

                // Prevent division by zero
                if (vector.X(node.Orientation) == 0)
                {
                    vectorSlope = 20;
                }
                else
                {
                    vectorSlope = vector.Y(node.Orientation) / vector.X(node.Orientation);
                }

                points3D.Add(new Vector3(point.x, point.y, (Sigmoid(vectorSlope) - 0.5f) * 30));
            }

            return points3D;
        }
        
        private static float Sigmoid(double value) { return 1.0f / (1.0f + (float) Math.Exp(-value)); }
    }
}