using System;

namespace Game.Orbits
{
    public static class Gooding
    {
        static double EighthRoot(double x)
        {
            return Math.Sqrt(Math.Sqrt(Math.Sqrt(x)));
        }

        /// <summary>
        /// A Lambert Solver.
        /// </summary>
        /// <remarks>
        /// This subroutine is a Lambert solver 
        /// by Robert H. Gooding of the Royal Aerospace 
        /// Establishment, Farnborough, Hants, England.
        /// It has been tested and slightly improved by Allan Klumpp of JPL 1990.
        /// References: R.H.Gooding, "A Procedure for the Solution of Lambert's
        ///             Orbital Boundary Value Problem," Celestial Mechanics
        ///             and Dynamic Astronomy 48, 1990.
        ///             Allan Klumpp, "Performance Comparison of Lambert
        ///             and Kepler Algorithms,"  JPL Interoffice Memo
        ///             314.1-0426-ARK, January 10, 1991.
        /// Recoded from the appendix of the Klumpp report by B.N.Lundberg 5-18-2004.
        /// Explicit typing and other (non-algorithmic) changes made by BNL for efficiency and appearance.
        /// Converted from Fortran to C by J.Anderson 8-03-2005.
        /// Converted from C to C# for Unity 2018.1 by K.M.Hylbom 6-12-2018.
        /// </remarks>
        /// <param name="gm">Gravitational constant [L^3/sec^2].</param>
        /// <param name="r1">Initial radial distance [L].</param>
        /// <param name="r2">Final radial distance [L].</param>
        /// <param name="th">Transfer angle [rad].</param>
        /// <param name="tdelt">Transfer time [sec].</param>
        /// <param name="n">Number of solutions found {0, 1, 2}.</param>
        /// <param name="vr11">Radial Velocity Component 1 for soln 1.</param>
        /// <param name="vt11">Tangential Velocity Component 1 for soln 1.</param>
        /// <param name="vr12">Radial Velocity Component 1 for soln 2.</param>
        /// <param name="vt12">Tangential Velocity Component 1 for soln 2.</param>
        /// <param name="vr21">Radial Velocity Component 2 for soln 1.</param>
        /// <param name="vt21">Tangential Velocity Component 2 for soln 1.</param>
        /// <param name="vr22">Radial Velocity Component 2 for soln 2.</param>
        /// <param name="vt22">Tangential Velocity Component 2 for soln 2.</param>
        /// <param name="igtl">Counter for calls to lamgtl.</param>
        public static void lamrhg(double gm, // Gravitational constant  [L^3/sec^2]
                           double r1,        // Initial radial distance [L]
                           double r2,        // Final radial distance   [L]
                           double th,        // th    - Transfer angle  [rad]
                           double tdelt,     // tdelt - Transfer time   [sec]
                           out int n,        // Number of solutions found {0, 1, 2}
                           out double vr11,  //     Radial Velocity Component 1 for soln 1
                           out double vt11,  // Tangential Velocity Component 1 for soln 1
                           out double vr12,  //     Radial Velocity Component 1 for soln 2
                           out double vt12,  // Tangential Velocity Component 1 for soln 2
                           out double vr21,  //     Radial Velocity Component 2 for soln 1
                           out double vt21,  // Tangential Velocity Component 2 for soln 1
                           out double vr22,  //     Radial Velocity Component 2 for soln 2
                           out double vt22,  // Tangential Velocity Component 2 for soln 2
                           out int igtl)     // Counter for calls to lamgtl 
        {
            // Set Iteration Counter.
            igtl = 0;

            // Initialize out parameters.
            vr11 = 0;
            vt11 = 0;
            vr12 = 0;
            vt12 = 0;
            vr21 = 0;
            vt21 = 0;
            vr22 = 0;
            vt22 = 0;

            // The next lines were added by Klumpp.
            double thr2 = th;
            int m = 0;
            while (thr2 > OrbitUtils.TwoPI)
            {
                thr2 -= OrbitUtils.TwoPI;
                m++;
            }
            thr2 /= 2.0;

            double dr = r1 - r2;
            double r1r2 = r1 * r2;
            double sthr2 = Math.Sin(thr2);
            double cthr2 = Math.Cos(thr2);
            double r1r2th = 4.0 * r1r2 * sthr2 * sthr2;
            double csq = dr * dr + r1r2th;
            double c = Math.Sqrt(csq);
            double s = (r1 + r2 + c) / 2.0;
            double gms = Math.Sqrt(gm * s / 2.0);
            double qsqfm1 = c / s;
            double q = Math.Sqrt(r1r2) * cthr2 / s;

            double rho, sig;
            if (Math.Abs(c) > double.Epsilon)
            {
                rho = dr / c;
                sig = r1r2th / csq;
            }
            else
            {
                rho = 0.0;
                sig = 1.0;
            }

            double t = 4.0 * gms * tdelt / (s * s);

            double x1, x2;
            lamgxl(m, q, qsqfm1, t, out n, out x1, out x2, ref igtl);
           
            double x;
            double unused, qzminx, qzplx, zplqx;
            double vr1, vt1, vr2, vt2;

            for (int i = 0; i < n; i++)
            {
                x = (i == 0 ? x1 : x2);

                lamgtl(m, q, qsqfm1, x, -1, out unused, out qzminx, out qzplx, out zplqx, ref igtl);
                
                vt2 = gms * zplqx * Math.Sqrt(sig);
                vr1 = gms * (qzminx - qzplx * rho) / r1;
                vt1 = vt2 / r1;
                vr2 = -gms * (qzminx + qzplx * rho) / r2;
                vt2 = vt2 / r2;

                if (i == 0)
                {
                    vr11 = vr1;
                    vt11 = vt1;
                    vr12 = vr2;
                    vt12 = vt2;
                }
                else
                {
                    vr21 = vr1;
                    vt21 = vt1;
                    vr22 = vr2;
                    vt22 = vt2;
                }
            }
        }

