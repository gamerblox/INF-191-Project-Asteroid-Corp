using System;
using System.Collections;
using Game.Bodies;
using Game.Orbits;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace Game.IO
{
    /// <summary>
    /// Provides methods for retrieving and storing user-input mission values 
    /// from input fields (of type double) and dropdowns.
    /// </summary>
    [AddComponentMenu("Custom IO/Mission Input Manager")]
    public class InputManager : MonoBehaviour
    {
        double[] units = { 1.0, 30.4375, 365.25 }; // days, months, years

        Color normalColor = new Color(50f / 255f, 50f / 255f, 50f / 255f);
        Color warningColor = new Color(160f / 255f, 0f, 0f );
        Color okayColor = new Color(0f, 115f / 255f, 0f);

        [SerializeField]
        Dropdown TimeOfDeflectionUnits;

        [SerializeField]
        Dropdown TransferTimeUnits;

        [SerializeField]
        Dropdown LaunchVehicleDropdown;

        [SerializeField]
        Slider DiameterSlider;

        [SerializeField]
        Slider DensitySlider;

        [SerializeField]
        Text NeoCharacterizationText;

        string sizeNeo;
        string compositionNeo;

        [SerializeField]
        Text MassText;

        [SerializeField]
        /// <summary>
        /// The <see cref="LaunchVehicleData"/> assets corresponding to the
        /// launch vehicle dropdown items.
        LaunchVehicleData[] LaunchVehicleOptions;

        double inputValueD;
        double inputValueTOF;

        bool valid;
        public bool ValidInput { get { return valid; } }

        /// <summary>
        /// The time of deflection, in days before Earth-impact.
        /// </summary>
        public double TimeOfDeflection; //{ get { return inputValueD * units[TimeOfDeflectionUnits.value]; } }

        /// <summary>
        /// The transfer time for the intercepting spacecraft [days].
        /// </summary>
        public double TransferTimeDays; // { get { return inputValueTOF * units[TransferTimeUnits.value]; } }

        /// <summary>
        /// Diameter of the NEO [km].
        /// </summary>
        double diameter;

        [SerializeField]
        Text DiameterText;

        /// <summary>
        /// Density of the NEO [g cm^-3]
        /// </summary>
        double density;

        [SerializeField]
        Text DensityText;

        [SerializeField]
        Text DensityUnitsText;

        /// <summary>
        /// Launch vehicle for this deflection mission.
        /// </summary>
        public LaunchVehicleData LaunchVehicleType;

        /// <summary>
        /// Mass of the NEO [kg] calculated from <see cref="diameter"/> and 
        /// <see cref="density"/> in <see cref="TryCalculateMass()"/>.
        /// </summary>
        public double Mass;

        /// <summary>
        /// True if values describe a valid mission (time of deflection and 
        /// transfer time are > 0 and a launch vehicle has been selected).
        /// </summary>
        /// <returns><c>true</c>, if valid mission values are valid, <c>false</c> otherwise.</returns>
        public bool IsValidMission()
        {
            return (TimeOfDeflection > 0.0 && TransferTimeDays > 0.0 && Mass > 0.0 && LaunchVehicleType != null);
        }

        void Awake()
        {
            if (LaunchVehicleDropdown.options.Count != LaunchVehicleOptions.Length)
            {
                Debug.LogWarning("Number of launch vehicle options does not match " +
                                 "number of items in the dropdown");
            }
            // Init new characterization string pair and set defaults
            sizeNeo = "Medium";
            compositionNeo = "Carbonaceous Asteroid (porous rock)";
            UpdateNeoCharacterizationText();

            // Retrieve defaults
            LaunchVehicleType = LaunchVehicleOptions[LaunchVehicleDropdown.value];
            diameter = DiameterSlider.value / 1000;
            DiameterText.text = diameter.ToString();
            density = DensitySlider.value / 100;
            DensityText.text = density.ToString();
            TryCalculateMass();

            // Set density units label (using the superscript '3' character)
            char super3 = '\u00B3';
            DensityUnitsText.text = "g/cm" + super3.ToString();
        }
        
        /// <summary>
        /// Parse time of deflection from <see cref="InputField"/>.
        /// </summary>
        /// <param name="input">Input.</param>
        public void OnInputTimeOfDeflection(string input)
        {
            if (double.TryParse(input, out inputValueD))
            {
                TimeOfDeflection = inputValueD * units[TimeOfDeflectionUnits.value];
            }
            else 
            {
                inputValueD = 0;
                TimeOfDeflection = 0;
            }
            //Debug.Log("Time of Deflection = " + TimeOfDeflection);
        }

        public void OnTimeOfDeflectionUnitsChanged(int index)
        {
            TimeOfDeflection = inputValueD * units[index];
        }

        /// <summary>
        /// Parse transfer time from <see cref="InputField"/>.
        /// </summary>
        /// <param name="input">Input.</param>
        public void OnInputTransferTime(string input)
        {
            if (double.TryParse(input, out inputValueTOF))
            {
                TransferTimeDays = inputValueTOF * units[TransferTimeUnits.value];
            }
            else
            {
                inputValueTOF = 0;
                TransferTimeDays = 0;
            }
        }

        public void OnTransferTimeUnitsChanged(int index)
        {
            TransferTimeDays = inputValueD * units[index];
        }

        /// <summary>
        /// Ons the dropdown launch vehicle.
        /// </summary>
        /// <param name="index">Index.</param>
        public void OnLaunchVehicleChanged(int index)
        {
            LaunchVehicleType = LaunchVehicleOptions[index];
        }

        /// <summary>
        /// Retrieve and store the diameter of the NEO from slider input. Add
        /// this method to a <see cref="Slider"/> object's <see cref="Slider.onValueChanged()"/>
        /// methods in the Inspector.
        /// </summary>
        /// <remarks>
        /// The intended use of this method makes use of a slider who's value 
        /// is given in meters but stored in kilometers.
        /// </remarks>
        public void OnDiameterInMetersSliderChanged()
        {
            // Get the slider value and apply units conversion [m] -> [km].
            diameter = DiameterSlider.value / 1000.0;

            // Update the slider value text.
            DiameterText.text = diameter.ToString();

            // Recalculate mass.
            TryCalculateMass();

            // Classify NEO by size and update characterization text.
            if (diameter <= 0.03)
            {
                sizeNeo = "Tiny";
            }
            else if (diameter <= 0.1)
            {
                sizeNeo = "Small";
            }
            else if (diameter <= 0.3)
            {
                sizeNeo = "Medium"; 
            }
            else 
            {
                sizeNeo = "Large";
            }
            UpdateNeoCharacterizationText();
        }

        /// <summary>
        /// Retrieve and store the density of the NEO from slider input. Add
        /// this method to a <see cref="Slider"/> object's <see cref="Slider.onValueChanged()"/>
        /// methods in the Inspector.
        /// </summary>
        /// <remarks>
        /// Slider values currently in use have been multiplied by a factor of
        /// 100, such that the minimum value of 50 corresponds to a density of
        /// 0.5 g/cm^3, and the maximum value of 750 corresponds to 7.5 g/cm^3.
        /// The reason for this decision was so that precision of the density
        /// slider could be limited to at most 2 decimal places using the slider
        /// option "Whole Numbers" and doing the conversion from script.
        /// </remarks>
        public void OnDensityInGramsPerMeter3SliderChanged()
        {
            // Get the slider value and apply units conversion.
            density = DensitySlider.value / 100.0;

            // Update the slider value text
            DensityText.text = density.ToString();

            // Recalculate mass
            TryCalculateMass();

            // Classify Neo by composition and update characterization text. 
            // Approximate "mean densities" of NEOs
            // Comet    |   Ice and Dust    |   ~ 0.60 g/cm^3   | Min to 1  |
            // C-type   |   Carbonaceous    |   ~ 1.38 g/cm^3   |   1 to 2  | 75%
            // S-type   | Silicate "Stony"  |   ~ 2.71 g/cm^3   |   2 to 4  | 17%
            // M-type   | Metallic (iron)   |   ~ 5.32 g/cm^3   |   4 to 8  |
            if (density < 1.0)
            {
                compositionNeo = "Comet (ice and dust)";
            }
            else if (density < 2.0)
            {
                compositionNeo = "Carbonaceous Asteroid (porous rock)";
            }
            else if (density < 4.0)
            {
                compositionNeo = "Silicate Asteroid (stony)";
            }
            else
            {
                compositionNeo = "Metallic Asteroid (iron)";
            }
            UpdateNeoCharacterizationText();
        }

        void UpdateNeoCharacterizationText()
        {
            NeoCharacterizationText.text = sizeNeo + " " + compositionNeo;
        }

        /// <summary>
        /// Calculates the mass of the NEO from the current diameter and density.
        /// If mass cannot be computed, sets <see cref="Mass"/> to -1.
        /// </summary>
        void TryCalculateMass()
        {
            if (diameter > 0.0 && density > 0.0)
            {
                Mass = OrbitUtils.MassOfSphere(diameter, density);
                MassText.text = Mass.ToString("E3") + " kg";
                MassText.color = normalColor;
            }
            else
            {
                Mass = double.NegativeInfinity;
                MassText.text = "NaN";
                MassText.color = warningColor;
            }
        }
    }
}
