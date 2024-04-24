﻿using System;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace pr5
{
    public partial class PositionPage : Page
    {
        private prEntities2 db;

        public PositionPage()
        {
            InitializeComponent();

            db = new prEntities2();
            LoadPositionsData();
        }

        private void LoadPositionsData()
        {
            PositionDataGrid.ItemsSource = db.Position.ToList();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(positionTitleTextBox.Text) || scheduleComboBox.Text == null)
                {
                    MessageBox.Show("Пожалуйста, заполните все поля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                Position newPosition = new Position()
                {
                    Position_Title = positionTitleTextBox.Text,
                    Schedule = scheduleComboBox.Text
                };

                db.Position.Add(newPosition);
                db.SaveChanges();

                LoadPositionsData();
                ClearInputFields();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine("Ошибка при добавлении новой должности:");
                sb.AppendLine(ex.Message);

                if (ex is DbEntityValidationException validationException)
                {
                    foreach (var eve in validationException.EntityValidationErrors)
                    {
                        sb.AppendLine($"Сущность типа \"{eve.Entry.Entity.GetType().Name}\" в состоянии \"{eve.Entry.State}\" имеет следующие ошибки валидации:");
                        foreach (var ve in eve.ValidationErrors)
                        {
                            sb.AppendLine($"- Свойство: \"{ve.PropertyName}\", Ошибка: \"{ve.ErrorMessage}\"");
                        }
                    }
                }

                MessageBox.Show(sb.ToString());
            }
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (PositionDataGrid.SelectedItem != null)
                {
                    Position selectedPosition = (Position)PositionDataGrid.SelectedItem;

                    if (string.IsNullOrWhiteSpace(positionTitleTextBox.Text) || scheduleComboBox.Text == null)
                    {
                        MessageBox.Show("Пожалуйста, заполните все поля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    selectedPosition.Position_Title = positionTitleTextBox.Text;
                    selectedPosition.Schedule = scheduleComboBox.Text;

                    db.SaveChanges();

                    LoadPositionsData();
                    ClearInputFields();
                }
                else
                {
                    MessageBox.Show("Пожалуйста, выберите должность для редактирования.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при редактировании должности: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (PositionDataGrid.SelectedItem != null)
                {
                    Position selectedPosition = (Position)PositionDataGrid.SelectedItem;

                    MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите удалить эту должность?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        db.Position.Remove(selectedPosition);
                        db.SaveChanges();

                        LoadPositionsData();
                        ClearInputFields();
                    }
                }
                else
                {
                    MessageBox.Show("Пожалуйста, выберите должность для удаления.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при удалении должности: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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
            else if (sender is ComboBox comboBox)
            {
                if (comboBox.Text == comboBox.Tag.ToString())
                {
                    comboBox.Text = "";
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


        private void PositionDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
         
            if (PositionDataGrid.SelectedItem != null)
            {
                Position selectedPosition = (Position)PositionDataGrid.SelectedItem;

                positionTitleTextBox.Text = selectedPosition.Position_Title;
                scheduleComboBox.Text = selectedPosition.Schedule;
            }
        }

        private void ClearInputFields()
        {
            positionTitleTextBox.Text = "Position Title";
            scheduleComboBox.Text = "";
        }
    }
}
