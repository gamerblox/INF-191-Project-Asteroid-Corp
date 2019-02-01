using System;
using UnityEngine;

namespace Game.Orbits
{
    /// <summary>
    /// A struct to hold the Lambert solution and computed transfer orbit.
    /// </summary>
    public class LambertSolution
    {
        /// <summary>
        /// The number of revolutions.
        /// </summary>
        public readonly int nRevs;
        /// <summary>
        /// The transfer angle [rad].
        /// </summary>
        public readonly double TA;
        /// <summary>
        /// The planar tranfer angle [rad].
        /// </summary>
        public readonly double pTA;
        /// <summary>
        /// Earth departure V-infinity.
        /// </summary>
        public readonly double Vinf;
        /// <summary>
        /// Launch energy C3.
        /// </summary>
        public readonly double C3;
        /// <summary>
        /// Object impact velocity.
        /// </summary>
        public readonly double Vimp;
        /// <summary>
        /// The arrival impact velocity vector.
        /// </summary>
        public readonly Vector3d arrDV;
        public readonly double VimpDotProduct;
        /// <summary>
        /// The transfer orbit.
        /// </summary>
        public readonly OrbitData TransferOrbit;

        public LambertSolution(int nRevs, double TA, double pTA, double Vinf, 
                               double C3, double Vimp, Vector3d arrDV, 
                               double VimpDotProduct, OrbitData transferOrbit)
        {
            this.nRevs = nRevs;
            this.TA = TA;
            this.pTA = pTA;
            this.Vinf = Vinf;
            this.C3 = C3;
            this.arrDV = arrDV;
            this.Vimp = Vimp;
            this.VimpDotProduct = VimpDotProduct;
            this.TransferOrbit = transferOrbit;
        }

        public string ToString(bool inDegrees)
        {
            double transferAngle = TA;
            double planarTransferAngle = pTA;
            if (inDegrees)
            {
                transferAngle *= OrbitUtils.Rad2Deg;
                planarTransferAngle *= OrbitUtils.Rad2Deg;
            }

            string s = "";
            s += string.Format("nRevs\t{0}\nTA\t{1:F5}\npTA\t{2:F5}\n",
                               nRevs, transferAngle, planarTransferAngle); //(inDegrees ? "deg" : "rad"));
            s += string.Format("Vinf\t{0:F5}\nC3\t{1:F5}\n", Vinf, C3);
            s += string.Format("ArrDV\n{0:E5}\n{1:E5}\n{2:E5}\n", arrDV.x, arrDV.y, arrDV.z);
            s += string.Format("Vimp\t{0:F5}\nVimp Dot Product\t{1:F5}", Vimp, VimpDotProduct);
            s += "Transfer Orbit:\n" + TransferOrbit.ToString(inDegrees);

            return s;
        }

        public override string ToString()
        {
            return ToString(false);
        }
    }

    public static class Lambert
    {
        /// <summary>
        /// Gets a new Lambert solution.
        /// </summary>
        /// <param name="from">Departure body orbit at time of impact.</param>
        /// <param name="to">Arrival body orbit at time of impact.</param>
        /// <param name="D">Number of days before impact the deflection is to occur.</param>
        /// <param name="TOF_days">Time of flight [days]</param>
        /// <param name="nRevs">Number of revolutions or -1 to search for optimal</param>
        public static LambertSolution GetTransferOrbit(OrbitData from, OrbitData to, 
                                          double D, double TOF_days, int nRevs = -1)
        {
            Debug.Assert(Math.Abs(from.Epoch - to.Epoch) <= double.Epsilon);

            // Make local copies of orbit parameters (passed by reference).
            OrbitData dep = new OrbitData(from);
            OrbitData arr = new OrbitData(to);
            dep.Step(-(D + TOF_days));
            arr.Step(-D);

            Vector3d pos1 = dep.Position;
            Vector3d vel1 = dep.Velocity;
            Vector3d pos2 = arr.Position;
            Vector3d vel2 = arr.Velocity;

            Vector3d[] depDV = new Vector3d[2];
            Vector3d[] arrDV = new Vector3d[2];
            double[] TA = new double[2];   // Transfer angle
            double[] Vinf = new double[2]; // Earth departure V-infinity
            double[] C3 = new double[2];

            int i;
            if (nRevs < 0)
            {
                // Search for the best number of revolutions...
                // Start the search with 0 and 1 revolutions.
                for (i = 0; i < 2; i++)
                {
                    nRevs = i;

                    // Calculate the Lambert arc from state1 to state2 in TOF days;
                    // Return the departure delta V and the arrival delta V.
                    CalcLambert(nRevs, pos1, vel1, pos2, vel2, TOF_days,
                                ref depDV[i], ref arrDV[i], out TA[i]);

                    // Earth departure V-infinity.
                    Vinf[i] = depDV[i].magnitude;
                    C3[i] = Vinf[i] * Vinf[i];
                    Debug.Log("Rev #" + nRevs + ": C3 = " + C3[i]);
                }

                // Storage index.
                i = 1;

                // Run until C3 does not improve.
                while (C3[i == 1 ? 1 : 0] < C3[i == 1 ? 0 : 1])
                {
                    // Alternate storing index.
                    i = (i == 0 ? 1 : 0);

                    // Next revolution.
                    nRevs++;

                    // Calculate the Lambert arc from state1 to state2 in TOF days;
                    // Return the departure delta V and the arrival delta V.
                    CalcLambert(nRevs, pos1, vel1, pos2, vel2, TOF_days,
                                ref depDV[i], ref arrDV[i], out TA[i]);

                    // Earth departure V-infinity.
                    Vinf[i] = depDV[i].magnitude;
                    C3[i] = Vinf[i] * Vinf[i];
                    Debug.Log("Rev #" + nRevs + ": C3 = " + C3[i]);
                }

                // Best solution is the previous iteration.
                nRevs--;
                i = (i == 0 ? 1: 0);
            }
            else
            {
                // Storage index.
                i = 0;

                // Calculate the Lambert arc from state1 to state2 in TOF days;
                // Return the departure delta V and the arrival delta V.
                CalcLambert(nRevs, pos1, vel1, pos2, vel2, TOF_days,
                            ref depDV[i], ref arrDV[i], out TA[i]);

                // Earth departure V-infinity.
                Vinf[i] = depDV[i].magnitude;
                C3[i] = Vinf[i] * Vinf[i];
            }

            if (depDV[i] == Vector3d.zero)
            {
                return null;
            }

            // Object impact velocity.
            double Vimp = arrDV[i].magnitude;

            // Planar transfer angle.
            double pTA = Math.Atan2(pos2.y, pos2.x) - Math.Atan2(pos1.y, pos1.x);
            if (pTA < 0.0)
            {
                pTA += OrbitUtils.TwoPI;
            }

            double VimpDotProduct = -arrDV[i].x * vel2.x - arrDV[i].y * vel2.y - arrDV[i].z * vel2.z / arrDV[i].magnitude / vel2.magnitude;

            vel1 += depDV[i]; // Transfer orbit velocity at departure
            OrbitData transferOrbit = new OrbitData(dep.Epoch, pos1, vel1, OrbitUtils.GM_SUN);

            return new LambertSolution(nRevs, TA[i], pTA, Vinf[i], C3[i], Vimp, 
                                     -arrDV[i], VimpDotProduct, transferOrbit);
        }

