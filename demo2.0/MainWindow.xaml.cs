using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using demo2._0;


namespace demo2._0
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string login = txtUsername.Text;
            string password = txtPassword.Password;

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Значение обоих полей должны быть заполнены!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            login = login.Trim();
            password = password.Trim();

            using (var context = new HotelManagementEntities1())
            {
                var user = context.Users.FirstOrDefault(u => u.username == login);

                if (user == null)
                {
                    MessageBox.Show("Пользователь с таким логином не найден.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (user.isLocked == true)
                {
                    MessageBox.Show("Вы заблокированы. Обратитесь к администратору.", "Доступ запрещен", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (user.password == password)
                {
                    user.lastLoginDate = DateTime.Now;
                    user.FailedLoginAttempts = 0;
                    context.SaveChanges();
                    MessageBox.Show("Вы успешно авторизовались!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                    AdminWindow adminWindow = new AdminWindow();
                    adminWindow.Show();
                    this.Close();
                }
                else
                {
                    user.FailedLoginAttempts = (user.FailedLoginAttempts ?? 0) + 1;
                    if (user.FailedLoginAttempts >= 3)
                    {
                        user.isLocked = true;
                        MessageBox.Show("Вы заблокированы из-за 3 неудачных попыток входа.", "Доступ запрещен", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        int attemptsLeft = 3 - (user.FailedLoginAttempts ?? 0);
                        MessageBox.Show($"Неправильный пароль. Осталось попыток: {attemptsLeft}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    context.SaveChanges();
                }
            }
        }
    }
}