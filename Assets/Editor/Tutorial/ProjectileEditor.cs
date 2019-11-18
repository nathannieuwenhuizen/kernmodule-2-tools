using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Projectile))]
public class ProjectileEditor : Editor
{
    [DrawGizmo(GizmoType.Selected | GizmoType.NonSelected)]
    static void DrawGizmosSelected(Projectile projectile, GizmoType gizmoType)
    {
        Gizmos.DrawSphere(projectile.transform.position, 0.125f);
    }

    void OnSceneGUI()
    {
        var projectile = target as Projectile;
        var transform = projectile.transform;
        projectile.damageRadius = Handles.RadiusHandle(
            transform.rotation,
            transform.position,
            projectile.damageRadius);
    }
}