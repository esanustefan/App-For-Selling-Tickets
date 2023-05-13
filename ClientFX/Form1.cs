using System;
using System.Windows.Forms;
using ConcursBaschet.domain;
using ConsoleApplication1;
using Services;

namespace Baschet_Server_Client3
{
    public partial class Form1 : Form
    {
        ServiceInterface service;
        private Form2 form2;
        public Form1(ServiceInterface serviceApp, Form2 form2) 
        {
            InitializeComponent();
            this.service = serviceApp;
            this.form2 = form2;
        }
        
        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string username = UsernameTextBox.Text;
                Console.WriteLine("Username: " + username);
                string parola = passwordTextBox.Text;
                Console.WriteLine("Parola: " + parola);
                if (username == "" || parola == "")
                {
                    MessageBox.Show("Username sau parola nu au fost introduse!");
                    return;
                }
                else
                {
                    UserDTO userDto = new UserDTO(username, parola);
                    User user = service.logIn(userDto, form2);
                    if (user == null)
                    {
                        UsernameTextBox.Clear();
                        passwordTextBox.Clear();
                        MessageBox.Show("Nu exista arbitru cu aceste date de intrare!");
                        return;
                    }
                    else
                    {
                        form2.initializeDataGridView();
                        form2.Show();
                        //mainController.ShowDialog();
                        //this.Hide();

                    }
                }
                
            } catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


        }


    }
}