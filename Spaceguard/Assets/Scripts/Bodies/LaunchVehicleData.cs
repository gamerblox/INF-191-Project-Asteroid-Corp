using System;
using UnityEngine;

namespace Game.Bodies
{
    [CreateAssetMenu(menuName = "Launch Vehicle Type")]
    public class LaunchVehicleData : ScriptableObject
    {
        [Header("Curve-Fitting C")]
        /// <summary>
        /// Coefficient, a, of the curve-fitting equation a * e^-(((C3-b)/c)^2)
        /// that approximates plot data based on launch vehicle performance.
        /// </summary>
        /// <remarks>
        /// Coefficients a, b, and c are not behicle performance parameters.
        /// References: Coefficients were provided by Jason Anderson
        /// </remarks>
        public double a;
       
        /// <summary>
        /// Coefficient, b, of the curve-fitting equation a * e^-(((C3-b)/c)^2)
        /// that approximates plot data based on launch vehicle performance.
        /// </summary>
        /// <remarks>
        /// Coefficients a, b, and c are not behicle performance parameters.
        /// References: Coefficients were provided by Jason Anderson
        /// </remarks>
        public double b;

        /// <summary>
        /// Coefficient, c, of the curve-fitting equation a * e^-(((C3-b)/c)^2)
        /// that approximates plot data based on launch vehicle performance.
        /// </summary>
        /// <remarks>
        /// Coefficients a, b, and c are not behicle performance parameters.
        /// References: Coefficients were provided by Jason Anderson
        /// </remarks>
        public double c;

        /// <summary>
        /// The maximum value for C3 for which it is valid to extrapolate.
        /// </summary>
        public double curveFittingMaxC3;

        [Header("Linear Interpolation")]
        /// <summary>
        /// Use high-end linear interpolation for this launch vehicle?
        /// </summary>
        public bool useLinearInterpolation;

        /// <summary>
        /// The point x0 (C3) on the line from point (x0, y0) to (x1, y1) defining 
        /// the linear interpolation, where the x-axis is the C3 and the y-axis 
        /// is the deliverable mass.
        /// </summary>
        public double x0_C3;

        /// <summary>
        /// The point y0 (mass) on the line from point (x0, y0) to (x1, y1) defining
        /// the linear interpolation, where the x-axis is the C3 and the y-axis 
        /// is the deliverable mass.
        /// </summary>
        public double y0_mass;

        /// <summary>
        /// The point x1 (C3) on the line from point (x0, y0) to (x1, y1) defining
        /// the linear interpolation, where the x-axis is the C3 and the y-axis 
        /// is the deliverable mass.
        /// </summary>
        public double x1_C3;

        /// <summary>
        /// The point y1 (mass) on the line from point (x0, y0) to (x1, y1) defining
        /// the linear interpolation, where the x-axis is the C3 and the y-axis 
        /// is the deliverable mass.
        /// </summary>
        public double y1_mass;
       
        /// <summary>
        /// Don't interpolate beyond this value.
        /// </summary>
        public double linearInterpMaxC3;

        /// <summary>
        /// Computes the deliverable mass for the launch vehicle given the
        /// launch energy C3 and the number of (instantaneous) launches 
        /// </summary>
        /// <returns>The deliverable payload mass.</returns>
        /// <param name="C3">C3.</param>
        /// <param name="numLaunches">Number of (instantaneous) launches.</param>
        public double ComputeDeliverableMass(double C3, int numLaunches = 1)
        {
            double mass;

            if (!useLinearInterpolation)
            {
                if (C3 > curveFittingMaxC3)
                {
                    mass = 0.0;
                }
                else 
                {
                    mass = a * Math.Exp(-Math.Pow((C3 - b) / c, 2));
                }
            }
            else
            {
                if (C3 <= curveFittingMaxC3)
                {
                    mass = a * Math.Exp(-Math.Pow((C3 - b) / c, 2));
                }
                else if (C3 > linearInterpMaxC3)
                {
                    mass = 0.0;
                }
                else 
                {
                    double t = (C3 - x0_C3) / (x1_C3 - x0_C3);
                    mass = (1 - t) * y0_mass + t * y1_mass;
                }
            }

            mass *= numLaunches;

            return mass > 0.0 ? mass : 0.0;
        }

        public double ComputeMaxC3ForMass(double mass)
        {
            double t = (mass - y0_mass) / (y1_mass - y0_mass);
            return (1 - t) * x0_C3 + t * x1_C3;
        }
    }
}