using System;
using UnityEngine;

namespace Game.Orbits
{
    /// <summary>
    /// A three-dimensional vector class with double precision. Use this class
    /// instead of Unity's <see cref="Vector3"/> to minimize floating-point error.
    /// </summary>
    [Serializable]
    public struct Vector3d : IEquatable<Vector3d>
    {
        private const double kEpsilon = 1e-15d;  // Vector3.kEpsilon = 1e-05f

#pragma warning disable IDE1006 // Naming Styles

        #region Static Properties
        public static Vector3d zero { get { return new Vector3d(0d, 0d, 0d); } }
        public static Vector3d one { get { return new Vector3d(1d, 1d, 1d); } }
        public static Vector3d up { get { return new Vector3d(0d, 1d, 0d); } }
        public static Vector3d down { get { return new Vector3d(0d, -1d, 0d); } }
        public static Vector3d left { get { return new Vector3d(-1d, 0d, 0d); } }
        public static Vector3d right { get { return new Vector3d(1d, 0d, 0d); } }
        public static Vector3d forward { get { return new Vector3d(0d, 0d, 1d); } }
        public static Vector3d back { get { return new Vector3d(0d, 0d, -1d); } }
        public static Vector3d positiveInfinity { get { return new Vector3d(double.PositiveInfinity, double.PositiveInfinity, double.PositiveInfinity); } }
        public static Vector3d negativeInfinity { get { return new Vector3d(double.NegativeInfinity, double.NegativeInfinity, double.NegativeInfinity); } }
        public static Vector3d NaN { get { return new Vector3d(double.NaN, double.NaN, double.NaN); } }
        #endregion

        #region Properties

        /// <summary>
        /// Returns the length of this vector (Read Only).
        /// </summary>
        /// <value>The magnitude.</value>
        public double magnitude { get { return Math.Sqrt(x * x + y * y + z * z); } }

        /// <summary>
        /// Returns this vector with a magnitude of 1 (Read Only).
        /// </summary>
        /// <value>The normalized.</value>
        public Vector3d normalized { get { return Vector3d.Normalize(this); } }

        /// <summary>
        /// Returns the squared length of this vector (Read Only).
        /// </summary>
        /// <value>The sqr magnitude.</value>
        public double sqrMagnitude { get { return x * x + y * y + z * z; } }

#pragma warning restore IDE1006 // Naming Styles

