using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace LottoProgram
{
    public sealed partial class MainPage : Page
    {
        private List<int> selectedNumbers;

        public MainPage()
        {
            // Initialisera sidan och skapa en lista för valda nummer
            this.InitializeComponent();
            selectedNumbers = new List<int>();
        }

        private void StartDrawing_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateInput())
            {
                // Hämta antalet dragningar och valda nummer från användargränssnittet
                int draws = int.Parse(drawsInput.Text);
                int[] chosenNumbers = selectedNumbers.ToArray();

                // Variabler för att hålla reda på antal vinster med 5, 6 och 7 rätt
                int wins5 = 0, wins6 = 0, wins7 = 0;
                Random random = new Random();

                // HashSet för att hålla koll på unika dragningar och undvika dubbletter
                HashSet<string> uniqueDraws = new HashSet<string>();

                for (int i = 0; i < draws; i++)
                {
                    // Generera 7 unika nummer för varje dragning
                    int[] drawnNumbers = Enumerable.Range(1, 35).OrderBy(x => random.Next()).Take(7).ToArray();

                    // Sortera för att göra jämförelsen positionsoberoende
                    Array.Sort(drawnNumbers);
                    string drawKey = string.Join(",", drawnNumbers.Select(n => n.ToString()));

                    // Kontrollera om denna kombination redan har dragits
                    if (uniqueDraws.Add(drawKey))
                    {
                        // Räkna antal korrekta nummer och öka vinster beroende på antalet rätt
                        int correctNumbers = drawnNumbers.Intersect(chosenNumbers).Count();

                        if (correctNumbers == 5)
                            wins5++;
                        else if (correctNumbers == 6)
                            wins6++;
                        else if (correctNumbers == 7)
                            wins7++;
                    }
                    else
                    {
                        // Gör om dragningen om samma kombination redan har dragits
                        i--;
                    }
                }

                // Visa resultatet i användargränssnittet
                resultText.Text = $"Antal vinster med 5 rätt: {wins5}\nAntal vinster med 6 rätt: {wins6}\nAntal vinster med 7 rätt: {wins7}";
            }
        }

        private bool ValidateInput()
        {
            // Validera antal dragningar
            if (string.IsNullOrWhiteSpace(drawsInput.Text) || !int.TryParse(drawsInput.Text, out int draws) || draws <= 0)
            {
                resultText.Text = "Ogiltigt antal dragningar. Ange ett positivt heltal.";
                return false;
            }

            // Lista av TextBox-kontroller för att ange varje nummer separat
            List<TextBox> numberInputs = new List<TextBox>
            {
                numberInput1, numberInput2, numberInput3, numberInput4, numberInput5, numberInput6, numberInput7
            };

            // Validera varje nummer
            selectedNumbers = new List<int>();

            foreach (TextBox textBox in numberInputs)
            {
                // Kontrollera om varje TextBox är tomt eller innehåller ogiltig inmatning
                if (string.IsNullOrWhiteSpace(textBox.Text) || !int.TryParse(textBox.Text, out int num) || !IsInRange(num))
                {
                    resultText.Text = "Ogiltiga nummer. Ange 7 unika heltal mellan 1 och 35.";
                    return false;
                }

                selectedNumbers.Add(num);
            }

            // Kontrollera att alla nummer är unika
            if (selectedNumbers.Distinct().Count() != 7)
            {
                resultText.Text = "Ogiltiga nummer. Ange 7 unika heltal mellan 1 och 35.";
                return false;
            }

            return true;
        }

        private bool IsInRange(int number)
        {
            // Kontrollera om ett nummer är inom det tillåtna intervallet (1-35)
            return number >= 1 && number <= 35;
        }
    }
}
