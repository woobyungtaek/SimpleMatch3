using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurveMatrix : MonoBehaviour
{
    private static Matrix4x4 bezierQuadraticMatrix = new Matrix4x4(
        new Vector4(1, -2, 1, 0),
        new Vector4(-2, 2, 0, 0),
        new Vector4(1, 0, 0, 0),
        new Vector4(0, 0, 0, 0)
        );
    private static Matrix4x4 bezierCubicMatrix = new Matrix4x4(
        new Vector4(-1, 3, -3, 1),
        new Vector4(3, -6, 3, 0),
        new Vector4(-3, 3, 0, 0),
        new Vector4(1, 0, 0, 0)
        );

    public static void CreateBezierCurveQuadratic(ref Matrix4x4 matrix, Vector3 start, Vector3 point, Vector3 goal)
    {
        matrix.SetColumn(0, start);
        matrix.SetColumn(1, point);
        matrix.SetColumn(2, goal);
        matrix.SetColumn(3, Vector3.zero);
        matrix *= bezierQuadraticMatrix.transpose;
    }
    public static void CreateBezierCurveCubic(ref Matrix4x4 matrix, Vector3 start, Vector3 point1, Vector3 point2, Vector3 goal)
    {
        matrix.SetColumn(0, start);
        matrix.SetColumn(1, point1);
        matrix.SetColumn(2, point2);
        matrix.SetColumn(3, goal);
        matrix *= bezierCubicMatrix.transpose;
    }
    
}
