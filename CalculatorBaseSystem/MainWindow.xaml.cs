using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace CalculatorBaseSystem
{
    public partial class MainWindow : Window
    {

        private List<string> conversionHistory = new List<string>();

        public MainWindow()
        {
            InitializeComponent();

            FromBaseComboBox.Items.Add("2");
            FromBaseComboBox.Items.Add("8");
            FromBaseComboBox.Items.Add("10");
            FromBaseComboBox.Items.Add("16");

            ToBaseComboBox.Items.Add("2");
            ToBaseComboBox.Items.Add("8");
            ToBaseComboBox.Items.Add("10");
            ToBaseComboBox.Items.Add("16");

            FromBaseComboBox.SelectedIndex = 0;
            ToBaseComboBox.SelectedIndex = 2;
            UpdateHint();
        }

        private void BaseComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            UpdateHint();
        }

        private void UpdateHint()
        {
            int fromBase = int.Parse(FromBaseComboBox.SelectedItem.ToString());
            if (fromBase == 2)
            {
                HintTextBlock.Text = "Допустимые символы: 0, 1";
            }
            else if (fromBase == 8)
            {
                HintTextBlock.Text = "Допустимые символы: 0-7";
            }
            else if (fromBase == 10)
            {
                HintTextBlock.Text = "Допустимые символы: 0-9";
            }
            else if (fromBase == 16)
            {
                HintTextBlock.Text = "Допустимые символы: 0-9, A-F";
            }
        }

        private void InputTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            int fromBase = int.Parse(FromBaseComboBox.SelectedItem.ToString());

            char inputChar = e.Text[0];
            if ((fromBase == 2 && (inputChar != '0' && inputChar != '1')) ||
                (fromBase == 8 && (inputChar < '0' || inputChar > '7')) ||
                (fromBase == 10 && (inputChar < '0' || inputChar > '9')) ||
                (fromBase == 16 && !IsHexCharacter(inputChar)))
            {
                e.Handled = true;
            }
        }

        private bool IsHexCharacter(char c)
        {
            return (c >= '0' && c <= '9') || (c >= 'A' && c <= 'F') || (c >= 'a' && c <= 'f');
        }

        private void ConvertButton_Click(object sender, RoutedEventArgs e)
        {
            string input = InputTextBox.Text;

            int fromBase = int.Parse(FromBaseComboBox.SelectedItem.ToString());
            int toBase = int.Parse(ToBaseComboBox.SelectedItem.ToString());

            try
            {

                int decimalValue = Convert.ToInt32(input, fromBase);

                string convertedValue = Convert.ToString(decimalValue, toBase).ToUpper();

                ResultTextBlock.Text = $"Результат: {convertedValue}";

                string historyItem = $"Из базы {fromBase}: {input} В базу {toBase}: {convertedValue}";
                conversionHistory.Add(historyItem);
                HistoryListBox.Items.Add(historyItem);
            }
            catch (FormatException)
            {
                ResultTextBlock.Text = "Неверный формат ввода!";
            }
            catch (OverflowException)
            {
                ResultTextBlock.Text = "Число слишком велико!";
            }
            catch (ArgumentException)
            {
                ResultTextBlock.Text = "Неверная система счисления!";
            }
            catch (Exception ex)
            {
                ResultTextBlock.Text = $"Ошибка: {ex.Message}";
            }
        }

        private void ClearHistoryButton_Click(object sender, RoutedEventArgs e)
        {
            HistoryListBox.Items.Clear();
            conversionHistory.Clear();
        }

        private void SaveHistoryButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog
            {
                FileName = "История",
                DefaultExt = ".txt",
                Filter = "Text documents (.txt)|*.txt"
            };

            bool? result = saveFileDialog.ShowDialog();

            if (result == true)
            {
                string filePath = saveFileDialog.FileName;

                try
                {
                    using (StreamWriter writer = new StreamWriter(filePath))
                    {
                        foreach (string item in conversionHistory)
                        {
                            writer.WriteLine(item);
                        }
                    }

                    MessageBox.Show("История успешно сохранена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при сохранении файла: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}