        /// <summary>
        /// Access the x, y, z components using [0], [1], [2] respectively.
        /// </summary>
        /// <param name="index">Index.</param>
        public double this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return x;
                    case 1: return y;
                    case 2: return z;
                    default:
                        throw new IndexOutOfRangeException("Invalid Vector3d index!");
                }
            }
            set
            {
                switch (index)
                {
                    case 0: x = value; break;
                    case 1: y = value; break;
                    case 2: z = value; break;
                    default:
                        throw new IndexOutOfRangeException("Invalid Vector3d index!");
                }
            }
        }

        /// <summary>
        /// X component of the vector.
        /// </summary>
        public double x;

        /// <summary>
        /// Y component of the vector.
        /// </summary>
        public double y;

        /// <summary>
        /// Z component of the vector.
        /// </summary>
        public double z;

        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new vector with given x, y, z components.
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="z">The z coordinate.</param>
        public Vector3d(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        /// <summary>
        /// Creates a new vector with given x, y components and sets z to zero.
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        public Vector3d(double x, double y)
        {
            this.x = x;
            this.y = y;
            z = 0d;
        }

        /// <summary>
        /// Creates a new vector from given Vector3.
        /// </summary>
        public Vector3d(Vector3 vector3)
        {
            x = vector3.x;
            y = vector3.y;
            z = vector3.z;
        }

        /// <summary>
        /// Creates a new vector from given Vector2 and sets z to zero.
        /// </summary>
        public Vector3d(Vector2 vector2)
        {
            x = vector2.x;
            y = vector2.y;
            z = 0d;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Determines whether the specified <see cref="object"/> is exactly equal to the current <see cref="T:OrbitPhysics.Vector3d"/>.
        /// </summary>
        /// <param name="obj">The <see cref="object"/> to compare with the current <see cref="T:OrbitPhysics.Vector3d"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="object"/> is exactly equal to the current
        /// <see cref="T:OrbitPhysics.Vector3d"/>; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            if (!(obj is Vector3d))
            {
                return false;
            }
            return Equals((Vector3d)obj);
        }

        /// <summary>
        /// Determines whether the specified <see cref="Vector3d"/> is exactly equal to the current <see cref="T:OrbitPhysics.Vector3d"/>.
        /// </summary>
        /// <param name="other">The <see cref="Vector3d"/> to compare with the current <see cref="T:OrbitPhysics.Vector3d"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="Vector3d"/> is exactly equal to the current
        /// <see cref="T:OrbitPhysics.Vector3d"/>; otherwise, <c>false</c>.</returns>
        public bool Equals(Vector3d other)
        {
            return x.Equals(other.x) && y.Equals(other.y) && z.Equals(other.z);
        }

        /// <summary>
        /// Set x, y and z components of an existing <see cref="Vector3d"/>.
        /// </summary>
        /// <param name="newX">New x.</param>
        /// <param name="newY">New y.</param>
        /// <param name="newZ">New z.</param>
        public void Set(double newX, double newY, double newZ)
        {
            x = newX;
            y = newY;
            z = newZ;
        }

        /// <summary>
        /// Returns a nicely formatted <see cref="T:System.String"/> that represents the current <see cref="T:OrbitPhysics.Vector3d"/>.
        /// </summary>
        /// <returns>A <see cref="T:System.String"/> that represents the current <see cref="T:OrbitPhysics.Vector3d"/>.</returns>
        public override string ToString()
        {
            return string.Format("({0}; {1}; {2})", x, y, z);
        }

        #endregion

        #region Static Methods

        /// <summary>
        /// Returns the angle in radians between <paramref name="from"/> and <paramref name="to"/>.
        /// </summary>
        /// <remarks>
        /// The angle returned is the unsigned angle between the two vectors.
        /// This means the smaller of the two possible angles between the two
        /// vectors is used. The result is never greater than π (i.e. 180 degrees).
        /// </remarks>
        /// <returns>The angle in radians between the two vectors.</returns>
        /// <param name="from">The vector from which the angular difference is measured.</param>
        /// <param name="to">The vector to which the angular difference is measured.</param>
        public static double Angle(Vector3d from, Vector3d to)
        {
            double dot = Dot(from.normalized, to.normalized);
            return Math.Acos(dot < -1.0 ? -1.0 : (dot > 1.0 ? 1.0 : dot)) * 57.29578d;
        }

        /// <summary>
        /// Returns a copy of <paramref name="vector"/> with its magnitude
        /// clamped to <paramref name="maxLength"/>.
        /// </summary>
        public static Vector3d ClampMagnitude(Vector3d vector, double maxLength)
        {
            if (vector.sqrMagnitude > maxLength * maxLength)
            {
                return vector.normalized * maxLength;
            }
            return vector;
        }

        /// <summary>
        /// Cross Product of two vectors.
        /// </summary>
        public static Vector3d Cross(Vector3d lhs, Vector3d rhs)
        {
            return new Vector3d(lhs.y * rhs.z - lhs.z * rhs.y,
                                lhs.z * rhs.x - lhs.x * rhs.z,
                                lhs.x * rhs.y - lhs.y * rhs.x);
        }

        /// <summary>
        /// Returns the distance between <paramref name="a"/> and <paramref name="b"/>.
        /// </summary>
        /// <remarks><see cref="Vector3d.Distance(Vector3d, Vector3d)"/>
        /// is the same as (a-b)<see cref="Vector3d.magnitude"/></remarks>
        public static double Distance(Vector3d a, Vector3d b)
        {
            Vector3d vector = new Vector3d(a.x - b.x, a.y - b.y, a.z - b.z);
            return Math.Sqrt(vector.x * vector.x + vector.y * vector.y + vector.z * vector.z);
        }

        /// <summary>
        /// Dot Product of two vectors.
        /// </summary>
        public static double Dot(Vector3d lhs, Vector3d rhs)
        {
            return lhs.x * rhs.x + lhs.y * rhs.y + lhs.z * rhs.z;
        }

        /// <summary>
        /// Linearly interpolates between two vectors.
        /// </summary>
        public static Vector3d Lerp(Vector3d a, Vector3d b, double t)
        {
            t = t < 0 ? 0 : (t < 1.0 ? 1.0 : t);
            return new Vector3d(a.x + (b.x - a.x) * t,
                                a.y + (b.y - a.y) * t,
                                a.z + (b.z - a.z) * t);
        }

        /// <summary>
        /// Linearly interpolates between two vectors without clamping the interpolant
        /// </summary>
        public static Vector3d LerpUnclamped(Vector3d a, Vector3d b, double t)
        {
            return new Vector3d(a.x + (b.x - a.x) * t,
                                a.y + (b.y - a.y) * t,
                                a.z + (b.z - a.z) * t);
        }

        /// <summary>
        /// Returns a vector that is made from the largest componenets of two vectors.
        /// </summary>
        public static Vector3d Max(Vector3d lhs, Vector3d rhs)
        {
            return new Vector3d(Math.Max(lhs.x, rhs.x), Math.Max(lhs.y, rhs.y), Math.Max(lhs.z, rhs.z));
        }

        /// <summary>
        /// Returns a vector that is made from the smallest componenets of two vectors.
        /// </summary>
        public static Vector3d Min(Vector3d lhs, Vector3d rhs)
        {
            return new Vector3d(Math.Min(lhs.x, rhs.x), Math.Min(lhs.y, rhs.y), Math.Min(lhs.z, rhs.z));
        }

        /// <summary>
        /// Moves <paramref name="current"/> in a straight line towards a 
        /// <paramref name="target"/> point.
        /// </summary>
        /// <param name="current"></param>
        public static Vector3d MoveTowards(Vector3d current, Vector3d target, double maxDistanceDelta)
        {
            Vector3d toVector = target - current;
            double dist = toVector.magnitude;
            if (dist <= maxDistanceDelta || dist < double.Epsilon)
            {
                return target;
            }
            return current + toVector / dist * maxDistanceDelta;
        }

        /// <summary>
        /// Normalize this instance.
        /// </summary>
        public static Vector3d Normalize(Vector3d value)
        {
            double mag = value.magnitude;
            if (mag > kEpsilon)
            {
                value = value / mag;
            }
            else
            {
                value = zero;
            }
            return value;
        }

        /// <summary>
        /// Projects a vector onto another vector.
        /// </summary>
        public static Vector3d Project(Vector3d vector, Vector3d onNormal)
        {
            double sqrMag = Dot(onNormal, onNormal);
            if (sqrMag < 1.40129846432482E-45d)
            {
                return zero;
            }
            return onNormal * Dot(vector, onNormal) / sqrMag;
        }

        /// <summary>
        /// Projects a vector onto a plane defined by a normal orthogonal to the plane
        /// </summary>
        public static Vector3d ProjectOnPlane(Vector3d vector, Vector3d planeNormal)
        {
            return vector - Project(vector, planeNormal);
        }

        /// <summary>
        /// Reflects a vector off the plane defined by a <paramref name="inNormal"/>.
        /// </summary>
        public static Vector3d Reflect(Vector3d inDirection, Vector3d inNormal)
        {
            return -2d * Dot(inNormal, inDirection) * inNormal + inDirection;
        }

        /// <summary>
        /// Multiplies two vectors component-wise.
        /// </summary>
        public static Vector3d Scale(Vector3d a, Vector3d b)
        {
            return new Vector3d(a.x * b.x, a.y * b.y, a.z * b.z);
        }

        /// <summary>
        /// Returns the signed angle in radians between <paramref name="from"/> and <paramref name="to"/>.
        /// </summary>
        public static double SignedAngle(Vector3d from, Vector3d to, Vector3d axis)
        {
            double unsignedAngle = Angle(from, to);
            double sign = Math.Sign(Dot(axis, Cross(from, to)));
            return unsignedAngle * sign;
        }

        /// <summary>
        /// Gradually changes a vector towards a desired goal over time.
        /// </summary>
        public static Vector3d SmoothDamp(Vector3d current, Vector3d target, ref Vector3d currentVelocity, double smoothTime)
        {
            double deltaTime = Time.deltaTime;
            double maxSpeed = double.PositiveInfinity;
            return SmoothDamp(current, target, ref currentVelocity, smoothTime, maxSpeed, deltaTime);
        }

        /// <summary>
        /// Gradually changes a vector towards a desired goal over time.
        /// </summary>
        public static Vector3d SmoothDamp(Vector3d current, Vector3d target, ref Vector3d currentVelocity, double smoothTime, double maxSpeed, double deltaTime)
        {
            smoothTime = Math.Max(0.0001d, smoothTime);
            double omega = 2d / smoothTime;

            double x = omega * deltaTime;
            double exp = 1d / (1d + x + 0.479999989271164d * x * x + 0.234999999403954d * x * x * x);
            Vector3d change = current - target; // vector
            Vector3d originalTo = target;       // vector3_1

            double maxChange = maxSpeed * smoothTime;
            change = ClampMagnitude(change, maxChange);
            target = current - change;

            Vector3d temp = (currentVelocity + omega * change) * deltaTime;
            currentVelocity = (currentVelocity - omega * temp) * exp;
            Vector3d output = target + (change + temp) * exp;

            if (Dot(originalTo - current, output - originalTo) > 0)
            {
                output = originalTo;
                currentVelocity = (output - originalTo) / deltaTime;
            }

            return output;
        }

        #endregion

        #region Operators

        /// <summary>
        /// Adds two vectors.
        /// </summary>
        public static Vector3d operator +(Vector3d a, Vector3d b)
        {
            return new Vector3d(a.x + b.x, a.y + b.y, a.z + b.z);
        }

        /// <summary>
        /// Subtracts one vector from another.
        /// </summary>
        public static Vector3d operator -(Vector3d a, Vector3d b)
        {
            return new Vector3d(a.x - b.x, a.y - b.y, a.z - b.z);
        }

        /// <summary>
        /// Negates the vector (i.e. multiplies by -1).
        /// </summary>
        public static Vector3d operator -(Vector3d a)
        {
            return new Vector3d(-a.x, -a.y, -a.z);
        }

        /// <summary>
        /// Multiplies a vector by a number.
        /// </summary>
        public static Vector3d operator *(Vector3d a, double d)
        {
            return new Vector3d(a.x * d, a.y * d, a.z * d);
        }

        /// <summary>
        /// Multiplies a vector by a number.
        /// </summary>
        public static Vector3d operator *(double d, Vector3d a)
        {
            return new Vector3d(a.x * d, a.y * d, a.z * d);
        }

        /// <summary>
        /// Divides a vector by a number.
        /// </summary>
        public static Vector3d operator /(Vector3d a, double d)
        {
            return new Vector3d(a.x / d, a.y / d, a.z / d);
        }

        public static bool operator ==(Vector3d lhs, Vector3d rhs)
        {
            return (lhs - rhs).sqrMagnitude < 0.0 / 1.0;
        }

        public static bool operator !=(Vector3d lhs, Vector3d rhs)
        {
            return (lhs - rhs).sqrMagnitude >= 0.0 / 1.0;
        }

        /// <summary>
        /// Converts a <see cref="Vector3d"/> to a <see cref="UnityEngine.Vector3"/>.
        /// </summary>
        public static explicit operator Vector3(Vector3d vector3d)
        {
            return new Vector3((float)vector3d.x, (float)vector3d.y, (float)vector3d.z);
        }

        /// <summary>
        /// Converts a <see cref="Vector3d"/> to a <see cref="UnityEngine.Vector2"/>.
        /// </summary>
        public static explicit operator Vector2(Vector3d vector3d)
        {
            return new Vector2((float)vector3d.x, (float)vector3d.y);
        }

        public override int GetHashCode()
        {
#pragma warning disable RECS0025 // Non-readonly field referenced in 'GetHashCode()'
            return x.GetHashCode() ^ y.GetHashCode() << 2 ^ z.GetHashCode();
#pragma warning restore RECS0025 // Non-readonly field referenced in 'GetHashCode()'
        }

        #endregion
    }
}