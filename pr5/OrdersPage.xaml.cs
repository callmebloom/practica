using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace pr5
{
    public partial class OrdersPage : Page
    {
        private prEntities2 db;
        private void LoadStoreAddresses()
        {
            storeAddressComboBox.ItemsSource = db.Store.ToList();
        }
        public OrdersPage()
        {
            InitializeComponent();
            db = new prEntities2();
            LoadComboBoxData();
            LoadOrdersData();
            LoadStoreAddresses();
        }
        private void ResetFilterButton_Click(object sender, RoutedEventArgs e)
        {
            ClientNameSearchTextBox.Text = "";
            LoadOrdersData();
        }
        private void LoadComboBoxData()
        {
            clientNameComboBox.ItemsSource = db.Client.ToList();
            clientAddressComboBox.ItemsSource = db.Store.ToList();
            employeeAddressComboBox.ItemsSource = db.Employees.ToList();
            statusComboBox.ItemsSource = db.Statuses.ToList();
        }

        private void LoadOrdersData()
        {
            if (db != null)
                OrdersDataGrid.ItemsSource = db.Orders.ToList();
        }

        private void ClientNameSearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchString = ClientNameSearchTextBox.Text.Trim().ToLower();
            if (db != null && OrdersDataGrid != null)
            {
                if (!string.IsNullOrWhiteSpace(searchString))
                {
                    var orders = db.Orders.ToList().Where(o => o.Client.First_Name.ToLower().Contains(searchString)).ToList();
                    OrdersDataGrid.ItemsSource = orders;
                }
                else
                {
                    LoadOrdersData();
                }
            }
        }
        private void OrdersDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (OrdersDataGrid.SelectedItem != null)
            {
                Orders selectedOrder = (Orders)OrdersDataGrid.SelectedItem;

                orderNumberTextBox.Text = selectedOrder.Order_Number;
                orderDatePicker.SelectedDate = selectedOrder.Order_Date;
                clientNameComboBox.SelectedValue = selectedOrder.Client_ID;
                clientAddressComboBox.SelectedValue = selectedOrder.Store_ID;
                employeeAddressComboBox.SelectedValue = selectedOrder.Employee_ID;
                statusComboBox.SelectedValue = selectedOrder.Status_ID;
            }
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ValidateInputs())
                {
                    Orders newOrder = new Orders()
                    {
                        Order_Number = orderNumberTextBox.Text,
                        Order_Date = orderDatePicker.SelectedDate ?? DateTime.Now,
                        Client_ID = (int)clientNameComboBox.SelectedValue,
                        Employee_ID = (int)employeeAddressComboBox.SelectedValue,
                        Store_ID = (int)clientAddressComboBox.SelectedValue,
                        Status_ID = (int)statusComboBox.SelectedValue
                    };

                    db.Orders.Add(newOrder);
                    db.SaveChanges();

                    LoadOrdersData();
                    ClearInputFields();
                }
                else
                {
                    MessageBox.Show("Пожалуйста, заполните все поля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при добавлении нового заказа: " + ex.Message);
            }
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (OrdersDataGrid.SelectedItem != null)
                {
                    if (ValidateInputs())
                    {
                        Orders selectedOrder = (Orders)OrdersDataGrid.SelectedItem;

                        selectedOrder.Order_Number = orderNumberTextBox.Text;
                        selectedOrder.Order_Date = orderDatePicker.SelectedDate ?? DateTime.Now;
                        selectedOrder.Client_ID = (int)clientNameComboBox.SelectedValue;
                        selectedOrder.Employee_ID = (int)employeeAddressComboBox.SelectedValue;
                        selectedOrder.Store_ID = (int)clientAddressComboBox.SelectedValue;
                        selectedOrder.Status_ID = (int)statusComboBox.SelectedValue;

                        db.SaveChanges();

                        LoadOrdersData();
                        ClearInputFields();
                    }
                    else
                    {
                        MessageBox.Show("Пожалуйста, заполните все поля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Пожалуйста, выберите заказ для редактирования.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при редактировании заказа: " + ex.Message);
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (OrdersDataGrid.SelectedItem != null)
                {
                    Orders selectedOrder = (Orders)OrdersDataGrid.SelectedItem;

                    MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите удалить этот заказ?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        db.Orders.Remove(selectedOrder);
                        db.SaveChanges();

                        LoadOrdersData();
                    }
                }
                else
                {
                    MessageBox.Show("Пожалуйста, выберите заказ для удаления.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при удалении заказа: " + ex.Message);
            }
        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(orderNumberTextBox.Text) ||
                clientNameComboBox.SelectedIndex == -1 ||
                clientAddressComboBox.SelectedIndex == -1 ||
                employeeAddressComboBox.SelectedIndex == -1 ||
                statusComboBox.SelectedIndex == -1)
            {
                return false;
            }
            return true;
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
        private void StoreAddressComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (storeAddressComboBox.SelectedItem != null)
            {
                int selectedStoreID = (int)storeAddressComboBox.SelectedValue;
                OrdersDataGrid.ItemsSource = db.Orders.Where(o => o.Store_ID == selectedStoreID).ToList();
            }
        }



        private void ClearInputFields()
        {
            orderNumberTextBox.Text = "Order Number";
            orderDatePicker.SelectedDate = DateTime.Now;
            clientNameComboBox.SelectedIndex = -1;
            clientAddressComboBox.SelectedIndex = -1;
            employeeAddressComboBox.SelectedIndex = -1;
            statusComboBox.SelectedIndex = -1;
        }
    }
}
