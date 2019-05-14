using System;
using UnityEngine;

namespace Game.Orbits
{
    [Flags]
    public enum Units
    {
        // Units of distance
        KM = 0,
        AU = 1,
        EARTH_RADII = 2,
        LUNAR = 4,
        // Units of time
        SEC = 8,
        HOUR = 16,
        DAY = 32,
        YEAR = 64,
        // Angular units
        RAD = 128,
        DEG = 256,
        // Options
        KM_S = KM | SEC,
        KM_D = KM | DAY,
        AU_D = AU | DAY,
        DEFAULT = KM | SEC | RAD
    }

    [Serializable]
    public class OrbitData
    {
        #region Fields
        /// <summary>
        /// The current epoch of reference [Julian Day Number].
        /// </summary>
        double epoch;
        /// <summary>
        /// The orbit's current epoch [Julian Day Number]. Orbital
        /// state vectors (position and velocity) and the anomalies
        /// are dependent on the epoch. Orbital elements are independent 
        /// from the orbit's epoch.
        /// </summary>
        public double Epoch 
        {
            get { return epoch; }
            set { Step(value - epoch); }
        }

        /// <summary>
        /// Standard gravitational parameter; the Keplerian GM [AU^3 day^-2]
        /// </summary>
        double mu;
        public double KeplerianGM { get { return mu; }}

        /// <summary>
        /// Semi-major axis, a [km].
        /// </summary>
        double a;
        /// <summary>
        /// Semi-major axis, a [km].
        /// </summary>
        public double SemiMajorAxis { get { return a; }}

        /// <summary>
        /// Eccentricity, e.
        /// </summary>
        double ecc;
        /// <summary>
        /// Eccentricity, e.
        /// </summary>
        public double Eccentricity { get { return ecc; }}

        /// <summary>
        /// Inclination, i [rad].
        /// </summary>
        double i;
        /// <summary>
        /// Inclination, i [rad].
        /// </summary>
        public double Inclination { get { return i; }}

        /// <summary>
        /// Longitude of the ascending node, OMEGA Ω [rad].
        /// </summary>
        double Om;
        /// <summary>
        /// Longitude of the ascending node, OMEGA Ω [rad].
        /// Also called the right ascension of the ascending node, RAAN.
        /// </summary>
        public double LongitudeOfAscendingNode { get { return Om; }}

        /// <summary>
        /// Argument of perifocus, ω [rad].
        /// </summary>
        double w;
        /// <summary>
        /// Argument of perifocus, ω [rad]. Also called argument of perihelion (Sun),
        /// perigee (Earth), pericenter, periapsis, etc.
        /// </summary>
        public double ArgumentOfPerifocus { get { return w; }}

        /// <summary>
        /// Mean anomaly, M [rad].
        /// </summary>
        double M;
        /// <summary>
        /// Mean anomaly, M [rad].
        /// </summary>
        public double MeanAnomaly { get { return M; }}

        /// <summary>
        /// True anomaly, ν [rad].
        /// </summary>
        double nu;
        /// <summary>
        /// True anomaly, ν [rad].
        /// </summary>
        public double TrueAnomaly { get { return nu; }}

        /// <summary>
        /// Eccentric anomaly, E [rad].
        /// </summary>
        double E;

        /// <summary>
        /// Position vector relative to central body [km].
        /// </summary>
        Vector3d pos;
        /// <summary>
        /// Position vector of orbiting object relative to central body [km].
        /// </summary>
        public Vector3d Position { 
            get { return pos; } 
            set { pos = value; UpdateElemsFromState(); }
        }

        /// <summary>
        /// Velocity vector of orbiting object relative to central body [km/s].
        /// </summary>
        Vector3d vel;
        public Vector3d Velocity 
        {
            get { return vel; }
            set { vel = value; UpdateElemsFromState(); }
        }

        double period;

        /// <summary>
        /// Mean motion, N [rad].
        /// </summary>
        double N;

