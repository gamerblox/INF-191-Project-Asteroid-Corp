using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.IO
{
    [AddComponentMenu("Custom IO/Scenario Dropdown Manager")]
    /// <summary>
    /// Provides methods for retrieving and storing dropdown field values used. 
    /// </summary>
    public class DropdownManager : MonoBehaviour
    {
        readonly List<string> budget_texts = new List<string>() { "5 Billion", "10 Billion", "100 Billion", "Unlimited" };
        readonly List<int> budget_values = new List<int>() { 5, 10, 100, 0 };
        readonly List<string> difficulty_texts = new List<string>() { "Precise", "Game" };
        readonly List<string> composition_texts = new List<string>() { "Rock", "Ice and Dust", "Iron" };
        readonly List<string> asteroidSize_texts = new List<string>() { "50 Meters", "100 Meters", "370 Meters", "1 Kilometer" };
        readonly List<int> asteroidSize_values = new List<int>() { 50, 100, 370, 1000 };
        readonly List<string> impactTime_texts = new List<string>() { "4 Years (48)", "8 Years (96)", "16 Years (192)" };
        readonly List<int> impactTime_values = new List<int>() { 48, 96, 192 };


        public Dropdown budget_dropdown;
        public Text budget_text;
        public Dropdown difficulty_dropdown;
        public Text difficulty_text;
        public Dropdown composition_dropdown;
        public Text composition_text;
        public Dropdown asteroidSize_dropdown;
        public Text asteroidSize_text;
        public Dropdown impactTime_dropdown;
        public Text impactTime_text;

        // Use this for initialization
        void Start()
        {
            PopulateList();
            budget_text.text = budget_texts[0];
            difficulty_text.text = difficulty_texts[0];
            composition_text.text = composition_texts[0];
            asteroidSize_text.text = asteroidSize_texts[0];
            impactTime_text.text = impactTime_texts[0];
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void Budget_IndexChanged(int index)
        {
            budget_text.text = budget_texts[index];
            if (index == 0)
            { budget_text.color = Color.grey; }
            else
            { budget_text.color = Color.white; }
        }

        public void Difficulty_IndexChanged(int index)
        {
            difficulty_text.text = difficulty_texts[index];
            if (index == 0)
            { difficulty_text.color = Color.grey; }
            else
            { difficulty_text.color = Color.white; }
        }

        public void AsteroidSize_IndexChanged(int index)
        {
            asteroidSize_text.text = asteroidSize_texts[index];
            if (index == 0)
            { asteroidSize_text.color = Color.grey; }
            else
            { asteroidSize_text.color = Color.white; }
        }

        public void Composition_IndexChanged(int index)
        {
            composition_text.text = composition_texts[index];
            if (index == 0)
            { composition_text.color = Color.grey; }
            else
            { composition_text.color = Color.white; }
        }

        public void ImpactTime_IndexChanged(int index)
        {
            impactTime_text.text = impactTime_texts[index];
            if (index == 0)
            { impactTime_text.color = Color.grey; }
            else
            { impactTime_text.color = Color.white; }
        }

        void PopulateList()
        {
            budget_dropdown.AddOptions(budget_texts);
            composition_dropdown.AddOptions(composition_texts);
            difficulty_dropdown.AddOptions(difficulty_texts);
            impactTime_dropdown.AddOptions(impactTime_texts);
            asteroidSize_dropdown.AddOptions(asteroidSize_texts);
        }
    }
}