        /// <summary>
        /// Calculate the Lambert solution.
        /// </summary>
        /// <param name="nRevs">Number of Revolutions</param>
        /// <param name="pos1">Position 1</param>
        /// <param name="pos2">Position 2</param>
        /// <param name="TOF_days">Time of Flight [days]</param>
        /// <param name="depDV">Departure Delta V (Output)</param>
        /// <param name="arrDV">Arrival Delta V (Output)</param>
        /// <param name="TA">Transfer Angle (Output)</param>
        static bool CalcLambert(int nRevs,
                                Vector3d pos1, Vector3d vel1, Vector3d pos2, Vector3d vel2,
                                double TOF_days,
                                ref Vector3d depDV, ref Vector3d arrDV,
                                out double TA)
        {
            // Radial Distance and Velocity Magnitudes.
            double r1 = pos1.magnitude; // dRad1
            double v1 = vel1.magnitude; // dVel1
            double r2 = pos2.magnitude; // dRad2
            double v2 = vel2.magnitude; // dVel2

            // Angular Momentum Unit Vector.
            Vector3d hhat = Vector3d.Cross(pos1, pos2).normalized;

            // Calculate the transfer angle (make sure it is positive).
            TA = Math.Acos(Vector3d.Dot(pos1, pos2) / r1 / r2);
            while (TA < 0.0)
            {
                TA += OrbitUtils.TwoPI;
            }

            // Check for retrograde arc.
            if (hhat.z < 0.0)
            {
                // Flip the transfer direction.
                hhat = -hhat;
                // Flip the transfer angle.
                TA = OrbitUtils.TwoPI - TA;
            }

            // Working in radians only so won't convert to degrees here

            // Calculate the Tangential Directions.
            Vector3d tanhat1 = Vector3d.Cross(hhat, pos1);
            Vector3d tanhat2 = Vector3d.Cross(hhat, pos2);

            // Call Lambert Solver
            double[] radvel1 = new double[2];
            double[] tanvel1 = new double[2];
            double[] radvel2 = new double[2];
            double[] tanvel2 = new double[2];
            int nSolns, igtl;
            Gooding.lamrhg(OrbitUtils.GM_SUN, r1, r2, TA + nRevs * OrbitUtils.TwoPI, TOF_days * OrbitUtils.DAY2SEC,
                           out nSolns,
                           out radvel1[0], out tanvel1[0], out radvel2[0], out tanvel2[0],
                           out radvel1[1], out tanvel1[1], out radvel2[1], out tanvel2[1],
                           out igtl);

            Vector3d[] dv1 = new Vector3d[2];
            Vector3d[] dv2 = new Vector3d[2];

            if (nSolns > 0)
            {
                for (int i = 0; i < nSolns; i++)
                {
                    dv1[i] = radvel1[i] * pos1 / r1 +
                             tanvel1[i] * tanhat1 / r1 - vel1;
                    dv2[i] = vel2 -
                             radvel2[i] * pos2 / r2 -
                             tanvel2[i] * tanhat2 / r2;
                }

                // If multiple solutions, pick the lower Delta V.
                if ((nSolns == 2) &&
                    ((dv1[0].magnitude + dv2[0].magnitude) > (dv1[1].magnitude + dv2[1].magnitude)))
                {
                    depDV = dv1[1];
                    arrDV = dv2[1];
                }
                else
                {
                    depDV = dv1[0];
                    arrDV = dv2[0];
                }

                return true;
            }
            return false;
        }
    }
}
