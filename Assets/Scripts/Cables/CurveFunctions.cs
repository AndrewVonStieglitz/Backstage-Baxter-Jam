using System;
using UnityEngine;

namespace Cables
{
    public static class CurveFunctions
    {
        public enum CurveFunction { Straight, Sine, Catenary, RightAngleCubic, TangentQuartic }

        public static Vector2 SinLerp(Vector2 a, Vector2 b, float t, OrientationUtil.Orientation o)
        {
            if (a == b) return a;

            OrientationUtil.OrientedVector2 oa = new OrientationUtil.OrientedVector2(a);
            OrientationUtil.OrientedVector2 ob = new OrientationUtil.OrientedVector2(b);
        
            OrientationUtil.OrientedVector2 diff = new OrientationUtil.OrientedVector2(b - a);

            OrientationUtil.OrientedVector2 oSinPoint;

            // Prevent division by zero
            if (diff.X(o) == 0)
            {
                oSinPoint = new OrientationUtil.OrientedVector2(
                    oa.X(o),
                    Mathf.Lerp(oa.Y(o), ob.Y(o), t)
                );
            }
            else
            {
                float x = Mathf.Lerp(oa.X(o), ob.X(o), t);
            
                oSinPoint = new OrientationUtil.OrientedVector2(
                    x,
                    diff.Y(o) / -2 * Mathf.Cos((x - oa.X(o)) * Mathf.PI / diff.X(o)) + diff.Y(o) / 2 + oa.Y(o)
                );
            }

            var sinPoint = new Vector2(oSinPoint.X(o), oSinPoint.Y(o));

            return sinPoint;
        }

        // https://gist.github.com/Farfarer/a765cd07920d48a8713a0c1924db6d70
        public static Vector2 CatenaryLerp(Vector2 start, Vector2 end, float t, float m_slack)
        {
            Vector3 m_start = start;
            Vector3 m_end = end;
        
            float lineDist = Vector3.Distance (m_end, m_start);
            float lineDistH = Vector3.Distance (new Vector3(m_end.x, m_start.y, m_end.z), m_start);
            float l = lineDist + Mathf.Max(0.0001f, m_slack);
            float r = 0.0f;
            float s = m_start.y;
            float u = lineDistH;
            float v = m_end.y;

            if ((u-r) == 0.0f)
                return m_start;

            float ztarget = Mathf.Sqrt(Mathf.Pow(l, 2.0f) - Mathf.Pow(v-s, 2.0f)) / (u-r);

            int loops = 30;
            int iterationCount = 0;
            int maxIterations = loops * 10; // For safety.
            bool found = false;

            float z = 0.0f;
            float ztest = 0.0f;
            float zstep = 100.0f;
            float ztesttarget = 0.0f;
            for (int i = 0; i < loops; i++) {
                for (int j = 0; j < 10; j++) {
                    iterationCount++;
                    ztest = z + zstep;
                    ztesttarget = (float)Math.Sinh(ztest)/ztest;

                    if (float.IsInfinity (ztesttarget))
                        continue;

                    if (ztesttarget == ztarget) {
                        found = true;
                        z = ztest;
                        break;
                    } else if (ztesttarget > ztarget) {
                        break;
                    } else {
                        z = ztest;
                    }

                    if (iterationCount > maxIterations) {
                        found = true;
                        break;
                    }
                }

                if (found)
                    break;
			
                zstep *= 0.1f;
            }

            float a = (u-r)/2.0f/z;
            float p = (r+u-a*Mathf.Log((l+v-s)/(l-v+s)))/2.0f;
            float q = (v+s-l*(float)Math.Cosh(z)/(float)Math.Sinh(z))/2.0f;
        
            var stepf = t;
            Vector3 pos = Vector3.zero;
            pos.x = Mathf.Lerp(m_start.x, m_end.x, stepf);
            pos.z = Mathf.Lerp(m_start.z, m_end.z, stepf);
            pos.y = a * (float)Math.Cosh(((stepf*lineDistH)-p)/a)+q;

            return (Vector2) pos;
        }

        public static Vector2 BezierLerp(Vector2 a, Vector2 b, float t)
        {
            Vector2 handle;

            if (b.y < a.y)
            {
                handle = new Vector2(a.x, b.y);
            }
            else
            {
                handle = new Vector2(b.x, a.y);
            }

            Vector2 aHandle = Vector2.Lerp(a, handle, t);
            Vector2 handleB = Vector2.Lerp(handle, b, t);
        
            return Vector2.Lerp(aHandle, handleB, t);
        }
    }
}