        /// <summary>
        /// This subroutine is for use with lamrhg, a Lambert solver.
        /// <remarks>
        /// This subroutine is for use with lamrhg, a Lambert solver
        /// by Robert H. Gooding of the Royal Aerospace 
        /// Establishment, Farnborough, Hants, England.
        /// It has been tested and slightly improved by Allan Klumpp of JPL 1990.
        /// References: R.H.Gooding, "A Procedure for the Solution of Lambert's
        ///             Orbital Boundary Value Problem," Celestial Mechanics
        ///             and Dynamic Astronomy 48, 1990.
        ///             Allan Klumpp, "Performance Comparison of Lambert
        ///             and Kepler Algorithms,"  JPL Interoffice Memo
        ///             314.1-0426-ARK, January 10, 1991.
        /// Recoded from the appendix of the Klumpp report by B.N.Lundberg 5-18-2004.
        /// Explicit typing and other (non-algorithmic) changes made by BNL for efficiency and appearance.
        /// Converted from Fortran to C by J.Anderson 8-03-2005.
        /// Converted from C to C# for Unity 2018.1 by K.M.Hylbom 6-12-2018.
        /// </remarks>
        /// <param name="m">Number of revolutions</param>
        /// <param name="tin">Time of flight [sec]</param>
        /// <param name="n">Number of solutions found {0, 1, 2}</param>
        /// <param name="igtl">Counter for calls to lamgtl</param>
        static void lamgxl(int m,           // Number of revs
                           double q,
                           double qsqfm1,
                           double tin,      // Time of flight [sec]
                           out int n,       // Number of solutions found {0, 1, 2}
                           out double x,
                           out double xpl,
                           ref int igtl)    // Counter for calls to lamgtl
        {
            const double tol = 3.0e-7;
            const double c0 = 1.70;
            const double c1 = 0.50;
            const double c2 = 0.030;
            const double c3 = 0.150;
            const double c41 = 1.0;
            const double c42 = 0.240;

            bool jumpto3 = false; // Bool flag added to replace a goto.

            // Initialize out parameters.
            x = 0;
            xpl = 0;

            double thr2 = Math.Atan2(qsqfm1, 2.0 * q) / Math.PI;

            double t0, dt, d2t, d3t, tdiff;

            // Initialize lamgtl out parameters.
            double xm = 0;
            double tmin = 0;
            double tdiffm = 0;
            double d2t2 = 0;

            if (m == 0)
            {
                // Single-rev starter from t(at x = 0) & bilinear (usually).
                n = 1;

                lamgtl(m, q, qsqfm1, 0.0, 0, out t0, out dt, out d2t, out d3t, ref igtl);

                tdiff = tin - t0;

                if (tdiff <= 0.0)
                {
                    x = t0 * tdiff / (-4.0 * tin);
                }
                else
                {
                    x = -tdiff / (tdiff + 4.0);
                    double w = x + c0 * Math.Sqrt(2.0 * (1.0 - thr2));
                    if (w < 0.0)
                    {
                        x = x - Math.Sqrt(EighthRoot(-w)) * (x + Math.Sqrt(tdiff / (tdiff + 1.50 * t0)));
                    }
                    w = 4.0 / (4.0 + tdiff);
                    x = x * (1.0 + x * (c1 * w - c2 * x * Math.Sqrt(w)));
                }
            }
            else
            {
                // With multirevs, first get t(min) as basis for starter.
                xm = 1.0 / (1.50 * (m + 0.5) * OrbitUtils.TwoPI);

                // Initialize out parameters.
                d2t = 0;

                if (thr2 < 0.5)
                {
                    xm = EighthRoot(2.0 * thr2) * xm;
                }
                if (thr2 > 0.5)
                {
                    xm = (2.0 - EighthRoot(2.0 - 2.0 * thr2)) * xm;
                }

                int i;
                for (i = 0; i < 12; i++)
                {
                    lamgtl(m, q, qsqfm1, xm, 3, out tmin, out dt, out d2t, out d3t, ref igtl);

                    if (Math.Abs(d2t) < double.Epsilon)
                    {
                        break;
                    }

                    double xmold = xm;
                    xm = xm - dt * d2t / (d2t * d2t - dt * d3t / 2.0);

                    double xtest = Math.Abs(xmold / xm - 1.0);
                    if (xtest <= tol)
                    {
                        break;
                    }
                }

                if (i == 12)
                {
                    n = -1;
                    return;
                }

                tdiffm = tin - tmin;

                if (tdiffm < 0.0)
                {
                    n = 0;
                    return;
                }
                if (Math.Abs(tdiffm) < double.Epsilon)
                {
                    x = xm;
                    n = 1;
                    return;
                }
                
                n = 3;
                if (Math.Abs(d2t) < double.Epsilon)
                {
                    d2t = 6.0 * m * Math.PI;
                }
                x = Math.Sqrt(tdiffm / (d2t / 2.0 + tdiffm / (1.0 - xm) / (1.0 - xm)));
                double w = xm + x;
                w = w * 4.0 / (4.0 + tdiffm) + (1.0 - w) * (1.0 - w);
                x = x * (1.0 - (1.0 + m + c41 * (thr2 - 0.5)) / (1.0 + c3 * m) *
                         x * (c1 * w + c2 * x * Math.Sqrt(w))) + xm;
                d2t2 = d2t / 2.0;

                if (x >= 1.0)
                {
                    n = 1;
                    jumpto3 = true;
                } // No finite solution with x > xm
            } // Now you have a starter, so proceed by Halley.

            double t;
            while (true)
            {
                if (!jumpto3)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        lamgtl(m, q, qsqfm1, x, 2, out t, out dt, out d2t, out d3t, ref igtl);
                        t = tin - t;
                        if (Math.Abs(dt) > double.Epsilon)
                        {
                            x = x + t * dt / (dt * dt + t * d2t / 2.0);
                        }
                    }

                    if (n != 3)
                    {
                        return;
                    }

                    n = 2;
                    xpl = x; // Second multirev starter.
                }

