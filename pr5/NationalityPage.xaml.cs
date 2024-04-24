using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace pr5
{
    public partial class NationalityPage : Page
    {
        private prEntities2 db;

        public NationalityPage()
        {
            InitializeComponent();
            db = new prEntities2();
            LoadNationalitiesData();
        }

        private void LoadNationalitiesData()
        {
            NationalityDataGrid.ItemsSource = db.Nationality.ToList();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string nationality = text1.Text.Trim();

                if (string.IsNullOrEmpty(nationality))
                {
                    MessageBox.Show("Пожалуйста, введите национальность.", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (db.Nationality.Any(n => n.Nationalityy == nationality))
                {
                    MessageBox.Show("Такая национальность уже существует.", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                Nationality newNationality = new Nationality()
                {
                    Nationalityy = nationality
                };

                db.Nationality.Add(newNationality);
                db.SaveChanges();

                LoadNationalitiesData();
                ClearTextBoxes();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при добавлении национальности: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (NationalityDataGrid.SelectedItem != null)
            {
                try
                {
                    Nationality selectedNationality = (Nationality)NationalityDataGrid.SelectedItem;
                    string newNationality = text1.Text.Trim();

                    if (string.IsNullOrEmpty(newNationality))
                    {
                        MessageBox.Show("Пожалуйста, введите национальность.", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    if (db.Nationality.Any(n => n.Nationalityy == newNationality && n.Nationality_ID != selectedNationality.Nationality_ID))
                    {
                        MessageBox.Show("Такая национальность уже существует.", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    selectedNationality.Nationalityy = newNationality;
                    db.SaveChanges();

                    LoadNationalitiesData();
                    ClearTextBoxes();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при редактировании национальности: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (NationalityDataGrid.SelectedItem != null)
            {
                try
                {
                    Nationality selectedNationality = (Nationality)NationalityDataGrid.SelectedItem;
                    db.Nationality.Remove(selectedNationality);
                    db.SaveChanges();

                    LoadNationalitiesData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при удалении национальности: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                if (textBox.Text == textBox.Tag.ToString())
                {
                    textBox.Text = "";
                }
            }
        }
        private bool isResettingText = false;

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                if (!isResettingText)
                {
                    try
                    {
                        if (!Regex.IsMatch(textBox.Text, @"^[а-яА-Яa-zA-Z0-9@.,]+$") || !Char.IsLetter(textBox.Text[0]))
                        {
                            MessageBox.Show("Пожалуйста, введите только буквы (включая русские), цифры и символ '@', и убедитесь, что первый символ - буква.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            isResettingText = true;
                            textBox.Text = textBox.Tag.ToString();
                            isResettingText = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка при проверке ввода: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
        private void NationalityDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (NationalityDataGrid.SelectedItem != null)
            {
                Nationality selectedNationality = (Nationality)NationalityDataGrid.SelectedItem;
                text1.Text = selectedNationality.Nationalityy;
            }
        }


        private void ClearTextBoxes()
        {
            text1.Text = "";
        }
    }
}
