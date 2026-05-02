using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;

namespace EWOMS_WPF_Administration.Login
{
    public partial class LoginView : Window
    {
        public LoginView()
        {
            InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var email = txtEmail.Text;
            var password = pwdBox.Password;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                txtMessage.Text = "Please enter email and password.";
                return;
            }

            try
            {
                using (var client = new HttpClient())
                {
                    var url = $"https://localhost:7107/api/ManageUsers/Login_MasterUser?Email={Uri.EscapeDataString(email)}&Password={Uri.EscapeDataString(password)}";
                    var response = await client.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        txtMessage.Text = "Login successful! Response: " + result;
                    }
                    else
                    {
                        txtMessage.Text = "Login failed. Check your credentials.";
                    }
                }
            }
            catch (Exception ex)
            {
                txtMessage.Text = "Error: " + ex.Message;
            }
        }
    }
}