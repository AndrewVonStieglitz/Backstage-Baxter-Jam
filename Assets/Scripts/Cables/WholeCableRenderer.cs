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
            for (var nodeIndex = 0; nodeIndex < Nodes.Count - 1; nodeIndex++)
            {
                points.AddRange(PointsBetweenPositions(Nodes[nodeIndex], Nodes[nodeIndex + 1]));
            }

            // Connection between the last node and the player
            // points.AddRange(PlayerSegmentPoints());

            // var points3D = SetZPositionsWithSlope(points).ToList();

            var points3D = SetZPositionsWithTriangleWave(points).ToList();
            
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

                var node = Nodes[Mathf.FloorToInt(pointIndex / (float) pointsBetweenNodes)];

                OrientationUtil.OrientedVector2 vector = new OrientationUtil.OrientedVector2(nextPoint - point);
            
                float vectorSlope;

                // Prevent division by zero
                var nodeOrientation = NodeOrientation(node).Inverse();
                
                if (vector.X(nodeOrientation) == 0)
                {
                    vectorSlope = 20;
                }
                else
                {
                    vectorSlope = vector.Y(nodeOrientation) / vector.X(nodeOrientation);
                }

                points3D.Add(new Vector3(point.x, point.y, (Sigmoid(vectorSlope) - 0.5f) * 30));
            }

            return points3D;
        }

        private IEnumerable<Vector3> SetZPositionsWithTriangleWave(List<Vector2> points)
        {
            var points3D = new List<Vector3>();
            
            
            for (var pointIndex = 0; pointIndex < points.Count - 1; pointIndex++)
            {
                var point = points[pointIndex];

                var indexInSegment = pointIndex % pointsBetweenNodes;

                var evenSegment = Mathf.FloorToInt(pointIndex / (float)pointsBetweenNodes) == 0;

                var t = (float) indexInSegment / pointsBetweenNodes;

                float zPos = 0;

                if (evenSegment)
                {
                    zPos = Mathf.Abs(2 * t - 1) - 1;
                }
                else
                {
                    zPos = -1 * (Mathf.Abs(2 * t - 1) - 1);
                }
                
                points3D.Add(new Vector3(point.x, point.y, zPos));
            }

            return points3D;
        }

        private static float Sigmoid(double value) { return 1.0f / (1.0f + (float) Math.Exp(-value)); }
    }
}