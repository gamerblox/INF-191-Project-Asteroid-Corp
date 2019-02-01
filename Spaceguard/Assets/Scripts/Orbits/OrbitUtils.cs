using System;
using UnityEngine;

namespace Game.Orbits
{
    /// <summary>
    /// Provides constants and conversion factors for trigonometric and other
    /// astrophysics values and measurements. Also contains static helper
    /// functions for calculations involved in orbital mechanics.
    /// </summary>
    public static class OrbitUtils
    {
        #region Constants
        /// <summary>
        /// The value of π = 3.14159265358979...
        /// </summary>
        public const double PI = Math.PI;

        /// <summary>
        /// The value of 2π = 6.28318530718...
        /// </summary>
        public const double TwoPI = PI * 2.0;

        /// <summary>
        /// Julian Day Number, Barycentric Dynamical Time on J2000.0.
        /// </summary>
        public const double J2000 = 2451545.0;

        /// <summary>
        /// Newton's gravitational constant G = 6.67259E-20 km^3 kg^-1 s^-2 [au^3 kg^-1 d^-2] 
        /// </summary>
        /// <remarks>6.67259E-20 km^3 kg^-1 s^-2</remarks>
        public const double G = 6.67408E-20; // km^3 kg^-1 s^-2
        //public const double G = 1.487803887307307E-34; // au^3 kg^-1 d^-2

        /// <summary>
        /// Keplerian standard gravitational parameter, μ, for Earth-Sun [au^3 d^-2]. 
        /// </summary>
        /// <remarks>1.3271283864237474E+11 km^3 s^-2</remarks>
        public const double GM_EARTH_SUN = 1.3271283864237474E+11; // km^3 s^-2
        //public const double GM_EARTH_SUN = 2.9591309705483544E-04; // au^3 d^-2

        /// <summary>
        /// Keplerian standard gravitational parameter, μ, for Asteroid-Sun [au^3 d^-2].
        /// </summary>
        /// <remarks>1.32712440041930E+11 km^3 s^-2</remarks>
        public const double GM_ASTEROID_SUN = 1.32712440041930E+11; // km^3 s^-2
        //public const double GM_ASTEROID_SUN = 2.9591220828559093E-04; // au^3 d^-2

        /// <summary>
        /// Standard gravitational parameter, μ, for Earth [km^3 s^-2].
        /// </summary>
        public const double GM_EARTH = 3.986004418E+05; // km^3 s^-2

        /// <summary>
        /// Standard gravitational parameter, μ, for the Sun [au^3 d^-2].
        /// </summary>
        /// <remarks>1.32712440018E+11 km^3 s^-2</remarks>
        public const double GM_SUN = 1.32712440018E+11; // km^3 s^-2
        //public const double GM_SUN = 2.9591220823221300E-04; // au^3 d^-2

        public const double MASS_SUN = 1.988919445342813E+30; // kg
        public const double MASS_EARTH = 5.974057670868442E+24; // kg

        /// <summary>
        /// The equatorial radius of Earth [km].
        /// </summary>
        public const double RADIUS_EARTH = 6.3781E+03;  // km
        public const double LUNAR_DIST = 384402.0; // km

        public const double EARTH_CAPTURE_DIST = RADIUS_EARTH + 1.5 * RADIUS_EARTH; // km

        /// <summary>
        /// The radius of the Sun [km].
        /// </summary>
        public const double RADIUS_SUN = 6.95700E+05;   // km

        /// <summary>
        /// Kilometers to Earth-radii conversion constant.
        /// </summary>
        public const double KM2ER = 1 / RADIUS_EARTH;

        /// <summary>
        /// AU-to-km conversion constant.
        /// </summary>
        public const double AU2KM = 149597870.700;

        /// <summary>
        /// km-to-AU conversion constant.
        /// </summary>
        public const double KM2AU = 1.0 / AU2KM;

        /// <summary>
        /// Days-to-seconds conversion constant.
        /// </summary>
        public const double DAY2SEC = 86400.0;

        /// <summary>
        /// Seconds-to-days conversion constant.
        /// </summary>
        public const double SEC2DAY = 1.0 / DAY2SEC;

        /// <summary>
        /// Degrees-to-radians conversion constant.
        /// </summary>
        public const double Deg2Rad = TwoPI / 360.0;

