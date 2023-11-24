using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Newtonsoft.Json;

namespace WpfAppAssignment4
{
    public partial class MainWindow : Window
    {
        private List<Building> buildings = new List<Building>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void AddBuildingButton_Click(object sender, RoutedEventArgs e)
        {
            // Validate and parse input fields
            if (!int.TryParse(BuildingSizeTextBox.Text, out int size) || size <= 0)
            {
                BuildingSizeTextBox.Background = Brushes.Red;
                MessageBox.Show("Invalid building size. Please enter a positive integer.");
                return;
            }

            if (!int.TryParse(LightBulbsTextBox.Text, out int lightBulbs) || lightBulbs < 0)
            {
                LightBulbsTextBox.Background = Brushes.Red;
                MessageBox.Show("Invalid number of light bulbs. Please enter a non-negative integer.");
                return;
            }

            if (!int.TryParse(OutletsTextBox.Text, out int outlets) || outlets < 0)
            {
                OutletsTextBox.Background = Brushes.Red;
                MessageBox.Show("Invalid number of outlets. Please enter a non-negative integer.");
                return;
            }

            if (!IsNumeric(CreditCardTextBox.Text) || CreditCardTextBox.Text.Length != 16)
            {
                CreditCardTextBox.Background = Brushes.Red;
                MessageBox.Show("Invalid credit card number. Must be a 16-digit numeric string.");
                return;
            }

            // BuildingType enum
            BuildingType buildingType = (BuildingType)BuildingTypeComboBox.SelectedIndex;

            // AdditionalFeatures text
            string additionalFeatures = AdditionalFeaturesTextBox.Text;

            // Create a new Building instance
            Building building = new Building
            {
                Name = CustomerNameTextBox.Text,
                BuildingType = buildingType,
                Size = size,
                LightBulbs = lightBulbs,
                Outlets = outlets,
                CreditCardNumber = CreditCardTextBox.Text,
                AdditionalFeatures = additionalFeatures
            };

            // Add the building to the list
            buildings.Add(building);

            // Sort buildings by building size before updating the UI
            buildings.Sort((b1, b2) => b1.Size.CompareTo(b2.Size));

            // Then, sort by customer name and building type
            buildings.Sort((b1, b2) =>
            {
                int sizeComparison = b1.Size.CompareTo(b2.Size);
                if (sizeComparison != 0)
                {
                    return sizeComparison;
                }

                int nameComparison = string.Compare(b1.Name, b2.Name, StringComparison.Ordinal);
                if (nameComparison != 0)
                {
                    return nameComparison;
                }

                return b1.BuildingType.CompareTo(b2.BuildingType);
            });

            // Update the building information text
            UpdateBuildingInfoText();

            // Clear input fields
            ClearInputFields();
        }

        private bool IsNumeric(string value)
        {
            return value.All(char.IsDigit);
        }

        private void SaveToFileButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Serialize buildings to JSON
                string json = JsonConvert.SerializeObject(buildings);

                // Choose the appropriate file path
                string filePath = "buildings.json";

                // Write JSON to file
                File.WriteAllText(filePath, json);

                MessageBox.Show("Buildings saved to file successfully!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving buildings: {ex.Message}");
            }
        }

        private void LoadFromFileButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Choose the appropriate file path
                string filePath = "buildings.json";

                // Check if the file exists
                if (File.Exists(filePath))
                {
                    // Read JSON from file
                    string json = File.ReadAllText(filePath);

                    // Deserialize JSON to list of buildings
                    buildings = JsonConvert.DeserializeObject<List<Building>>(json);

                    // Sort buildings by customer name before updating the UI
                    buildings.Sort((b1, b2) => string.Compare(b1.Name, b2.Name, StringComparison.Ordinal));

                    // Update the building information text
                    UpdateBuildingInfoText();

                    MessageBox.Show("Buildings loaded from file successfully!");
                }
                else
                {
                    MessageBox.Show("File not found. No buildings loaded.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading buildings: {ex.Message}");
            }
        }

        private void UpdateBuildingInfoText()
        {
            // Clear existing text
            BuildingInfoTextBox.Text = "Sorted Customer Information:\n";

            // Iterate through sorted buildings and append to the text box
            foreach (var building in buildings)
            {
                BuildingInfoTextBox.Text += $"{building}\n";
            }
        }

        private void ClearInputFields()
        {
            // Clear input fields
            CustomerNameTextBox.Text = "";
            BuildingTypeComboBox.SelectedIndex = -1;
            BuildingSizeTextBox.Text = "";
            LightBulbsTextBox.Text = "";
            OutletsTextBox.Text = "";
            CreditCardTextBox.Text = "";
            AdditionalFeaturesTextBox.Text = "";

            // Reset background color of text boxes
            CustomerNameTextBox.Background = Brushes.White;
            BuildingSizeTextBox.Background = Brushes.White;
            LightBulbsTextBox.Background = Brushes.White;
            OutletsTextBox.Background = Brushes.White;
            CreditCardTextBox.Background = Brushes.White;
            AdditionalFeaturesTextBox.Background = Brushes.White;
        }
    }

    public class Building
    {
        public string Name { get; set; } = "";
        public BuildingType BuildingType { get; set; }
        public int Size { get; set; }
        public int LightBulbs { get; set; }
        public int Outlets { get; set; }
        public string CreditCardNumber { get; set; } = "";
        public string AdditionalFeatures { get; set; } = "";

        public override string ToString()
        {
            // Include additional features in the building information
            return $"Customer Name: {Name}\nBuilding Type: {BuildingType}\nBuilding Size: {Size} sq. ft.\n" +
                   $"Number of Light Bulbs: {LightBulbs}\nNumber of Outlets: {Outlets}\nCredit Card Number: {CreditCardNumber}\n" +
                   $"Additional Features: {AdditionalFeatures}\n";
        }
    }

    public enum BuildingType
    {
        House,
        Barn,
        Garage
    }
}
