using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spline
{
    List<Vector3> data;
    List<Vector3> points; // control points

    public List<Vector3> Points { get { return points; } }

    public Spline(List<Vector3> points, List<Vector3> data)
    {
        this.data = data;
        this.points = points;
    }

    public Vector3 GeneratePoint(float t)
    {
        int ti = (int)t + 1;
        
        //Debug.Log("t : " + t);
        //Debug.Log("Data count: " + data.Count);
        //Debug.Log("Points count: " + points.Count);


        return (data[ti] * (t - ti + 1)) / 6 * (((t - ti + 1) * (t - ti + 1)) - 1) +
               (data[ti - 1] * (ti - t)) / 6 * (((ti - t) * (ti - t)) - 1) +
                points[ti]* (t - ti + 1) + points[ti - 1] * (ti - t);
    }

    public Vector3 GeneratePoint(int i, float time, float upper, float lower)
    { 
        int ti = i + 1; 
        float t = i + (time - lower) / (upper - lower);

        return (data[ti] * (t - ti + 1)) / 6 * (((t - ti + 1) * (t - ti + 1)) - 1) +
               (data[ti - 1] * (ti - t)) / 6 * (((ti - t) * (ti - t)) - 1) +
                points[ti]* (t - ti + 1) + points[ti - 1] * (ti - t);
    }

    Vector3 GenerateTangent(float t)
    {
        Vector3 result = new Vector3();
        int ti = (int)t + 1;

        result = (data[ti] / 6) * (3 * (t - ti + 1) * (t - ti + 1) - 1) -
                 (data[ti - 1] / 6) * (3 * (ti - t) * (ti - t) - 1) +
                 (points[ti] - points[ti - 1]);
        
        result.Normalize();

        return result;
    }

    Vector3 GenerateTangent(int i, float t)
    {
        Vector3 result = new Vector3();

        int ti = i + 1; 

        result = (data[ti] / 6) * (3 * (t - ti + 1) * (t - ti + 1) - 1) -
                 (data[ti - 1] / 6) * (3 * (ti - t) * (ti - t) - 1) +
                 (points[ti] - points[ti - 1]);

        result.Normalize();

        return result; 
    }

    Vector3 GenerateAcceleration(float t)
    {
        int ti = (int)t + 1;
        return data[ti] * (t - ti + 1) + data[ti - 1] * (ti - t);
    }

    Vector3 GenerateAcceleration(int i, float t)
    {
        int ti = i + 1;
        return data[ti] * (t - ti + 1) + data[ti - 1] * (ti - t);
    }
    // Vector3 GenerateBinormal(Vector3 tangent, float t)
    // {
    //     int ti = (int)t + 1;
        
    //     return Vector3.Cross(tangent, acceleration);
    // }

    Matrix4x4 GetTNBMatrix(Vector3 tangent, Vector3 normal, Vector3 binormal)
    {
        Matrix4x4 TNB = new Matrix4x4();
        
        TNB[0, 0] = normal.x;
        TNB[1, 0] = normal.y;
        TNB[2, 0] = normal.z;

        TNB[0, 1] = binormal.x;
        TNB[1, 1] = binormal.y;
        TNB[2, 1] = binormal.z;

        TNB[0, 2] = tangent.x;
        TNB[1, 2] = tangent.y;
        TNB[2, 2] = tangent.z;

        return TNB;
    }
    public Quaternion GenerateOrientation(float t)
    {
        Vector3 tangent = GenerateTangent(t);
        Vector3 acceleration = GenerateAcceleration(t);
        Vector3 binormal = Vector3.Cross(tangent, acceleration);

        binormal.Normalize();
        
        // ector3 binormal = GenerateBinormal(tangent, t);
        Vector3 normal = Vector3.Cross(binormal, tangent);
        // Matrix4x4 TNB = GetTNBMatrix(tangent, normal, binormal);
        
        Matrix4x4 TNB = new Matrix4x4();

        TNB[0, 0] = normal.x;
        TNB[1, 0] = normal.y;
        TNB[2, 0] = normal.z;

        TNB[0, 1] = binormal.x;
        TNB[1, 1] = binormal.y;
        TNB[2, 1] = binormal.z;

        TNB[0, 2] = tangent.x;
        TNB[1, 2] = tangent.y;
        TNB[2, 2] = tangent.z;

        Quaternion q = new Quaternion();

        float trace = normal.x + binormal.y + tangent.z;
       
        if( trace > 0 )
        {
            float s = 0.5f / Mathf.Sqrt(trace+ 1.0f);
            q.w = 0.25f / s;
            q.x = ( TNB[2, 1] - TNB[1, 2] ) * s;
            q.y = ( TNB[0, 2] - TNB[2, 0] ) * s;
            q.z = ( TNB[1, 0] - TNB[0, 1] ) * s;
        } 
        else if ( TNB[0, 0] > TNB[1, 1] && TNB[0, 0] > TNB[2, 2] ) 
        {
            float s = 2.0f * Mathf.Sqrt( 1.0f + TNB[0, 0] - TNB[1, 1] - TNB[2, 2]);
            q.w = (TNB[2, 1] - TNB[1, 2] ) / s;
            q.x = 0.25f * s;
            q.y = (TNB[0, 1] + TNB[1, 0] ) / s;
            q.z = (TNB[0, 2] + TNB[2, 0] ) / s;
        }
        else if (TNB[1, 1] > TNB[2, 2]) 
        {
            float s = 2.0f * Mathf.Sqrt( 1.0f + TNB[1, 1] - TNB[0, 0] - TNB[2, 2]);
            q.w = (TNB[0, 2] - TNB[2, 0] ) / s;
            q.x = (TNB[0, 1] + TNB[1, 0] ) / s;
            q.y = 0.25f * s;
            q.z = (TNB[1, 2] + TNB[2, 1] ) / s;
        } 
        else 
        {
            float s = 2.0f * Mathf.Sqrt( 1.0f + TNB[2, 2] - TNB[0, 0] - TNB[1, 1] );
            q.w = (TNB[1, 0] - TNB[0, 1] ) / s;
            q.x = (TNB[0, 2] + TNB[2, 0] ) / s;
            q.y = (TNB[1, 2] + TNB[2, 1] ) / s;
            q.z = 0.25f * s;
        }

        return q;
    }
    public Quaternion GenerateOrientation(int i, float t, float upper, float lower)
    {
        float time = i + (t - lower) / (upper - lower);
        Vector3 tangent = GenerateTangent(i, time);
        Vector3 acceleration = GenerateAcceleration(i, time);
        Vector3 binormal = Vector3.Cross(tangent, acceleration);

        binormal.Normalize();
        
        // ector3 binormal = GenerateBinormal(tangent, t);
        Vector3 normal = Vector3.Cross(binormal, tangent);
        // Matrix4x4 TNB = GetTNBMatrix(tangent, normal, binormal);
        
        Matrix4x4 TNB = new Matrix4x4();

        TNB[0, 0] = normal.x;
        TNB[1, 0] = normal.y;
        TNB[2, 0] = normal.z;

        TNB[0, 1] = binormal.x;
        TNB[1, 1] = binormal.y;
        TNB[2, 1] = binormal.z;

        TNB[0, 2] = tangent.x;
        TNB[1, 2] = tangent.y;
        TNB[2, 2] = tangent.z;

        Quaternion q = new Quaternion();

        float trace = normal.x + binormal.y + tangent.z;
       
        if( trace > 0 )
        {
            float s = 0.5f / Mathf.Sqrt(trace+ 1.0f);
            q.w = 0.25f / s;
            q.x = ( TNB[2, 1] - TNB[1, 2] ) * s;
            q.y = ( TNB[0, 2] - TNB[2, 0] ) * s;
            q.z = ( TNB[1, 0] - TNB[0, 1] ) * s;
        } 
        else if ( TNB[0, 0] > TNB[1, 1] && TNB[0, 0] > TNB[2, 2] ) 
        {
            float s = 2.0f * Mathf.Sqrt( 1.0f + TNB[0, 0] - TNB[1, 1] - TNB[2, 2]);
            q.w = (TNB[2, 1] - TNB[1, 2] ) / s;
            q.x = 0.25f * s;
            q.y = (TNB[0, 1] + TNB[1, 0] ) / s;
            q.z = (TNB[0, 2] + TNB[2, 0] ) / s;
        }
        else if (TNB[1, 1] > TNB[2, 2]) 
        {
            float s = 2.0f * Mathf.Sqrt( 1.0f + TNB[1, 1] - TNB[0, 0] - TNB[2, 2]);
            q.w = (TNB[0, 2] - TNB[2, 0] ) / s;
            q.x = (TNB[0, 1] + TNB[1, 0] ) / s;
            q.y = 0.25f * s;
            q.z = (TNB[1, 2] + TNB[2, 1] ) / s;
        } 
        else 
        {
            float s = 2.0f * Mathf.Sqrt( 1.0f + TNB[2, 2] - TNB[0, 0] - TNB[1, 1] );
            q.w = (TNB[1, 0] - TNB[0, 1] ) / s;
            q.x = (TNB[0, 2] + TNB[2, 0] ) / s;
            q.y = (TNB[1, 2] + TNB[2, 1] ) / s;
            q.z = 0.25f * s;
        }

        return q;
    }
}