        /// <summary>
        /// Radians-to-degrees conversion constant.
        /// </summary>
        public const double Rad2Deg = 1.0 / Deg2Rad;

        #endregion

        #region Methods

        /// <summary>
        /// Solves Kepler's Equation, M = E - e sin E, for eccentric anomaly, E,
        /// using Newton's method.
        /// </summary>
        /// <returns>Eccentric anomaly, E [rad].</returns>
        /// <param name="ecc">Eccentricity</param>
        /// <param name="M">Mean anomaly [rad].</param>
        /// <param name="tol">The tolerance level for convergence.</param>
        /// <param name="maxIterations">Maximum number of iterations to attempt.</param>
        public static double SolveKeplerEqForEccentricAnom(double ecc, double M,
                                                           double tol = 2e-8,
                                                           int maxIterations = 8)
        {
            double TOL = tol; // The tolerance level.
            int MAX_ITER = maxIterations;

            M = NormalizeAngle(M, -PI, PI);
            double E0 = M + ecc * Math.Sin(M);
            int n = 0; // iteration counter

            // Iterate with n = 0, 1, 2, ..., until |deltaE| <= TOL.
            double E = E0;
            double deltaE, deltaM;

            do
            {
                deltaM = M - (E - ecc * Math.Sin(E));
                deltaE = deltaM / (1 - ecc * Math.Cos(E));
                E = E + deltaE;
                if (n > MAX_ITER - 1)
                {
                    Debug.LogWarningFormat("Kepler Equation failed to converge: E = {0:F5}, n = {1}", E, n + 1);
                    return E;
                }

            } while (Math.Abs(deltaE) > TOL && n++ < MAX_ITER);

            return E;
        }

        /// <summary>
        /// Calculates the change in velocity of an object from an impact/deflection
        /// with a smaller body (e.g. kinetic impactor spacecraft) in the ecliptic plane. 
        /// </summary>
        /// <returns>The delta V of <paramref name="deflected"/> in the ecliptic.</returns>
        /// <param name="deflected">Orbit of the object being deflected at time of impact [km-s].</param>
        /// <param name="impactor">Orbit of the impacting object at time of impact [km-s].</param>
        /// <param name="massDeflected">Mass of the object being deflected [kg].</param>
        /// <param name="massImpactor">Mass of the impacting object [kg].</param>
        /// <param name="beta">The momentum enhancement factor from ejecta (1 = no enhacement).</param>
        public static Vector3d CalculateDeflectionDeltaVEcliptic(OrbitData deflected, OrbitData impactor,
                                                                 double massDeflected, double massImpactor, double beta = 1.0)
        {
            // Velocity of the impactor relative to the object being deflected.
            Vector3d vrel = impactor.Velocity - deflected.Velocity;
            //Vector3d vhat = deflected.Velocity.normalized;
            //Vector3d vrelhat = vrel.normalized;

            //double Vimp = vrel.magnitude;
            //double VimpDotP = Vector3d.Dot(vhat, vrelhat);

            // Delta V from deflection, in the ecliptic plane.
            return vrel * beta * massImpactor / massDeflected;
        }

