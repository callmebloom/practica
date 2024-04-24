using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace pr5
{
    public partial class RolesPage : Page
    {
        private prEntities2 db;

        public RolesPage()
        {
            InitializeComponent();
    
            db = new prEntities2();

            LoadRolesData();
        }

        private void LoadRolesData()
        {
            RolesDataGrid.ItemsSource = db.Roles.ToList();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(RoleNameTextBox.Text))
                {
                    var newRole = new Roles()
                    {
                        Roless = RoleNameTextBox.Text
                    };

                    db.Roles.Add(newRole);
                    db.SaveChanges();

                    LoadRolesData();
                }
                else
                {
                    MessageBox.Show("Введите название роли.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при добавлении роли: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (RolesDataGrid.SelectedItem != null)
            {
                try
                {
                    Roles selectedRole = (Roles)RolesDataGrid.SelectedItem;
                    selectedRole.Roless = RoleNameTextBox.Text;

                    db.SaveChanges();

                    LoadRolesData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при редактировании роли: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Выберите роль для редактирования.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (RolesDataGrid.SelectedItem != null)
            {
                try
                {
                    Roles selectedRole = (Roles)RolesDataGrid.SelectedItem;

                    db.Roles.Remove(selectedRole);
                    db.SaveChanges();

                    LoadRolesData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при удалении роли: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Выберите роль для удаления.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private bool isResettingText = false;
        private void RolesDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (RolesDataGrid.SelectedItem != null)
            {
                Roles selectedRole = (Roles)RolesDataGrid.SelectedItem;
                RoleNameTextBox.Text = selectedRole.Roless;
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
