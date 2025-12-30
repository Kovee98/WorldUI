using System;

namespace WorldUI {
    public enum WorldUILayoutType {
        Simple,
        Line,
        Spline
    }

    public enum WorldUIUpdaterMode {
        Self,
        Children,
        SelfAndChildren,
        All,
        Manual
    }

    public enum WorldUIAlignment {
        Start,
        Center,
        End
    }

    public enum WorldUISplineAlignmentType {
        Offset,
        Bounds
    }

    public enum WorldUISplineAlignment {
        Start,
        Center,
        End,
        Between,
        Around,
        Evenly
    }

    // think of as what to use for the base (i.e. if increasing x value, whose x-axis to use?)
    /*
        World   - use world x-axis (using target as the origin, or Vector3.zero if target is null)
        Local   - use x-axis of target (using target as the origin, or Vector3.zero if target is null)
        Self    - use my own x-axis (using target as the origin, or Vector3.zero if target is null)
        Screen  - use the camera's x-axis for pixel-perfect positioning (does not use target - uses the main camera behind the scenes)
    */
    public enum WorldUITransformControllerPositionMode {
        World,
        Local,
        Self,
        Screen
    }

    public enum WorldUITransformControllerScreenAnchors {
        TopLeft,
        TopCenter,
        TopRight,

        MiddleLeft,
        MiddleCenter,
        MiddleRight,

        BottomLeft,
        BottomCenter,
        BottomRight
    }

    public enum WorldUITransformControllerRotationMode {
        World,
        Local,
        Match,
        LookAt,
        LookAway,
        LookAtScreen,
        LookAwayFromScreen,
    }

    public enum WorldUITransformControllerScaleMode {
        World,
        Match,
        Screen  // constant size on the screen
    }

    [Flags]
    public enum WorldUIRendererFilter {
        None = 0,
        BillboardRenderer = 1 << 0,
        LineRenderer = 1 << 1,
        MeshRenderer = 1 << 2,
        ParticleSystemRenderer = 1 << 3,
        SkinnedMeshRenderer = 1 << 4,
        SpriteMask = 1 << 5,
        SpriteRenderer = 1 << 6,
        TrailRenderer = 1 << 7,
        SpriteShapeRenderer = 1 << 8,
        Disabled = 1 << 9
    }

    public enum WorldUIBorderLocation {
        // faces
        TopFace,
        BottomFace,
        LeftFace,
        RightFace,
        FrontFace,
        BackFace,

        // corners
        TopFrontLeft,
        TopFrontRight,
        TopBackLeft,
        TopBackRight,

        BottomFrontLeft,
        BottomFrontRight,
        BottomBackLeft,
        BottomBackRight,

        // edges
        TopFrontEdge,
        TopLeftEdge,
        TopRightEdge,
        TopBackEdge,

        BottomFrontEdge,
        BottomLeftEdge,
        BottomRightEdge,
        BottomBackEdge,

        MiddleFrontLeftEdge,
        MiddleFrontRightEdge,
        MiddleBackLeftEdge,
        MiddleBackRightEdge
    }
}