        /// <summary>
        /// Calculates the change in velocity of an object from an impact/deflection
        /// with a smaller body (e.g. kinetic impactor spacecraft) in the as (ΔVA, ΔVC, ΔVN).
        /// </summary>
        /// <remarks>
        /// The ACN coordinate system is a coordinate system set up at the center of mass
        /// of the asteroid at the time of deflection and used to express the magnitude
        /// and direction of the velocity change imparted to the asteroid. Its pricipal
        /// directions are:
        ///     Along-track direction (A): in the asteroid's orbital plane and along the
        ///         direction of the asteroid's current velocity vector about the Sun,
        ///     Normal direction (N): perpendicular to the asteroid's orbit plane and is
        ///         aligned with the positive orbital angular momentum vector (<c>r x v</c>)
        ///         where r is the vector from the Sun to the asteroid's current position,
        ///     Cross-track direction (C): in the orbital plane, perpendicular to the
        ///         along-track direction (<c>C = N x A</c>), and positive in the general
        ///         direction of the Sun.
        /// References: NDA Handbook v6.3
        ///             Transformation from ecliptic to ACN coordinate system
        ///             provided by Jason Anderson of Aerospace 07-2018.
        /// </remarks>
        /// <returns>The delta V of <paramref name="deflected"/> in the ACN coordinate system.</returns>
        /// <param name="deflected">Orbit of the object being deflected at time of impact [km-s].</param>
        /// <param name="impactor">Orbit of the impacting object at time of impact [km-s].</param>
        /// <param name="massDeflected">Mass of the object being deflected [kg].</param>
        /// <param name="massImpactor">Mass of the impacting object [kg].</param>
        /// <param name="beta">The momentum enhacement factor from ejecta (1 = no enhancement).</param>
        public static Vector3d CalculateDeflectionDeltaVInAcn(OrbitData deflected, OrbitData impactor, 
                                                              double massDeflected, double massImpactor, double beta = 1.0)
        {
            Vector3d deltaVEcliptic = CalculateDeflectionDeltaVEcliptic(deflected, impactor, massDeflected, massImpactor, beta);

            // Velocity unit vector of object to deflect.
            Vector3d vhat = deflected.Velocity.normalized;

            // Angular momentum vector unit vector: hhat = pos x vel
            Vector3d hhat = Vector3d.Cross(deflected.Position, deflected.Velocity).normalized;

            Vector3d along = vhat;
            Vector3d normal = hhat.normalized;
            Vector3d cross = Vector3d.Cross(normal, along);

            double va = Vector3d.Dot(deltaVEcliptic, along);
            double vc = Vector3d.Dot(deltaVEcliptic, cross);
            double vn = Vector3d.Dot(deltaVEcliptic, normal);

            return new Vector3d(va, vc, vn);
        }

        // TODO: This function does not behave as expected and/or has not been successfully tested/implemented.
        public static void GetPositionsAlongEllipse(ref Vector3[] positions, OrbitData orbit, int resolution)
        {
            if (resolution < 2)
            {
                positions = new Vector3[0];
                return;
            }

            double a = orbit.SemiMajorAxis;
            double ecc = orbit.Eccentricity;
            double w = orbit.ArgumentOfPerifocus;
            double nu = orbit.TrueAnomaly;

            // Trigometrics for any values used more than once in calculations
            double cosI = Math.Cos(orbit.Inclination);
            double cosOm = Math.Cos(orbit.LongitudeOfAscendingNode);
            double sinOm = Math.Sin(orbit.LongitudeOfAscendingNode);

            double angle = TwoPI / resolution;

            if (ecc < 1.0 && ecc >= 0.0)
            {
                if (positions == null || positions.Length != resolution)
                {
                    positions = new Vector3[resolution];
                }

                for (int j = 0; j < resolution; j++)
                {
                    // Radial distance [km]
                    double r = orbit.SemiMajorAxis * (1.0 - ecc * ecc) / (1.0 + ecc * Math.Cos(j * angle));

                    // Flight path angle [rad]
                    //double fpa = Math.Atan2(1.0 + ecc * Math.Cos(nu), ecc * Math.Sin(nu));
                    double fpa = Math.Atan(ecc * Math.Sin(nu) / (1.0 + ecc * Math.Cos(nu)));

                    double cosNuPlusW = Math.Cos(nu + w);
                    double sinNuPlusW = Math.Sin(nu + w);

                    float x = (float)(r * (cosNuPlusW * cosOm - sinNuPlusW * cosI * sinOm));
                    float y = (float)(r * (cosNuPlusW * sinOm + sinNuPlusW * cosI * cosOm));
                    float z = (float)(r * (sinNuPlusW * Math.Sin(orbit.Inclination)));

                    positions[j] = new Vector3(x, y, z);
                }
            }
            else
            {
                Debug.LogWarning("Cannot draw orbital ellipse: orbit is parabolic/hyperbolic");
            }
        }