                lamgtl(m, q, qsqfm1, 0.0, 0, out t0, out dt, out d2t, out d3t, ref igtl);

                double tdiff0 = t0 - tmin;
                tdiff = tin - t0;

                if (tdiff <= 0.0)
                {
                    x = xm - Math.Sqrt(tdiffm / (d2t2 - tdiffm * (d2t2 / tdiff0 - 1.0 / (xm * xm))));
                }
                else
                {
                    x = -tdiff / (tdiff + 4.0);
                    // BNL???  ij = 200   ! What is the function of this line in Gooding and Klumpp memos?
                    double w = x + c0 * Math.Sqrt(2.0 * (1.0 - thr2));

                    if (w < 0.0)
                    {
                        x = x - Math.Sqrt(EighthRoot(-w)) * (x + Math.Sqrt(tdiff / (tdiff + 1.50 * t0)));
                    }
                    w = 4.0 / (4.0 + tdiff);
                    x = x * (1.0 + (1.0 + m + c42 * (thr2 - 0.50)) / (1.0 + c3 * m) *
                             x * (c1 * w - c2 * x * Math.Sqrt(w)));

                    if (x <= -1.0)
                    {
                        n = n - 1;
                        if (n == 1)
                        {
                            x = xpl;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// This subroutine is for use with lamrhg, a Lambert solver.
        /// <remarks>
        /// This subroutine is for use with lamrhg, a Lambert solver
        /// by Robert H. Gooding of the Royal Aerospace 
        /// Establishment, Farnborough, Hants, England.
        /// It has been tested and slightly improved by Allan Klumpp of JPL 1990.
        /// References: R.H.Gooding, "A Procedure for the Solution of Lambert's
        ///             Orbital Boundary Value Problem," Celestial Mechanics
        ///             and Dynamic Astronomy 48, 1990.
        ///             Allan Klumpp, "Performance Comparison of Lambert
        ///             and Kepler Algorithms,"  JPL Interoffice Memo
        ///             314.1-0426-ARK, January 10, 1991.
        /// Recoded from the appendix of the Klumpp report by B.N.Lundberg 5-18-2004.
        /// Explicit typing and other (non-algorithmic) changes made by BNL for efficiency and appearance.
        /// Converted from Fortran to C by J.Anderson 8-03-2005.
        /// Converted from C to C# for Unity 2018.1 by K.M.Hylbom 6-12-2018.
        /// </remarks>
        /// <param name="m">Number of revolutions</param>
        /// <param name="n">Number of solutions found {0, 1, 2}</param>
        /// <param name="igtl">Counter for calls to lamgtl</param>
        /// </summary>
        static void lamgtl(int m, double q, double qsqfm1, double x, int n,
                           out double t, out double dt, out double d2t,
                           out double d3t, ref int igtl)
        {
            const double sw = 0.40;

            // Initialize out parameters.
            t = 0;
            dt = 0;
            d2t = 0;
            d3t = 0;

          
            // Update iteration counter.
            igtl = igtl + 1;

            // Logical expressions.
            bool lm1 = (n == -1);
            bool l1 = (n >= 1);
            bool l2 = (n >= 2);
            bool l3 = (n == 3);

            double qsq = q * q;
            double xsq = x * x;
            double u = (1.0 - x) * (1.0 + x);

            if (!lm1)
            {
                dt = 0.0;
                d2t = 0.0;
                d3t = 0.0;
            }

            if (lm1 || (m > 0) || (x < 0.0) || (Math.Abs(u) > sw))
            {
                // Direct computation.
                double y = Math.Sqrt(Math.Abs(u));
                double z = Math.Sqrt(qsqfm1 + qsq * xsq);
                double qx = q * x;

                double a = double.NaN;
                double b = double.NaN;
                double aa = double.NaN;
                double bb = double.NaN;

                if (qx <= 0.0)
                {
                    a = z - qx;
                    b = q * z - x;
                }

                if ((qx < 0.0) && lm1)
                {
                    aa = qsqfm1 / a;
                    bb = qsqfm1 * (qsq * u - xsq) / b;
                }

                if ((Math.Abs(qx) < double.Epsilon) && lm1 || (qx > 0.0))
                {
                    aa = z + qx;
                    bb = q * z + x;
                }

                if (qx > 0.0)
                {
                    a = qsqfm1 / aa;
                }

                if ((qx < 0.0) && lm1)
                {
                    aa = qsqfm1 / a;
                    bb = qsqfm1 * (qsq * u - xsq) / b;
                }

                if ((Math.Abs(qx) < double.Epsilon) && (lm1) || (qx > 0.0))
                {
                    aa = z + qx;
                    bb = q * z + x;
                }

                if (qx > 0.0)
                {
                    a = qsqfm1 / aa;
                    b = qsqfm1 * (qsq * u - xsq) / bb;
                }

                if (!lm1)
                {
                    double g;
                    if (qx * u >= 0.0)
                        g = x * z + q * u;
                    else
                        g = (xsq - qsq * u) / (x * z - q * u);

                    double f = a * y;

                    if (x <= 1.0)
                    {
                        t = m * Math.PI + Math.Atan2(f, g);
                    }
                    else
                    {
                        if (f > sw)
                        {
                            t = Math.Log(f + g);
                        }
                        else
                        {
                            double fg1 = f / (g + 1.0);
                            double term = 2.0 * fg1;
                            double fg1sq = fg1 * fg1;
                            t = term;
                            double twoi1 = 1.0;
                            double told;
                            do
                            {
                                twoi1 = twoi1 + 2.0;
                                term = term * fg1sq;
                                told = t;
                                t = t + term / twoi1;
                            } while (Math.Abs(t - told) > double.Epsilon);
                        }
                    }

                    t = 2.0 * (t / y + b) / u;

                    if (l1 && (Math.Abs(z) > double.Epsilon))
                    {
                        double qz = q / z;
                        double qz2 = qz * qz;
                        qz = qz * qz2;
                        dt = (3.0 * x * t - 4.0 * (a + qx * qsqfm1) / z) / u;

                        d2t = double.NaN;
                        if (l2)
                        {
                            d2t = (3.0 * t + 5.0 * x * dt + 4.0 * qz * qsqfm1) / u;
                        }
                        if (l3)
                        {
                            d3t = (8.0 * dt + 7.0 * x * d2t - 12.0 * qz * qz2 * x * qsqfm1) / u;
                        }
                    }
                }
                else
                {
                    dt = b;
                    d2t = bb;
                    d3t = aa;
                }
            }
            else
            {
                // Use series.
                double u0i = 1.0;
                double u1i = double.NaN;
                double u2i = double.NaN;
                double u3i = double.NaN;

                if (l1)
                    u1i = 1.0;
                if (l2)
                    u2i = 1.0;
                if (l3)
                    u3i = 1.0;

                double term = 4.0;
                double tq = q * qsqfm1;

                double tqsum;
                if (q < 0.5)
                    tqsum = 1.0 - q * qsq;
                else
                    tqsum = (1.0 / (1.0 + q) + q) * qsqfm1;

                double ttmold = term / 3.0;
                t = ttmold * tqsum;

                // Start of loop.
                int i = 0;
                double told;
                do
                {
                    i++;
                    double p = i;
                    u0i = u0i * u;

                    if (l1 && (i > 1))
                        u1i = u1i * u;
                    if (l2 && (i > 2))
                        u2i = u2i * u;
                    if (l3 && (i > 3))
                        u3i = u3i * u;

                    term = term * (p - 0.50) / p;
                    tq = tq * qsq;
                    tqsum = tqsum + tq;

                    told = t;
                    double tterm = term / (2.0 * p + 3.0);
                    double tqterm = tterm * tqsum;
                    t = t - u0i * ((1.50 * p + 0.250) * tqterm / (p * p - 0.250) - ttmold * tq);
                    ttmold = tterm;
                    tqterm = tqterm * p;

                    if (l1)
                        dt = dt + tqterm * u1i;
                    if (l2)
                        d2t = d2t + tqterm * u2i * (p - 1.0);
                    if (l3)
                        d3t = d3t + tqterm * u3i * (p - 1.0) * (p - 2.0);
                } while ((i < n) || (Math.Abs(t - told) > double.Epsilon));

                if (l3)
                    d3t = 8.0 * x * (1.50 * d2t - xsq * d3t);
                if (l2)
                    d2t = 2.0 * (2.0 * xsq * d2t - dt);
                if (l1)
                    dt = -2.0 * x * dt;

                t = t / xsq;
            }
        }
    }
}