        #endregion

        #region Constructors
        public OrbitData() { }

        public OrbitData(double epoch, double mu, double a, double ecc, double i, double Om, double w, double M, double nu)
        {
            this.epoch = epoch;
            this.mu = mu;
            this.a = a;
            this.ecc = ecc;
            this.i = i;
            this.Om = Om;
            this.w = w;
            this.M = M;
            this.nu = nu;

            N = Math.Sqrt(mu / (a * a * a));
            period = OrbitUtils.TwoPI * Math.Sqrt(a * a * a / mu);
            UpdateStateFromElems();
            Debug.LogFormat("r = {0:E10}, v = {1:E10}", pos, vel);
        }

        /// <summary>Constructs a new <see cref="OrbitData"/> based on the name 
        /// of the orbit to initialize.
        /// 
        /// Current initialization of all bodies is for epoch J2000.0.
        /// Construction by <paramref name="name"/> available for:
        ///     Earth,
        ///     PDC17a.
        /// </summary>
        /// <remarks>
        /// The data used for initialization was retrieved from JPL's HORIZONS
        /// System, which provides access to key solar system data and highly
        /// accurate ephemerides for solar system objects. HORIZONS is provided
        /// by the Solar System Dynamics Group of the Jet Propulsion Laboratory.
        /// https://ssd.jpl.nasa.gov/horizons.cgi
        /// 
        /// "PDC17a" refers to a fictional asteroid, dubbed "2017 PDC" used in 
        /// the hypothetical impact scenario exercise during the International 
        /// Academy of Astronautics (IAA) 2017 Planetary Defense Conference in 
        /// Toyko, Japan, May 15-19, 2017. https://cneos.jpl.nasa.gov/pd/cs/pdc17/
        /// </remarks>
        /// <param name="name">The name of the orbit to initialize.</param>
        public OrbitData(string name, Units units = Units.KM_S)
        {
            bool km_sec = (units & Units.KM_S) != 0;
            bool km_day = (units & Units.KM_D) != 0;
            bool au_day = (units & Units.AU_D) != 0;

            switch (name.ToLower())
            {
                case "earth":
                    if (km_day)
                    {
                        mu = 9.9069603195178176E+20; // km^3 d^-2
                        epoch = 2460512.500000000;   // 2024-Jul-21 08:17:00.0000
                        ecc = 1.757248349510941E-02;
                        i = 3.725596059425610E-03 * OrbitUtils.Deg2Rad;
                        Om = 1.676963142208247E+02 * OrbitUtils.Deg2Rad;
                        w = 2.960923139782446E+02 * OrbitUtils.Deg2Rad;
                        M = 1.951261813216196E+02 * OrbitUtils.Deg2Rad;
                        nu = 1.946116523341310E+02 * OrbitUtils.Deg2Rad;
                        a = 1.494606976586629E+08;
                    }
                    else if (km_sec)
                    {
                        mu = 1.3271283864237474E+11;
                        epoch = 2460511.50;
                        ecc = 1.755515200922813E-02;
                        //qr = 1.468432479088422E+08;
                        i = 3.415029833502834E-03 * OrbitUtils.Deg2Rad;
                        Om = 1.743237727544583E+02 * OrbitUtils.Deg2Rad;
                        w = 2.891171874912577E+02 * OrbitUtils.Deg2Rad;
                        //Tp = 2.460679198076920E+06;
                        //N = 1.142247979038247E-05 * OrbitUtils.Deg2Rad; // mean motion
                        M = 1.944983899148370E+02 * OrbitUtils.Deg2Rad;
                        nu = 1.940052537229808E+02 * OrbitUtils.Deg2Rad;
                        a = 1.494671667413757E+08;
                        //ad = 1.520910855739092E+08; // aphelion distance
                        //pr = 3.151679903195047E+07; // mean sidereel period
                    }
                    else if (au_day)
                    {
                        mu = 2.9591309705483544E-04;
                        epoch = 2460511.845138889;
                        ecc = 1.756626879605684E-02;
                        // qr = 9.815559444772426E-01;
                        i = 3.516016131060297E-03 * OrbitUtils.Deg2Rad;
                        Om = 1.718949799164563E+02 * OrbitUtils.Deg2Rad;
                        w = 2.916672713700146E+02 * OrbitUtils.Deg2Rad;
                        // Tp = 2.460679319913221E+06;
                        // N = 9.869315614182272E-01 * OrbitUtils.Deg2Rad;
                        M = 1.947138594699715E+02 * OrbitUtils.Deg2Rad;
                        nu = 1.942132348290812E+02 * OrbitUtils.Deg2Rad;
                        a = 9.991065181306177E-01;
                        // ad = 1.016657091783993E+00;
                        // pr = 3.647669342772639E+02;
                    }
                    else
                    {
                        Debug.LogWarning("Cannot initialize orbit for " + name + ": bad units");
                    }
                    break;

                case "pdc17a":
                    if (km_sec)
                    {
                        //epoch = 2462239.406944; //date of impact in julian calendar
                        //mu = 1.3271244004193930E+11; // in km^3/s^2
                        //nu = -179.7029817948 * OrbitUtils.Deg2Rad; //the number i get is -179.7029817948 but i put it in positive degrees
                        //a = 1.3799480581751686E+8; //in km. in au = .9224383019077086
                        //ecc = .1911953048308701;
                        //i = 3.331369520013644 * OrbitUtils.Deg2Rad; //put in rad
                        //Om = 204.4460289189818 * OrbitUtils.Deg2Rad; //put in rad
                        //w = 126.401879524849 * OrbitUtils.Deg2Rad; //put in rad
                        //M = 180.4293730454418 * OrbitUtils.Deg2Rad; //put in rad
                        //period = 323.596949048484; //in days. in year it is: .88596

                        //in km-s
                        //epoch = 2462239.406944; //date of impact in julian calendar
                        //mu = 1.3271244004193930E+11; // in km^3/s^2
                        //nu = 180.297018205 * OrbitUtils.Deg2Rad; //the number i get is -179.7029817948 but i put it in positive degrees
                        //a = 1.3799480581751686E+8; //in km. in au = .9224383019077086
                        //ecc = .1911953048308701;
                        //i = 3.331369520013644 * OrbitUtils.Deg2Rad; //put in rad
                        //Om = 204.4460289189818 * OrbitUtils.Deg2Rad; //put in rad
                        //w = 126.401879524849 * OrbitUtils.Deg2Rad; //put in rad
                        //M = 180.4293730454418 * OrbitUtils.Deg2Rad; //put in rad
                        //period = 323.596949048484; //in days. in year it is: .88596

                        mu = 1.3271244004193930E+11;
                        epoch = 2460511.5; // 2024-Jul-20
                        ecc = 6.067308998464853E-01;
                        //qr = 1.318222284687151E+08; // perihelion distance
                        i = 6.297860669424182E+00 * OrbitUtils.Deg2Rad;
                        Om = 2.980466946281459E+02 * OrbitUtils.Deg2Rad;
                        w = 3.116195745824791E+02 * OrbitUtils.Deg2Rad;
                        //Tp = 2.460345623238127E+06; // time of perihelion
                        N = 3.401184666189607E-06 * OrbitUtils.Deg2Rad; // mean motion
                        M = 4.874493591005381E+01 * OrbitUtils.Deg2Rad;
                        nu = 1.218024541247387E+02 * OrbitUtils.Deg2Rad;
                        a = 3.351959978987864E+08;
                        //ad = 5.385697673288578E+08; // aphelion distance
                        //pr = 1.058454730725671E+08; // mean sidereel period
                    }
                    else if (km_day)
                    {
                        mu = 9.9069305641547517E+20; // km^3 d^-2
                        epoch = 2460512.500000000;   // 2024-Jul-21 08:17:00.0000
                        ecc = 6.067305783811098E-01;
                        i = 6.297858601278221E+00 * OrbitUtils.Deg2Rad;
                        Om = 2.980466307458600E+02 * OrbitUtils.Deg2Rad;
                        w = 3.116197271660765E+02 * OrbitUtils.Deg2Rad;
                        M = 4.903874628554143E+01 * OrbitUtils.Deg2Rad;
                        nu = 1.220721328669098E+02 * OrbitUtils.Deg2Rad;
                        a = 3.351962701009536E+08;
                    }
                    else if (au_day)
                    {
                        mu = 2.9591220828559093E-04;
                        epoch = 2460511.845138889; // 2024-Jul-20 08:17:00.0000
                        ecc = 6.067306208299826E-01;
                        // qr = 8.811777038321502E-01;  // perhilion distance
                        i = 6.297858542904073E+00 * OrbitUtils.Deg2Rad;
                        Om = 2.980466617932057E+02 * OrbitUtils.Deg2Rad;
                        w = 3.116196342717132E+02 * OrbitUtils.Deg2Rad;
                        // Tp = 2.460345621875274E+06;  // time of perhelion
                        // N = 2.938624012781988E-01;   // mean motion
                        M = 4.884676739427096E+01 * OrbitUtils.Deg2Rad;
                        nu = 1.218961183208733E+02 * OrbitUtils.Deg2Rad;
                        a = 2.240646616555420E+00;
                        // ad = 3.600115529278689E+00;  // aphelion distance
                        // pr = 1.225063153483146E+03;  // period
                    }
                    else
                    {
                        Debug.LogWarning("Cannot initialize orbit for " + name + ": bad units");
                    }
                    break;

                default:
                    Debug.LogErrorFormat("Cannot initialize orbit by name for '{0}'", name);
                    break;
            }

            N = Math.Sqrt(mu / (a * a * a));
            period = OrbitUtils.TwoPI * Math.Sqrt(a * a * a / mu);
            UpdateStateFromElems();
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="other">Orbit to copy.</param>
        public OrbitData(OrbitData other) {
            epoch = other.epoch;
            mu = other.mu;
            a = other.a;
            ecc = other.ecc;
            i = other.i;
            Om = other.Om;
            w = other.w;
            M = other.M;
            nu = other.nu;
            E = other.E;
            pos = other.pos;
            vel = other.vel;
            N = other.N;
            period = other.period;
        }

        /// <summary>
        /// Constructs a new <see cref="OrbitData"/> given the body's position
        /// and velocity vectors at epoch using the standard gravitational parameter.
        /// </summary>
        /// <param name="epoch">Epoch [Julian Day Number].</param>
        /// <param name="position">Position [km].</param>
        /// <param name="velocity">Velocity [km/s].</param>
        /// <param name="mu">Standard gravitational parameter, GM [km^3 s^-2].</param>
        public OrbitData(double epoch, Vector3d position, Vector3d velocity, double mu = OrbitUtils.GM_SUN)
        {
            this.epoch = epoch;
            this.mu = mu;
            pos = position;
            vel = velocity;
            UpdateElemsFromState();
            N = Math.Sqrt(mu / (a * a * a));
            period = OrbitUtils.TwoPI * Math.Sqrt(a * a * a / mu);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Steps the orbit by <paramref name="deltaTime"/>.
        /// Recalculates the anomalies for the new deltaTime, then updates the
        /// position and velocity at the new anomalies.
        /// </summary>
        /// <param name="deltaTime">Delta time [days].</param>
        public void Step(double deltaTime)
        {
            // Recalculate anomalies M, E, ν (nu) for time t = t0 + deltaTime.
            //M = M + (deltaTime * OrbitUtils.DAY2SEC) * Math.Sqrt(mu / (a * a * a));
            M = M + (deltaTime * OrbitUtils.DAY2SEC) * N;
            M = OrbitUtils.NormalizeAngle(M, 0.0, OrbitUtils.TwoPI);

            E = OrbitUtils.SolveKeplerEqForEccentricAnom(ecc, M, 1e-10, 20);

            nu = 2 * Math.Atan2(Math.Sqrt(1 + ecc) * Math.Sin(E / 2),
                                Math.Sqrt(1 - ecc) * Math.Cos(E / 2));

            // Calculate the position and velocity for the new epoch.
            UpdateStateFromElems();

            // Update epoch to reflect the timestep.
            epoch = epoch + deltaTime;
        }

        public void ToEpoch(double epoch)
        {
            Step(epoch - this.epoch);
        }

        public void InDeltaTimeDays(double deltaTimeDays)
        {
            Step(deltaTimeDays * OrbitUtils.DAY2SEC);
        }

        public void InDeltaTimeMonths(double deltaTimeMonths)
        {
            // Julian days in a year / 12 = 365.25/12 = 30.435 days per month.
            Step(deltaTimeMonths * 30.4375 * OrbitUtils.DAY2SEC);
        }

        ///// <summary>
        ///// Advances the orbit to the given epoch; only the anomalies and the 
        ///// state vectors are changed, the orbital elements remain the same.
        ///// </summary>
        ///// <param name="epoch">Epoch.</param>
        //public void AdvanceToEpoch(double epoch)
        //{
        //    double deltaTime = epoch - this.epoch;
        //    Step(deltaTime);
        //}

        public double GetPeriod()
        {
            return OrbitUtils.TwoPI * Math.Sqrt(a * a * a / mu);
        }

        public OrbitData CopyOf()
        {
            return new OrbitData(this);
        }

        /// <summary>
        /// <see cref="UnityEngine.Debug.Log(object)"/> the OrbitData.
        /// </summary>
        /// <param name="inDegrees">If set to <c>true</c>, logs the angles in degrees; default (false) in radians.</param>
        public void Log(bool inDegrees = false)
        {
            double _i = i;
            double _Om = Om;
            double _w = w;
            double _M = M;
            double _nu = nu;
            double _E = E;

            if (inDegrees)
            {
                _i *= OrbitUtils.Rad2Deg;
                _Om *= OrbitUtils.Rad2Deg;
                _w *= OrbitUtils.Rad2Deg;
                _M *= OrbitUtils.Rad2Deg;
                _nu *= OrbitUtils.Rad2Deg;
                _E *= OrbitUtils.Rad2Deg;
            }

            Debug.LogFormat("EPOCH {0:F5}, Date {1}", epoch, OrbitUtils.JDN2CalendarString(epoch));
            Debug.LogFormat(" A = {0:E5}, EC = {1:E5}", a, ecc);
            Debug.LogFormat("IN = {0:E5}, OM = {1:E5}, W = {2:E5}", _i, _Om, _w);
            Debug.LogFormat("MA = {0:E5}, TA = {1:E5}, E = {2:E5}", _M, _nu, _E);
            Debug.LogFormat("r = ({0:E15}, {1:E15}, {2:E15}), v = ({3:E15}, {4:E5}, {5:E5})",
                            pos.x, pos.y, pos.z, vel.x, vel.y, vel.z);
        }

        public override string ToString()
        {
            return ToString(true);
        }

        public string ToString(bool inDegrees, bool labels = true)
        {
            string[] label = { "SMA", "Ecc", "Inc", "LAN", "ArgP", "TA", "MA", "Period" };
            string angle = inDegrees ? "deg" : "rad";
            string[] units = { "km", "", angle, angle, angle, angle, angle, "sec" };
            double[] elements = { a, ecc, i, Om, w, nu, M, GetPeriod()};
            if (inDegrees) {
                for (int i = 2; i < elements.Length - 1; i++) {
                    elements[i] *= OrbitUtils.Rad2Deg;
                }
            }

            string s = "";
            s += Epoch + "\n";
            if (labels)
            {
                for (int i = 0; i < elements.Length; i++)
                {
                    s += string.Format("{0}\t{1:E15}\t{2}\n", label[i], elements[i], units[i]);
                }
            }
            else 
            {
                for (int i = 0; i < elements.Length; i++)
                {
                    s += string.Format("{0:E15}\n", elements[i]);
                }
            }

            //double perihelion = a * (1 - ecc);
            //double aphelion = a * (1 + ecc);

            return s;
        }

        #endregion

        #region Private Methods

        void UpdateStateFromElems()
        {
            // Radial distance [km]
            double r = a * (1.0 - ecc * ecc) / (1.0 + ecc * Math.Cos(nu));

            // Velocity magnitude [km/s]
            double v = Math.Sqrt(mu * (2.0 / r - 1.0 / a));

            // Flight path angle [rad]
            //double fpa = Math.Atan2(1.0 + ecc * Math.Cos(nu), ecc * Math.Sin(nu));
            double fpa = Math.Atan(ecc * Math.Sin(nu) / (1.0 + ecc * Math.Cos(nu)));

            double cosNuPlusW = Math.Cos(nu + w);
            double sinNuPlusW = Math.Sin(nu + w);
            double cosI = Math.Cos(i);
            double sinI = Math.Sin(i);
            double cosOm = Math.Cos(Om);
            double sinOm = Math.Sin(Om);
            double cosNuPlusWMinFpa = Math.Cos(nu + w - fpa);
            double sinNuPlusWMinFpa = Math.Sin(nu + w - fpa);

            pos.x = r * (cosNuPlusW * cosOm - sinNuPlusW * cosI * sinOm);
            pos.y = r * (cosNuPlusW * sinOm + sinNuPlusW * cosI * cosOm);
            pos.z = r * (sinNuPlusW * sinI);
            vel.x = v * (-sinNuPlusWMinFpa * cosOm - cosNuPlusWMinFpa * cosI * sinOm);
            vel.y = v * (-sinNuPlusWMinFpa * sinOm + cosNuPlusWMinFpa * cosI * cosOm);
            vel.z = v * (cosNuPlusWMinFpa * sinI);
        }

        /// <summary>
        /// Recalculate the orbital elements (a, ecc, i, Om, w) and anomalies
        /// from the orbit's current state vectors (position, velocity).
        /// </summary>
        void UpdateElemsFromState()
        {
            // Calculate specific angular momentum vector, h.
            Vector3d h = Vector3d.Cross(pos, vel);

            double r = pos.magnitude;        // Radius
            double v2 = vel.sqrMagnitude;    // ||Velocity||^2

            // Calculate eccentricity vector, e, and node vector, n.
            Vector3d e = Vector3d.Cross(vel, h) / mu - pos / r;
            Vector3d node = Vector3d.Cross(Vector3d.forward, h);

            a = 1 / (2 / r - v2 / mu);
            ecc = e.magnitude;

            i = Math.Acos(h.z / h.magnitude);

            double n = node.magnitude;
            Om = Math.Acos(node.x / n);
            if (node.y < 0)
            {
                Om = OrbitUtils.TwoPI - Om;
            }

            w = Math.Acos(Vector3d.Dot(node, e) / (n * ecc));
            if (e.z < 0)
            {
                w = OrbitUtils.TwoPI - w;
            }

            nu = Math.Acos(Vector3d.Dot(e, pos) / (ecc * r));
            if (Vector3d.Dot(pos, vel) < 0)
            {
                nu = OrbitUtils.TwoPI - nu;
            }

            // Eccentric anomaly from true anomaly
            E = 2 * Math.Atan2(Math.Tan(nu / 2), Math.Sqrt((1 + ecc) / (1 - ecc)));
            M = E - ecc * Math.Sin(E);

            N = Math.Sqrt(mu / (a * a * a));
            period = OrbitUtils.TwoPI * Math.Sqrt(a * a * a / mu);
        }

        #endregion

        #region NOT USING THESE FUNCTIONS?

        void UpdateElemsFromState2()
        {
            // Radius and velocity magnitudes.
            double r = pos.magnitude;
            double v = vel.magnitude;

            // Radius vector dot velocity vector
            double RdotV = Vector3d.Dot(pos, vel);

            // Angular momentum: h = r x v
            Vector3d h = Vector3d.Cross(pos, vel);

            // Eccentricity vector
            Vector3d e = 1.0 / mu * ((v * v - mu / r) * pos - RdotV * vel);

            // Eccentricity
            ecc = e.magnitude;

            // Semi-latus rectum vector
            Vector3d p = Vector3d.Cross(h, e);
            double P = p.magnitude;

            // Orbit unit vectors
            Vector3d hhat = h.normalized;
            Vector3d ehat = e.normalized;
            Vector3d phat = p.normalized;

            // Ascending node vector: n = zhat x hhat
            Vector3d n = Vector3d.Cross(Vector3d.forward, hhat);

            // Semi-major axis
            a = r / (2.0 - r * v * v / mu);

            // Inclination
            if (h.z <= -1.0)
            {
                i = Math.PI;
            }
            else if (h.z >= 1.0)
            {
                i = 0.0;
            }
            else
            {
                i = Math.Acos(hhat.z);
            }

            // Longitude of the ascending node (RAAN)
            Om = Math.Atan2(n.y, n.x);

            // Argument of perifocus
            w = -Math.Atan2(Vector3d.Dot(n, phat), Vector3d.Dot(n, ehat));

            // True anomaly
            nu = Math.Atan2(Vector3d.Dot(pos, phat), Vector3d.Dot(pos, ehat));

            // Enforce positive angles
            Om = Om < 0.0 ? Om + OrbitUtils.TwoPI : Om;
            w = w < 0.0 ? w + OrbitUtils.TwoPI : w;
            nu = nu < 0.0 ? nu + OrbitUtils.TwoPI : nu;
        }

        public Vector3d PositionInOrbitalPlane()
        {
            double rx = a * (Math.Cos(E) - ecc);
            double ry = a * Math.Sqrt(1 - ecc * ecc) * Math.Sin(E);
            return new Vector3d(rx, ry, 0);
        }

        public Vector3d VelocityInOrbitalPlane()
        {
            double cosE = Math.Cos(E);
            double r = a * (1 - ecc * cosE);
            double k = Math.Sqrt(mu * a) / r;
            double vx = k * -Math.Sin(E);
            double vy = k * Math.Sqrt(1 - ecc * ecc) * cosE;
            return new Vector3d(vx, vy, 0);
        }

        /// <summary>
        /// Updates the position and velocity of the body in its orbit based on
        /// the current anomaly. Reference plane is the ecliptic plane (J2000).
        /// </summary>
        void UpdatePositionAndVelocity()
        {
            // Heliocentric coordinates in its orbital plane r'.
            double rx = a * (Math.Cos(E) - ecc);
            double ry = a * Math.Sqrt(1 - ecc * ecc) * Math.Sin(E);

            // Heliocentric velocity in its orbital plane v'.
            double cosE = Math.Cos(E);
            double r = a * (1 - ecc * cosE);
            double k = Math.Sqrt(mu * a) / r;
            double vx = k * -Math.Sin(E);
            double vy = k * Math.Sqrt(1 - ecc * ecc) * cosE;

            double[,] T = OrbitUtils.GetTransformToEcliptic(i, Om, w);
            pos = OrbitUtils.ApplyTransform(rx, ry, T);
            vel = OrbitUtils.ApplyTransform(vx, vy, T);
        }
        #endregion

        #region OrbitRenderer Helper Methods
        /// <summary>
        /// Gets the points on orbit ellipse.
        /// </summary>
        /// <param name="vertices">Reference array storing the vertices of the ellipse.</param>
        /// <param name="resolution">Number of vertices to draw ellipse.</param>
        public void GetEllipseFromTrueAnom(ref Vector3[] vertices, int resolution)
        {
            if (resolution < 2)
            {
                vertices = new Vector3[0];
                return;
            }

            //if (ecc < 1 && ecc >= 0)
            if (vertices == null || vertices.Length != resolution)
            {
                vertices = new Vector3[resolution];
            }

            // Resolution as the angular distance (nu) between each vertex. 
            double nuResolution = OrbitUtils.TwoPI / resolution;
            double[,] T = OrbitUtils.GetTransformToEcliptic(i, Om, w);
            for (int i = 0; i < resolution; i++)
            {
                vertices[i] = GetVertexOfOrbitEllipse(a, ecc, i * nuResolution, T);
            }
        }

        public void GetEllipse(ref Vector3[] positions, int resolution)
        {
            if (resolution < 2)
            {
                positions = new Vector3[0];
                return;
            }

            if (positions == null || positions.Length != resolution)
            {
                positions = new Vector3[resolution];
            }

            period = GetPeriod();
            double slice = period / resolution;

            OrbitData clone = new OrbitData(this);
            positions[0] = (Vector3)(clone.Position * OrbitUtils.KM2AU);
            for (int j = 1; j < resolution; j++)
            {
                clone.Epoch += slice;
                positions[j] = (Vector3)(clone.Position * OrbitUtils.KM2AU);
            }
        }

        public void GetEllipseFromMeanAnom(ref Vector3[] positions, int resolution)
        {
            if (resolution < 2)
            {
                positions = new Vector3[0];
                return;
            }

            if (positions == null || positions.Length != resolution)
            {
                positions = new Vector3[resolution];
            }

            // Resolution as the angular distance (M) between each vertex.
            double maResolution = OrbitUtils.TwoPI / resolution;
            for (int i = 0; i < resolution; i++)
            {
                positions[i] = (Vector3)GetPositionFromMeanAnom(i * maResolution);
            }
        }

        static Vector3 GetVertexOfOrbitEllipse(double a, double ecc, double nu, double[,] T)
        {
            double E = OrbitUtils.TrueAnom2EccentricAnom(nu, ecc);
            double rx = a * (Math.Cos(E) - ecc);
            double ry = a * Math.Sqrt(1 - ecc * ecc) * Math.Sin(E);
            return (Vector3)OrbitUtils.ApplyTransform(rx, ry, T);
        }

        /// <summary>
        /// Returns the a position  
        /// </summary>
        /// <returns>The to ecliptic.</returns>
        /// <param name="MA">The mean anomaly [rad].</param>
        public Vector3d GetPositionFromMeanAnom(double MA)
        {
            double EA = OrbitUtils.MeanAnom2EccentricAnom(ecc, MA);
            double xv = a * (Math.Cos(EA) - ecc);
            double yv = a * Math.Sqrt(1.0 - ecc * ecc) * Math.Sin(EA);

            double v = Math.Atan2(yv, xv); // True anomaly, nu
            double r = Math.Sqrt(xv * xv + yv * yv); // Radius

            // Precalculate and store trig terms
            double sinOm = Math.Sin(Om);
            double cosOm = Math.Cos(Om);
            double sinNuW = Math.Sin(nu + w);
            double cosNuW = Math.Cos(nu + w);
            double cosI = Math.Cos(i);

            double xh = cosOm * cosNuW - sinOm * sinNuW * cosI;
            double yh = sinOm * cosNuW + cosOm * sinNuW * cosI;
            double zh = sinNuW * Math.Sin(i);

            return new Vector3d(r * xh, r * yh, r * zh);
        }

        #endregion
    }
}