        /// <summary>
        /// The planetocentric velocity for a small body encountering a planet
        /// that moves on a circular orbit around the Sun.
        /// </summary>
        /// <remarks>
        /// To simplify the formula, use a system of units such that the distance of the 
        /// planet to the Sun is 1 and the period is 2PI. Assume both the mass of the 
        /// Sun and the gravitational constant are equal to 1. Disregard the mass of
        /// the planet in the heliocentric orbit of both the planet and the small body,
        /// thus the heliocentric velocity of the planet is also 1.
        /// 
        /// The planetocentric reference plane: Y-axis coincides with the direction of
        /// motion of the planet, and the Sun is on the negative X-axis.
        /// </remarks>
        /// <returns>The velocity.</returns>
        /// <param name="a">The alpha component.</param>
        /// <param name="ecc">Ecc.</param>
        /// <param name="i">The index.</param>
        public static Vector3d PlanetocentricVelocity(double a, double ecc, double i)
        {
            double term = a * (1 - ecc * ecc);
            double x = Math.Sqrt(2 - (1 / a) - term);
            double y = Math.Sqrt(term) * Math.Cos(i) - 1;
            double z = Math.Sqrt(term) * Math.Sin(i);
            Vector3d U = new Vector3d(x, y, z);

            double u = Math.Sqrt(3 - (1 / a) - 2 * (Math.Sqrt(term) * Math.Cos(i)));
            Debug.Assert(Math.Abs(U.magnitude - u) <= double.Epsilon);
            Debug.Assert(Math.Abs(Math.Sqrt(3 - TisserandParameter(a, ecc, i))) - u <= double.Epsilon);

            return U;
        }

        public static double TisserandParameter(double a, double ecc, double i)
        {
            return (1 / a) + 2 * Math.Sqrt(a * (1 - ecc * ecc)) * Math.Cos(i);
        }

        public static Vector3d IncomingAsymptoteDirection(Vector3d U)
        {
            double theta = U.y / U.magnitude;
            double phi = U.x / U.z;
            double sinTheta = Math.Sin(theta);
            return U.magnitude * new Vector3d(sinTheta * Math.Sin(phi), 
                                              Math.Cos(theta), 
                                              sinTheta * Math.Cos(phi));
        }

        /// <summary>
        /// Gets the 3x2 transformation matrix from orbital to ecliptic plane.
        /// The returned transform can be used to transform a position and
        /// velocity vector from the orbital plane to the ecliptic plane.
        /// </summary>
        /// <returns>The transform to ecliptic.</returns>
        /// <param name="i">The inclination [rad].</param>
        /// <param name="Om">The longitude of the ascending node [rad].</param>
        /// <param name="w">The argument of pericenter [rad].</param>
        public static double[,] GetTransformToEcliptic(double i, double Om, double w)
        {
            // Precalculate and store terms used in transformations.
            double sinI = Math.Sin(i);
            double cosI = Math.Cos(i);
            double sinOm = Math.Sin(Om);
            double cosOm = Math.Cos(Om);
            double sinW = Math.Sin(w);
            double cosW = Math.Cos(w);
            double sinWsinOm = sinW * sinOm;
            double sinWcosOm = sinW * cosOm;
            double cosWsinOm = cosW * sinOm;
            double cosWcosOm = cosW * cosOm;

            double[,] T = new double[3, 2];
            T[0, 0] =  cosWcosOm - sinWsinOm * cosI;
            T[0, 1] = -sinWcosOm - cosWsinOm * cosI;
            T[1, 0] =  cosWsinOm + sinWcosOm * cosI;
            T[1, 1] = -sinWsinOm + cosWcosOm * cosI;
            T[2, 0] =  sinW * sinI;
            T[2, 1] =  cosW * sinI;

            return T;
        }

        /// <summary>
        /// Gets the 3x2 transformation matrix from orbital to ecliptic plane.
        /// The returned transform can be used to transform a position and
        /// velocity vector from the orbital plane to the ecliptic plane.
        /// </summary>
        /// <returns>The transform to ecliptic.</returns>
        /// <param name="i">The inclination [rad].</param>
        /// <param name="Om">The longitude of the ascending node [rad].</param>
        /// <param name="w">The argument of pericenter [rad].</param>
        /// <param name="nu">The true anomaly at epoch [rad].</param>
        public static double[,] GetTransformToEcliptic(double i, double Om, double w, double nu)
        {
            // Precalculate and store terms used in transformations.
            double sinI = Math.Sin(i);
            double cosI = Math.Cos(i);
            double sinOm = Math.Sin(Om);
            double cosOm = Math.Cos(Om);
            double sinW = Math.Sin(w + nu);
            double cosW = Math.Cos(w + nu);
            double sinWsinOm = sinW * sinOm;
            double sinWcosOm = sinW * cosOm;
            double cosWsinOm = cosW * sinOm;
            double cosWcosOm = cosW * cosOm;

            double[,] T = new double[3, 2];
            T[0, 0] = cosWcosOm - sinWsinOm * cosI;
            T[0, 1] = -sinWcosOm - cosWsinOm * cosI;
            T[1, 0] = cosWsinOm + sinWcosOm * cosI;
            T[1, 1] = -sinWsinOm + cosWcosOm * cosI;
            T[2, 0] = sinW * sinI;
            T[2, 1] = cosW * sinI;

            return T;
        }

