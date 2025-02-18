namespace Excavator
{
    public static class ExcavatorConstants
    {
        public static class PhysicsConstants
        {
            public const float JointMovementThreshold = 0.15f;
            public const float IKControllerNormalizationRange = 5f;
            public const float SandLoadMass = 12000f;

            public static class Angles
            {
                public const float MinBoomAngle = -0.0885F;
                public const float MaxBoomAngle = 0.8400F;

                public const float MinStickAngle = 0.3826F;
                public const float MaxStickAngle = 0.9659F;

                public const float MinBucketAngle = -0.8657F;
                public const float MaxBucketAngle = 0.2164F;
            }
        }

        public static class IKConstants
        {
            public static class Angles
            {
                public const float MinBoomAngle = -360F;
                public const float MaxBoomAngle = 360F;

                public const float MinStickAngle = 30F;
                public const float MaxStickAngle = 135F;

                public const float MinBucketAngle = -25F;
                public const float MaxBucketAngle = 120F;
            }
        }
    }
}