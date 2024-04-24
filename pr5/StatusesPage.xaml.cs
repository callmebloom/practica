using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace pr5
{
    public partial class StatusesPage : Page
    {
        private prEntities2 db;

        public StatusesPage()
        {
            InitializeComponent();

            db = new prEntities2();
            LoadStatusesData();
        }

        private void LoadStatusesData()
        {
            try
            {
                StatusesDataGrid.ItemsSource = db.Statuses.ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке данных: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string statusType = statusTypeTextBox.Text.Trim();

                if (string.IsNullOrEmpty(statusType))
                {
                    MessageBox.Show("Тип статуса не может быть пустым.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                Statuses newStatus = new Statuses()
                {
                    Status_Type = statusType
                };

                db.Statuses.Add(newStatus);
                db.SaveChanges();

                LoadStatusesData();
                statusTypeTextBox.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при добавлении нового статуса: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (StatusesDataGrid.SelectedItem != null)
                {
                    Statuses selectedStatus = (Statuses)StatusesDataGrid.SelectedItem;

                    string statusType = statusTypeTextBox.Text.Trim();

                    if (string.IsNullOrEmpty(statusType))
                    {
                        MessageBox.Show("Тип статуса не может быть пустым.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    selectedStatus.Status_Type = statusType;

                    db.SaveChanges();

                    LoadStatusesData();
                    statusTypeTextBox.Clear();
                }
                else
                {
                    MessageBox.Show("Пожалуйста, выберите статус для редактирования.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при редактировании статуса: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (StatusesDataGrid.SelectedItem != null)
                {
                    Statuses selectedStatus = (Statuses)StatusesDataGrid.SelectedItem;

                    MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите удалить этот статус?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        db.Statuses.Remove(selectedStatus);
                        db.SaveChanges();

                        LoadStatusesData();
                    }
                }
                else
                {
                    MessageBox.Show("Пожалуйста, выберите статус для удаления.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при удалении статуса: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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
        private void StatusesDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (StatusesDataGrid.SelectedItem != null)
            {
                Statuses selectedStatus = (Statuses)StatusesDataGrid.SelectedItem;
                statusTypeTextBox.Text = selectedStatus.Status_Type;
            }
        }

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


    }
}