        /// <summary>
        /// Applies the matrix transformation to the point (x, y) and 
        /// returns the resultant vector.
        /// </summary>
        /// <returns>The transformed vector.</returns>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="T">The transformation matrix.</param>
        public static Vector3d ApplyTransform(double x, double y, double[,] T)
        {
            if (T.GetLength(0) != 3 || T.GetLength(1) != 2)
            {
                Debug.LogError("Transformation matrix must be 3x2");
                return Vector3d.zero;
            }
            double Tx = T[0, 0] * x + T[0, 1] * y;
            double Ty = T[1, 0] * x + T[1, 1] * y;
            double Tz = T[2, 0] * x + T[2, 1] * y;
            return new Vector3d(Tx, Ty, Tz);
        }

        /// <summary>
        /// Returns the eccentric anomaly from the mean anomaly.
        /// </summary>
        /// <returns>Eccentric anomaly [rad].</returns>
        /// <param name="M">Mean anomaly [rad].</param>
        /// <param name="ecc">Eccentricity.</param>
        public static double MeanAnom2EccentricAnom(double M, double ecc)
        {
            double E = M + ecc * Math.Sin(M) * (1.0 + ecc * Math.Cos(M));
            double E0 = E;
            double E1 = 0;
            // converge on a solution for eccentric anomaly (wont work for ecc near or above 1)
            for (int j = 0; j < 100; j++)
            {
                E1 = E0 - (E0 - ecc * Math.Sin(E0) - M) / (1 - ecc * Math.Cos(E0));
                if (Math.Abs(E1 - E0) < 0.001)
                    break;
                E0 = E1;
            }
            return E1;
        }

        /// <summary>
        /// Returns the true anomaly from the true anomaly.
        /// </summary>
        /// <returns>The eccentric anomaly [rad].</returns>
        /// <param name="nu">True anomaly [rad].</param>
        /// <param name="ecc">Eccentricity.</param>
        public static double TrueAnom2EccentricAnom(double nu, double ecc)
        {
            return 2 * Math.Atan2(Math.Tan(nu / 2), Math.Sqrt((1 + ecc) / (1 - ecc)));
        }

        /// <summary>
        /// Returns the mean anomaly from the eccentric anomaly.
        /// </summary>
        /// <returns>The eccentric anomaly [rad].</returns>
        /// <param name="E">Eccentric anomaly [rad].</param>
        /// <param name="ecc">Eccentricity.</param>
        public static double EccentricAnom2MeanAnom(double E, double ecc)
        {
            return E - ecc * Math.Sin(E);
        }

        /// <summary>
        /// Converts a date from its Julian Day Number (JDN) to a <see cref="System.DateTime"/>.
        /// </summary>
        /// <returns>The calendar date and time.</returns>
        /// <param name="J">The Julian Day Number, Barycentric Dynamical Time.</param>
        public static string JDN2CalendarString(double J)
        {
            if (J < 4480)
            {
                Debug.LogWarning("Conversion algorithm only valid for dates after JDN 4480");
            }

            int g = (int)Math.Floor((int)Math.Floor((J - 4479.5) / 36524.25) * 0.75 + 0.5) - 37;

            double N = J + g;

            // Year of common era (in the style with New Year on Jan 1)
            // and the day of the year reckoned from March 1 = 0 are given by:
            int A = (int)Math.Floor(N / 365.25) - 4712;
            int d = (int)Math.Floor((N - 59.25) % 365.25);

            // The month M (January = 1) and the day of the month D are found from d':
            int M = ((int)Math.Floor((d + 0.5) / 30.6) + 2) % 12 + 1;
            int D = (int)Math.Floor((d + 0.5) % 30.6) + 1;

            string month;
            switch (M)
            {
                case 1: month = "January"; break;
                case 2: month = "February"; break;
                case 3: month = "March"; break;
                case 4: month = "April"; break;
                case 5: month = "May"; break;
                case 6: month = "June"; break;
                case 7: month = "July"; break;
                case 8: month = "August"; break;
                case 9: month = "September"; break;
                case 10: month = "October"; break;
                case 11: month = "November"; break;
                case 12: month = "December"; break;
                default:
                    month = "??";
                    Debug.LogWarning("Month out of range " + M);
                    break;
            }
            return string.Format("{0}-{1}-{2}", A, month, D);
        }

        /// <summary>
        /// Converts a given Julian Day Number to a Gregorian date and returns
        /// the DateTime.
        /// </summary>
        /// <remarks>
        /// References: https://quasar.as.utexas.edu/BillInfo/JulianDatesG.html
        /// </remarks>
        /// <returns>The Gregorian date.</returns>
        /// <param name="J">Julian Day Number</param>
        public static DateTime JDN2DateTime(double J)
        {
            if (J < 4480)
            {
                Debug.LogWarning("Conversion algorithm only valid for dates after JDN 4480");
            }

            int g = (int)Math.Floor((int)Math.Floor((J - 4479.5) / 36524.25) * 0.75 + 0.5) - 37;

            double N = J + g;

            // Year of common era (in the style with New Year on Jan 1)
            // and the day of the year reckoned from March 1 = 0 are given by:
            int A = (int)Math.Floor(N / 365.25) - 4712;
            int d = (int)Math.Floor((N - 59.25) % 365.25);

            // The month M (January = 1) and the day of the month D are found from d':
            int M = ((int)Math.Floor((d + 0.5) / 30.6) + 2) % 12 + 1;
            int D = (int)Math.Floor((d + 0.5) % 30.6) + 1;

            DateTime date = new DateTime();
            try {
                date = new DateTime(A, M, D);
            }
            catch (Exception)
            {
                // Implementation does not seem to handle leap years correctly,
                // so first try and correct an invalid leap day Feb-29 => Feb-28
                try
                {
                    //Debug.LogFormat("Attempting correction on invalid DateTime: {0}-{1}-{2} (YYYY-MM-DD)", A, M, D);
                    date = new DateTime(A, M, D - 1);
                }
                catch (Exception e)
                {
                    Debug.LogWarningFormat("Unable to convert to DateTime: {0}-{1}-{2} (YYYY-MM-DD)", A, M, D - 1);
                    Debug.LogException(e);
                    Debug.DebugBreak();
                }
            }
            return date;
        }

        #endregion

        #region Private Helper Methods

        /// <summary>
        /// Normalizes an angle between a minimum and maximum double value.
        /// Convention is range (-PI, +PI] or [0, 2 PI) for inclusive/exclusive.
        /// </summary>
        /// <returns>The normalized angle.</returns>
        /// <param name="value">The value of the angle [rad].</param>
        /// <param name="min">The minimum {-PI or 0}.</param>
        /// <param name="min">The maximum {+PI or 2 PI}.</param>
        public static double NormalizeAngle(double value, double min, double max)
        {
            value = value - min;
            max = max - min;
            if (Math.Abs(max) < double.Epsilon)
            {
                return min;
            }

            value = value % max;
            value = value + min;
            if (min < 0)
            {
                while (value < min)
                {
                    value = value + max;
                }
            }
            else
            {
                while (value <= min)
                {
                    value = value + max;
                }
            }

            return value;
        }

        /// <summary>
        /// Calculates the mass of a sphere from its diameter and density.
        /// </summary>
        /// <remarks>
        /// Pay special attention to units for input parameters to ensure 
        /// the calculation works as expected. 
        /// </remarks>
        /// <returns>The of sphere.</returns>
        /// <param name="diameter">Diameter [km].</param>
        /// <param name="density">Density [g cm^-3].</param>
        public static double MassOfSphere(double diameter, double density)
        {
            double radius = (diameter / 2) * 1e5; // [km] -> [cm]
            double volume = (4.0 / 3.0) * (PI * Math.Pow(radius, 3)); // [cm^3]
            double mass = (density * volume) / 1000; // [g] -> [kg]

            return mass;
        }

        #endregion
    }
}
